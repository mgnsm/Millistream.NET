using System;
using System.Runtime.CompilerServices;

namespace Millistream.Streaming.DataTypes.Formatting
{
    internal unsafe ref struct NumberBuffer
    {
        public int DigitsCount;
        public int Scale;
        public bool IsNegative;
        public bool HasNonZeroTail;
        public NumberBufferKind Kind;
        public Span<byte> Digits;

        internal NumberBuffer(NumberBufferKind kind, byte* digits, int digitsLength)
        {
            DigitsCount = 0;
            Scale = 0;
            IsNegative = false;
            HasNonZeroTail = false;
            Kind = kind;
            Digits = new Span<byte>(digits, digitsLength);
            Digits[0] = (byte)('\0');
        }

        internal byte* GetDigitsPointer() => (byte*)Unsafe.AsPointer(ref Digits[0]);
    }
}