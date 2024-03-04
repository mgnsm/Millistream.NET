namespace Millistream.Streaming
{
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