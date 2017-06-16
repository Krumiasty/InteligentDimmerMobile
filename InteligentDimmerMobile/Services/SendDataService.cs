using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InteligentDimmerMobile.Configuration;
using InteligentDimmerMobile.Model;

namespace InteligentDimmerMobile.Services
{
    public static class SendDataService
    {
        public static async Task SendData(BluetoothSocket socket)
        {
            await socket.OutputStream.WriteAsync(new byte[]
            {
                ControlData.StartByte,
                ControlData.CommandByte,
                ControlData.SeparatorByte1,
                ControlData.DataByte1,
                ControlData.SeparatorByte2,
                ControlData.DataByte2,
                ControlData.EndByte
            }, 0, Constants.BytesNumber);

        }
    }
}