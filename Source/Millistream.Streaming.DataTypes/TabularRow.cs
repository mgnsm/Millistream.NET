using System;
using System.Collections.Generic;
using System.Linq;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents a pipe (|) separated row of space separated columns.
    /// </summary>
    internal readonly struct TabularRow : IEquatable<TabularRow>
    {
        #region Constants
        private const char ColumnSeparator = ' ';
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="TabularRow"/>.
        /// </summary>
        /// <param name="rowData">A memory span that contains the space separated column values.</param>
        /// <param name="tabularData">An array that contains pipe (|) separated rows of space separated columns.</param>
        /// <param name="offset">The start index of the pipe separated row in <paramref name="tabularData"/>.</param>
        internal TabularRow(ReadOnlySpan<char> rowData, char[] tabularData, int offset)
        {
            const char EscapeChar = '\\';
            List<ReadOnlyMemory<char>> cells = null;
            if (tabularData != null && tabularData.Length > 0)
            {
                cells = new List<ReadOnlyMemory<char>>();
                int index;
                while ((index = rowData.IndexOf(ColumnSeparator, EscapeChar)) != -1)
                {
                    int start = index + 1;
                    if (index > 0 || rowData.Length == 1)
                    {
                        cells.Add(new ReadOnlyMemory<char>(tabularData, offset, index));
                        while (start < rowData.Length - 1 && rowData[start] == ColumnSeparator)
                        {
                            cells.Add(ReadOnlyMemory<char>.Empty);
                            start++;
                        }
                    }
                    offset += start;
                    rowData = rowData.Slice(start);
                }

                if (rowData.Length > 0)
                    cells.Add(new ReadOnlyMemory<char>(tabularData, offset, rowData.Length));
            }
            Cells = cells;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the space separated column values of the <see cref="TabularRow"/>.
        /// </summary>
        internal readonly IReadOnlyList<ReadOnlyMemory<char>> Cells { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified <see cref="TabularRow"/> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>true if the <paramref name="other"/> parameter equals the value of this instance; otherwise, false.</returns>
        public bool Equals(TabularRow other)
        {
            if (Cells == null && other.Cells == null)
                return true;
            if (Cells == null || other.Cells == null || Cells.Count != other.Cells.Count)
                return false;

            for (int i = 0; i < Cells.Count; ++i)
            {
                ReadOnlySpan<char> cellValue = Cells[i].Span;
                ReadOnlySpan<char> otherCellValue = other.Cells[i].Span;
                for (int j = 0; j < cellValue.Length; ++j)
                    if (cellValue[j] != otherCellValue[j])
                        return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            int hash = 0;
            if (Cells != null)
            {
                foreach (ReadOnlyMemory<char> cellValue in Cells)
                {
                    ReadOnlySpan<char> span = cellValue.Span;
                    for (int i = 0; i < cellValue.Span.Length; i++)
                        hash = HashCode.Combine(hash, span[i].GetHashCode());
                }
            }
            return hash;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if value is an instance of <see cref="TabularRow"/> and equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object obj) => obj is TabularRow tabularRow && Equals(tabularRow);

        /// <summary>
        /// Converts the value of the current <see cref="TabularRow"/> object to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="TabularRow"/> object.</returns>
        public readonly override string ToString() =>
            Cells != null ? string.Join(ColumnSeparator, Cells.Select(x => x.Span.ToString()))
                : string.Empty;
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="TabularRow"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(TabularRow left, TabularRow right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="TabularRow"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(TabularRow left, TabularRow right) => !left.Equals(right);
        #endregion
    }
}