using System;
using System.Diagnostics;
using System.Text;

namespace Millistream.Streaming.DataTypes
{
    /// <summary>
    /// Represents in a date in one of the following formats: YYYY-MM-DD, YYYY-MM, YYYY, YYYY-Qx (where x is a quarter between 1 and 4), YYYY-Tx (where x is a tertiary between 1 and 3), YYYY-Hx (where x is a semi-annual between 1 and 2), and YYYY-Wxx (where x is a ISO-8601 week number between 1 and 53).
    /// </summary>
    public readonly struct Date : IEquatable<Date>
    {
        #region Constructors
        /// <summary>
        /// Creates an instance of a <see cref="Date"/> with a format of YYYY-MM-DD (<see cref="DateFormat.Day"/>).
        /// </summary>
        /// <param name="day">A specific day of a specific month of a specific year.</param>
        public Date(Day day)
        {
            Day = day;
            Month = day.Month;
            Quarter = default;
            SemiAnnual = default;
            Tertiary = default;
            Year = day.Year;
            Week = default;
            Format = DateFormat.Day;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Date"/> with a format of YYYY-MM (<see cref="DateFormat.Month"/>).
        /// </summary>
        /// <param name="month">A specific month of a specific year.</param>
        public Date(Month month)
        {
            Day = default;
            Month = month;
            Quarter = default;
            SemiAnnual = default;
            Tertiary = default;
            Year = month.Year;
            Week = default;
            Format = DateFormat.Month;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Date"/> with a format of YYYY-Qx (<see cref="DateFormat.Quarter"/>) where x is a quarter between 1 and 4.
        /// </summary>
        /// <param name="quarter">A specific quarter of a specific year.</param>
        public Date(Quarter quarter)
        {
            Day = default;
            Month = default;
            Quarter = quarter;
            SemiAnnual = default;
            Tertiary = default;
            Year = quarter.Year;
            Week = default;
            Format = DateFormat.Quarter;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Date"/> with a format of YYYY-Hx (<see cref="DateFormat.SemiAnnual"/>) where x is a semi-annual between 1 and 2. 
        /// </summary>
        /// <param name="semiAnnual">A specific semi-annual of a specific year.</param>
        public Date(SemiAnnual semiAnnual)
        {
            Day = default;
            Month = default;
            Quarter = default;
            SemiAnnual = semiAnnual;
            Tertiary = default;
            Year = semiAnnual.Year;
            Week = default;
            Format = DateFormat.SemiAnnual;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Date"/> with a format of YYYY-Tx (<see cref="DateFormat.Tertiary"/>) where x is a tertiary between 1 and 3. 
        /// </summary>
        /// <param name="tertiary">A specific tertiary of a specific year.</param>
        public Date(Tertiary tertiary)
        {
            Day = default;
            Month = default;
            Quarter = default;
            SemiAnnual = default;
            Tertiary = tertiary;
            Year = tertiary.Year;
            Week = default;
            Format = DateFormat.Tertiary;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Date"/> with a format of YYYY (<see cref="DateFormat.Year"/>). 
        /// </summary>
        /// <param name="year">A specific year.</param>
        public Date(Year year)
        {
            Day = default;
            Month = default;
            Quarter = default;
            SemiAnnual = default;
            Tertiary = default;
            Year = year;
            Week = default;
            Format = DateFormat.Year;
        }

        /// <summary>
        /// Creates an instance of a <see cref="Date"/> with a format of YYYY-Wxx (<see cref="DateFormat.Week"/>) where x is a ISO-8601 week number between 1 and 53. 
        /// </summary>
        /// <param name="week">A specific week of a specific year.</param>
        public Date(Week week)
        {
            Day = default;
            Month = default;
            Quarter = default;
            SemiAnnual = default;
            Tertiary = default;
            Year = week.Year;
            Week = week;
            Format = DateFormat.Week;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the day of a <see cref="Date"/> with a format of YYYY-MM-DD (<see cref="DateFormat.Day"/>).
        /// </summary>
        public readonly Day? Day { get; }

        /// <summary>
        /// Gets the format of the <see cref="Date"/>.
        /// </summary>
        public readonly DateFormat Format { get; }

        /// <summary>
        /// Gets the <see cref="Month"/> of a <see cref="Date"/> with a format of YYYY-MM (<see cref="DateFormat.Month"/>).
        /// </summary>
        public readonly Month? Month { get; }

        /// <summary>
        /// Gets the <see cref="Quarter"/> of a <see cref="Date"/> with a format of YYYY-Qx (<see cref="DateFormat.Quarter"/>).
        /// </summary>
        public readonly Quarter? Quarter { get; }

        /// <summary>
        /// Gets the <see cref="SemiAnnual"/> of a <see cref="Date"/> with a format of YYYY-Hx (<see cref="DateFormat.SemiAnnual"/>).
        /// </summary>
        public readonly SemiAnnual? SemiAnnual { get; }

        /// <summary>
        /// Gets the <see cref="Tertiary"/> of a <see cref="Date"/> with a format of YYYY-Tx (<see cref="DateFormat.Tertiary"/>).
        /// </summary>
        public readonly Tertiary? Tertiary { get; }

        /// <summary>
        /// Gets the <see cref="Year"/> of the <see cref="Date"/>.
        /// </summary>
        public readonly Year Year { get; }

        /// <summary>
        /// Gets the <see cref="Week"/> of a <see cref="Date"/> with a format of YYYY-Wxx (<see cref="DateFormat.Week"/>).
        /// </summary>
        public readonly Week? Week { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Converts a memory span that contains a UTF-8 string representation of a date to its <see cref="Date"/> equivalent. Valid formats are YYYY-MM-DD, YYYY-MM, YYYY, YYYY-Qx (where x is a quarter between 1 and 4), YYYY-Tx (where t is a tertiary between 1 and 3), YYYY-Hx (where x is a semi-annual between 1 and 2) and YYYY-Wxx (where xx is a ISO-8601 week number between 1 and 53).
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Date"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Date Parse(ReadOnlySpan<char> value) =>
            TryParse(value, out Date date) ? date : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Converts a memory span that contains the bytes of a UTF-8 string representation of a date to its <see cref="Date"/> equivalent. Valid formats are YYYY-MM-DD, YYYY-MM, YYYY, YYYY-Qx (where x is a quarter between 1 and 4), YYYY-Tx (where t is a tertiary between 1 and 3), YYYY-Hx (where x is a semi-annual between 1 and 2) and YYYY-Wxx (where xx is a ISO-8601 week number between 1 and 53).
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <returns>A <see cref="Date"/> object that is equivalent to the value contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Date Parse(ReadOnlySpan<byte> value) =>
            TryParse(value, out Date date) ? date : throw new ArgumentException(Constants.ParseArgumentExceptionMessage, nameof(value));

        /// <summary>
        /// Tries to convert a memory span that contains a UTF-8 string representation of a date to its <see cref="Date"/> equivalent and returns a value that indicates whether the conversion succeeded. Valid formats are YYYY-MM-DD, YYYY-MM, YYYY, YYYY-Qx (where x is a quarter between 1 and 4), YYYY-Tx (where t is a tertiary between 1 and 3), YYYY-Hx (where x is a semi-annual between 1 and 2) and YYYY-Wxx (where xx is a ISO-8601 week number between 1 and 53).
        /// </summary>
        /// <param name="value">The memory span that contains the UTF-8 string value to parse.</param>
        /// <param name="date">Contains the <see cref="Date"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<char> value, out Date date)
        {
            if (value.Length > 10)
            {
                date = default;
                return false;
            }
            Span<byte> bytes = stackalloc byte[value.Length];
            Encoding.UTF8.GetBytes(value, bytes);
            return TryParse(bytes, out date);
        }

        /// <summary>
        /// Tries to convert a memory span that contains the bytes of a UTF-8 string representation of a date to its <see cref="Date"/> equivalent and returns a value that indicates whether the conversion succeeded. Valid formats are YYYY-MM-DD, YYYY-MM, YYYY, YYYY-Qx (where x is a quarter between 1 and 4), YYYY-Tx (where t is a tertiary between 1 and 3), YYYY-Hx (where x is a semi-annual between 1 and 2) and YYYY-Wxx (where xx is a ISO-8601 week number between 1 and 53).
        /// </summary>
        /// <param name="value">The memory span that contains the bytes of the UTF-8 string value to parse.</param>
        /// <param name="date">Contains the <see cref="Date"/> value equivalent to the value contained in <paramref name="value"/>, if the conversion succeeded, or default if the conversion failed.</param>
        /// <returns>true if the <paramref name="value"/> parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse(ReadOnlySpan<byte> value, out Date date)
        {
            switch (value.Length)
            {
                case 4: //YYYY
                    if (Year.TryParse(value, out Year year))
                    {
                        date = new Date(year);
                        return true;
                    }
                    break;
                case 7:
                    byte c = value[5];
                    switch (c)
                    {
                        case (byte)'H': //YYYY-Hx (x is the semi-annual; 1-2)
                            if (DataTypes.SemiAnnual.TryParse(value, out SemiAnnual semiAnnual))
                            {
                                date = new Date(semiAnnual);
                                return true;
                            }
                            break;
                        case (byte)'Q': //YYYY-Qx (x is the quarter; 1-4)
                            if (DataTypes.Quarter.TryParse(value, out Quarter quarter))
                            {
                                date = new Date(quarter);
                                return true;
                            }
                            break;
                        case (byte)'T': //YYYY-Tx (x is the tertiary; 1-3)
                            if (DataTypes.Tertiary.TryParse(value, out Tertiary tertiary))
                            {
                                date = new Date(tertiary);
                                return true;
                            }
                            break;
                        case (byte)'W': //YYYY-Wx (x is the ISO-8601 week number; 1-53)
                            if (DataTypes.Week.TryParse(value, out Week weekWithOneDigit))
                            {
                                date = new Date(weekWithOneDigit);
                                return true;
                            }
                            break;
                        default: //YYYY-MM
                            if (DataTypes.Month.TryParse(value, out Month month))
                            {
                                date = new Date(month);
                                return true;
                            }
                            break;
                    }
                    break;
                case 8: //YYYY-Wxx (x is the ISO-8601 week number; 1-53)
                    if (DataTypes.Week.TryParse(value, out Week week))
                    {
                        date = new Date(week);
                        return true;
                    }
                    break;
                case 10: //YYYY-MM-DD
                    if (DataTypes.Day.TryParse(value, out Day day))
                    {
                        date = new Date(day);
                        return true;
                    }
                    break;
            }
            date = default;
            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified <see cref="Date"/> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>true if the <paramref name="other"/> parameter equals the value of this instance; otherwise, false.</returns>
        public readonly bool Equals(Date other)
        {
            if (Format != other.Format)
                return false;

            switch (Format)
            {
                case DateFormat.Day:
                    return Day == other.Day;
                case DateFormat.Month:
                    return Month == other.Month;
                case DateFormat.Quarter:
                    return Quarter == other.Quarter;
                case DateFormat.SemiAnnual:
                    return SemiAnnual == other.SemiAnnual;
                case DateFormat.Tertiary:
                    return Tertiary == other.Tertiary;
                case DateFormat.Week:
                    return Week == other.Week;
                case DateFormat.Year:
                    return Year == other.Year;
                default:
                    Debug.Fail($"Unknown {nameof(DateFormat)}.");
                    return false;
            }
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public readonly override int GetHashCode()
        {
            switch (Format)
            {
                case DateFormat.Day:
                    return Day.GetHashCode();
                case DateFormat.Month:
                    return Month.GetHashCode();
                case DateFormat.Quarter:
                    return Quarter.GetHashCode();
                case DateFormat.SemiAnnual:
                    return SemiAnnual.GetHashCode();
                case DateFormat.Tertiary:
                    return Tertiary.GetHashCode();
                case DateFormat.Week:
                    return Week.GetHashCode();
                case DateFormat.Year:
                    return Year.GetHashCode();
                default:
                    Debug.Fail($"Unknown {nameof(DateFormat)}.");
                    return 0;
            }
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if value is an instance of <see cref="Date"/> and equals the value of this instance; otherwise, false.</returns>
        public readonly override bool Equals(object obj) => obj is Date date && Equals(date);

        /// <summary>
        /// Converts the value of the current <see cref="Date"/> object to its equivalent string representation.
        /// </summary>
        /// <returns> A string representation of the value of the current <see cref="Date"/> object.</returns>
        public readonly override string ToString()
        {
            switch (Format)
            {
                case DateFormat.Day:
                    return Day?.ToString();
                case DateFormat.Month:
                    return Month?.ToString();
                case DateFormat.Quarter:
                    return Quarter?.ToString();
                case DateFormat.SemiAnnual:
                    return SemiAnnual?.ToString();
                case DateFormat.Tertiary:
                    return Tertiary?.ToString();
                case DateFormat.Week:
                    return Week?.ToString();
                case DateFormat.Year:
                    return Year.ToString();
                default:
                    Debug.Fail($"Unknown {nameof(DateFormat)}.");
                    return null;
            }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Indicates whether two <see cref="Date"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, false.</returns>
        public static bool operator ==(Date left, Date right) => left.Equals(right);

        /// <summary>
        /// Indicates whether two <see cref="Date"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>true if the values of <paramref name="left"/> and <paramref name="right"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(Date left, Date right) => !left.Equals(right);
        #endregion
    }
}