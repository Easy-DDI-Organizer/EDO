using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Reflection;
using System.Diagnostics;
using EDO.Core.Model;

namespace EDO.DataCategory.BookForm
{
    public class BookFieldVM :BaseVM
    {
        public BookFieldVM(Book book, string label, string propertyName)
        {
            this.book = book;
            this.label = label;
            this.propertyInfo = book.GetType().GetProperty(propertyName);
            Debug.Assert(propertyInfo != null);
            this.v= (string)propertyInfo.GetValue(book, null);
        }

        private Book book;

        private PropertyInfo propertyInfo;

        private string label;
        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                if (label != value)
                {
                    label = value;
                    NotifyPropertyChanged("Label");
                }
            }
        }

        private string v;
        public string Value
        {
            get
            {
                return v;
            }
            set
            {
                if (v != value)
                {
                    v = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public void UpdateModel()
        {
            propertyInfo.SetValue(book, v, null);                
        }
    }
}
