using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Event :ICloneable, IIDPropertiesProvider
    {
        public static void RemoveByTitle(List<Event> events, string title)
        {
            int upper = events.Count - 1;
            for (int i = upper ; i >= 0; i--)
            {
                if (events[i].Title == title)
                {
                    events.RemoveAt(i);
                }
            }
        }

        public  string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public Event()
        {
            Id = IDUtils.NewGuid();
            DateRange = new DateRange();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public DateRange DateRange { get; set; }
        public string Memo { get; set; }
        public int No { get; set; }

        public object Clone()
        {
            Event cloneEvent =  MemberwiseClone() as Event;
            cloneEvent.DateRange = this.DateRange.Clone() as DateRange;
            return cloneEvent;
        }

    }

}
