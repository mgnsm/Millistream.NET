namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a date format.
    /// </summary>
    public enum DateFormat
    {
        /// <summary>
        /// YYYY
        /// </summary>
        Year,
        /// <summary>
        /// YYYY-MM
        /// </summary>
        Month,
        /// <summary>
        /// YYYY-MM-DD
        /// </summary>
        Day,
        /// <summary>
        /// YYYY-Qx
        /// </summary>
        Quarter,
        /// <summary>
        /// YYYY-Tx
        /// </summary>
        Tertiary,
        /// <summary>
        /// YYYY-Hx
        /// </summary>
        SemiAnnual,
        /// <summary>
        /// YYYY-Wxx
        /// </summary>
        Week
    }
}