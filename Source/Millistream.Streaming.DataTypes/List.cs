using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a list of instrument references (type: <see cref="InsRef"/>). 
    /// </summary>
    public readonly struct List : IEnumerable<InsRef>, IEnumerable, IReadOnlyCollection<InsRef>, IReadOnlyList<InsRef>, IEquatable<List>
    {
        #region Constants
        private const char AddPrefix = '+';
        private const char RemovePrefix = '-';
        private const char ReplacePrefix = '=';
        private const char SpacePrefix = ' ';
        #endregion

        #region Fields
        private readonly IEnumerable<InsRef> _instrumentReferences;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="List"/>.
        /// </summary>
        /// <param name="prefix">A value that determines whether the instrument references in <paramref name="instrumentReferences"/> should be concatenated to a current value, removed from a current value or replace a current value.</param>
        /// <param name="instrumentReferences">A sequence of instrument references that belongs to the <see cref="List"/>.</param>
        public List(ListPrefix prefix, IEnumerable<InsRef> instrumentReferences)
        {
            _instrumentReferences = instrumentReferences ?? throw new ArgumentNullException(nameof(instrumentReferences));
            Prefix = prefix;

            PrefixCharacter = prefix switch
            {
                ListPrefix.Add => AddPrefix,
                ListPrefix.Remove => RemovePrefix,
                ListPrefix.Replace => ReplacePrefix,
                _ => default(char?),
            };

            if (instrumentReferences is ICollection<InsRef> collection)
            {
                Count = collection.Count;
            }
            else
            {
                int count = 0;
                using (IEnumerator<InsRef> enumerator = instrumentReferences.GetEnumerator())
                    while (enumerator?.MoveNext() == true)
                        count++;
                Count = count;
            }
        }

        /// <summary>
        /// Creates an instance of a <see cref="List"/>.
        /// </summary>
        /// <param name="instrumentReferences">A sequence of instrument references that belongs to the <see cref="List"/>.</param>
        public List(IEnumerable<InsRef> instrumentReferences)
            : this(ListPrefix.Undefined, instrumentReferences) { }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of <see cref="InsRef"/> values contained in the <see cref="List"/>.
        /// </summary>
        public readonly int Count { get; }

        /// <summary>
        /// Gets the prefix that determines whether the instrument references in the <see cref="List"/> should be concatenated to a current value, removed from a current value or replace a current value.
        /// </summary>
        public readonly ListPrefix Prefix { get; }

        /// <summary>
        /// Gets the prefix character that determines whether the instrument references in the <see cref="List"/> should be concatenated to a current value, removed from a current value or replace a current value.
        /// </summary>
        public readonly char? PrefixCharacter { get; }

        /// <summary>
        /// Gets the <see cref="InsRef"/> element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="InsRef"/> to get.</param>
        /// <returns>The <see cref="InsRef"/> element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public readonly InsRef this[int index]
        {
            get
            {
                if (_instrumentReferences == null)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _instrumentReferences.ElementAt(index);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a space separated list of instrument references to its <see cref="List"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="List"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static List Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out List @list) ? @list : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a space separated list of instrument references to its <see cref="List"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="List"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static List Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out List @list) ? @list : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a space separated list of instrument references to its <see cref="List"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="list">The converted <see cref="List"/> value or default depending on whether the conversion succeeded or failed.</param>
        /// <returns>true if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out List @list)
        {
            //trim leading spaces
            while (value.Length > 0 && value[0] == SpacePrefix)
                value = value.Slice(1);

            ListPrefix prefix = ListPrefix.Undefined;
            if (value.Length > 0)
            {
                switch (value[0])
                {
                    case AddPrefix:
                        prefix = ListPrefix.Add;
                        value = value.Slice(1);
                        break;
                    case RemovePrefix:
                        prefix = ListPrefix.Remove;
                        value = value.Slice(1);
                        break;
                    case ReplacePrefix:
                        prefix = ListPrefix.Replace;
                        value = value.Slice(1);
                        break;
                }
            }

            @list = default;

            List<InsRef> instrumentReferences = new List<InsRef>();
            int index;
            while ((index = value.IndexOf(SpacePrefix)) != -1 && index < value.Length - 1)
            {
                ReadOnlySpan<char> longChars = value.Slice(0, index);
                if (!InsRef.TryParse(longChars, out InsRef insRef))
                    return false;
                instrumentReferences.Add(insRef);
                value = value.Slice(index + 1);
            }

            if (value.Length > 0)
            {
                if (!InsRef.TryParse(value, out InsRef insRef))
                    return false;
                instrumentReferences.Add(insRef);
            }

            if (instrumentReferences.Count == 0)
                return false;

            @list = new List(prefix, instrumentReferences);
            return true;
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a space separated list of instrument references to its <see cref="List"/> equivalent and returns a value that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="list">The converted <see cref="List"/> value or default depending on whether the conversion succeeded or failed.</param>
        /// <returns>true if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out List @list)
        {
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            return TryParse(chars, out @list);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List"/>.
        /// </summary>
        /// <returns> A enumerator for the <see cref="List"/>.</returns>
        public readonly IEnumerator<InsRef> GetEnumerator() => _instrumentReferences?.GetEnumerator();

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified <see cref="List"/> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>true if the <paramref name="other"/> parameter equals the value of this instance; otherwise, false.</returns>
        public readonly bool Equals(List other) => Count == other.Count
            && Prefix == other.Prefix
            && ((_instrumentReferences == null && other._instrumentReferences == null)
            || (_instrumentReferences != null
                && other._instrumentReferences != null 
                && _instrumentReferences.SequenceEqual(other._instrumentReferences)));

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode()
        {
            int hash = HashCode.Combine(Count, (int)Prefix);
            if (_instrumentReferences != null)
                foreach (InsRef insRef in _instrumentReferences)
                    hash = HashCode.Combine(hash, unchecked((int)(ulong)insRef));
            return hash;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if value is an instance of <see cref="List"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is List list && Equals(list);

        /// <summary>
        /// Converts the current <see cref="List"/> object to a space separated string of instrument references.
        /// </summary>
        /// <returns>A space separated string of instrument reference.</returns>
        public readonly override string ToString() => _instrumentReferences != null ? 
            $"{PrefixCharacter}{string.Join(SpacePrefix, _instrumentReferences)}" : string.Empty;

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="List"/>.
        /// </summary>
        /// <returns> A enumerator for the <see cref="List"/>.</returns>
        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="List"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(List left, List right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="List"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(List left, List right) => !left.Equals(right);
        #endregion
    }
}