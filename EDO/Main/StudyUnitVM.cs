using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EDO.EventCategory.EventForm;
using System.Collections.ObjectModel;
using EDO.StudyCategory.MemberForm;
using EDO.StudyCategory.AbstractForm;
using EDO.StudyCategory.CoverageForm;
using EDO.StudyCategory.FundingInfoForm;
using EDO.SamplingCategory.SamplingForm;
using EDO.QuestionCategory.ConceptForm;
using EDO.QuestionCategory.CodeForm;
using EDO.QuestionCategory.CategoryForm;
using EDO.VariableCategory.VariableForm;
using EDO.DataCategory.DataSetForm;
using EDO.DataCategory.DataFileForm;
using System.Windows.Input;
using EDO.Core.ViewModel;
using EDO.Core.View;
using EDO.Core.Model;
using EDO.QuestionCategory.QuestionForm;
using System.Diagnostics;
using EDO.Core.Util;
using EDO.QuestionCategory.SequenceForm;
using EDO.QuestionCategory.QuestionGroupForm;
using System.Windows;
using EDO.DataCategory.BookForm;
using EDO.Properties;

namespace EDO.Main
{
    public class StudyUnitVM :EDOUnitVM, IOrderedObject
    {

        public static List<string> GetAllPathNames(List<StudyUnitVM> studyUnits)
        {
            List<string> pathNames = new List<string>();
            foreach (StudyUnitVM studyUnit in studyUnits)
            {
                pathNames.Add(studyUnit.PathName);
            }
            return pathNames;
        }

        public static List<string> GetStudyUnitGuids(List<StudyUnitVM> studyUnits)
        {
            List<string> guids = new List<string>();
            foreach (StudyUnitVM studyUnit in studyUnits)
            {
                guids.Add(studyUnit.Id);
            }
            return guids;
        }

        public static StudyUnitVM Find(List<StudyUnitVM> studyUnits, string studyUnitId)
        {
            foreach (StudyUnitVM studyUnit in studyUnits)
            {
                if (studyUnit.Id == studyUnitId)
                {
                    return studyUnit;
                }
            }
            return null;
        }

        private MemberFormVM memberForm;

        private AbstractFormVM abstractForm;

        private CoverageFormVM coverageForm;

        private FundingInfoFormVM fundingInfoForm;

        private SamplingFormVM samplingForm;

        private ConceptFormVM conceptForm;

        private CategoryFormVM categoryForm;

        private CodeFormVM codeForm;

        private QuestionFormVM questionForm;

        private SequenceFormVM sequenceForm;

        private QuestionGroupFormVM questionGroupForm;

        private VariableFormVM variableForm;

        private DataSetFormVM dataSetForm;

        private DataFileFormVM dataFileForm;

        private BookFormVM bookForm;

