using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using System.Collections;
using EDO.Core.Util;

namespace EDO.Core.View
{
    public class SelectObjectWindowVM<T> :BaseVM, ISelectObjectWindowVM
        where T: class
    {
        public SelectObjectWindowVM(List<T> allObjects)
            : this(new ObservableCollection<T>(allObjects))
        {
        }


        public SelectObjectWindowVM(ObservableCollection<T> allObjects) :this(allObjects, "Title")
        {
        }

        public SelectObjectWindowVM(ObservableCollection<T> allObjects, string displayMemberPath)
        {
            this.allObjects = allObjects;
            this.objects = new ObservableCollection<T>();
            DisplayMemberPath = displayMemberPath;
            Filter("");
        }

        private ObservableCollection<T> allObjects;
        private ObservableCollection<T> objects;
        public string DisplayMemberPath { get; set; }

        private object selectedObject;
        public object SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (selectedObject != value)
                {
                    selectedObject = value;
                    NotifyPropertyChanged("SelectedObject");
                }
            }
        }

        public IEnumerable Objects { get { return objects; } }

        public void Filter(string text)
        {
            string lowerText = text.ToLower();
            objects.Clear();
            foreach (T obj in allObjects)
            {
                string objValue = (string)PropertyPathHelper.GetValue(obj, DisplayMemberPath) as string;
                if (string.IsNullOrEmpty(lowerText) || (objValue != null && objValue.ToLower().Contains(lowerText)))
                {
                    objects.Add(obj);
                }
            }
        }
    }
}
