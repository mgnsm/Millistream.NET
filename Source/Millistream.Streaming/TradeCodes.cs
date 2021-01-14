using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Values for the <see cref="Field.MDF_F_TRADECODE"/> field
    /// </summary>
    [Flags]
    public enum TradeCodes : uint
    {
        MDF_TC_OFFHOURS = 1,
        MDF_TC_OUTSIDESPREAD = 2,
        MDF_TC_REPORTED = 4,
        MDF_TC_CORRECTION = 8,
        MDF_TC_CANCEL = 16,
        MDF_TC_UPDATEHIGHLOW = 32,
        MDF_TC_UPDATEVOLUME = 64,
        MDF_TC_UPDATELAST = 128,
        MDF_TC_ODDLOT = 256,
        MDF_TC_DELAYED = 512,
        MDF_TC_DARKPOOL = 1024
    }
}