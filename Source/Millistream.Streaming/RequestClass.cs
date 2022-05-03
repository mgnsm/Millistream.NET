namespace Millistream.Streaming
{
    /// <summary>
    /// Request Classes
    /// </summary>
    public enum RequestClass : uint
    {
        MDF_RC_NEWSHEADLINE = 0,
        MDF_RC_QUOTE = 1,
        MDF_RC_TRADE = 2,
        MDF_RC_ORDER = 3,
        MDF_RC_BASICDATA = 4,
        MDF_RC_PRICEHISTORY = 5,
        MDF_RC_FIELDSREFERENCE = 6,
        MDF_RC_INSREF = 7,
        MDF_RC_NEWSCONTENT = 8,
        MDF_RC_CORPORATEACTION = 9,
        MDF_RC_TRADESTATE = 10,
        MDF_RC_FUNDAMENTALS = 11,
        MDF_RC_PERFORMANCE = 12,
        MDF_RC_KEYRATIOS = 13,
        MDF_RC_ESTIMATES = 14,
        MDF_RC_ESTIMATESHISTORY = 15,
        MDF_RC_NETORDERIMBALANCE = 16,
        MDF_RC_L10N = 17,
        MDF_RC_CI = 18,
        MDF_RC_CIHISTORY = 19,
        MDF_RC_PRIIP = 20,
        MDF_RC_MIFID = 21,
        MDF_RC_MIFIDHISTORY = 22,
        MDF_RC_MAPPINGS = 23,
        MDF_RC_MBO = 24,
        MDF_RC_GREEKS = 25,
        MDF_RC_QUOTEBBO = 26,
        MDF_RC_QUOTEEX = 27
    }
}