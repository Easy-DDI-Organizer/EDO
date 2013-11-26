using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using EDO.Core.Model;
using System.Collections.ObjectModel;

namespace EDO.Core.View
{
    /// <summary>
    /// DateRangePicker2.xaml の相互作用ロジック
    /// </summary>
    public partial class DateRangePicker2 : UserControl
    {
        public static int? ToInt(string numString)
        {
            if (string.IsNullOrEmpty(numString))
            {
                return null;
            }
            int year;
            try
            {
                year = int.Parse(numString);
            }
            catch (Exception)
            {
                return null;
            }
            return year;
        }

        public static int? ToValidYear(object value)
        {
            int? year = ToInt((string)value);
            if (year == null)
            {
                return null;
            }
            if (year < 1 || year > 9999)
            {
                return null;
            }
            return year;
        }

        public static int? ToValidMonth(object value)
        {
            int? month = ToInt((string)value);
            if (month == null)
            {
                return null;
            }
            if (month < 1 || month > 12)
            {
                return null;
            }
            return month;
        }

        public static int? ToValidDay(int? year, int? month, object value)
        {
            int? day = ToInt((string)value);
            if (year == null || month == null || day == null)
            {
                return day; // year or month がnullの場合は現在の値をキープする
            }
            if (!IsValidDay(year.Value, month.Value, day.Value))
            {
                return null;
            }
            return day;
        }

        public static bool IsNeedClearDay(int? year, int? month, int? day)
        {
            if (year == null || month == null || day == null)
            {
                return false;
            }
            return !IsValidDay(year.Value, month.Value, day.Value);
        }