        public StudyUnitVM(MainWindowVM mainWindowVM, StudyUnit studyUnit) :base(mainWindowVM, studyUnit)
        {
            this.studyUnit = studyUnit;
 
            EventFormVM eventFormViewModel = new EventFormVM(this);
            MenuItemVM categoryEvent = new MenuItemVM(MenuElem.C_EVENT, eventFormViewModel); //1
            MenuItemVM menuEvent = new MenuItemVM(MenuElem.M_EVENT, eventFormViewModel); //2
            menuEvent.IsSelected = true;
            categoryEvent.Add(menuEvent);
            this.MenuItems.Add(categoryEvent);

            memberForm = new MemberFormVM(this);
            abstractForm = new AbstractFormVM(this);
            coverageForm = new CoverageFormVM(this);
            fundingInfoForm = new FundingInfoFormVM(this);

            MenuItemVM categoryStudy = new MenuItemVM(MenuElem.C_STUDY, memberForm); //3
            MenuItemVM menuStudyMember = new MenuItemVM(MenuElem.M_MEMBER, memberForm); //4
            MenuItemVM menuStudyAbstract = new MenuItemVM(MenuElem.M_ABSTRACT, abstractForm); //5
            MenuItemVM menuStudyRange = new MenuItemVM(MenuElem.M_COVERAGE, coverageForm); //6
            MenuItemVM menuFund = new MenuItemVM(MenuElem.M_FUNDING_INFO, fundingInfoForm); //7
            categoryStudy.Add(menuStudyMember);
            categoryStudy.Add(menuStudyAbstract);
            categoryStudy.Add(menuStudyRange);
            categoryStudy.Add(menuFund);
            this.MenuItems.Add(categoryStudy);

            samplingForm = new SamplingFormVM(this);

            MenuItemVM categoryScheme = new MenuItemVM(MenuElem.C_SAMPLING, samplingForm); //8
            MenuItemVM menuScheme = new MenuItemVM(MenuElem.M_SAMPLING, samplingForm); //9
            categoryScheme.Add(menuScheme);
            this.MenuItems.Add(categoryScheme);

            conceptForm = new ConceptFormVM(this);
            categoryForm = new CategoryFormVM(this);
            codeForm = new CodeFormVM(this);
            questionForm = new QuestionFormVM(this); //QuestionFormはconcept、category、codeよりあとに生成する必要あり
            questionGroupForm = new QuestionGroupFormVM(this); // QuestionGroupFormは、SequenceFormより先に生成する必要あり
            sequenceForm = new SequenceFormVM(this);

            MenuItemVM categoryQuestion = new MenuItemVM(MenuElem.C_QUESTION, conceptForm); //10
            MenuItemVM menuConcept = new MenuItemVM(MenuElem.M_CONCEPT, conceptForm); //11
            MenuItemVM menuQuestion = new MenuItemVM(MenuElem.M_QUESTION, questionForm); //12
            MenuItemVM menuCategory = new MenuItemVM(MenuElem.M_CATEGORY, categoryForm); //13
            MenuItemVM menuCode = new MenuItemVM(MenuElem.M_CODE, codeForm); //14
            MenuItemVM menuSequence = new MenuItemVM(MenuElem.M_SEQUENCE, sequenceForm); //15
            MenuItemVM menuQuestionGroup = new MenuItemVM(MenuElem.M_QUESTION_GROUP, questionGroupForm);
            categoryQuestion.Add(menuConcept);
            categoryQuestion.Add(menuQuestion);
            categoryQuestion.Add(menuCategory);
            categoryQuestion.Add(menuCode);
            categoryQuestion.Add(menuSequence);
            categoryQuestion.Add(menuQuestionGroup);
            this.MenuItems.Add(categoryQuestion);

            variableForm = new VariableFormVM(this);
            MenuItemVM categoryVariable = new MenuItemVM(MenuElem.C_VARIABLE, variableForm); //16
            MenuItemVM menuVariable = new MenuItemVM(MenuElem.M_VARIABLE, variableForm); //17
            categoryVariable.Add(menuVariable);
            this.MenuItems.Add(categoryVariable);

            dataSetForm = new DataSetFormVM(this);
            dataFileForm = new DataFileFormVM(this);
            bookForm = new BookFormVM(this);
            MenuItemVM categoryData = new MenuItemVM(MenuElem.C_DATA, dataSetForm); //18
            MenuItemVM menuDataRelation = new MenuItemVM(MenuElem.M_DATA_SET, dataSetForm); //19
            MenuItemVM menuPhysicalStructure = new MenuItemVM(MenuElem.M_DATA_FILE, dataFileForm); //20
            MenuItemVM menuBooks = new MenuItemVM(MenuElem.M_BOOKS, bookForm);
            categoryData.Add(menuDataRelation);
            categoryData.Add(menuPhysicalStructure);
            categoryData.Add(menuBooks);
            this.MenuItems.Add(categoryData);
            this.SelectedMenuItem = categoryEvent;

            OnRemoveBooks();
        }

        public MemberFormVM MemberForm { get { return memberForm; } }

        public AbstractFormVM AbstractForm { get { return abstractForm; } }

        public FundingInfoFormVM FundingInfoForm { get { return fundingInfoForm; } }

        public CoverageFormVM CoverageForm { get { return coverageForm; } }

