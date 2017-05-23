using InteligentDimmerMobile.Model;

namespace InteligentDimmerMobile.Services
{
    public static class PrepareDataService
    {
        public static void PrepareData(byte secondByte, byte fourthByte)
        {
            ControlData.StartByte = 0xAA;
            ControlData.CommandByte = secondByte;
            ControlData.SeparatorByte = 0xBB;
            ControlData.DataByte = fourthByte;
            ControlData.EndByte = 0xCC;
        }
    }
}