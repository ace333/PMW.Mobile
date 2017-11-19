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
    }

    public static class APIConstant
    {
        public static string ApiAddress = "";
        public static string HeartRateAddress = "HeartRate";
        public static string AcceleroAddress = "Accelero";
    }
}