using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model.Layout
{
    public class DateTimeLayout :ResponseLayout
    {
        public DateTimeLayoutCalendarEra CalendarEra { get; set; }

        public LayoutStyle Style { get; set; }
    }

}
