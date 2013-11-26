using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using EDO.DataCategory.BookForm;
using System.Windows.Input;
using EDO.Core.Util;

namespace EDO.StudyCategory.AbstractForm
{
    public class AbstractFormVM :FormVM
    {
        public AbstractFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            abstractModel = studyUnit.AbstractModel;
            this.IsIgnoreValidation = true;

            books = new ObservableCollection<BookVM>();
        }

        private ObservableCollection<BookVM> books;
        public ObservableCollection<BookVM> Books { get { return books; } }

        private Abstract abstractModel;

        public string Title
        {
            get
            {
                return abstractModel.Title;
            }
            set
            {
                if (abstractModel.Title != value)
                {
                    abstractModel.Title = value;
                    NotifyPropertyChanged("Title");
                    StudyUnit.InitTitle();
                    StudyUnit.NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public string Purpose
        {
            get
            {
                return abstractModel.Purpose;
            }
            set
            {
                if (abstractModel.Purpose != value)
                {
                    abstractModel.Purpose = value;
                    NotifyPropertyChanged("Purpose");
                    Memorize();
                }
            }
        }

        public string Summary
        {
            get
            {
                return abstractModel.Summary;
            }
            set
            {
                if (abstractModel.Summary != value)
                {
                    abstractModel.Summary = value;
                    NotifyPropertyChanged("Summary");
                    Memorize();
                }
            }
        }


        private bool isExpanded;
        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                if (isExpanded != value)
                {
                    isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        private object selectedBookItem;
        public object SelectedBookItem
        {
            get
            {
                return selectedBookItem;
            }
            set
            {
                if (selectedBookItem != value)
                {
                    selectedBookItem = value;
                    NotifyPropertyChanged("SelectedBookItem");
                }
            }
        }

        public BookVM SelectedBook
        {
            get
            {
                return SelectedBookItem as BookVM;
            }
        }

        private ICommand addBookCommand;
        public ICommand AddBookCommand
        {
            get
            {
                if (addBookCommand == null)
                {
                    addBookCommand = new RelayCommand(param => AddBook(), param => CanAddBook);
                }
                return addBookCommand;
            }
        }

        public bool CanAddBook
        {
            get
            {
                return true;
            }
        }

        private BookRelation FromRelation()
        {
            return BookRelation.CreateAbstract();
        }

        public void AddBook()
        {
            BookVM newBook = StudyUnit.AddBook(FromRelation());
            UpdateSelectedItem(newBook);
        }

        private ICommand editBookCommand;
        public ICommand EditBookCommand
        {
            get
            {
                if (editBookCommand == null)
                {
                    editBookCommand = new RelayCommand(param => EditBook(), param => CanEditBook);
                }
                return editBookCommand;
            }
        }

        public bool CanEditBook
        {
            get
            {
                return SelectedBook != null;
            }
        }

        private void UpdateSelectedItem(BookVM newBook)
        {
            if (newBook != null)
            {
                SelectedBookItem = books.Contains(newBook) ? newBook: null;
            }
        }

        public void EditBook()
        {
            BookVM newBook = StudyUnit.EditBook(SelectedBook);
            UpdateSelectedItem(newBook);
        }

        private ICommand removeBookCommand;
        public ICommand RemoveBookCommand
        {
            get
            {
                if (removeBookCommand == null)
                {
                    removeBookCommand = new RelayCommand(param => RemoveBook(), param => CanRemoveBook);
                }
                return removeBookCommand;
            }
        }

        public bool CanRemoveBook
        {
            get
            {
                return SelectedBook != null;
            }
        }

        public void RemoveBook()
        {
            StudyUnit.RemoveBook(SelectedBook);
        }

        private ICommand selectBookCommand;

        public ICommand SelectBookCommand
        {
            get
            {
                if (selectBookCommand == null)
                {
                    selectBookCommand = new RelayCommand(param => SelectBook(), param => CanSelectBook);
                }
                return selectBookCommand;
            }
        }

        public bool CanSelectBook
        {
            get
            {
                return true;
            }
        }

        public void SelectBook()
        {
            BookVM book = StudyUnit.SelectBook(FromRelation());
            if (book != null)
            {
                SelectedBookItem = book;
            }
        }
    }
}
