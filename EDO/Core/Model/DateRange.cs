using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;
using System.Diagnostics;
using EDO.Properties;

namespace EDO.Core.Model
{
    public class DateRange : INotifyPropertyChanged, IComparable, ICloneable
    {
        static DateTime? ParseDateTime(string str)
        {
            DateTime? time = null;
            try
            {
                time = DateTime.Parse(str);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return time;
        }

        public static DateRange Parse(string str, char separator)
        {
            DateRange range = new DateRange();
            string[] elems = str.Split(separator);
            if (elems.Length != 2)
            {
                return range;
            }
            DateTime? fromDate = ParseDateTime(elems[0]);
            if (fromDate == null)
            {
                //fromが解析できない場合からのDateRangeを返す
                return range;
            }
            //toはnullでも良い。
            DateTime? toDate = ParseDateTime(elems[1]);
            range = new DateRange(fromDate, toDate);
            return range;
        }

        public DateRange(DateTime? fromDateTime, DateTime? toDateTime)
            : this(new DateUnit(fromDateTime), new DateUnit(toDateTime))
        {
        }

        public DateRange()
            : this(new DateUnit(), new DateUnit())
        {
        }

        public DateRange(DateUnit fromDate, DateUnit toDate)
        {
            this.fromDate = fromDate;
            this.toDate = toDate;
        }

        private DateUnit fromDate;
        public DateUnit FromDate
        {
            get
            {
                return fromDate;
            }
        }
        private DateUnit toDate;
        public DateUnit ToDate
        {
            get
            {
                return toDate;
            }
        }


        public DateTime? FromDateTime
        {
            get
            {
                return fromDate.Date;
            }
        }


        public DateTime? ToDateTime
        {
            get
            {
                return toDate.Date;
            }
        }

        override
        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (FromDateTime != null)
            {
                sb.Append(FromDate.ToString());
            }
            if (ToDateTime != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(Resources.FromTo);
                }
                sb.Append(ToDate.ToString());
            }
            return sb.ToString();
        }

        public string ToStringJa()
        {
            StringBuilder sb = new StringBuilder();
            if (FromDay != null)
            {
                sb.Append(FromDate.ToStringJa());
            }
            if (ToDateTime != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(Resources.FromTo);
                }
                sb.Append(ToDate.ToStringJa());
            }
            return sb.ToString();
        }

        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        private int CompareDate(DateTime? d1, DateTime? d2)
        {
            if (d1 == null && d2 == null)
            {
                return 0;
            }
            if (d1 != null && d2 == null)
            {
                return -1;
            }
            if (d1 == null && d2 != null)
            {
                return 1;
            }
            return DateTime.Compare(d1.Value, d2.Value);
        }

        public int CompareTo(object otherObject)
        {
            DateRange other = otherObject as DateRange;
            if (other == null)
            {
                return -1;
            }
            int r = CompareDate(FromDateTime, other.FromDateTime);
            if (r != 0)
            {
                return r;
            }
            return CompareDate(ToDateTime, other.ToDateTime);
        }

        public bool IsEmpty
        {
            get
            {
                return FromDateTime == null && ToDateTime == null;
            }
        }

        public bool IsFromDateOnly
        {
            get
            {
                return FromDateTime != null && ToDateTime == null;
            }
        }

        public object Clone()
        {
            DateUnit newFromDate = null;
            DateUnit newToDate = null;
            if (fromDate != null)
            {
                newFromDate = (DateUnit)fromDate.Clone();
            }
            if (toDate != null)
            {
                newToDate = (DateUnit)toDate.Clone();
            }
            return new DateRange(newFromDate, newToDate);
        }

        public int? FromYear
        {
            get
            {
                return fromDate.Year;
            }
            set
            {
                fromDate.Year = value;
            }
        }

        public int? FromMonth
        {
            get
            {
                return fromDate.Month;
            }
            set
            {
                fromDate.Month = value;
            }
        }

        public int? FromDay
        {
            get
            {
                return fromDate.Day;
            }
            set
            {
                fromDate.Day = value;
            }
        }

        public int? ToYear
        {
            get
            {
                return toDate.Year;
            }
            set
            {
                toDate.Year = value;
            }
        }

        public int? ToMonth
        {
            get
            {
                return toDate.Month;
            }
            set
            {
                toDate.Month = value;
            }
        }

        public int? ToDay
        {
            get
            {
                return toDate.Day;
            }
            set
            {
                toDate.Day = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            DateRange other = (DateRange)obj;
            return DateUnit.EqualsDateUtil(FromDate, other.FromDate) && DateUnit.EqualsDateUtil(ToDate, other.ToDate);
        }

        public override int GetHashCode()
        {
            int hashCode1 = FromDate == null ? 0 : FromDate.GetHashCode();
            int hashCode2 = ToDate == null ? 0 : ToDate.GetHashCode();
            return hashCode1 ^ hashCode2;
        }

        public static bool EqualsDateRange(DateRange range1, DateRange range2)
        {
            if (range1 == null)
            {
                if (range2 == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return range1.Equals(range2);
        }
    }
}
