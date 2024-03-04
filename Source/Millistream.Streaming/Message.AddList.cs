using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_list.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddList(uint tag, string value) =>
            _nativeImplementation.mdf_message_add_list_str(_handle, tag, value) == 1;

        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_list.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddList(uint tag, string value) overload instead.")]
        public bool AddList(Field tag, string value) =>
            AddList((uint)tag, value);

        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_list.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool AddList(uint tag, ReadOnlySpan<byte> value)
        {
            fixed (byte* bytes = value)
                return _nativeImplementation.mdf_message_add_list(Handle, tag, (IntPtr)bytes) == 1;
        }

        /// <summary>
        /// Adds a list field to the current active message. A list field is a space separated list of instrument references. The first position in the value can be:
        /// <para>'+'   (the supplied list should be added to the current value)<br/>
        /// '-' (the supplied list should be removed from the current value)<br/>
        /// '=' (the supplied list is the current value)</para>
        /// If there is no such prefix it is interpreted as if it was prefixed with a '='.  There is a current soft limit of 1.000.000 instrument references per list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The list field value as a memory span that contains a null-terminated sequence of UTF-8 encoded bytes.</param>
        /// <returns><see langword="true" /> if the field was successfully added, or <see langword="false" /> if the value could not be added (because there was no more memory, the message handle does not contain any messages, or the supplied value is not of the type specified).</returns>
        /// <remarks>The corresponding native function is mdf_message_add_list.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the AddList(uint tag, ReadOnlySpan<byte> value) overload instead.")]
        public bool AddList(Field tag, ReadOnlySpan<byte> value) =>
            AddList((uint)tag, value);
    }
}