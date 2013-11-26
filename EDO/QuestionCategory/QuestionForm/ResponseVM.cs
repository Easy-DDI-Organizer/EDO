using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.QuestionCategory.CodeForm;
using EDO.QuestionCategory.CategoryForm;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using EDO.Properties;
using EDO.Core.Util;
using System.Windows;
using EDO.Core.Model.Layout;
using EDO.DataCategory.BookForm;

namespace EDO.QuestionCategory.QuestionForm
{
    public class ResponseVM :BaseVM, ISelectableObject
    {
        public static readonly string EMPTY_TITLE = Resources.UntitledResponse; //無題の回答方法;

        private Response response;

        public ResponseVM()
            : this(new Response() { TypeCode = null}, null)
        {
        }

        public ResponseVM(Response response)
            : this(response, null)
        {
        }

        public ResponseVM(Response response, CodeSchemeVM codeScheme)
        {
            this.response = response;
            this.isQuestionDesignMode = true;
            if (codeScheme == null)
            {
                codeScheme = new CodeSchemeVM();
                codeScheme.OwnerResponse = this;
            }
            books = new ObservableCollection<BookVM>();
            Init(response.DetailTypeCode, response.Min, response.Max, codeScheme, new List<MissingValue>(response.MissingValues), response.Layout);
        }

        public Response Response { get { return response; } }

        public string Id { get { return response.Id; } }

        public bool IsTypeChoices { get { return response.IsTypeChoices; } }
        public bool IsTypeUnknown { get { return response.IsTypeUnknown; } }
        public bool IsTypeNumber { get { return response.IsTypeNumber; } }
        public bool IsTypeFree { get { return response.IsTypeFree; } }
        public bool IsTypeDateTime { get { return response.IsTypeDateTime; } }

        private ResponseLayoutVM layout;
        public ResponseLayoutVM Layout { get { return layout; } }

        private ObservableCollection<BookVM> books;
        public ObservableCollection<BookVM> Books { get { return books; } }

        private bool isQuestionDesignMode;
        public bool IsQuestionDesignMode
        {
            get
            {
                return isQuestionDesignMode;
            }
            set
            {
                if (isQuestionDesignMode != value)
                {
                    isQuestionDesignMode = value;
                    NotifyPropertyChanged("IsQuestionDesignMode");
                    NotifyPropertyChanged("Header");
                }
            }
        }

