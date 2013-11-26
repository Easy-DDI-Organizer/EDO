using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using EDO.Core;
using EDO.Core.Model;

namespace EDO.Core.View
{
    public class DateRangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateRange dateRange = (DateRange)value;
            if (dateRange == null)
            {
                return "";
            }
            return dateRange.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
