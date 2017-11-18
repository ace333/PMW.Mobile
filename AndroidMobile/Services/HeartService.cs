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

namespace AndroidMobile.Services
{
    [Service]
    public class HeartService : Service
    {
        private List<int> _heartValues;
        private bool _isStarted;
        
        Timer _timer;
        Handler _handler;
        private Action _action;

        public override void OnCreate()
        {
            base.OnCreate();
            _heartValues = MainActivity.heartValues;
        }

        private void _heartValueUpdating(object state)
        {
            if(MainActivity._heartBuff >= 5)
            {
                var array = _heartValues.ToArray();
                Log.Debug("HEART DEBUG : ", array.Length.ToString());
                _heartValues.RemoveRange(0, 5);
                MainActivity._heartBuff = 0;
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
                //_handler.Post(_action);
                _timer = new Timer(_heartValueUpdating, DateTime.Now, 0, 50);
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