        public string Title
        {
            get
            {
                return response.Title;
            }
            set
            {
                if (response.Title != value)
                {
                    //if (string.IsNullOrEmpty(value)) {
                    //    value = EMPTY_TITLE;
                    //}
                    response.Title = value;
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public void SetTitle(string value)
        {
            if (response.Title != value)
            {
                //if (string.IsNullOrEmpty(value)) {
                //    value = EMPTY_TITLE;
                //}
                response.Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        private string questionTitle;
        public string QuestionTitle {
            get
            {
                return questionTitle;
            }
            set
            {
                if (questionTitle != value)
                {
                    questionTitle = value;
                    NotifyPropertyChanged("QuestionTitle");
                    NotifyPropertyChanged("Header");
                }
            }
        }

        public string Header
        {
            get
            {
                string questionTitle = this.QuestionTitle;
                if (string.IsNullOrEmpty(questionTitle))
                {
                    questionTitle = this.IsQuestionDesignMode ? Resources.Question : Resources.Variable; //質問 : 変数;
                }
                string suffix = this.IsQuestionDesignMode ? Resources.ResponseMethod : Resources.TypeDetail; //回答方法 : 型の詳細;
                return questionTitle + Resources.Possession + suffix;
            }
        }

        public string TypeCode
        {
            get
            {
                return response.TypeCode;
            }
            set
            {
                if (response.TypeCode != value)
                {
                    if (!StudyUnit.ConfirmChangeType(this))
                    {
                        return;
                    }

                    response.TypeCode = value;
                    response.Title = null;
                    Init();
                    NotifyPropertyChanged("Title");
                    NotifyPropertyChanged("TypeCode");
//                    Memorize();
                }
            }
        }

        public string TypeName
        {
            //コードブックで利用
            get
            {
                return Option.FindLabel(Options.ResponseTypes, response.TypeCode);
            }
        }

        #region 選択肢

        private CodeSchemeVM codeScheme;

        public CodeSchemeVM CodeScheme { 
            get 
            { 
                return codeScheme; 
            } 
            set 
            {
                Debug.Assert(value != null);
                codeScheme = value;                
                response.CodeSchemeId = codeScheme.Id;
            } 
        }

        public ObservableCollection<CodeVM> Codes { get { return codeScheme.Codes; } }

        public List<CodeVM> ValidCodes
        {
            get
            {
                List<CodeVM> codes = new List<CodeVM>();
                foreach (CodeVM code in Codes)
                {
                    if (code.IsValid)
                    {
                        codes.Add(code);
                    }
                }
                return codes;
            }
        }

        #endregion

        #region 日付・数値

        public ObservableCollection<Option> DetailTypes { get; set; }

        public string DetailTypeCode {
            get
            {
                return response.DetailTypeCode;
            }
            set
            {
                if (response.DetailTypeCode != value)
                {
                    response.DetailTypeCode = value;
                    NotifyPropertyChanged("DetailTypeCode");
                    Memorize();
                }
            }
        }

        #endregion

        #region 自由記述・数値

        public decimal? Max
        {
            get
            {
                return response.Max;
            }
            set
            {
                if (response.Max != value)
                {
                    response.Max = value;
                    NotifyPropertyChanged("Max");
                    Memorize();
                }
            }
        }

        public decimal? Min
        {
            get
            {
                return response.Min;
            }
            set
            {
                if (response.Min != value)
                {
                    response.Min = value;
                    NotifyPropertyChanged("Min");
                    Memorize();
                }
            }
        }
        #endregion

        private void Clear()
        {
            codeScheme = null;
            Min = null;
            Max = null;
            DetailTypes.Clear();
        }

        private ResponseLayoutVM CreateLayout(Response response, ResponseLayout layoutModel)
        {
            ResponseLayout newLayoutModel = layoutModel;
            ResponseLayoutVM layout = null;
            if (response.IsTypeChoices)
            {
                if (newLayoutModel as ChoicesLayout == null)
                {
                    newLayoutModel = new ChoicesLayout();
                }
                layout = new ChoicesLayoutVM((ChoicesLayout)newLayoutModel);
            }
            else if (response.IsTypeDateTime)
            {
                if (newLayoutModel as DateTimeLayout == null)
                {
                    newLayoutModel = new DateTimeLayout();
                }
                layout = new DateTimeLayoutVM((DateTimeLayout)newLayoutModel);
            }
            else if (response.IsTypeFree)
            {
                if (newLayoutModel as FreeLayout == null)
                {
                    newLayoutModel = new FreeLayout();
                }
                layout = new FreeLayoutVM((FreeLayout)newLayoutModel);
            }
            else if (response.IsTypeNumber)
            {
                if (newLayoutModel as NumericLayout  == null)
                {
                    newLayoutModel = new NumericLayout();
                }
                layout = new NumericLayoutVM((NumericLayout)newLayoutModel);
            }
            else
            {
                if (newLayoutModel as UnknownLayout == null)
                {
                    newLayoutModel = new UnknownLayout();
                }
                layout = new UnknownLayoutVM((UnknownLayout)newLayoutModel);
            }
            layout.Parent = this;
            return layout;
        }

        private void Init(string detailTypeCode, decimal? min, decimal? max, CodeSchemeVM codeScheme, List<MissingValue> missingValueModels, ResponseLayout layoutModel)
        {
            DetailTypeCode = detailTypeCode;
            Min = min;
            Max = max;
            CodeScheme = codeScheme;

            DetailTypes = new ObservableCollection<Option>();
            if (response.IsTypeChoices)
            {
            } else if (response.IsTypeDateTime)
            {
                DetailTypes = Options.DateTimeTypes;
            }
            else if (response.IsTypeFree)
            {
            }
            else if (response.IsTypeNumber)
            {
                DetailTypes = Options.NumberTypes;
            }

            missingValues = new ObservableCollection<MissingValueVM>();
            foreach (MissingValue mv in missingValueModels)
            {
                MissingValueVM missingValue = new MissingValueVM(mv) { Parent = this };
                missingValues.Add(missingValue);
            }
            modelSyncher = new ModelSyncher<MissingValueVM, MissingValue>(this, missingValues, response.MissingValues);


            ResponseLayout newLayoutModel = layoutModel;

            layout = CreateLayout(response, layoutModel);
            response.Layout = layout.Layout;
        }


        private ModelSyncher<MissingValueVM, MissingValue> modelSyncher;
        private ObservableCollection<MissingValueVM> missingValues;
        public ObservableCollection<MissingValueVM> MissingValues { get { return missingValues; } }

        private void Init()
        {
            //回答方法の種類の切り替え時に呼び出される。画面を初期化しなおす
            Init(null, null, null, new CodeSchemeVM() { OwnerResponse = this }, new List<MissingValue>(), null);
        }


        public ResponseVM Dup()
        {
            //新しいIDを割り当るなど通常のCloneと意味合いが違うので別の名前をつける。
            Response newResponseModel = response.Dup();
            //カテゴリースキーム・コードスキームは再利用
            ResponseVM newResponse = new ResponseVM(newResponseModel, codeScheme);
            newResponse.Parent = Parent; //InitでStudyUnitにアクセスするのでParentの設定が必要
            return newResponse;
        }

        private void CheckChoicesValue(string value)
        {
            bool found = false;
            foreach (CodeVM code in codeScheme.Codes)
            {
                if (code.Value == value)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                throw new ApplicationException(Resources.InvalidCategoryCode);//選択肢のコードが見つかりません
            }
        }

        private void CheckNumberValue(string value)
        {
        }

        private void CheckFreeValue(string value)
        {
        }

        private void CheckDateTimeValue(string value)
        {
        }

        public void CheckaResponseValue(string value)
        {
            if (IsTypeChoices)
            {
                CheckChoicesValue(value);
            }
            else if (IsTypeNumber)
            {
                CheckNumberValue(value);
            }
            else if (IsTypeFree)
            {
                CheckFreeValue(value);
            }
            else if (IsTypeDateTime)
            {
                CheckDateTimeValue(value);
            }
        }

        private object selectedMissingValueItem;
        public object SelectedMissingValueItem
        {
            get
            {
                return selectedMissingValueItem;
            }
            set
            {
                if (selectedMissingValueItem != value)
                {
                    selectedMissingValueItem = value;
                    NotifyPropertyChanged("SelectedMissingValueItem");
                }
            }
        }

        public MissingValueVM SelectedMissingValue
        {
            get
            {
                return SelectedMissingValueItem as MissingValueVM;
            }
        }

        private ICommand removeMissingValueCommand;
        public ICommand RemoveMissingValueCommand
        {
            get
            {
                if (removeMissingValueCommand == null)
                {
                    removeMissingValueCommand = new RelayCommand(param => RemoveMissingValue(), param => CanRemoveMissingValue);
                }
                return removeMissingValueCommand;
            }
        }

        public bool CanRemoveMissingValue
        {
            get
            {
                if (SelectedMissingValue == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveMissingValue()
        {
            MissingValues.Remove(SelectedMissingValue);
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    NotifyPropertyChanged("SelectedIndex");
                }
            }
        }

        private string questionId;
        public string ParentId
        {
            get
            {
                return questionId;
            }
            set
            {
                if (questionId != value)
                {
                    questionId = value;
                    NotifyPropertyChanged("QuestionId");
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
            return IsQuestionDesignMode ? BookRelation.CreateQuestion(ParentId) : BookRelation.CreateVariable(ParentId);
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
