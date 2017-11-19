using Android.App;
using Android.Widget;
using Android.OS;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.Generic;
using Plugin.BLE;
using Android.Bluetooth;
using System;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.EventArgs;
using Android.Content;
using AndroidMobile.Services;
using System.Threading;
using System.Threading.Tasks;

namespace AndroidMobile
{
    [Activity(Label = "AndroidMobile", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Plugin.BLE.Abstractions.Contracts.IAdapter _bleAdapter;
        private IDevice _currDevice;
        private IBluetoothLE _bluetooth;

        private Intent _heartService;
        private Intent _acceleroService;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _bluetooth = CrossBluetoothLE.Current;
            _bleAdapter = CrossBluetoothLE.Current.Adapter;
            _bleAdapter.ScanTimeout = 10000;

            InitializeButtons();

            _bleAdapter.DeviceDiscovered += _bleAdapter_DeviceDiscovered;
            _bleAdapter.DeviceConnected += _bleAdapter_DeviceConnected;

            _heartService = new Intent(this, typeof(HeartService));
            _acceleroService = new Intent(this, typeof(AcceleroService));
        }

        private void _bleAdapter_DeviceConnected(object sender, DeviceEventArgs e)
        {
            StartService(_heartService);
            StartService(_acceleroService);

            StartCharacteristics();
        }

        private async void StartCharacteristics()
        {
            string errorstr;
            if (_bluetooth.IsAvailable)
                if (_bluetooth.IsOn)
                {

                    try
                    {
                        if (_bleAdapter.ConnectedDevices.Count > 0)
                        {
                            var services = await _currDevice.GetServicesAsync();

                            IList<ICharacteristic> accereloChar = await services[2].GetCharacteristicsAsync();
                            IList<ICharacteristic> heartChar = await services[3].GetCharacteristicsAsync();

                            heartChar[0].ValueUpdated += heartValue_updated;
                            await heartChar[0].StartUpdatesAsync();

                            accereloChar[0].ValueUpdated += acceleroX_updated;
                            await accereloChar[0].StartUpdatesAsync();

                            accereloChar[1].ValueUpdated += acceleroY_updated;
                            await accereloChar[1].StartUpdatesAsync();

                            accereloChar[2].ValueUpdated += acceleroZ_updated;
                            await accereloChar[2].StartUpdatesAsync();

                        }
                    }
                    catch (DeviceDiscoverException er)
                    {
                        errorstr = er.Message;
                    }
                }
        }

        private void InitializeButtons()
        {
            Button disconnetButton = FindViewById<Button>(Resource.Id.disconnectButton);
            Button scanButton = FindViewById<Button>(Resource.Id.scanButton);
            Button connButton = FindViewById<Button>(Resource.Id.connectButton);
            Button startHeartServiceButton = FindViewById<Button>(Resource.Id.startHeartServiceButton);

            disconnetButton.Click += disconnect_delegate;
            scanButton.Click += bleScan;
            connButton.Click += bleconnectAsync;
            startHeartServiceButton.Click += StartHeartServiceButton_Click;
        }

        private void StartHeartServiceButton_Click(object sender, EventArgs e)
        {
            StartService(_heartService);
        }

        private async void bleconnectAsync(object sender, EventArgs e)
        {
            //
        }

        private void acceleroZ_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            AcceleroService._acceleroZ.Add(value);
        }

        private void acceleroY_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            AcceleroService._acceleroY.Add(value);
        }

        private void acceleroX_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            AcceleroService._acceleroX.Add(value);
        }

        private void heartValue_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            HeartService._heartValues.Add(value);
        }

        private void bleScan(object sender, EventArgs e)
        {
            _bleAdapter.StartScanningForDevicesAsync();
        }

        private void disconnect_delegate(object sender, EventArgs e)
        {
            _bleAdapter.DisconnectDeviceAsync(_currDevice);

            StopService(_heartService);
            StopService(_acceleroService);
        }

        private void _bleAdapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            _currDevice = e.Device;

            Toast.MakeText(this, _currDevice.Name + " - discovered!", ToastLength.Long).Show();
            BluetoothDevice dev = _currDevice.NativeDevice as BluetoothDevice;

            if (dev.Address == "EE:8D:AD:5B:2B:E3")
                ConnectToPMDDeviceAsync();
        }

        public async void StopScanningForDevice()
        {
            if (_bleAdapter.IsScanning)
            {
                await _bleAdapter.StopScanningForDevicesAsync();
                Toast.MakeText(this, "Scanning stoped!", ToastLength.Short).Show();
            }
        }

        private async void ConnectToPMDDeviceAsync()
        {
            string errorstr = "";

            if (_bleAdapter.DiscoveredDevices.Count > 0)
            {
                try
                {
                    await _bleAdapter.ConnectToDeviceAsync(_currDevice);

                    StopScanningForDevice();
                    Toast.MakeText(this, "Connected to " + _currDevice.Name + "!", ToastLength.Short).Show();
                }
                catch (DeviceConnectionException er)
                {
                    errorstr = er.Message;
                }
            }
        }
    }
}

