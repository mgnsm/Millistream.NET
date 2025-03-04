namespace Millistream.Streaming
{
    /// <summary>
    /// The possible delay values
    /// </summary>
    public static class DelayValues
    {
        public const byte MDF_DLY_REALTIME = 0;
        public const byte MDF_DLY_DELAY = 1;
        public const byte MDF_DLY_EOD = 2;
        public const byte MDF_DLY_NEXTDAY = 3;
        public const byte MDF_DLY_T1 = 4;
        public const byte MDF_DLY_ANY = 14;
        public const byte MDF_DLY_BEST = 15;
    }
}
