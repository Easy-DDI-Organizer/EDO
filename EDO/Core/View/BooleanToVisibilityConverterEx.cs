using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace EDO.Core.View
{
    public class BooleanToVisibilityConverterEx :IValueConverter
    {
        public bool Reverse { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else if (value is Nullable<bool>)
            {
                Nullable<bool> tmp = (Nullable<bool>)value;
                bValue = tmp.HasValue ? tmp.Value : false;
            }
            if (Reverse)
            {
                bValue = !bValue;
            }
            return (bValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bResult = false;
            if (value is Visibility)
            {
                bResult =  (Visibility)value == Visibility.Visible;
            }
            if (Reverse)
            {
                bResult = !bResult;
            }
            return bResult;
        }
    }
}
