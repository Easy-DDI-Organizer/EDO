using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using EDO.Core.Util;
using System.Windows.Input;
using System.Windows;
using EDO.Properties;

namespace EDO.DataCategory.BookForm
{
    public class EditBookWindowVM :BaseVM
    {
        private static ObservableCollection<BookFieldVM> CreateBookFields(Book book)
        {
            ObservableCollection<BookFieldVM> fields = new ObservableCollection<BookFieldVM>();
            BookFieldVM title = new BookFieldVM(book, Resources.Title, "Title");
            BookFieldVM author = new BookFieldVM(book, Resources.Author, "Author");
            BookFieldVM editor = new BookFieldVM(book, Resources.Editor, "Editor");
            BookFieldVM announcementDate = new BookFieldVM(book, Resources.AnnouncementDate, "AnnouncementDate");
            BookFieldVM startPage = new BookFieldVM(book, Resources.StartPage, "StartPage");
            BookFieldVM endPage = new BookFieldVM(book, Resources.EndPage, "EndPage");
            BookFieldVM publisher = new BookFieldVM(book, Resources.Publisher, "Publisher");
            BookFieldVM city = new BookFieldVM(book, Resources.City, "City");
            BookFieldVM bookName = new BookFieldVM(book, Resources.BookName, "BookName");
            BookFieldVM magazineName = new BookFieldVM(book, Resources.MagazineName, "MagazineName");
            BookFieldVM volume = new BookFieldVM(book, Resources.Volume, "Volume");
            BookFieldVM number = new BookFieldVM(book, Resources.BookNumber, "Number");
            BookFieldVM chapter = new BookFieldVM(book, Resources.Chapter, "Chapter");
            BookFieldVM universityName = new BookFieldVM(book, Resources.UniversityName, "UniversityName");
            BookFieldVM departmentName = new BookFieldVM(book, Resources.DepartmentName, "DepartmentName");
            BookFieldVM summary = new BookFieldVM(book, Resources.Summary, "Summary");
            BookFieldVM url = new BookFieldVM(book, "URL", "Url");
            BookFieldVM language = new BookFieldVM(book, Resources.Language, "Language");
            if (book.IsBookTypeBook)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(editor);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(publisher);
                fields.Add(city);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            else if (book.IsBookTypeBookChapter)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(editor);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(publisher);
                fields.Add(city);
                fields.Add(bookName);
                fields.Add(chapter);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            } 
            else if (book.IsBookTypeTreatiseWithPeerReview)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(publisher);
                fields.Add(city);
                fields.Add(magazineName);
                fields.Add(volume);
                fields.Add(number);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            else if (book.IsBookTypeTreatiseWithoutPeerReview)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(publisher);
                fields.Add(city);
                fields.Add(magazineName);
                fields.Add(volume);
                fields.Add(number);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            else if (book.IsBookTypeSocietyAbstract)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(editor);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(publisher);
                fields.Add(city);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            else if (book.IsBookTypeReport)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(editor);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(publisher);
                fields.Add(city);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            else if (book.IsBookTypeThesis)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(universityName);
                fields.Add(departmentName);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            else if (book.IsBookTypeWebpage)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(announcementDate);
                fields.Add(publisher);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            else if (book.IsBookTypeOther)
            {
                fields.Add(title);
                fields.Add(author);
                fields.Add(editor);
                fields.Add(announcementDate);
                fields.Add(startPage);
                fields.Add(endPage);
                fields.Add(publisher);
                fields.Add(city);
                fields.Add(summary);
                fields.Add(url);
                fields.Add(language);
            }
            return fields;
        }


        public EditBookWindowVM(Book book)
        {
            if (book == null)
            {
                book = new Book() { BookTypeCode = Options.BOOK_TYPE_BOOK };
            }
            else
            {
                book = book.DeepCopy(true);
            }
            Book = book;
            bookTypes = Options.BookTypes;
            bookFields = new ObservableCollection<BookFieldVM>();
            InitBookFiels();

            bookRelations = new ObservableCollection<BookRelationVM>();

        }

        public Book Book { get; set; }

        public Window Window { get; set; }

        private ObservableCollection<Option> bookTypes;
        public ObservableCollection<Option> BookTypes { get { return bookTypes; } }

        private ObservableCollection<BookRelationVM> bookRelations;
        public ObservableCollection<BookRelationVM> BookRelations { get { return bookRelations; } }

        private ModelSyncher<BookRelationVM, BookRelation> modelSyncher;

        public void Init(BookRelation fromRelation)
        {
            foreach (BookRelation relationModel in Book.BookRelations)
            {
                BookRelationVM relation = CreateRelation(relationModel);
                bookRelations.Add(relation);
            }
            if (fromRelation != null && FindExistRelation(fromRelation) == null)
            {
                Book.BookRelations.Add(fromRelation);
                bookRelations.Add(CreateRelation(fromRelation));
            }
            modelSyncher = new ModelSyncher<BookRelationVM, BookRelation>(this, bookRelations, Book.BookRelations);
        }

        private BookRelationVM CreateRelation(BookRelation relationModel)
        {
            ObservableCollection<ITitleProvider> objects = StudyUnit.RelatedMetaData(relationModel.BookRelationType);
            string typeName = BookRelationItem.GetLabel(relationModel.BookRelationType);
            ITitleProvider obj = SelectMetaDataWindowVM.Find(objects, relationModel.MetadataId);
            string metaDataName = null;
            if (obj != null)
            {
                metaDataName = obj.Title;
            }
            BookRelationVM relation = new BookRelationVM(relationModel)
            {
                Parent = this,
                TypeName = typeName,
                MetaDataName = metaDataName
            };
            return relation;
        }

