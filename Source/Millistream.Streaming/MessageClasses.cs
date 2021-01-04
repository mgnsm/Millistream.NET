using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Message Classes
    /// </summary>
    [Flags]
    public enum MessageClasses : uint
    {
        MDF_MC_UNDEF, 
        MDF_MC_NEWSHEADLINE,
        MDF_MC_QUOTE,
        MDF_MC_TRADE = 4,
        MDF_MC_ORDER = 8,
        MDF_MC_BASICDATA = 16,
        MDF_MC_PRICEHISTORY = 32,
        MDF_MC_NEWSCONTENT = 64,
        MDF_MC_CORPORATEACTION = 128,
        MDF_MC_TRADESTATE = 256,
        MDF_MC_FUNDAMENTALS = 512,
        MDF_MC_PERFORMANCE = 1024,
        MDF_MC_KEYRATIOS = 2048,
        MDF_MC_ESTIMATES = 4096,
        MDF_MC_ESTIMATESHISTORY = 8192,
        MDF_MC_NETORDERIMBALANCE = 16384,
        MDF_MC_L10N = 32768,
        MDF_MC_CI = 65536,
        MDF_MC_CIHISTORY = 131072,
        MDF_MC_PRIIP = 262144,
        MDF_MC_MIFID = 524288,
        MDF_MC_MIFIDHISTORY = 1048576,
        MDF_MC_MAPPINGS = 2097152,
        MDF_MC_MBO = 4194304,
        MDF_MC_GREEKS = 8388608,
        MDF_MC_QUOTEBBO = 16777216
    }
}