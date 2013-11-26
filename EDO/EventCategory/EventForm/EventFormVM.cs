using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.EventCategory.EventForm;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Core.Util;
using EDO.Main;
using System.Windows.Input;

namespace EDO.EventCategory.EventForm
{
    public class EventFormVM :FormVM
    {
        public EventFormVM(StudyUnitVM studyUnit) : base(studyUnit)
        {
            events = new ObservableCollection<EventVM>();
            foreach (Event eventModel in studyUnit.EventModels)
            {
                EventVM ev = new EventVM(eventModel);
                InitEvent(ev);
                events.Add(ev);
            }
            modelSyncher = new ModelSyncher<EventVM, Event>(this, events, studyUnit.EventModels);

            contents = new ObservableCollection<string>();
            foreach (Option option in Options.EventTypes)
            {
                contents.Add(option.Label);
            }
        }

        private int NextEventNo
        {
            get
            {
                return EventVM.GetMaxNo(events) + 1;
            }
        }

        public override void InitRow(object row)
        {
            if (row is EventVM)
            {
                InitEvent((EventVM)row);
            }
        }

        public void InitEvent(EventVM eventVM)
        {
            eventVM.Parent = this;
            if (eventVM.No == 0)
            {
                eventVM.No = NextEventNo;
            }
        }

        private ModelSyncher<EventVM, Event> modelSyncher;
        private ObservableCollection<EventVM> events;
        public ObservableCollection<EventVM> Events { get { return events; } }

        private ObservableCollection<string> contents;
        public ObservableCollection<string> Contents { get { return contents; } }

        private object selectedEventItem;
        public object SelectedEventItem
        {
            get
            {
                return selectedEventItem;
            }
            set
            {
                if (selectedEventItem != value)
                {
                    selectedEventItem = value;
                    NotifyPropertyChanged("SelectedEventItem");
                }
            }
        }

        public EventVM SelectedEvent
        {
            get
            {
                return SelectedEventItem as EventVM;
            }
        }

        private ICommand removeEventCommand;
        public ICommand RemoveEventCommand
        {
            get{
                if (removeEventCommand == null)
                {
                    removeEventCommand = new RelayCommand(param => RemoveEvent(), param => this.CanRemoveEvent);
                }
                return removeEventCommand;
            }
        }

        public bool CanRemoveEvent
        {
            get
            {
                if (SelectedEvent == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveEvent()
        {
            Events.Remove(SelectedEvent);
            SelectedEventItem = null;
        }

    }
}
