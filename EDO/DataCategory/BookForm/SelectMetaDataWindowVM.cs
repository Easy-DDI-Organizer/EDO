using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Core.Util;
using System.Collections.ObjectModel;
using System.Collections;
using EDO.QuestionCategory.ConceptForm;
using EDO.QuestionCategory.QuestionForm;

namespace EDO.DataCategory.BookForm
{
    public class SelectMetaDataWindowVM :BaseVM
    {
        public static ITitleProvider Find(ObservableCollection<ITitleProvider> objects, string metaDataId)
        {
            foreach (ITitleProvider obj in objects)
            {
                if (obj.Id == metaDataId)
                {
                    return obj;
                }
            }
            return null;
        }

        public SelectMetaDataWindowVM(BookRelationType bookRelationType, string metaDataId)
        {
            this.bookRelationType = bookRelationType;

            bookRelationItems = new ObservableCollection<BookRelationItem>();
            bookRelationItems.AddRange(BookRelationItem.All);
            this.allObjects = new ObservableCollection<ITitleProvider>();
            this.objects = new ObservableCollection<ITitleProvider>();
            this.metaDataId = metaDataId;
        }

        private string metaDataId;

        public void Init()
        {
            allObjects.Clear();
            objects.Clear();

            allObjects.AddRange(StudyUnit.RelatedMetaData(bookRelationType));

            Filter("");
            SearchText = "";

            SelectedObject = Find(objects, metaDataId);
            metaDataId = null;
        }

        private ObservableCollection<BookRelationItem> bookRelationItems;
        public ObservableCollection<BookRelationItem> BookRelationItems { get { return bookRelationItems; } }

        private BookRelationType bookRelationType;
        public BookRelationType BookRelationType
        {
            get
            {
                return bookRelationType;
            }
            set
            {
                if (bookRelationType != value)
                {
                    bookRelationType = value;
                    Init();
                    NotifyPropertyChanged("BookRelationType");
                    NotifyPropertyChanged("IsSelectable");
                }
            }
        }

        private ObservableCollection<ITitleProvider> allObjects;
        private ObservableCollection<ITitleProvider> objects;

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
            foreach (ITitleProvider obj in allObjects)
            {
                string objValue = obj.Title as string;
                if (string.IsNullOrEmpty(lowerText) || (objValue != null && objValue.ToLower().Contains(lowerText)))
                {
                    objects.Add(obj);
                }
            }
        }

        public bool IsSelectable
        {
            get
            {
                return bookRelationType != BookRelationType.Abstract;
            }
        }

        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                if (searchText != value)
                {
                    searchText = value;
                    NotifyPropertyChanged("Searchtext");
                }
            }
        }
    }
}
