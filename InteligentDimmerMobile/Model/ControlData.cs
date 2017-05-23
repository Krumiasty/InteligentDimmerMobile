namespace InteligentDimmerMobile.Model
{
    public static class ControlData
    {
        public static byte StartByte { get; set; }
        public static byte CommandByte { get; set; }
        public static byte SeparatorByte { get; set; }
        public static byte DataByte { get; set; }
        public static byte EndByte { get; set; }
    }
}