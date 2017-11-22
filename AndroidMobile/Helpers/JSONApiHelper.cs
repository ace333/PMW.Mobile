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
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AndroidMobile.Constants;

namespace AndroidMobile.Helpers
{
    public static class JSONApiHelper
    {
        public async static void DoPostRequestAsync(string measurementPath, int[] values)
        {
            using (var client = new HttpClient())
            {
                var uri = string.Format("{0}/{1}", APIConstant.ApiAddress, measurementPath);

                var json = JsonConvert.SerializeObject(values);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage result = await client.PostAsync(uri, content);
            }
        }

    }
}