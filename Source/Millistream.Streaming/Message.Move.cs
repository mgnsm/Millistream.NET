using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    public partial class Message
    {
        /// <summary>
        /// Moves all messages from <paramref name="source"/> with an insref matching <paramref name="sourceInsref"/> to <paramref name="destination"/> and changes the insref to <paramref name="destinationInsRef"/>. If <paramref name="destination"/> is set to the same message handle as <paramref name="source"/> or if <paramref name="destination"/> is <see langword="null" />, then the change from <paramref name="sourceInsref"/> to <paramref name="destinationInsRef"/> will be done in-place in <paramref name="source"/>.
        /// <para>If both <paramref name="sourceInsref"/> and <paramref name="destinationInsRef"/> is set to <see cref="ulong.MaxValue"/> then the insrefs in <paramref name="source"/> will not be changed and all the messages regardless of insref will be moved to <paramref name="destination"/>.</para>
        /// </summary>
        /// <param name="source">The message handle to move messages from.</param>
        /// <param name="destination">The message handle to move messages to.</param>
        /// <param name="sourceInsref">The instrument reference of the messages to be moved.</param>
        /// <param name="destinationInsRef">The new instrument reference of the moved or modified messages.</param>
        /// <returns><see langword="true" /> if the operation was successfull, or <see langword="false" /> if it failed.</returns>
        /// <remarks>The corresponding native function is mdf_message_move.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool Move(Message source, Message destination, ulong sourceInsref, ulong destinationInsRef) => source != null
            && source._nativeImplementation.mdf_message_move != default
            && source._nativeImplementation.mdf_message_move(source._handle, destination?._handle ?? IntPtr.Zero, sourceInsref, destinationInsRef) == 1;
    }
}