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

        public static int _acceleroIncrement_X;
        public static int _acceleroIncrement_Y;
        public static int _acceleroIncrement_Z;

        private static int _actualLength;

        private bool _isStarted;

        Timer _timer;

        public override void OnCreate()
        {
            base.OnCreate();
            _acceleroX = new List<int>();
            _acceleroY = new List<int>();
            _acceleroZ = new List<int>();
        }

        private void _acceleroValueUpdating(object state)
        {
            if(_acceleroIncrement_X >= StaticValues.MaxValue && _acceleroIncrement_Y >= StaticValues.MaxValue && _acceleroIncrement_Z >= StaticValues.MaxValue)
            {
                var xArr = _acceleroX.GetRange(_actualLength, StaticValues.MaxValue).ToArray();
                var yArr = _acceleroY.GetRange(_actualLength, StaticValues.MaxValue).ToArray();
                var zArr = _acceleroZ.GetRange(_actualLength, StaticValues.MaxValue).ToArray();

                var finalArray = new int[xArr.Length + yArr.Length + zArr.Length];
                xArr.CopyTo(finalArray, 0);
                yArr.CopyTo(finalArray, StaticValues.MaxValue);
                zArr.CopyTo(finalArray, StaticValues.MaxValue * 2);

                Log.Debug("ACCELERO DEBUG : ", finalArray.Length.ToString());
                
                if(_actualLength == 1000)
                {
                    _acceleroX.RemoveRange(0, StaticValues.ClearValue);
                    _acceleroY.RemoveRange(0, StaticValues.ClearValue);
                    _acceleroZ.RemoveRange(0, StaticValues.ClearValue);
                }

                _acceleroIncrement_X = 0;
                _acceleroIncrement_Y = 0;
                _acceleroIncrement_Z = 0;

                _actualLength += StaticValues.MaxValue;

                JSONApiHelper.DoPostRequestAsync(APIConstant.AcceleroAddress, finalArray);
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