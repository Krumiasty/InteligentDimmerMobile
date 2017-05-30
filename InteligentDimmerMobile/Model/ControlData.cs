namespace InteligentDimmerMobile.Model
{
    public static class ControlData
    {
        public static byte StartByte { get; set; }
        public static byte CommandByte { get; set; }
        public static byte SeparatorByte1 { get; set; }
        public static byte DataByte1 { get; set; }
        public static byte SeparatorByte2 { get; set; }
        public static byte DataByte2 { get; set; }
        public static byte EndByte { get; set; }
    }
}