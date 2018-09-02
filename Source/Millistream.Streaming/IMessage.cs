using System;

namespace Millistream.Streaming
{
    /// <summary>
    /// Represents a message to which you can add fields of different types.
    /// </summary>
    internal interface IMessage
    {
        /// <summary>
        /// Adds a new message to the native message handle. The corresponding native method is mdf_message_add.
        /// </summary>
        /// <param name="instrumentReference">The reference for the instrument for which the message is created for.</param>
        /// <param name="messageReference">The type of the message to create.</param>
        /// <returns>Returns true if a new message was added to the message handle or false if there was an error.</returns>
        bool Add(ulong instrumentReference, MessageReference messageReference);

        /// <summary>
        /// Adds a date field to the message. Please note that all dates and times are expressed in UTC. Also note that the time part of the date will be ignored. You should use any of the AddTime* methods to add a separate time field to the message. The corresponding native method is mdf_message_add_date.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The date.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddDate(Field tag, DateTime value);

        /// <summary>
        /// Adds a list field to the message. A list field is a list of instrument references. The corresponding native method is mdf_message_add_list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="instrumentReferences">The list of instrument references.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddInstrumentReferences(Field tag, ulong[] instrumentReferences);

        /// <summary>
        /// Adds a scaled signed 64-bit integer field to the message. decimals can be between 0 and 19. A value of 12345 with decimals set to 2 will be encoded as "123.45". The corresponding native method is mdf_message_add_int.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled signed 64-bit integer.</param>
        /// <param name="decimals">The number of decimals. Can be between 0 and 19.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddInt64(Field tag, long value, sbyte decimals);

        /// <summary>
        /// Adds a list of request classes to the message. The corresponding native method is mdf_message_add_list.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="instrumentReferences">The list of request classes.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddRequestClasses(RequestClass[] requestClasses);

        /// <summary>
        /// Adds a UTF-8 string field to the message. The corresponding native method is mdf_message_add_string.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">A UTF-8 string.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddString(Field tag, string value);

        /// <summary>
        /// Adds a time field to the current active message. Please note that all times and dates are expressed in UTC. The corresponding native method is mdf_message_add_time2.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The time.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddTime(Field tag, TimeSpan value);

        /// <summary>
        /// Adds an unsigned 32-bit integer field to the message. The corresponding native method is mdf_message_add_numeric.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The unsigned 32-bit integer.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddUInt32(Field tag, uint value);

        /// <summary>
        /// Adds a scaled unsigned 64-bit integer field to the message. decimals can be between 0 and 19. A value of 12345 with decimals set to 2 will be encoded as "123.45". The corresponding native method is mdf_message_add_uint.
        /// </summary>
        /// <param name="tag">The field tag.</param>
        /// <param name="value">The scaled unsigned 64-bit integer.</param>
        /// <param name="decimals">The number of decimals. Can be between 0 and 19.</param>
        /// <returns>Returns true if the field was successfully added, or false if the value could not be added for some reason.</returns>
        bool AddUInt64(Field tag, ulong value, sbyte decimals);
    }
}
