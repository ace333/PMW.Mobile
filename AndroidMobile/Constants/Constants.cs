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

namespace AndroidMobile.Constants
{
    public static class StaticValues
    {
        public static int MaxValue = 5;
        public static int ClearValue = 1000;
        public static string DeviceAddress = "EE:8D:AD:5B:2B:E3";
    }

    public static class APIConstant
    {
        public static string ApiAddress = "http://pmw.arres.pl";
        public static string HeartRateAddress = "HeartRate";
        public static string AcceleroAddress = "Accelero";
    }
}