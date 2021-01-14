namespace Millistream.Streaming
{
    /// <summary>
    /// Corporate Action Types
    /// </summary>
    public enum CorporateAction : uint
    {
        MDF_CA_DIVIDEND,
        MDF_CA_SPLIT,
        MDF_CA_RIGHTSISSUE,
        MDF_CA_BONUSISSUE,
        MDF_CA_DIRECTEDISSUE,
        MDF_CA_SHAREREDEMPTION,
        MDF_CA_SPINOFF,
        MDF_CA_STOCKDIVIDEND,
        MDF_CA_STOCKDIVIDENDEX,
        MDF_CA_UNKNOWN,
        MDF_CA_IPO,
        MDF_CA_CURRENCYCONVERSION,
        MDF_CA_NOMINALVALUE,
        MDF_CA_CHANGEINUNDERLYING,
        MDF_CA_CHANGEOFBASICDATA,
        MDF_CA_CALENDAR,
        MDF_CA_INSIDERTRADING,
        MDF_CA_SPLITANDREDEMPTION,
        MDF_CA_EXCHANGECLOSED,
        MDF_CA_MAJORHOLDERS,
        MDF_CA_SHARELOAN
    }
}