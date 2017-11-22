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
using Android.Util;
using System.Threading;
using AndroidMobile.Constants;
using AndroidMobile.Helpers;

namespace AndroidMobile.Services
{
    [Service]
    public class HeartService : Service
    {
        public static List<int> _heartValues;
        public static int _heartIncrement;

        private static int _actualLength;

        private bool _isStarted;
        
        Timer _timer;

        public override void OnCreate()
        {
            base.OnCreate();
            _heartValues = new List<int>();
        }


        private void _heartValueUpdating(object state)
        {
            if(_heartIncrement >= StaticValues.MaxValue)
            {
                var array = _heartValues.GetRange(_actualLength, StaticValues.MaxValue).ToArray();

                Log.Debug("HEART DEBUG : ", array.Length.ToString());

                if(_actualLength == StaticValues.ClearValue)
                    _heartValues.RemoveRange(0, StaticValues.ClearValue);

                _heartIncrement = 0;

                _actualLength += StaticValues.MaxValue;

                JSONApiHelper.DoPostRequestAsync(APIConstant.HeartRateAddress, array);
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if(!_isStarted)
            {
                _timer = new Timer(_heartValueUpdating, DateTime.Now, 0, 100);
                _isStarted = true;
            }

            return StartCommandResult.NotSticky;

        }

        public override void OnDestroy()
        {
            _isStarted = false;
            base.OnDestroy();
        }   
    }
}