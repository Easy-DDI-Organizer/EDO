using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
using EDO.DataCategory.BookForm;
using System.Windows.Input;
using EDO.Core.Util;

namespace EDO.QuestionCategory.ConceptForm
{
    public class ConceptVM : BaseVM, IEditableObject, IStringIDProvider, ITitleProvider
    {
        public static ConceptVM Find(ICollection<ConceptVM> concepts, string conceptId)
        {
            if (conceptId == null)
            {
                return null;
            }
            foreach (ConceptVM concept in concepts)
            {
                if (concept.Id == conceptId)
                {
                    return concept;
                }
            }
            return null;
        }

        public ConceptVM()
            : this(new Concept())
        {
        }

        private Concept concept;
        private Concept bakConcept;

        public ConceptVM(Concept concept)
        {
            this.concept = concept;
            this.books = new ObservableCollection<BookVM>();
        }

        public Concept Concept { get { return concept; } }

        public override object Model
        {
            get
            {
                return concept;
            }
        }

        private ObservableCollection<BookVM> books;
        public ObservableCollection<BookVM> Books { get { return books; } }

        public string Id { get { return concept.Id; } }

        public string Title
        {
            get
            {
                return concept.Title;
            }
            set
            {
                if (concept.Title != value)
                {
                    concept.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Content {
            get
            {
                return concept.Content;
            }
            set
            {
                if (concept.Content != value)
                {
                    concept.Content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        #region IEditableObject メンバー

        public bool InEdit { get { return inEdit; } }

        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit)
            {
                return;
            }
            inEdit = true;

            bakConcept = concept.Clone() as Concept;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            this.Title = bakConcept.Title;
            this.Content = bakConcept.Content;
        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            bakConcept = null;
            Memorize();
        }

        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = EMPTY_VALUE;
            }
        }

        #endregion

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
            return BookRelation.CreateConcept(Id);
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
                SelectedBookItem = books.Contains(newBook) ? newBook : null;
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
