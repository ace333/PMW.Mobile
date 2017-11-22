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
using AndroidMobile.Constants;

namespace AndroidMobile
{
    [Activity(Label = "Patient Monitoring Workflow", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Plugin.BLE.Abstractions.Contracts.IAdapter _bleAdapter;
        private IDevice _currDevice;
        private IBluetoothLE _bluetooth;

        private Intent _heartService;
        private Intent _acceleroService;

        private Button _isConnectedButton;
        private Button _isTrasmittinButton;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            _bluetooth = CrossBluetoothLE.Current;
            _bleAdapter = CrossBluetoothLE.Current.Adapter;
            _bleAdapter.ScanTimeout = 10000;

            InitializeButtons();

            _bleAdapter.DeviceDiscovered += _bleAdapter_DeviceDiscovered;

            _heartService = new Intent(this, typeof(HeartService));
            _acceleroService = new Intent(this, typeof(AcceleroService));
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

                            _isTrasmittinButton.SetBackgroundColor(Android.Graphics.Color.LawnGreen);

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
            Button scanButton = FindViewById<Button>(Resource.Id.scanButton);
            Button transmitButton = FindViewById<Button>(Resource.Id.trasmitData);

            scanButton.SetTextColor(Android.Graphics.Color.White);
            transmitButton.SetTextColor(Android.Graphics.Color.White);
            transmitButton.Enabled = false;

            _isConnectedButton = FindViewById<Button>(Resource.Id.connectedButton);
            _isConnectedButton.SetBackgroundColor(Android.Graphics.Color.PaleVioletRed);
            _isTrasmittinButton = FindViewById<Button>(Resource.Id.transmittingButton);
            _isTrasmittinButton.SetBackgroundColor(Android.Graphics.Color.PaleVioletRed);

            scanButton.Click += bleScan;
            transmitButton.Click += trasmitData;
        }

        private void trasmitData(object sender, EventArgs e)
        {
            _isTrasmittinButton.SetBackgroundColor(Android.Graphics.Color.LawnGreen);

            StartService(_heartService);
            StartService(_acceleroService);

            StartCharacteristics();
        }

        private void acceleroZ_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            AcceleroService._acceleroZ.Add(value);
            AcceleroService._acceleroIncrement_Z++;
        }

        private void acceleroY_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            AcceleroService._acceleroY.Add(value);
            AcceleroService._acceleroIncrement_Y++;
        }

        private void acceleroX_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            AcceleroService._acceleroX.Add(value);
            AcceleroService._acceleroIncrement_X++;
        }

        private void heartValue_updated(object sender, CharacteristicUpdatedEventArgs e)
        {
            var value = e.Characteristic.Value[0];
            HeartService._heartValues.Add(value);
            HeartService._heartIncrement++;
        }

        private void bleScan(object sender, EventArgs e)
        {
            _bleAdapter.StartScanningForDevicesAsync();
        }

        private void _bleAdapter_DeviceDiscovered(object sender, DeviceEventArgs e)
        {
            _currDevice = e.Device;
            BluetoothDevice dev = _currDevice.NativeDevice as BluetoothDevice;

            if (dev.Address == StaticValues.DeviceAddress)
            {
                Toast.MakeText(this, _currDevice.Name + " - discovered!", ToastLength.Short).Show();
                ConnectToPMDDeviceAsync();
            }
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
                    _isConnectedButton.SetBackgroundColor(Android.Graphics.Color.LawnGreen);

                    StopScanningForDevice();
                    Toast.MakeText(this, "Connected to " + _currDevice.Name + "!", ToastLength.Short).Show();

                    Button transmitButton = FindViewById<Button>(Resource.Id.trasmitData);
                    transmitButton.Enabled = true;
                }
                catch (DeviceConnectionException er)
                {
                    errorstr = er.Message;
                }
            }
        }
    }
}

