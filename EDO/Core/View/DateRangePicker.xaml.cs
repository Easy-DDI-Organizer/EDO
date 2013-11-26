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
using EDO.Core;
using EDO.Core.Model;

namespace EDO.Core.View
{
    /// <summary>
    /// DateRangePicker.xaml の相互作用ロジック
    /// </summary>
    public partial class DateRangePicker : UserControl
    {
        public static DependencyProperty DateRangeProperty;
        public static DependencyProperty FromDateProperty;
        public static DependencyProperty ToDateProperty;

        static DateRangePicker()
        {
            DateRangeProperty = DependencyProperty.Register("DateRange", typeof(DateRange), typeof(DateRangePicker),
                new FrameworkPropertyMetadata(new DateRange(), new PropertyChangedCallback(OnDateRangeChanged)));

            FromDateProperty = DependencyProperty.Register("FromDate", typeof(DateTime?), typeof(DateRangePicker),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnFromDateChanged)));

            ToDateProperty = DependencyProperty.Register("ToDate", typeof(DateTime?), typeof(DateRangePicker),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnToDateChanged)));
        }


        private static void OnDateRangeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DateRangePicker dateRangePicker = (DateRangePicker)sender;

            DateRange oldDateRange = (DateRange)e.OldValue;
            DateRange newDateRange = (DateRange)e.NewValue;
            dateRangePicker.FromDate = newDateRange.FromDateTime;
            dateRangePicker.ToDate = newDateRange.ToDateTime;
            dateRangePicker.OnDateRangeChanged(oldDateRange, newDateRange);
        }

        private static void OnFromDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DateRangePicker dateRangePicker = (DateRangePicker)sender;
            DateRange oldDateRange = dateRangePicker.DateRange;
            DateRange newDateRange = new DateRange((DateTime?)e.NewValue, oldDateRange.ToDateTime);
            dateRangePicker.DateRange = newDateRange;
        }

        private static void OnToDateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            DateRangePicker dateRangePicker = (DateRangePicker)sender;
            DateRange oldDateRange = dateRangePicker.DateRange;
            DateRange newDateRange = new DateRange(oldDateRange.FromDateTime, (DateTime?)e.NewValue);
            dateRangePicker.DateRange = newDateRange;
        }

        public static readonly RoutedEvent DateRangeChangedEvent =
             EventManager.RegisterRoutedEvent("DateRangeChanged", RoutingStrategy.Bubble,
                 typeof(RoutedPropertyChangedEventHandler<DateRange>), typeof(DateRangePicker));

        public event RoutedPropertyChangedEventHandler<DateRange> DateRangeChanged
        {
            add { AddHandler(DateRangeChangedEvent, value); }
            remove { RemoveHandler(DateRangeChangedEvent, value); }
        }


        public DateRangePicker()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(DateRangePicker_Loaded);
        }

        private void InitControls()
        {
            fromDatePicker.SelectedDate = this.FromDate;
            toDatePicker.SelectedDate = this.ToDate;
            rangeCheckBox.IsEnabled = false;
            toDatePicker.IsEnabled = false;
            UpdateRangeControls();
        }

        public void UpdateRangeControls()
        {
            bool isEnabledRangeCheckBox = this.FromDate != null;
            bool isCheckedRangeCheckBox = this.ToDate != null;
            bool isEnabledToDatePicker = this.ToDate != null;
            rangeCheckBox.IsEnabled = isEnabledRangeCheckBox;
            rangeCheckBox.IsChecked = isCheckedRangeCheckBox;
            toDatePicker.IsEnabled = isEnabledToDatePicker;
        }

        public void DateRangePicker_Loaded(object sender, EventArgs e)
        {
            InitControls();
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

        public DateTime? FromDate
        {
            get
            {                
                return (DateTime?)GetValue(FromDateProperty);
            }
            set
            {
//                rangeCheckBox.IsEnabled = value != null;
                SetValue(FromDateProperty, value);
            }
        }

        public DateTime? ToDate
        {
            get
            {
                return (DateTime?)GetValue(ToDateProperty);
            }
            set
            {
                SetValue(ToDateProperty, value);
            }
        }

        private void OnDateRangeChanged(DateRange oldValue, DateRange newValue)
        {
            UpdateRangeControls();
            RoutedPropertyChangedEventArgs<DateRange> args = new RoutedPropertyChangedEventArgs<DateRange>(oldValue, newValue);
            args.RoutedEvent = DateRangePicker.DateRangeChangedEvent;
            RaiseEvent(args);
        }
            

        private void rangeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            toDatePicker.IsEnabled = true;
        }

        private void rangeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            toDatePicker.IsEnabled = false;
            this.ToDate = null;
            InitControls();
        }
    }
}
