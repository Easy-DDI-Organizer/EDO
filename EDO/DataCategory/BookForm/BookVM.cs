using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Properties;

namespace EDO.DataCategory.BookForm
{
    public class BookVM : BaseVM, IStringIDProvider
    {
        private Book book;

        public BookVM(Book book)
        {
            this.book = book;

        }

        public Book Book { get { return book; } }

        public override object Model { get { return book; } }

        public string Id { get { return book.Id; } }

        public string BookTypeCode
        {
            get
            {
                return book.BookTypeCode;
            }
            set
            {
                if (book.BookTypeCode != value)
                {
                    book.BookTypeCode = value;
                    NotifyPropertyChanged("BookTypeCode");
                }
            }
        }

        public string Author
        {
            get
            {
                return book.Author;
            }
            set
            {
                if (book.Author != value)
                {
                    book.Author = value;
                    NotifyPropertyChanged("Author");
                }
            }
        }

        public string Title
        {
            get
            {
                return book.Title;
            }
            set
            {
                if (book.Title != value)
                {
                    book.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string AnnouncementDate
        {
            get
            {
                return book.AnnouncementDate;
            }
            set
            {
                if (book.AnnouncementDate != value)
                {
                    book.AnnouncementDate = value;
                    NotifyPropertyChanged("AnnouncementDate");
                }
            }
        }

        public string RelationCount
        {
            get
            {
                return book.BookRelations.Count + Resources.Case;
            }
        }

        public bool ContainsRelation(BookRelation relation)
        {
            return Book.ContainsBookRelation(relation);
        }
    }
}
