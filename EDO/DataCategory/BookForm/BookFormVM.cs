using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Main;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using EDO.Core.Util;
using System.Windows.Input;
using EDO.Core.View;
using EDO.QuestionCategory.QuestionForm;
using EDO.QuestionCategory.ConceptForm;
using EDO.VariableCategory.VariableForm;
using System.Windows;
using EDO.Properties;

namespace EDO.DataCategory.BookForm
{
    public class BookFormVM : FormVM
    {
        public BookFormVM(StudyUnitVM studyUnit)
            : base(studyUnit)
        {
            books = new ObservableCollection<BookVM>();
            foreach (Book bookModel in studyUnit.BookModels)
            {
                BookVM book = new BookVM(bookModel) { Parent = this };
                books.Add(book);
            }
            modelSyncher = new ModelSyncher<BookVM, Book>(this, books, studyUnit.BookModels);
        }

        private ModelSyncher<BookVM, Book> modelSyncher;
        private ObservableCollection<BookVM> books;
        public ObservableCollection<BookVM> Books { get { return books; } }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedBookItem = EDOUtils.FindOrFirst(books, state.State1);
            }
        }

        public override VMState SaveState()
        {
            VMState state = new VMState();
            if (SelectedBook != null)
            {
                state.State1 = SelectedBook.Id;
            }
            return state;
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

        private BookVM AddOrEditBook(Book book, BookRelation relation)
        {
            EditBookWindowVM vm = new EditBookWindowVM(book) { Parent = this };
            vm.Init(relation);
            EditBookWindow window = new EditBookWindow(vm);
            window.Owner = Application.Current.MainWindow;
            BookVM newBook = null;
            if (window.ShowDialog() == true)
            {
                newBook = new BookVM(vm.Book) { Parent = this };

            }
            window.Content = null;
            return newBook;
        }

        public void AddBook()
        {
            BookVM newBook = AddBookExternal(null);
            if (newBook != null)
            {
                SelectedBookItem = newBook;
            }
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

        public void EditBook()
        {
            BookVM newBook = EditBookExternal(SelectedBook);
            if (newBook != null)
            {
                SelectedBookItem = newBook;
            }
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
            RemoveBookExternal(SelectedBook);
        }

        public List<BookVM> GetBooks(BookRelation relation)
        {
            List<BookVM> resultBooks = new List<BookVM>();
            foreach (BookVM book in books)
            {
                if (book.ContainsRelation(relation))
                {
                    resultBooks.Add(book);
                }
            }
            return resultBooks;
        }

        public BookVM AddBookExternal(BookRelation relation)
        {
            BookVM newBook = AddOrEditBook(null, relation);
            if (newBook != null)
            {
                using (UndoTransaction tx = new UndoTransaction(UndoManager))
                {
                    books.Add(newBook);
                    StudyUnit.OnRemoveBooks();
                    tx.Commit();
                }
            }
            return newBook;
        }

        public BookVM EditBookExternalTest(BookVM targetBook)
        {
            if (targetBook == null)
            {
                return null;
            }
            EditBookWindowVM vm = new EditBookWindowVM(targetBook.Book) { Parent = this };
            vm.Init(null);
            EditBookWindow window = new EditBookWindow(vm);
            BookVM newBook = null;
            if (window.ShowDialog() == true)
            {
                newBook = new BookVM(vm.Book) { Parent = this };
            }
            window.Content = null;
            return newBook;
        }


        public BookVM EditBookExternal(BookVM targetBook)
        {
            if (targetBook == null)
            {
                return null;
            }
            BookVM newBook = AddOrEditBook(targetBook.Book, null);
            if (newBook != null)
            {
                using (UndoTransaction tx = new UndoTransaction(UndoManager))
                {
                    int index = books.IndexOf(targetBook);
                    books.RemoveAt(index);
                    books.Insert(index, newBook);
                    StudyUnit.OnRemoveBooks();
                    tx.Commit();
                }
            }
            return newBook;
        }

        public void RemoveBookExternal(BookVM targetBook)
        {
            if (targetBook == null)
            {
                return;
            }
            using (UndoTransaction tx = new UndoTransaction(UndoManager))
            {
                books.Remove(targetBook);
                StudyUnit.OnRemoveBooks();
                tx.Commit();
            }
            if (SelectedBook == targetBook)
            {
                SelectedBookItem = null;
            }
        }

        public BookVM SelectBookExternal(BookRelation relation)
        {
            SelectObjectWindowVM<BookVM> vm = new SelectObjectWindowVM<BookVM>(books);
            SelectObjectWindow window = new SelectObjectWindow(vm);
            BookVM book = SelectObjectWindow.Select(Resources.SelectBook, vm) as BookVM; //文献の選択
            if (book != null)
            {
                using (UndoTransaction tx = new UndoTransaction(UndoManager))
                {
                    if (book.Book.FindRelation(relation) == null)
                    {
                        //ViewModelはEditBookWindowが表示されるたびに作りなおされるのでここで生成する必要はない。
                        book.Book.BookRelations.Add(relation);
                    }
                    StudyUnit.OnRemoveBooks();
                    tx.Commit();
                }
            }
            return book;
        }

        private void RemoveBookRelationOfMetaData(string metaDataId)
        {
            foreach (BookVM book in books)
            {
                for (int i = book.Book.BookRelations.Count - 1; i >= 0; i--)
                {
                    BookRelation relation = book.Book.BookRelations[i];
                    if (relation.MetadataId == metaDataId)
                    {
                        book.Book.BookRelations.RemoveAt(i);
                    }
                }
            }
        }

        public void RemoveQuestion(ConceptVM concept)
        {
            RemoveBookRelationOfMetaData(concept.Id);
        }

        public void OnRemoveQuestion(QuestionVM question)
        {
            RemoveBookRelationOfMetaData(question.Id);
        }

        public void OnRemoveConcepts(List<ConceptVM> concepts)
        {
            foreach (ConceptVM concept in concepts)
            {
                OnRemoveConcept(concept);
            }
        }

        public void OnRemoveConcept(ConceptVM concept)
        {
            RemoveBookRelationOfMetaData(concept.Id);
        }

        public void OnRemoveVariable(VariableVM variable)
        {
            RemoveBookRelationOfMetaData(variable.Id);
        }
    }
}
