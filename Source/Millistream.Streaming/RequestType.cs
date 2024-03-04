using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Request Types
    /// </summary>
    [Obsolete("This enumeration is deprecated and has been replaced by the constants in the RequestTypes class.")]
    public enum RequestType : uint
    {
        MDF_RT_IMAGE = 1,
        MDF_RT_STREAM,
        MDF_RT_FULL
    }
}