        private void InitBookFiels() 
        {
            bookFields.Clear();
            ObservableCollection<BookFieldVM> newBookFields = CreateBookFields(Book);
            bookFields.AddRange(newBookFields);
        }

        public string BookTypeCode
        {
            get
            {
                return Book.BookTypeCode;
            }
            set
            {
                if (Book.BookTypeCode != value)
                {
                    UpdateModel();
                    Book.BookTypeCode = value;
                    InitBookFiels();
                    NotifyPropertyChanged("BookTypeCode");
                }
            }
        }

        private ObservableCollection<BookFieldVM> bookFields;
        public ObservableCollection<BookFieldVM> BookFields { get { return bookFields; } }


        public void UpdateModel()
        {
            foreach (BookFieldVM bookField in bookFields)
            {
                bookField.UpdateModel();
            }
        }

        public void Validate()
        {
            UpdateModel();
            if (string.IsNullOrEmpty(Book.Title))
            {
                throw new ApplicationException(Resources.InputTitle);
            }
        }

        private object selectedRelationItem;
        public object SelectedRelationItem
        {
            get
            {
                return selectedRelationItem;
            }
            set
            {
                if (selectedRelationItem != value)
                {
                    selectedRelationItem = value;
                    NotifyPropertyChanged("SelectedRelationItem");
                }
            }
        }

        public BookRelationVM SelectedRelation
        {
            get
            {
                return SelectedRelationItem as BookRelationVM;
            }
        }


        private ICommand addRelationCommand;
        public ICommand AddRelationCommand
        {
            get
            {
                if (addRelationCommand == null)
                {
                    addRelationCommand = new RelayCommand(param => AddRelation(), param => CanAddRelation);
                }
                return addRelationCommand;
            }
        }

        public bool CanAddRelation
        {
            get
            {
                return true;
            }
        }

        private BookRelationVM FindExistRelation(BookRelation relationModel)
        {
            foreach (BookRelationVM relation in bookRelations)
            {
                BookRelation thisRelationModel = relation.BookRelation;
                if (thisRelationModel.BookRelationType == relationModel.BookRelationType
                    && thisRelationModel.MetadataId == relationModel.MetadataId)
                {
                    return relation;
                }
            }
            return null;
        }

        private BookRelationVM SelectRelation(BookRelation selectedRelationModel)
        {
            BookRelationType type = BookRelationType.Abstract;
            string metaDataId = null;
            if (selectedRelationModel != null)
            {
                type = selectedRelationModel.BookRelationType;
                metaDataId = selectedRelationModel.MetadataId;
            }
            SelectMetaDataWindowVM vm = new SelectMetaDataWindowVM(type, metaDataId) { Parent = this };
            vm.Init();
            SelectMetaDataWindow window = new SelectMetaDataWindow(vm);
            window.Owner = Window;
            BookRelationVM relation = null;
            if (window.ShowDialog() == true)
            {
                BookRelation relationModel = new BookRelation();
                relationModel.BookRelationType = vm.BookRelationType;
                ITitleProvider selectedObject = vm.SelectedObject as ITitleProvider;
                if (selectedObject != null)
                {
                    relationModel.MetadataId = selectedObject.Id;
                }
                relation = FindExistRelation(relationModel);
                if (relation == null)
                {
                    relation = CreateRelation(relationModel);
                }
            }
            return relation;
        }


        public void AddRelation()
        {
            BookRelationVM newRelation = SelectRelation(null);
            if (newRelation != null)
            {
                if (!bookRelations.Contains(newRelation))
                {
                    bookRelations.Add(newRelation);
                }
                SelectedRelationItem = newRelation;
            }
        }

        private ICommand editRelationCommand;
        public ICommand EditRelationCommand
        {
            get
            {
                if (editRelationCommand == null)
                {
                    editRelationCommand = new RelayCommand(param => EditRelation(), param => CanEditRelation);
                }
                return editRelationCommand;
            }
        }

        public bool CanEditRelation
        {
            get
            {
                return SelectedRelation != null;
            }
        }

        public void EditRelation()
        {
            if (SelectedRelation == null)
            {
                return;
            }
            BookRelationVM newRelation = SelectRelation(SelectedRelation.BookRelation);
            if (newRelation != null)
            {
                if (!bookRelations.Contains(newRelation))
                {
                    int index = bookRelations.IndexOf(SelectedRelation);
                    bookRelations.RemoveAt(index);
                    bookRelations.Insert(index, newRelation);
                }
                SelectedRelationItem = newRelation;
            }
        }

        private ICommand removeRelationCommand;
        public ICommand RemoveRelationCommand
        {
            get
            {
                if (removeRelationCommand == null)
                {
                    removeRelationCommand = new RelayCommand(param => RemoveRelation(), param => CanRemoveRelation);
                }
                return removeRelationCommand;
            }
        }

        public bool CanRemoveRelation
        {
            get
            {
                return SelectedRelation != null;
            }
        }

        public void RemoveRelation()
        {
            bookRelations.Remove(SelectedRelation);
        }
    }
}
