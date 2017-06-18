namespace InteligentDimmerMobile.Model
{
    public enum Command : byte
    {
        OnOffTriak,
        SetPower,
        StroboscopeMode,
        TestDiodesOnOff, 
        TestDiodesToggle,
        SetStructureTime,
        WriteStructureToDevice,
        FirstTimeStamp,
        SecondTimeStamp,
        SetAlarm,
        Ping
    }
}