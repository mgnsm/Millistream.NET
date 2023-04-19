using System;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// Fetches the next field from the current message.
        /// </summary>
        /// <param name="tag">The field tag. This should match a <see cref="Field"/> value.</param>
        /// <param name="value">A memory span that contains the bytes of the UTF-8 string representation of the field value.</param>
        /// <returns><see langword="true" /> if a field was returned, or <see langword="false" /> if there are no more fields in the current message.</returns>
        /// <remarks>The corresponding native function is mdf_get_next_field.</remarks>
        public unsafe bool GetNextField(out uint tag, out ReadOnlySpan<byte> value)
        {
            tag = default;
            IntPtr pointer = default;

            int ret = _nativeImplementation.mdf_get_next_field(_feedHandle, ref tag, ref pointer);
            if (ret != 1)
            {
                value = default;
                return false;
            }

            if (pointer != IntPtr.Zero)
            {
                unsafe
                {
                    byte* p = (byte*)pointer;
                    int fieldOffset = 0;
                    while (*(p + fieldOffset++) != 0) ;
                    value = new ReadOnlySpan<byte>(p, fieldOffset - 1);
                }
            }
            else
            {
                value = default;
            }

            return true;
        }

        /// <summary>
        /// Fetches the next field from the current message.
        /// </summary>
        /// <param name="field">The field tag.</param>
        /// <param name="value">A memory span that contains the bytes of the UTF-8 string representation of the field value.</param>
        /// <returns><see langword="true" /> if a field was returned, or <see langword="false" /> if there are no more fields in the current message.</returns>
        /// <exception cref="InvalidOperationException">An unknown/undefined field tag was fetched.</exception>
        /// <remarks>The corresponding native function is mdf_get_next_field.</remarks>
        public bool GetNextField(out Field field, out ReadOnlySpan<byte> value)
        {
            bool ret = GetNextField(out uint tag, out value);
            switch (ret)
            {
                case true:
                    if (!s_fields.Contains(tag))
                        throw new InvalidOperationException($"{tag} is an unknown tag / field.");
                    field = (Field)tag;
                    break;
                default:
                    field = default;
                    break;
            }
            return ret;
        }
    }
}