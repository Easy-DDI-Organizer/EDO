using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core;
using EDO.Core.Model;
using System.ComponentModel;
using EDO.Core.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using System.Diagnostics;
using EDO.Core.Util;

namespace EDO.EventCategory.EventForm
{
    public class EventVM :BaseVM, IEditableObject
    {

        public static int GetMaxNo(ObservableCollection<EventVM> events)
        {
            int maxNo = 0;
            foreach (EventVM eventItem in events)
            {
                if (eventItem.No > maxNo)
                {
                    maxNo = eventItem.No;
                }
            }
            return maxNo;
        }


        private Event eventModel;
        private Event bakEventModel;

        public EventVM() :this(new Event())
        {
        }

        public EventVM(Event eventModel)
        {
            this.eventModel = eventModel;
        }

        public override object Model
        {
            get
            {
                return eventModel;
            }
        }

        public string Title {
            get
            {
                return eventModel.Title;
            }
            set
            {
                if (eventModel.Title != value)
                {
                    eventModel.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public DateRange DateRange {
            get
            {
                return eventModel.DateRange;
            }

            set
            {
                if (!DateRange.EqualsDateRange(eventModel.DateRange, value))
                {
                    eventModel.DateRange = value;
                    NotifyPropertyChanged("DateRange");
                }
            }
        }

        public string Memo {
            get
            {
                return eventModel.Memo;
            }
            set
            {
                if (eventModel.Memo != value)
                {
                    eventModel.Memo = value;
                    NotifyPropertyChanged("Memo");
                }
            }
        }



        public int No
        {
            get
            {
                return eventModel.No;
            }
            set
            {
                if (eventModel.No != value)
                {
                    eventModel.No = value;
                    NotifyPropertyChanged("No");
                }
            }
        }        

        #region IEditableObject メンバー

        private bool inEdit = false;

        public void BeginEdit()
        {
            if (inEdit)
            {
                return;
            }
            inEdit = true;

            bakEventModel = eventModel.Clone() as Event;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            DateRange = bakEventModel.DateRange;
            Memo = bakEventModel.Memo;
            bakEventModel = null;
        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            bakEventModel = null;
            Memorize();
        }

        #endregion

        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = EMPTY_VALUE; ;
            }
        }
    }
}