        public SamplingFormVM SamplingForm { get { return samplingForm; } }

        public QuestionFormVM QuestionForm { get { return questionForm; } }

        public SequenceFormVM SequenceForm { get { return sequenceForm; } }

        public ConceptFormVM ConceptForm { get { return conceptForm; } }

        public CategoryFormVM CategoryForm { get { return categoryForm; } }

        public CodeFormVM CodeForm { get { return codeForm; } }

        public VariableFormVM VariableForm { get { return variableForm; } }

        public DataSetFormVM DataSetForm { get { return dataSetForm; } }

        public DataFileFormVM DataFileForm { get { return dataFileForm; } }

        public QuestionGroupFormVM QuestionGroupForm { get { return questionGroupForm; } }

        #region ModelへのDelegateメソッド

        private StudyUnit studyUnit;
        public StudyUnit StudyUnitModel { get { return studyUnit; } }
        public List<Member> MemberModels { get { return studyUnit.Members; } }
        public List<Organization> OrganizationModels { get { return studyUnit.Organizations; } }
        public List<Event> EventModels { get { return studyUnit.Events; } }
        public Abstract AbstractModel { get { return studyUnit.Abstract; } }
        public Coverage CoverageModel { get { return studyUnit.Coverage; } }
        public List<FundingInfo> FundingInfoModels { get { return studyUnit.FundingInfos; } }
        public List<ConceptScheme> ConceptSchemeModels { get { return studyUnit.ConceptSchemes; } }
        public List<Sampling> SamplingModels { get { return studyUnit.Samplings; } }
        public List<Universe> AllUniverseModels { get { return studyUnit.AllUniverses; } }
        public Universe FindMainUniverseModel() { return studyUnit.FindMainUniverse(); }
        public List<Question> QuestionModels { get { return studyUnit.Questions; } }
        public List<Concept> AllConceptModels{ get { return ConceptScheme.GetConcepts(studyUnit.ConceptSchemes); }}
        public List<CategoryScheme> CategorySchemeModels { get { return studyUnit.CategorySchemes; } }
        public List<CodeScheme> CodeSchemeModels { get { return studyUnit.CodeSchemes; } }
        public VariableScheme VariableSchemeModel { get { return studyUnit.VariableScheme; } }
        public List<Variable> VariableModels { get { return studyUnit.Variables;}}
        public List<DataSet> DataSetModels { get { return studyUnit.DataSets; } }
        public DataSet FindDataSetModel(string dataSetId) { return studyUnit.FindDataSet(dataSetId); }
        public List<DataFile> DataFileModels { get { return studyUnit.DataFiles; } }
        public List<ControlConstructScheme> ControlConstructSchemeModels { get { return studyUnit.ControlConstructSchemes; }  }
        public List<QuestionGroup> QuestionGroupModels { get { return studyUnit.QuestionGroups; } }
        public List<Book> BookModels { get { return studyUnit.Books; } }
        #endregion

        #region 各フォームへのDelegateメソッド
        public ObservableCollection<ConceptVM> AllConcepts
        {
            get
            {
                return conceptForm.AllConcepts;
            }
        }

        public ObservableCollection<QuestionVM> AllQuestions
        {
            get
            {
                return questionForm.AllQuestions;
            }
        }

        public ObservableCollection<QuestionVM> QuestionsFor(ConceptVM concept)
        {
            return questionForm.QuestionsFor(concept);
        }

        //public QuestionVM QuestionFor(ResponseVM response)
        //{
        //    return questionForm.QuestionFor(response);
        //}

        public ObservableCollection<QuestionVM> AllChoicesQuestions
        {
            get
            {
                return questionForm.AllChoicesQuestions;
            }
        }

        public ObservableCollection<VariableVM> Variables
        {
            get
            {
                return variableForm.Variables;
            }
        }

        public ResponseVM CreateResponse(Response responseModel)
        {
            return questionForm.CreateResponse(responseModel);
        }

        public ResponseVM SelectAndCreateResponse(ResponseVM sourceResponse)
        {
            return questionForm.SelectAndCreateResponse(sourceResponse);
        }

