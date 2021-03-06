﻿// Original code taken from Dan Fernandez's BUILD talk: Building Apps for the Kinect for Windows SDK
// http://channel9.msdn.com/Events/TechEd/NorthAmerica/2013/DEV-B305
// Original source: http://video.ch9.ms/sessions/teched/na/2013/DEVB305_BuildingAppsWithKinect.zip
// Modifications by Donna Malayeri (@lindydonna)
// Additional Modifications by Michael Melancon (@michaelmelancon)

using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Coding4Fun.Toolkit.Controls.Common
{
    public static class HueLightingWrapper
    {
        private const int TOTAL_BULBS = 1;  // currently have only 1 bulb set up for demo
        private const string HUE_LIGHT_IP = "192.168.0.198";
        private const string HUE_LIGHT_USERNAME = "newdeveloper";
        private static Color lastColor;

        public static void SetHue(Color color)
        {
            if (color == lastColor)
                return;

            try
            {
                var drawColor = System.Drawing.Color.FromArgb(color.R, color.G, color.B);

                // build our State object
                var state = new
                {
                    on = true,
                    hue = (int)(drawColor.GetHue() * 182.04), // we convert the hue value into degrees by multiplying the value by 182.04
                    sat = (int)(drawColor.GetSaturation() * 254)
                };

                // convert it to json:
                var jsonObj = JsonConvert.SerializeObject(state);

                for (int i = 1; i <= TOTAL_BULBS; i++)
                {
                    // set the api url to set the state
                    var uri = new Uri(string.Format("http://{0}/api/{1}/lights/{2}/state", HUE_LIGHT_IP, HUE_LIGHT_USERNAME, i));

                    var client = new WebClient();

                    client.UploadStringCompleted += client_UploadStringCompleted;

                    // Invoke the PUT method to set the state of the bulb
                    client.UploadStringAsync(uri, "PUT", jsonObj, color);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public static void TurnOff()
        {
            try
            {
                var state = new { on = false, };

                // convert it to json:
                var jsonObj = JsonConvert.SerializeObject(state);

                for (int i = 1; i <= TOTAL_BULBS; i++)
                {
                    // set the api url to set the state
                    var uri = new Uri(string.Format("http://{0}/api/{1}/lights/{2}/state", HUE_LIGHT_IP, HUE_LIGHT_USERNAME, i));

                    var client = new WebClient();

                    client.UploadStringCompleted += client_UploadStringCompleted;

                    // Invoke the PUT method to set the state of the bulb
                    client.UploadString(uri, "PUT", jsonObj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        static void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.UserState != null)
                {
                    lastColor = (Color)e.UserState;
                }

                Debug.WriteLine(e.Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
