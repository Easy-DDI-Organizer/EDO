using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EDO.Properties;

namespace EDO.Core.Model
{
    public enum DateUnitType : int
    {
        Empty,
        Year,
        YearMonth,
        YearMonthDay
    };

    public class DateUnit :ICloneable
    {
        public static DateUnitType GetType(int? year, int? month, int? day)
        {
            if (year != null && month != null && day != null)
            {
                return DateUnitType.YearMonthDay;
            }
            if (year != null && month != null)
            {
                return DateUnitType.YearMonth;
            }
            if (year != null)
            {
                return DateUnitType.Year;
            }
            return DateUnitType.Empty;
        }

        public static DateTime? CreateDateTime(int? year, int? month, int? day)
        {
            DateUnitType type = GetType(year, month, day);
            DateTime? dateTime = null;
            switch (type)
            {
                case DateUnitType.YearMonthDay:
                    dateTime = new DateTime(year.Value, month.Value, day.Value);
                    break;
                case DateUnitType.YearMonth:
                    dateTime = new DateTime(year.Value, month.Value, 1);
                    break;
                case DateUnitType.Year:
                    dateTime = new DateTime(year.Value, 1, 1);
                    break;
            }
            return dateTime;
        }

        public static string ToString(int? year, int? month, int? day)
        {
            DateUnitType type = GetType(year, month, day);
            string result = "";
            switch (type)
            {
                case DateUnitType.YearMonthDay:
                    result = string.Format("{0}/{1}/{2}", ToYearString(year), ToMonthString(month), ToDayString(day));
                    break;
                case DateUnitType.YearMonth:
                    result = string.Format("{0}/{1}", ToYearString(year), ToMonthString(month));
                    break;
                case DateUnitType.Year:
                    result = ToYearString(year);
                    break;
            }
            return result;
        }

        public static string ToStringJa(int? year, int? month, int? day)
        {
            DateUnitType type = GetType(year, month, day);
            string result = "";
            switch (type)
            {
                case DateUnitType.YearMonthDay:
                    result = string.Format(Resources.YearMonthDay, ToYearString(year), ToMonthString(month), ToDayString(day));
                    break;
                case DateUnitType.YearMonth:
                    result = string.Format(Resources.YearMonth, ToYearString(year), ToMonthString(month));
                    break;
                case DateUnitType.Year:
                    result = ToYearString(year) + Resources.Year;
                    break;
            }
            return result;
        }

        public static string ToYearString(int? year)
        {
            return string.Format("{0:0000}", year.Value);
        }

        public static string ToMonthString(int? month)
        {
            return string.Format("{0:00}", month.Value);
        }

        public static string ToDayString(int? day)
        {
            return string.Format("{0:00}", day.Value);
        }

        public DateUnit()
        {
        }

        public DateUnit(DateTime dateTime)
        {
            Init(dateTime);
        }

        public DateUnit(DateTime? dateTime)
        {
            if (dateTime != null)
            {
                Init(dateTime.Value);
            }
        }

        private void Init(DateTime dateTime)
        {
            Year = dateTime.Year;
            Month = dateTime.Month;
            Day = dateTime.Day;
        }

        public DateUnit(int year)
        {
            Year = year;
        }

        public DateUnit(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public DateUnit(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public int? Year { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }

        public DateTime? Date
        {
            get
            {
                return CreateDateTime(Year, Month, Day);
            }
        }

        public override string ToString()
        {
            return ToString(Year, Month, Day);
        }

        public string ToStringJa()
        {
            return ToStringJa(Year, Month, Day);
        }

        public string ToYearString()
        {
            return ToYearString(Year);
        }

        public string ToYearMonthString(string separator = "-")
        {
            return ToYearString(Year) + separator + ToMonthString(Month);
        }

        public string ToYearMonthDayString(string separator = "-")
        {
            return ToYearString(Year) + separator + ToMonthString(Month) + separator + ToDayString(Day);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            DateUnit other = (DateUnit)obj;
            return (Year == other.Year && Month == other.Month && Day == other.Day);
        }

        private int ToHash(int? value)
        {
            return value == null ? 0 : (int)value;
        }

        public override int GetHashCode()
        {
            return ToHash(Year) ^ ToHash(Month) ^ ToHash(Day);
        }

        public static bool EqualsDateUtil(DateUnit date1, DateUnit date2)
        {
            if (date1 == null)
            {
                if (date2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return date1.Equals(date2);
        }

        public DateUnitType DateUnitType
        {
            get
            {
                return GetType(Year, Month, Day);
            }
        }

    }
}
