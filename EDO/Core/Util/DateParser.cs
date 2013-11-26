using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using System.Text.RegularExpressions;

namespace EDO.Core.Util
{
    public class DataParseException : Exception
    {
    }

    public class DateParser
    {

        public static void Validate(int year)
        {
            Validate(year, 1, 1);
        }

        public static void Validate(int year, int month)
        {
            Validate(year, month, 1);
        }

        public static void Validate(int year, int month, int day)
        {
            try
            {
                // DateTimeで許される日付範囲を正しいとみなす(ref http://msdn.microsoft.com/ja-jp/library/xcfzdy4x(v=vs.80).aspx)
                // year: 1～9999
                // month: 1～12
                // day: その年月の範囲
                DateTime dateTime = new DateTime(year, month, day);
            } catch (Exception)
            {
                throw new DataParseException();
            }
        }

        public static DateUnit Parse(string str)
        {
            Regex regex = new Regex(@"^(\d\d\d\d)$");
            Match m = regex.Match(str);
            if (m.Success)
            {
                int year = Int32.Parse(m.Groups[1].Value);
                Validate(year);
                return new DateUnit(year);
            }
            regex = new Regex(@"^(\d\d\d\d)[/-](\d\d?)$");
            m = regex.Match(str);
            if (m.Success)
            {
                int year = Int32.Parse(m.Groups[1].Value);
                int month = Int32.Parse(m.Groups[2].Value);
                Validate(year, month);
                return new DateUnit(year, month);
            }

            regex = new Regex(@"^(\d\d?)[/-](\d\d\d\d)$");
            m = regex.Match(str);
            if (m.Success)
            {
                int month = Int32.Parse(m.Groups[1].Value);
                int year = Int32.Parse(m.Groups[2].Value);
                Validate(year, month);
                return new DateUnit(year, month);
            }

            try
            {
                DateTime dateTime = DateTime.Parse(str);
                return new DateUnit(dateTime);
            }
            catch (Exception)
            {
            }
            throw new DataParseException();
        }

        public static DateRange Parse(string fromDateStr, string toDateStr)
        {
            DateUnit fromDate = null;
            DateUnit toDate = null;
            if (!string.IsNullOrEmpty(fromDateStr))
            {
                fromDate = DateParser.Parse(fromDateStr);
            }
            if (fromDate == null)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(toDateStr))
            {
                toDate = DateParser.Parse(toDateStr);
            }
            if (toDate == null)
            {
                toDate = new DateUnit();
            }
            return new DateRange(fromDate, toDate);
        }
    }
}