        public List<ResponseVM> FindResponses(CodeSchemeVM codeScheme)
        {
            List<ResponseVM> questionResponses = questionForm.FindResponses(codeScheme);
            List<ResponseVM> variableResponses = variableForm.FindResponses(codeScheme);
            List<ResponseVM> allResponses = new List<ResponseVM>();
            allResponses.AddRange(questionResponses);
            allResponses.AddRange(variableResponses);
            return allResponses;
        }

        public ObservableCollection<CategoryVM> AllCategories
        {
            get
            {
                return categoryForm.AllCategories;
            }
        }

        public ObservableCollection<MemberVM> Members
        {
            get
            {
                return memberForm.Members;
            }
        }

        public MemberVM FindMember(string memberId)
        {
            return MemberVM.FindById(Members, memberId);
        }

        public ObservableCollection<UniverseVM> Universes
        {
            get
            {
                return samplingForm.AllUniverses;
            }
        }

        public ObservableCollection<FundingInfoVM> FundingInfos
        {
            get
            {
                return fundingInfoForm.FundingInfos;
            }
        }

        public string DefaultUniverseGuid
        {
            get
            {
                return samplingForm.DefaultUniverseGuid;
            }
        }

        public ObservableCollection<CodeSchemeVM> CodeSchemes
        {
            get
            {
                return codeForm.CodeSchemes;
            }
        }

        public CodeSchemeVM FindCodeScheme(Response response)
        {
            return FindCodeScheme(response.CodeSchemeId);
        }

        public CodeSchemeVM FindCodeScheme(string codeSchemeId)
        {
            return codeForm.FindCodeScheme(codeSchemeId);
        }

        public bool ContainsCodeByCategoryId(string categoryId)
        {
            return codeForm.FindCodeByCategoryId(categoryId) != null;
        }

        public CategorySchemeVM FindCategoryScheme(string categorySchemeId)
        {
            return categoryForm.FindCategoryScheme(categorySchemeId);
        }

        public CategorySchemeVM FindCategorySchemeByResponse(ResponseVM response)
        {
            return categoryForm.FindCategorySchemeByResponseId(response.Id);
        }

        public CategoryVM FindCategory(string categoryId)
        {
            return categoryForm.FindCategory(categoryId);
        }

        public ConceptVM FindConcept(string conceptId)
        {
            return conceptForm.FindConcept(conceptId);
        }

        public VariableVM FindVariable(string variableId)
        {
            return variableForm.FindVariable(variableId);
        }

        public List<VariableVM> FindVariablesByUniverseId(string universeId)
        {
            return variableForm.FindVariablesByUniverseId(universeId);
        }

        public DataSetVM FindDataSet(string dataSetId)
        {
            return dataSetForm.FindDataSet(dataSetId);
        }

        public List<DataSetVM> FindDataSetsByVariableId(string variableId)
        {
            return dataSetForm.FindDataSetsByVariableId(variableId);
        }
        public QuestionVM FindQuestion(string questionId)
        {
            return questionForm.FindQuestion(questionId);
        }
        public QuestionGroupVM FindQuestionGroup(string questionGroupId)
        {
            return questionGroupForm.FindQuestionGroup(questionGroupId);
        }
        public ObservableCollection<ControlConstructSchemeVM> ControlConstructSchemes
        {
            get
            {
                return sequenceForm.ControlConstructSchemes;
            }
        }

        public ObservableCollection<DataSetVM> DataSets
        {
            get
            {
                return dataSetForm.DataSets;
            }
        }

        public ObservableCollection<DataFileVM> DataFiles
        {
            get
            {
                return dataFileForm.DataFiles;
            }
        }

        public ObservableCollection<QuestionGroupVM> QuestionGroups
        {
            get
            {
                return questionGroupForm.QuestionGroups;
            }
        }

