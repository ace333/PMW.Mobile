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
        private bool _isStarted;
        
        Timer _timer;

        public override void OnCreate()
        {
            base.OnCreate();
            _heartValues = new List<int>();
        }

        private async void SendValues(int[] values)
        {
            bool result = await JSONApiHelper.DoPostRequestAsync(APIConstant.HeartRateAddress, values);
            Log.Debug("ACCELERO POST REQUEST : ", result.ToString());
        }

        private void _heartValueUpdating(object state)
        {
            if(_heartValues.Count >= 5)
            {
                var array = _heartValues.GetRange(0, 5).ToArray();

                SendValues(array);

                Log.Debug("HEART DEBUG : ", array.Length.ToString());
                _heartValues.RemoveRange(0, 5);
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