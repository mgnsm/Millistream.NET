using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Message References
    /// </summary>
    [Obsolete("This enumeration is deprecated and has been replaced by the constants in the MessageReferences class.")]
    public enum MessageReference
    {
        MDF_M_MESSAGESREFERENCE = 0,
        MDF_M_LOGON = 1,
        MDF_M_LOGOFF = 2,
        MDF_M_LOGONGREETING = 3,
        MDF_M_NEWSHEADLINE = 4,
        MDF_M_QUOTE = 5,
        MDF_M_TRADE = 6,
        MDF_M_BIDLEVELINSERT = 7,
        MDF_M_ASKLEVELINSERT = 8,
        MDF_M_BIDLEVELDELETE = 9,
        MDF_M_ASKLEVELDELETE = 10,
        MDF_M_BIDLEVELUPDATE = 11,
        MDF_M_ASKLEVELUPDATE = 12,
        MDF_M_INSTRUMENTRESET = 13,
        MDF_M_ORDERBOOKFLUSH = 14,
        MDF_M_BASICDATA = 15,
        MDF_M_PRICEHISTORY = 16,
        MDF_M_INSTRUMENTDELETE = 17,
        MDF_M_FIELDSREFERENCE = 18,
        MDF_M_REQUEST = 19,
        MDF_M_REQUESTFINISHED = 20,
        MDF_M_INSREF = 21,
        MDF_M_NEWSCONTENT = 22,
        MDF_M_CORPORATEACTION = 23,
        MDF_M_TRADESTATE = 24,
        MDF_M_FUNDAMENTALS = 25,
        MDF_M_PERFORMANCE = 26,
        MDF_M_KEYRATIOS = 27,
        MDF_M_ESTIMATES = 28,
        MDF_M_ESTIMATESHISTORY = 29,
        MDF_M_NETORDERIMBALANCE = 30,
        MDF_M_UNSUBSCRIBE = 31,
        MDF_M_L10N = 32,
        MDF_M_CI = 33,
        MDF_M_CIHISTORY = 34,
        MDF_M_PRIIP = 35,
        MDF_M_MIFID = 36,
        MDF_M_MIFIDHISTORY = 37,
        MDF_M_MAPPINGS = 38,
        MDF_M_MBOADD = 39,
        MDF_M_MBOUPDATE = 40,
        MDF_M_MBODELETE = 41,
        MDF_M_GREEKS = 42,
        MDF_M_QUOTEBBO = 43,
        MDF_M_QUOTEEX = 44
    }
}