        public ObservableCollection<ITitleProvider> RelatedMetaData(BookRelationType type)
        {
            ObservableCollection<ITitleProvider> objects = new ObservableCollection<ITitleProvider>();
            if (type == BookRelationType.Concept)
            {
                ObservableCollection<ConceptVM> concepts = AllConcepts;
                foreach (ConceptVM concept in concepts)
                {
                    objects.Add(concept);
                }
            }
            else if (type == BookRelationType.Question)
            {
                ObservableCollection<QuestionVM> questions = AllQuestions;
                foreach (QuestionVM question in questions)
                {
                    objects.Add(question);
                }
            }
            else if (type == BookRelationType.Variable)
            {
                ObservableCollection<VariableVM> variables = Variables;
                foreach (VariableVM variable in variables)
                {
                    objects.Add(variable);
                }
            }
            return objects;
        }

        public ObservableCollection<BookVM> Books
        {
            get
            {
                return bookForm.Books;
            }
        }

        #endregion

        public void InitTitle()
        {
            if (string.IsNullOrEmpty(studyUnit.Abstract.Title))
            {
                studyUnit.Abstract.Title = EDOUtils.OrderTitle(this);
            }
        }


        public override string Title
        {
            get
            {
                return studyUnit.Abstract.Title;
            }
            set
            {
                if (studyUnit.Abstract.Title != value)
                {
                    studyUnit.Abstract.Title = value;
                    InitTitle();
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string DefaultDataSetId
        {
            get
            {
                return studyUnit.DefaultDataSetId;
            }
            set
            {
                if (studyUnit.DefaultDataSetId != value)
                {
                    studyUnit.DefaultDataSetId = value;
                    NotifyPropertyChanged("DefaultDataSetId");
                }
            }
        }

        public string DefaultControlConstructSchemeId
        {
            get
            {
                return studyUnit.DefaultControlConstructSchemeId;
            }
            set
            {
                if (studyUnit.DefaultControlConstructSchemeId != value)
                {
                    studyUnit.DefaultControlConstructSchemeId = value;
                    NotifyPropertyChanged("DefaultControlConstructSchemeId");
                }
            }
        }

        public bool CanAddCategoryScheme
        {
            get
            {
                return true;
            }
        }

        public void AddCategoryScheme()
        {
            categoryForm.AddCategoryScheme();
        }

        public bool CanAddCodeScheme
        {
            get
            {
                return true;
            }
        }

        public void AddCodeScheme()
        {
            codeForm.AddCodeScheme();
        }

        public bool CanAddFromCategoryScheme
        {
            get
            {
                return codeForm.SelectedCodeScheme != null;
            }
        }

        public void AddFromCategoryScheme()
        {
            codeForm.AddFromCategoryScheme();
        }

        public bool CanAddFromCategory
        {
            get
            {
                return codeForm.SelectedCodeScheme != null;
            }
        }

        public void AddFromCategory()
        {
            codeForm.AddFromCategory();
        }

        public bool CanAddDataSet
        {
            get
            {
                return true;
            }
        }

        public void AddDataSet()
        {
            dataSetForm.AddDataSet();
        }


        public bool CanAddQuestionGroup
        {
            get
            {
                return true;
            }
        }

        public void AddQuestionGroup()
        {
            questionGroupForm.AddQuestionGroup();
        }


        public bool IsDefaultDataSet(DataSetVM dataSet)
        {
            return studyUnit.DefaultDataSetId == dataSet.Id;
        }

        public void RemoveDataSet(DataSetVM dataSet)
        {
            UndoManager.IsEnabled = false; 
            dataSetForm.DataSets.Remove(dataSet);
            dataFileForm.RemoveByDataSetId(dataSet.Id);
            UndoManager.IsEnabled = true;
            Memorize();
        }

        public void CompleteResponse()
        {
            if (SelectedMenuItem.Content == QuestionForm)
            {
                CompleteResponse(QuestionForm.SelectedQuestion.Response);
            }
            else if (SelectedMenuItem.Content == VariableForm)
            {
                CompleteResponse(VariableForm.SelectedVariable.Response);
            }
        }

        public void CompleteResponse(ResponseVM response)
        {
            if (!response.IsTypeChoices)
            {
                return;
            }

            CodeSchemeVM codeScheme = response.CodeScheme;
            if (!codeForm.CodeSchemes.Contains(codeScheme))
            {
                //設問設計画面で全く新規に追加された場合
                //タイトルを設定する(コードが全く追加されておらずかつレスポンスのタイトルも空白の場合、null代入により「無題の」が設定される。
                codeScheme.Title = string.IsNullOrEmpty(response.Title) ? CodeVM.JoinLabels(codeScheme.Codes) : response.Title;
                //CodeSchemeを追加する
                codeForm.CodeSchemes.Add(codeScheme);
            }

            List<CodeVM> orphanCodes = new List<CodeVM>();
            foreach (CodeVM code in codeScheme.Codes)
            {
                if (!categoryForm.Contains(code.CategoryId))
                {
                    //設問に対応したカテゴリースキームが存在しない場合、孤立したコードは覚えておく。
                    orphanCodes.Add(code);
                }
            }

            if (orphanCodes.Count > 0) {
                //孤立したコード(カテゴリーが存在既存のカテゴリ集合に含まれていない場合)
                //カテゴリスキームを新規生成してそこにカテゴリを追加する。
                CategorySchemeVM categoryScheme = categoryForm.FindCategorySchemeByResponseId(response.Id);
                if (categoryScheme == null)
                {
                    categoryScheme = new CategorySchemeVM();
                    categoryScheme.ResponseId = response.Id;
                    categoryScheme.Title = string.IsNullOrEmpty(response.Title) ? CodeVM.JoinLabels(orphanCodes) : response.Title; //全体からではなく孤立したコードから生成する
                    categoryForm.CategorySchemes.Add(categoryScheme);
                }
                //カテゴリを追加していく(コードは自動的に追加されるがカテゴリは自動では追加されないため)
                foreach (CodeVM code in orphanCodes)
                {
                    //それぞれのCodeVMにはCategoryVMが含まれているのでそれをCategorySchemeに追加していく。
                    categoryScheme.Categories.Add(code.Category);
                }
            }
        }

        public void CompleteQuestions()
        {
            //設問設計画面の終了時の処理

            //回答を完成させる
            ObservableCollection<QuestionVM> questions = questionForm.AllQuestions;
            foreach (QuestionVM question in questions) {
                CompleteResponse(question.Response);
            }
            //変数を作る
            variableForm.CreateVariables(questions);
            //変数登録画面終了時の処理を呼び出す
            CompleteVariables();

            //質問の順序を作る
            sequenceForm.CreateConstructs(questions);

            //質問グループから外れたものは外す
            questionGroupForm.SyncQuestions();
        }

        public void CompleteVariables()
        {
            //変数登録画面終了時の処理

            //回答を完成させる
            ObservableCollection<VariableVM> variables = variableForm.Variables;
            foreach (VariableVM variable in variables)
            {
                CompleteResponse(variable.Response);
            }
            dataSetForm.CreateDataSets(variables);
            CompleteDataSets();
        }

        public void CompleteDataSets()
        {
            //データファイルを作る
            dataFileForm.CreateDetaFiles(dataSetForm.DataSets);
        }


        public int OrderNo { get; set; }
        public string OrderPrefix { get; set; }

        public void RemoveCodeSchemeFromResponse(CodeSchemeVM codeScheme)
        {
            //コードスキームは複数の回答から参照されることがある。
            //さらに質問設計・変数設計どちらからも参照されることがあるので、
            //1:多の関係になる。
            List<ResponseVM> responses = FindResponses(codeScheme);
            foreach (ResponseVM response in responses)
            {
                //回答方法を選択肢のままにしておくと「無題の選択肢郡」がCompleteVariableなどで生成されてしまう。
                //回答方法をUnknownにもどす。
                response.TypeCode = Options.RESPONSE_TYPE_UNKNOWN.Code;
            }
        }


        public void OnRemoveVariable(VariableVM variable)
        {
            dataSetForm.RemoveVariable(variable);
            bookForm.OnRemoveVariable(variable);
        }

        public void RemoveUniverseFromVariable(UniverseVM universe)
        {
            variableForm.RemoveUniverse(universe);
        }

        public bool CanRemoveConceptScheme(ConceptSchemeVM conceptScheme)
        {
            ObservableCollection<ConceptVM> concepts = conceptScheme.Concepts;
            foreach (ConceptVM concept in concepts)
            {
                if (!CanRemoveConcept(concept))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanRemoveConcept(ConceptVM conceptVM)
        {
            ObservableCollection<QuestionVM> questions = questionForm.QuestionsFor(conceptVM);
            return questions.Count == 0;
        }

        public void OnRemoveConcepts(List<ConceptVM> concepts)
        {
            variableForm.OnRemoveConcepts(concepts);
            bookForm.OnRemoveConcepts(concepts);
        }

        public void OnRemoveConcept(ConceptVM concept)
        {
            questionForm.OnRemoveConcept(concept);
            variableForm.OnRemoveConcept(concept);
            bookForm.OnRemoveConcept(concept);
        }

        public void OnRemoveQuestion(QuestionVM question)
        {
            //Memorizeはこの処理の後で呼び出されるので、以下のメソッド内ではよびだしてはならない。
            variableForm.OnRemoveQuestion(question);
            sequenceForm.OnRemoveQuestion(question);
            bookForm.OnRemoveQuestion(question);
            questionGroupForm.OnRemoveQuestion(question);
        }

        public bool IsDefaultControlConstructScheme(ControlConstructSchemeVM scheme)
        {
            return studyUnit.DefaultControlConstructSchemeId == scheme.Id;
        }


        public bool ConfirmChangeType(ResponseVM response)
        {
            if (!response.IsTypeChoices)
            {
                return true;
            }

            bool containsInQuestionGroup = false;
            foreach (QuestionGroupVM questionGroup in QuestionGroups)
            {
                foreach (QuestionVM question in questionGroup.Questions)
                {
                    if (question.Response == response)
                    {
                        containsInQuestionGroup = true;
                        break;
                    }
                }
            }
            if (!containsInQuestionGroup)
            {
                return true;
            }
            // 質問グループから外れますがよろしいですか? 確認
            MessageBoxResult result = MessageBox.Show(Resources.ConfirmRemoveFromQuestionGroup, Resources.Confirmation, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return false;
            }
            return true;
        }

        public void OnRemoveBooks()
        {
            List<BookVM> books = bookForm.GetBooks(BookRelation.CreateAbstract());
            abstractForm.Books.Clear();
            abstractForm.Books.AddRange(books);

            foreach (ConceptVM concept in AllConcepts)
            {
                books = bookForm.GetBooks(BookRelation.CreateConcept(concept.Id));
                concept.Books.Clear();
                concept.Books.AddRange(books);
            }

            foreach (QuestionVM question in AllQuestions)
            {
                books = bookForm.GetBooks(BookRelation.CreateQuestion(question.Id));
                question.Response.Books.Clear();
                question.Response.Books.AddRange(books);
            }

            foreach (VariableVM variable in Variables)
            {
                books = bookForm.GetBooks(BookRelation.CreateVariable(variable.Id));
                variable.Response.Books.Clear();
                variable.Response.Books.AddRange(books);
            }
        }

        public BookVM AddBook(BookRelation relation)
        {
            return bookForm.AddBookExternal(relation);
        }

        public BookVM EditBook(BookVM book)
        {
            return bookForm.EditBookExternal(book);
        }

        public void RemoveBook(BookVM book)
        {
            bookForm.RemoveBookExternal(book);
        }

        public BookVM SelectBook(BookRelation bookRelation)
        {
            return bookForm.SelectBookExternal(bookRelation);
        }


        public void RemoveConcept(ConceptVM concept)
        {
            ConceptSchemeVM conceptScheme = conceptForm.FindConceptScheme(concept);
            if (conceptScheme == null)
            {
                return;
            }
            conceptScheme.RemoveConceptExternal(concept);
        }

    }
}
