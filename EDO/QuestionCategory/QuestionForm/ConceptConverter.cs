using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using EDO.QuestionCategory.ConceptForm;
using EDO.Properties;

namespace EDO.QuestionCategory.QuestionForm
{
    public class ConceptConverter :IValueConverter
    {
        #region IValueConverter メンバー

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ConceptVM concept = (ConceptVM)value;
            if (value == null)
            {
                return "";
            }
            return string.Format(Resources.QuestionForMeasure, concept.Title); //を測定するために使う質問項目
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
