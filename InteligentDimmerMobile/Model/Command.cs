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

namespace InteligentDimmerMobile.Model
{
    public enum Command : byte
    {
        OnOffTriak,
        SetPower,
        StroboscopeMode,
        TestDiodesOnOff, // nvm
        TestDiodesToggle, // nvm
        SetStructureTime,
        WriteStructureToDevice,
        FirstTimeStamp,
        SecondTimeStamp,
        SetAlarm,
        Ping
    }
}