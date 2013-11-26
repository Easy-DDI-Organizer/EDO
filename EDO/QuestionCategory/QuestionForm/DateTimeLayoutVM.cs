using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model.Layout;
using EDO.Core.Model;

namespace EDO.QuestionCategory.QuestionForm
{
    public class DateTimeLayoutVM :ResponseLayoutVM
    {
        public DateTimeLayoutVM(DateTimeLayout dateTimeLayout)
            : base(dateTimeLayout)
        {
        }

        public DateTimeLayout DateTimeLayout { get { return (DateTimeLayout)Layout; } }

        public DateTimeLayoutCalendarEra CalendarEra
        {
            get
            {
                return DateTimeLayout.CalendarEra;
            }
            set
            {
                if (DateTimeLayout.CalendarEra != value)
                {
                    DateTimeLayout.CalendarEra = value;
                    NotifyPropertyChanged("CalendarEra");
                    Memorize();
                }
            }
        }

        public LayoutStyle Style
        {
            get
            {
                return DateTimeLayout.Style;
            }
            set
            {
                if (DateTimeLayout.Style != value)
                {
                    DateTimeLayout.Style = value;
                    NotifyPropertyChanged("Style");
                    Memorize();
                }
            }
        }


    }
}
