using Millistream.Streaming.Interop;
using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a managed message handle (mdf_message_t) that can contain several messages for efficiency.
    /// </summary>
    /// <remarks>Handles are not thread-safe. If multiple threads will share access to a single handle, the accesses has to be serialized using a mutex or other forms of locking mechanisms. The API as such is thread-safe so multiple threads can have local handles without the need for locks.</remarks>
    public sealed unsafe partial class Message : IMessage, IDisposable
    {
        private readonly NativeImplementation _nativeImplementation;
#pragma warning disable CS0618
        private CompressionLevel _compressionLevel = CompressionLevel.Z_BEST_SPEED;
#pragma warning restore CS0618
        private bool _utf8Validation = true;
        private byte _delay;
        private IntPtr _handle;

        /// <summary>
        /// Creates an instance of the <see cref="Message"/> class.
        /// </summary>
        /// <exception cref="DllNotFoundException">The native dependency is missing.</exception>
        /// <remarks>The corresponding native function is mdf_message_create.</remarks>
        public Message()
            : this(default, false) { }

        /// <summary>
        /// Creates an instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="nativeLibraryPath">The path of the native dependency.</param>
        /// <exception cref="ArgumentNullException"><paramref name="nativeLibraryPath"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="DllNotFoundException">The native dependency can't be found.</exception>
        /// <remarks>The corresponding native function is mdf_message_create.</remarks>
        public Message(string nativeLibraryPath)
            : this(nativeLibraryPath, true) { }

        internal Message(NativeImplementation nativeImplementation)
        {
            _nativeImplementation = nativeImplementation ?? throw new ArgumentNullException(nameof(nativeImplementation));
            _handle = _nativeImplementation.mdf_message_create();
        }

        private Message(string nativeLibraryPath, bool validateArgument)
        {
            _nativeImplementation = string.IsNullOrEmpty(nativeLibraryPath) ?
                (validateArgument ? throw new ArgumentNullException(nameof(nativeLibraryPath)) : NativeImplementation.Default)
                : new NativeImplementation(nativeLibraryPath);
            _handle = _nativeImplementation.mdf_message_create();
        }

        ~Message() => _nativeImplementation?.mdf_message_destroy(_handle);

        /// <summary>
        /// Gets or sets the zlib compression level used for the <see cref="AddString(uint, string)"/> and <see cref="AddString(uint, string, int)"/> methods.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_message_set_property with an option of <see cref="MDF_MSG_OPTION.MDF_MSG_OPT_COMPRESSION"/> or mdf_message_set_compression_level.</remarks>
        [Obsolete("The CompressionLevel enumeration is deprecated and will be removed in a future version. The type of this property will then be changed to System.Byte.")]
        public CompressionLevel CompressionLevel
        {
            get => _compressionLevel;
            set
            {
                if (_nativeImplementation.mdf_message_set_property != default)
                {
                    if (_nativeImplementation.mdf_message_set_property(_handle, MDF_MSG_OPTION.MDF_MSG_OPT_COMPRESSION, (int)value) == 1)
                        _compressionLevel = value;
                }
                else if (_nativeImplementation.mdf_message_set_compression_level != default)
                {
                    if (_nativeImplementation.mdf_message_set_compression_level(_handle, (int)value) == 1)
                        _compressionLevel = value;
                }
            }
        }

        /// <summary>
        /// Gets the total number of messages in the message handle (the number of active + the number of reused messages currently not used for active messages).
        /// </summary>
        /// <remarks>The corresponding native function is mdf_message_get_num.</remarks>
        public int Count => _nativeImplementation.mdf_message_get_num(_handle);

        /// <summary>
        /// Gets the number of active messages in the message handle.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_message_get_num_active.</remarks>
        public int ActiveCount => _nativeImplementation.mdf_message_get_num_active(_handle);

        /// <summary>
        /// Gets the number of added fields to the current message.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_message_get_num_fields.</remarks>
        public int FieldCount => _nativeImplementation.mdf_message_get_num_fields != default ?
            _nativeImplementation.mdf_message_get_num_fields(_handle) : default;

        /// <summary>
        /// Enables or disables the UTF-8 validation performed in <see cref="AddString(uint, string)"/> and <see cref="AddString(uint, string, int)"/>. It's enabled by default.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_message_set_property with an option of <see cref="MDF_MSG_OPTION.MDF_MSG_OPT_UTF8"/> or mdf_message_set_utf8_validation.</remarks>
        public bool Utf8Validation
        {
            get => _utf8Validation;
            set
            {
                if (_nativeImplementation.mdf_message_set_property != default)
                {
                    if (_nativeImplementation.mdf_message_set_property(_handle, MDF_MSG_OPTION.MDF_MSG_OPT_UTF8, value ? 1 : 0) == 1)
                        _utf8Validation = value;
                }
                else if (_nativeImplementation.mdf_message_set_utf8_validation != default)
                {
                    if (_nativeImplementation.mdf_message_set_utf8_validation(_handle, value ? 1 : 0) == 1)
                        _utf8Validation = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the intended delay of the message. The default value is 0.
        /// </summary>
        /// <remarks>The corresponding native function for setting the value is mdf_message_set_property with an option of <see cref="MDF_MSG_OPTION.MDF_MSG_OPT_DELAY"/>.</remarks>
        public byte Delay
        {
            get => _delay;
            set
            {
                if (_nativeImplementation.mdf_message_set_property != default
                    && _nativeImplementation.mdf_message_set_property(_handle, MDF_MSG_OPTION.MDF_MSG_OPT_DELAY, value) == 1)
                    _delay = value;
            }
        }

        internal IntPtr Handle
        {
            get => _handle;
            private set => _handle = value;
        }

        /// <summary>
        /// Resets the message handle (sets the number of active messages to zero) so it can be reused. The memory allocated for the current messages in the handle is retained for performance reasons and will be reused when you add new messages to the handle.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_message_reset.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset() => _nativeImplementation.mdf_message_reset(_handle);

        /// <summary>
        /// Removes the current active message from the message handle and all the fields that you have added for this message. Points the current message at the previous message in the message handle if it exists, so repeated calls will reset the whole message handle just like <see cref="Reset()"/> had been called.
        /// </summary>
        /// <returns><see langword="true" /> if there are more active messages in the message handle or <see langword="false" /> if the message handle is now empty.</returns>
        /// <remarks>The corresponding native function is mdf_message_del.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Delete() => _nativeImplementation.mdf_message_del(_handle) == 1;

        /// <summary>
        /// Destroys the message handle and frees all allocated memory.
        /// </summary>
        /// <remarks>The corresponding native function is mdf_message_destroy.</remarks>
        public void Dispose()
        {
            _nativeImplementation.mdf_message_destroy(_handle);
            _handle = default;
            GC.SuppressFinalize(this);
        }
    }
}