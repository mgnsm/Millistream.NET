using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Millistream.Streaming.DataTypes.Formatting
{
    internal ref struct ValueStringBuilder
    {
        private char[] _arrayToReturnToPool;

        public ValueStringBuilder(Span<char> initialBuffer)
        {
            _arrayToReturnToPool = null;
            RawChars = initialBuffer;
            Length = 0;
        }

        public ValueStringBuilder(int initialCapacity)
        {
            _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
            RawChars = _arrayToReturnToPool;
            Length = 0;
        }

        public int Length { get; set; }

        public int Capacity => RawChars.Length;

        public void EnsureCapacity(int capacity)
        {
            if (capacity > RawChars.Length)
                Grow(capacity - Length);
        }

        /// <summary>
        /// Get a pinnable reference to the builder.
        /// Does not ensure there is a null char after <see cref="Length"/>
        /// This overload is pattern matched in the C# 7.3+ compiler so you can omit
        /// the explicit method call, and write eg "fixed (char* c = builder)"
        /// </summary>
        public ref char GetPinnableReference() => ref MemoryMarshal.GetReference(RawChars);

        /// <summary>
        /// Get a pinnable reference to the builder.
        /// </summary>
        /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
        public ref char GetPinnableReference(bool terminate)
        {
            if (terminate)
            {
                EnsureCapacity(Length + 1);
                RawChars[Length] = '\0';
            }
            return ref MemoryMarshal.GetReference(RawChars);
        }

        public ref char this[int index] => ref RawChars[index];

        public override string ToString()
        {
            string s = RawChars.Slice(0, Length).ToString();
            Dispose();
            return s;
        }

        /// <summary>Returns the underlying storage of the builder.</summary>
        public Span<char> RawChars { get; private set; }

        /// <summary>
        /// Returns a span around the contents of the builder.
        /// </summary>
        /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
        public ReadOnlySpan<char> AsSpan(bool terminate)
        {
            if (terminate)
            {
                EnsureCapacity(Length + 1);
                RawChars[Length] = '\0';
            }
            return RawChars.Slice(0, Length);
        }

        public ReadOnlySpan<char> AsSpan() => RawChars.Slice(0, Length);
        public ReadOnlySpan<char> AsSpan(int start) => RawChars[start..Length];
        public ReadOnlySpan<char> AsSpan(int start, int length) => RawChars.Slice(start, length);

        public bool TryCopyTo(Span<char> destination, out int charsWritten)
        {
            if (RawChars.Slice(0, Length).TryCopyTo(destination))
            {
                charsWritten = Length;
                Dispose();
                return true;
            }
            else
            {
                charsWritten = 0;
                Dispose();
                return false;
            }
        }

        public void Insert(int index, char value, int count)
        {
            if (Length > RawChars.Length - count)
                Grow(count);

            int remaining = Length - index;
            RawChars.Slice(index, remaining).CopyTo(RawChars.Slice(index + count));
            RawChars.Slice(index, count).Fill(value);
            Length += count;
        }

        public void Insert(int index, string s)
        {
            if (s == null)
                return;

            int count = s.Length;

            if (Length > (RawChars.Length - count))
                Grow(count);

            int remaining = Length - index;
            RawChars.Slice(index, remaining).CopyTo(RawChars.Slice(index + count));
            s.AsSpan().CopyTo(RawChars.Slice(index));
            Length += count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c)
        {
            int pos = Length;
            if ((uint)pos < (uint)RawChars.Length)
            {
                RawChars[pos] = c;
                Length = pos + 1;
            }
            else
            {
                GrowAndAppend(c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string s)
        {
            if (s == null)
                return;

            int pos = Length;
            if (s.Length == 1 && (uint)pos < (uint)RawChars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
            {
                RawChars[pos] = s[0];
                Length = pos + 1;
            }
            else
            {
                AppendSlow(s);
            }
        }

        private void AppendSlow(string s)
        {
            int pos = Length;
            if (pos > RawChars.Length - s.Length)
                Grow(s.Length);

            s.AsSpan().CopyTo(RawChars.Slice(pos));
            Length += s.Length;
        }

        public void Append(char c, int count)
        {
            if (Length > RawChars.Length - count)
                Grow(count);

            Span<char> dst = RawChars.Slice(Length, count);
            for (int i = 0; i < dst.Length; i++)
                dst[i] = c;
            Length += count;
        }

        public unsafe void Append(char* value, int length)
        {
            int pos = Length;
            if (pos > RawChars.Length - length)
                Grow(length);

            Span<char> dst = RawChars.Slice(Length, length);
            for (int i = 0; i < dst.Length; i++)
                dst[i] = *value++;
            Length += length;
        }

        public void Append(ReadOnlySpan<char> value)
        {
            int pos = Length;
            if (pos > RawChars.Length - value.Length)
                Grow(value.Length);

            value.CopyTo(RawChars.Slice(Length));
            Length += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<char> AppendSpan(int length)
        {
            int origPos = Length;
            if (origPos > RawChars.Length - length)
                Grow(length);

            Length = origPos + length;
            return RawChars.Slice(origPos, length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void GrowAndAppend(char c)
        {
            Grow(1);
            Append(c);
        }

        /// <summary>
        /// Resize the internal buffer either by doubling current buffer size or
        /// by adding <paramref name="additionalCapacityBeyondPos"/> to
        /// <see cref="Length"/> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        /// Number of chars requested beyond current position.
        /// </param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Grow(int additionalCapacityBeyondPos)
        {
            char[] poolArray = ArrayPool<char>.Shared.Rent(Math.Max(Length + additionalCapacityBeyondPos, RawChars.Length * 2));

            RawChars.Slice(0, Length).CopyTo(poolArray);

            char[] toReturn = _arrayToReturnToPool;
            RawChars = _arrayToReturnToPool = poolArray;
            if (toReturn != null)
                ArrayPool<char>.Shared.Return(toReturn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            char[] toReturn = _arrayToReturnToPool;
            this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
            if (toReturn != null)
                ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}