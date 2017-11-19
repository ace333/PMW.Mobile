using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using Android.Util;
using AndroidMobile.Helpers;
using AndroidMobile.Constants;

namespace AndroidMobile.Services
{
    [Service]
    public class AcceleroService : Service
    {
        public static List<int> _acceleroX;
        public static List<int> _acceleroY;
        public static List<int> _acceleroZ;
        private bool _isStarted;

        Timer _timer;

        public override void OnCreate()
        {
            base.OnCreate();
            _acceleroX = new List<int>();
            _acceleroY = new List<int>();
            _acceleroZ = new List<int>();
        }

        private async void SendValues(int[] values)
        {
            bool result = await JSONApiHelper.DoPostRequestAsync(APIConstant.AcceleroAddress, values);
            Log.Debug("ACCELERO POST REQUEST : ", result.ToString());
        }

        private void _acceleroValueUpdating(object state)
        {
            if(_acceleroX.Count >= 5 && _acceleroY.Count >= 5 && _acceleroZ.Count >= 5)
            {
                var xArr = _acceleroX.GetRange(0, 5).ToArray();
                var yArr = _acceleroY.GetRange(0, 5).ToArray();
                var zArr = _acceleroZ.GetRange(0, 5).ToArray();

                var finalArray = new int[xArr.Length + yArr.Length + zArr.Length];
                xArr.CopyTo(finalArray, 0);
                yArr.CopyTo(finalArray, 5);
                zArr.CopyTo(finalArray, 10);

                SendValues(finalArray);

                Log.Debug("ACCELERO DEBUG : ", finalArray.Length.ToString());

                _acceleroX.RemoveRange(0, 5);
                _acceleroY.RemoveRange(0, 5);
                _acceleroZ.RemoveRange(0, 5);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (!_isStarted)
            {
                _timer = new Timer(_acceleroValueUpdating, DateTime.Now, 0, 100);
                _isStarted = true;
            }

            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _isStarted = false;
        }
    }
}