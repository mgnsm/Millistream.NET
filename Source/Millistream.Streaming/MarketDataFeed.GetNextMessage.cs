using System;

namespace Millistream.Streaming
{
    public partial class MarketDataFeed<TCallbackUserData, TStatusCallbackUserData>
    {
        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="mref">The fetched message reference. This should match a <see cref="MessageReferences"/> value.</param>
        /// <param name="mclass">The fetched message class. This should match a <see cref="MessageClasses"/> value. The message class is normally only used internally and is supplied to the client for completeness and transparency. The client should under most circumstances only use the message reference in order to determine which message it has received.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="mref"/>, <paramref name="mclass"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <remarks>The corresponding native function is mdf_get_next_message.</remarks>
        public unsafe bool GetNextMessage(out int mref, out int mclass, out ulong insref)
        {
            mref = default;
            mclass = default;
            insref = default;
            return _nativeImplementation.mdf_get_next_message(_feedHandle, ref mref, ref mclass, ref insref) == 1;
        }

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="mref">The fetched message reference. This should match a <see cref="MessageReferences"/> value.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="mref"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <remarks>The corresponding native function is mdf_get_next_message2. If this function isn't included in the installed version of the native library, the mdf_get_next_message function will be called instead.</remarks>
        public unsafe bool GetNextMessage(out ushort mref, out ulong insref)
        {
            mref = default;
            insref = default;

            if (_nativeImplementation.mdf_get_next_message2 == default)
            {
                bool ret = GetNextMessage(out int messageReference, out _, out insref);
                if (messageReference >= ushort.MinValue && messageReference <= ushort.MaxValue)
                    mref = (ushort)messageReference;
                return ret;
            }

            return _nativeImplementation.mdf_get_next_message2(_feedHandle, ref mref, ref insref) == 1;
        }

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="messageReference">The fetched message reference.</param>
        /// <param name="messageClasses">The fetched message class(es). The message class is normally only used internally and is supplied to the client for completeness and transparency. The client should under most circumstances only use the message reference in order to determine which message it has received.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="messageReference"/>, <paramref name="messageClasses"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <exception cref="InvalidOperationException">An unknown/undefined message reference was fetched.</exception>
        /// <remarks>The corresponding native function is mdf_get_next_message.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the GetNextMessage(out int mref, out int mclass, out ulong insref) overload instead.")]
        public bool GetNextMessage(out MessageReference messageReference, out MessageClasses messageClasses, out ulong insref)
        {
            bool ret = GetNextMessage(out int mref, out int mclass, out insref);
            switch (ret)
            {
                case true:
                    if (!s_messageReferences.Contains(mref))
                        throw new InvalidOperationException($"{mref} is an unknown message reference.");
                    messageReference = (MessageReference)mref;
                    messageClasses = (MessageClasses)mclass;
                    break;
                default:
                    messageReference = default;
                    messageClasses = default;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Fetches a message from the current consumed data if one is present and fills the output parameters with values representing the message fetched.
        /// </summary>
        /// <param name="messageReference">The fetched message reference.</param>
        /// <param name="insref">The fetched instrument reference, which is the unique id of an instrument.</param>
        /// <returns><see langword="true" /> if a message was returned (and the <paramref name="messageReference"/> and <paramref name="insref"/> fields will be filled) or <see langword="false" /> if there are no more messages in the current consumed data (or an error occured).</returns>
        /// <exception cref="InvalidOperationException">An unknown/undefined message reference was fetched.</exception>
        /// <remarks>The corresponding native function is mdf_get_next_message.</remarks>
        [Obsolete("This overload is deprecated and will be removed in a future version. Use the GetNextMessage(out ushort mref, out ulong insref) overload instead.")]
        public bool GetNextMessage(out MessageReference messageReference, out ulong insref)
        {
            bool ret = GetNextMessage(out ushort mref, out insref);
            switch (ret)
            {
                case true:
                    if (!s_messageReferences.Contains(mref))
                        throw new InvalidOperationException($"{mref} is an unknown message reference.");
                    messageReference = (MessageReference)mref;
                    break;
                default:
                    messageReference = default;
                    break;
            }
            return ret;
        }
    }
}