using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;

namespace EDO.DataCategory.BookForm
{
    public class BookRelationVM :BaseVM
    {
        public BookRelationVM(BookRelation bookRelation)
        {
            this.bookRelation = bookRelation;
        }

        private BookRelation bookRelation;

        public BookRelation BookRelation { get { return bookRelation; } }

        public override object Model { get { return bookRelation; } }


        private string typeName;
        public string TypeName
        {
            get
            {
                return typeName;
            }
            set
            {
                if (typeName != value)
                {
                    typeName = value;
                    NotifyPropertyChanged("TypeName");
                }
            }
        }

        private string metaDataName;
        public string MetaDataName
        {
            get
            {
                return metaDataName;
            }
            set
            {
                if (metaDataName != value)
                {
                    metaDataName = value;
                    NotifyPropertyChanged("MetaDataName");
                }
            }
        }

    }
}
