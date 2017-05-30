using InteligentDimmerMobile.Model;

namespace InteligentDimmerMobile.Services
{
    public static class PrepareDataService
    {
        public static void PrepareData(byte secondByte, byte fourthByte, byte sixthByte)
        {
            ControlData.StartByte = 0xAA;
            ControlData.CommandByte = secondByte;
            ControlData.SeparatorByte1 = 0xBB;
            ControlData.DataByte1 = fourthByte;
            ControlData.SeparatorByte2 = 0xBB;
            ControlData.DataByte2 = sixthByte;
            ControlData.EndByte = 0xCC;
        }
    }
}