        public static bool IsValidDay(int year, int month, int day)
        {
            bool result = false;
            try
            {
                DateTime d = new DateTime(year, month, day);
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static void UpdateDays(ObservableCollection<int>days, int? year, int? month)
        {
            if (year == null || month == null)
            {
                return;
            }
            int lastDay = DateTime.DaysInMonth(year.Value, month.Value);
            int curLastDay = days.Last();
            if (lastDay > curLastDay)
            {
                for (int i = curLastDay + 1; i <= lastDay; i++)
                {
                    days.Add(i);
                }
            }
            else if (lastDay < curLastDay)
            {
                for (int i = curLastDay; i > lastDay; i--)
                {
                    days.Remove(i);
                }
            }
        }

        public static DependencyProperty DateRangeProperty;
        public static DependencyProperty FromYearProperty;
        public static DependencyProperty FromMonthProperty;
        public static DependencyProperty FromDayProperty;

        public static DependencyProperty ToYearProperty;
        public static DependencyProperty ToMonthProperty;
        public static DependencyProperty ToDayProperty;

        static DateRangePicker2()
        {
            DateRangeProperty = DependencyProperty.Register("DateRange", typeof(DateRange), typeof(DateRangePicker2),
                new FrameworkPropertyMetadata(new DateRange(), new PropertyChangedCallback(OnDateRangeChanged)));
            FromYearProperty = DependencyProperty.Register("FromYear", typeof(string), typeof(DateRangePicker2),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFromYearChanged), new CoerceValueCallback(CoerceFromYear)));
            FromMonthProperty = DependencyProperty.Register("FromMonth", typeof(string), typeof(DateRangePicker2),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFromMonthChanged), new CoerceValueCallback(CoerceFromMonth)));
            FromDayProperty = DependencyProperty.Register("FromDay", typeof(string), typeof(DateRangePicker2),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFromDayChanged), new CoerceValueCallback(CoerceFromDay)));
            ToYearProperty = DependencyProperty.Register("ToYear", typeof(string), typeof(DateRangePicker2),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnToYearChanged), new CoerceValueCallback(CoerceToYear)));
            ToMonthProperty = DependencyProperty.Register("ToMonth", typeof(string), typeof(DateRangePicker2),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnToMonthChanged), new CoerceValueCallback(CoerceToMonth)));
            ToDayProperty = DependencyProperty.Register("ToDay", typeof(string), typeof(DateRangePicker2),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnToDayChanged), new CoerceValueCallback(CoerceToDay)));
        }

        private static void OnTextFieldChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e, Action<DateRange, string> action)
        {
            DateRangePicker2 dateRangePicker = (DateRangePicker2)sender;
            DateRange oldDateRange = dateRangePicker.DateRange;
            DateRange newDateRange = (DateRange)oldDateRange.Clone();
            action(newDateRange, (string)e.NewValue);
            dateRangePicker.DateRange = newDateRange;
        }

        private static object CoerceFromYear(DependencyObject d, object value)
        {
            return ToValidYear(value).ToString();
        }

        private static void OnFromYearChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Action<DateRange, string> action = (newDateRange, newValue) => newDateRange.FromYear = ToValidYear((string)newValue);
            OnTextFieldChanged(sender, e, action);
        }

        private static object CoerceFromMonth(DependencyObject d, object value)
        {
            return ToValidMonth(value).ToString();
        }

        private static void OnFromMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Action<DateRange, string> action = (newDateRange, newValue) => {
                newDateRange.FromMonth = ToValidMonth((string)newValue);
                if (IsNeedClearDay(newDateRange.FromYear, newDateRange.FromMonth, newDateRange.FromDay)) {
                    newDateRange.FromDay = null;
                }
            };
            OnTextFieldChanged(sender, e, action);
        }

        private static object CoerceFromDay(DependencyObject d, object value)
        {
            DateRangePicker2 dateRangePicker = (DateRangePicker2)d;
            DateRange dateRange = dateRangePicker.DateRange;
            return ToValidDay(dateRange.FromYear, dateRange.FromMonth, value).ToString();
        }

        private static void OnFromDayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DateRangePicker2 dateRangePicker = (DateRangePicker2)sender;
            DateRange dateRange = dateRangePicker.DateRange;
            Action<DateRange, string> action = (newDateRange, newValue) =>
            {
                newDateRange.FromDay = ToValidDay(dateRange.FromYear, dateRange.FromMonth, (string)newValue);
            };
            OnTextFieldChanged(sender, e, action);
        }

        private static object CoerceToYear(DependencyObject d, object value)
        {
            return ToValidYear(value).ToString();
        }

        private static void OnToYearChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Action<DateRange, string> action = (newDateRange, newValue) => newDateRange.ToYear = ToValidYear((string)newValue);
            OnTextFieldChanged(sender, e, action);
        }

        private static object CoerceToMonth(DependencyObject d, object value)
        {
            return ToValidMonth(value).ToString();
        }

        private static void OnToMonthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Action<DateRange, string> action = (newDateRange, newValue) =>
            {
                newDateRange.ToMonth = ToValidMonth((string)newValue);
                if (IsNeedClearDay(newDateRange.ToYear, newDateRange.ToMonth, newDateRange.ToDay))
                {
                    newDateRange.ToDay = null;
                }
            };
            OnTextFieldChanged(sender, e, action);
        }

        private static object CoerceToDay(DependencyObject d, object value)
        {
            DateRangePicker2 dateRangePicker = (DateRangePicker2)d;
            DateRange dateRange = dateRangePicker.DateRange;
            return ToValidDay(dateRange.ToYear, dateRange.ToMonth, value).ToString();
        }

        private static void OnToDayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DateRangePicker2 dateRangePicker = (DateRangePicker2)sender;
            DateRange dateRange = dateRangePicker.DateRange;
            Action<DateRange, string> action = (newDateRange, newValue) =>
            {
                newDateRange.ToDay = ToValidDay(dateRange.ToYear, dateRange.ToMonth, (string)newValue);
            };
            OnTextFieldChanged(sender, e, action);
        }


        private static void OnDateRangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //DateRangePickerに新しいDateRangeがセットされときによびだされる。
            DateRangePicker2 dateRangePicker = (DateRangePicker2)sender;


            DateRange oldDateRange = (DateRange)e.OldValue;
            DateRange newDateRange = (DateRange)e.NewValue;

            if (DateRange.EqualsDateRange(oldDateRange, newDateRange))
            {
                return;
            }

            DateUnit fromDate = newDateRange.FromDate;
            dateRangePicker.FromYear = fromDate.Year.ToString();
            dateRangePicker.FromMonth = fromDate.Month.ToString();
            dateRangePicker.FromDay = fromDate.Day.ToString();
            DateUnit toDate = newDateRange.ToDate;
            dateRangePicker.ToYear = toDate.Year.ToString();
            dateRangePicker.ToMonth = toDate.Month.ToString();
            dateRangePicker.ToDay = toDate.Day.ToString();

            //DateRangePickerの変更を監視しているオブジェクトに変更を通知。
            dateRangePicker.OnDateRangeChanged(oldDateRange, newDateRange);
        }


        public static readonly RoutedEvent DateRangeChangedEvent =
             EventManager.RegisterRoutedEvent("DateRangeChanged", RoutingStrategy.Bubble,
                 typeof(RoutedPropertyChangedEventHandler<DateRange>), typeof(DateRangePicker2));

        public event RoutedPropertyChangedEventHandler<DateRange> DateRangeChanged
        {
            add { AddHandler(DateRangeChangedEvent, value); }
            remove { RemoveHandler(DateRangeChangedEvent, value); }
        }

        private ObservableCollection<int> years;
        private ObservableCollection<int> months;
        private ObservableCollection<int> fromDays;
        private ObservableCollection<int> toDays;

        public DateRangePicker2()
        {
            InitializeComponent();
//            DataContext = this;

            years = new ObservableCollection<int>();
            int curYear = DateTime.Today.Year;
            for (int year = curYear + 1; year >= curYear - 80; year--)
            {
                years.Add(year);
            }

            months = new ObservableCollection<int>();
            for (int month = 1; month <= 12; month++)
            {
                months.Add(month);
            }

            fromDays = new ObservableCollection<int>();
            toDays = new ObservableCollection<int>();
            for (int day = 1; day <= 31; day++)
            {
                fromDays.Add(day);
                toDays.Add(day);
            }

            fromYear.ItemsSource = years;
            toYear.ItemsSource = years;
            fromMonth.ItemsSource = months;
            toMonth.ItemsSource = months;
            fromDay.ItemsSource = fromDays;
            toDay.ItemsSource = toDays;

            fromMonth.IsEnabled = false;
            fromDay.IsEnabled = false;
            toMonth.IsEnabled = false;
            toDay.IsEnabled = false;
        }

        private void OnFromYearTextChanged(object sender, TextChangedEventArgs e)
        {
            fromMonth.IsEnabled = !string.IsNullOrEmpty(fromYear.Text);
            fromDay.IsEnabled = !string.IsNullOrEmpty(fromYear.Text) && !string.IsNullOrEmpty(fromMonth.Text);
        }

        private void OnFromMonthTextChanged(object sender, TextChangedEventArgs e)
        {
            fromDay.IsEnabled = !string.IsNullOrEmpty(fromMonth.Text);
        }

        private void OnToYearTextChanged(object sender, TextChangedEventArgs e)
        {
            toMonth.IsEnabled = !string.IsNullOrEmpty(toYear.Text);
            toDay.IsEnabled = !string.IsNullOrEmpty(toYear.Text) && !string.IsNullOrEmpty(toMonth.Text);
        }

        private void OnToMonthTextChanged(object sender, TextChangedEventArgs e)
        {
            toDay.IsEnabled = !string.IsNullOrEmpty(toMonth.Text);
        }

        private void OnFromYearDropDownOpened(object sender, EventArgs e)
        {
        }

        public DateRange DateRange
        {
            get
            {
                return (DateRange)GetValue(DateRangeProperty);
            }
            set
            {
                SetValue(DateRangeProperty, value);
            }
        }

        public string FromYear
        {
            get
            {
                return (string)GetValue(FromYearProperty);
            }
            set
            {
                SetValue(FromYearProperty, value);
            }
        }


        public string FromMonth
        {
            get
            {
                return (string)GetValue(FromMonthProperty);
            }
            set
            {
                SetValue(FromMonthProperty, value);
            }
        }

        public string FromDay
        {
            get
            {
                return (string)GetValue(FromDayProperty);
            }
            set
            {
                SetValue(FromDayProperty, value);
            }
        }

        public string ToYear
        {
            get
            {
                return (string)GetValue(ToYearProperty);
            }
            set
            {
                SetValue(ToYearProperty, value);
            }
        }


        public string ToMonth
        {
            get
            {
                return (string)GetValue(ToMonthProperty);
            }
            set
            {
                SetValue(ToMonthProperty, value);
            }
        }

        public string ToDay
        {
            get
            {
                return (string)GetValue(ToDayProperty);
            }
            set
            {
                SetValue(ToDayProperty, value);
            }
        }

        private void OnDateRangeChanged(DateRange oldValue, DateRange newValue)
        {
            UpdateDays(fromDays, newValue.FromYear, newValue.FromMonth);
            UpdateDays(toDays, newValue.ToYear, newValue.ToMonth);

            RoutedPropertyChangedEventArgs<DateRange> args = new RoutedPropertyChangedEventArgs<DateRange>(oldValue, newValue);
            args.RoutedEvent = DateRangePicker.DateRangeChangedEvent;
            RaiseEvent(args);
        }

    }
}
