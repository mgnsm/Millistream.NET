namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a prefix for a <see cref="List"/>.
    /// </summary>
    public enum ListPrefix
    {
        /// <summary>
        ///  The supplied list of insref lacks a prefix and should be treated like it was prefixed with <see cref="Replace"/>.
        /// </summary>
        Undefined,
        /// <summary>
        ///  The supplied list of insrefs should be concatenated to the current value.
        /// </summary>
        Add,
        /// <summary>
        ///  The supplied list of insrefs should be removed from the current value.
        /// </summary>
        Remove,
        /// <summary>
        /// The supplied list is the current value, that is it should replace the current value.
        /// </summary>
        Replace,
    }
}