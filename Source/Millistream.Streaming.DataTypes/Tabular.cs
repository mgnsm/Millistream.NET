using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a tabular of pipe (|) separated rows of space separated columns.
    /// </summary>
    public readonly struct Tabular : IEquatable<Tabular>
    {
        #region Constants
        private const char RowSeparator = '|';
        #endregion

        #region Fields
        private readonly List<TabularRow> _data;
        #endregion

        #region Constructors
        private Tabular(List<TabularRow> data)
        {
            _data = data;
            Rows = data.Count;

            //set the Columns property to Cells.Count of the TabularRow with the largest number of cells
            //(in case the rows have a different number of cells for some reason)
            int largestNumberOfCells = 0;
            foreach (TabularRow tabularRow in data)
                if (tabularRow.Cells != null && tabularRow.Cells.Count > largestNumberOfCells)
                    largestNumberOfCells = tabularRow.Cells.Count;
            Columns = largestNumberOfCells;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of columns of the <see cref="Tabular"/>.
        /// </summary>
        public readonly int Columns { get; }

        /// <summary>
        /// Gets the number of rows of the <see cref="Tabular"/>.
        /// </summary>
        public readonly int Rows { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets a column value at a specified row and column index.
        /// </summary>
        /// <param name="row">The zero-based row index.</param>
        /// <param name="column">The zero-based column index.</param>
        /// <returns>A memory span that contains a UTF-8 string representation of the column value.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ReadOnlyMemory<char> GetData(int row, int column)
        {
            if (row < 0 || row > Rows - 1)
                throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column > Columns - 1)
                throw new ArgumentOutOfRangeException(nameof(column));

            TabularRow tabularRow = _data[row];
            return (tabularRow.Cells == null || column > tabularRow.Cells.Count - 1)
                ? ReadOnlyMemory<char>.Empty : tabularRow.Cells[column];
        }

        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a tabular of pipe (|) separated rows of space separated columns (e.g. "1 2 3|4 5 6") to its <see cref="Tabular"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Tabular"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        public static Tabular Parse(ReadOnlySpan<char> value)
        {
            const char EscapeChar = '\\';

            List<TabularRow> rows = new List<TabularRow>();
            char[] data = value.ToArray();

            int offset = 0;
            int index;
            while ((index = value.IndexOf(RowSeparator, EscapeChar)) != -1)
            {
                ReadOnlySpan<char> rowData = value.Slice(0, index);
                rows.Add(new TabularRow(rowData, data, offset));
                int start = index + 1;
                offset += start;
                value = value.Slice(start);
            }

            if (value.Length > 0)
                rows.Add(new TabularRow(value.Slice(0), data, offset));

            return new Tabular(rows);
        }

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a tabular of pipe (|) separated rows of space separated columns (e.g. "1 2 3|4 5 6") to its <see cref="Tabular"/> equivalent.
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Tabular"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        public static Tabular Parse(ReadOnlySpan<byte> value)
        {
            Span<char> chars = stackalloc char[value.Length];
            Encoding.UTF8.GetChars(value, chars);
            return Parse(chars);
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified <see cref="Tabular"/> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>true if the <paramref name="other"/> parameter equals the value of this instance; otherwise, false.</returns>
        public readonly bool Equals(Tabular other) => 
            Columns == other.Columns && Rows == other.Rows
                && ((_data == null && other._data == null)
                    || (_data != null && other._data != null && _data.SequenceEqual(other._data)));

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int hash = HashCode.Combine(Columns, Rows);
            if (_data != null)
                foreach (TabularRow tabularRow in _data)
                    hash = HashCode.Combine(hash, tabularRow.GetHashCode());
            return hash;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if value is an instance of <see cref="Tabular"/> and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object obj) => obj is Tabular tabular && Equals(tabular);

        /// <summary>
        /// Converts the value of the current <see cref="Tabular"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="Tabular"/> object.</returns>
        public readonly override string ToString() => 
            _data != null ? string.Join(RowSeparator, _data.Select(x => x.ToString())) 
                : string.Empty;
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Tabular"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Tabular left, Tabular right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Tabular"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Tabular left, Tabular right) => !left.Equals(right);
        #endregion
    }
}