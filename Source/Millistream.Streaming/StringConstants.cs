namespace Millistream.Streaming
{
    /// <summary>
    /// Constants defined as strings to make them easier to use in <see cref="Message.AddNumeric(uint, string)"/>.
    /// </summary>
    public static class StringConstants
    {
        /// <summary>
        /// Request classes to be used when issuing requests to the server/system.
        /// </summary>
        public static class RequestClasses
        {
            public const string All = "*";
            public const string MDF_RC_NEWSHEADLINE = "0";
            public const string MDF_RC_QUOTE = "1";
            public const string MDF_RC_TRADE = "2";
            public const string MDF_RC_ORDER = "3";
            public const string MDF_RC_BASICDATA = "4";
            public const string MDF_RC_PRICEHISTORY = "5";
            public const string MDF_RC_FIELDSREFERENCE ="6";
            public const string MDF_RC_INSREF = "7";
            public const string MDF_RC_NEWSCONTENT = "8";
            public const string MDF_RC_CORPORATEACTION = "9";
            public const string MDF_RC_TRADESTATE = "10";
            public const string MDF_RC_FUNDAMENTALS = "11";
            public const string MDF_RC_PERFORMANCE = "12";
            public const string MDF_RC_KEYRATIOS = "13";
            public const string MDF_RC_ESTIMATES = "14";
            public const string MDF_RC_ESTIMATESHISTORY ="15";
            public const string MDF_RC_NETORDERIMBALANCE = "16";
            public const string MDF_RC_L10N = "17";
            public const string MDF_RC_CI = "18";
            public const string MDF_RC_CIHISTORY = "19";
            public const string MDF_RC_PRIIP = "20";
            public const string MDF_RC_MIFID = "21";
            public const string MDF_RC_MIFIDHISTORY = "22";
            public const string MDF_RC_MAPPINGS = "23";
            public const string MDF_RC_MBO = "24";
            public const string MDF_RC_GREEKS = "25";
        }

        /// <summary>
        /// Request types defined as strings for easier use with <see cref="Message.AddNumeric(uint, string)"/>.
        /// </summary>
        public static class RequestTypes
        {
            /// <summary>
            /// For requesting a snapshot of the current values.
            /// </summary>
            public const string MDF_RT_IMAGE = "1";

            /// <summary>
            /// For requesting streaming data.
            /// </summary>
            public const string MDF_RT_STREAM = "2";

            /// <summary>
            /// For requesting both <see cref="MDF_RT_IMAGE"/> and <see cref="MDF_RT_STREAM"/> requests.
            /// </summary>
            public const string MDF_RT_FULL = "3";
        }
    }
}