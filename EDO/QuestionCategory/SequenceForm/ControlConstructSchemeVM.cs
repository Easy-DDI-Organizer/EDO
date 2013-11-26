using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Core.Util;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.QuestionForm;
using System.Windows.Input;
using System.Windows;
using EDO.Core.View;
using EDO.DataCategory.DataSetForm;
using System.Diagnostics;
using EDO.Properties;
using EDO.QuestionCategory.SequenceForm.Chart;
using EDO.QuestionCategory.QuestionGroupForm;

namespace EDO.QuestionCategory.SequenceForm
{
    public class ControlConstructSchemeVM :BaseVM, IOrderedObject, IStringIDProvider
    {
        public ControlConstructSchemeVM()
            : this(new ControlConstructScheme())
        {
        }

        public ControlConstructSchemeVM(ControlConstructScheme controlConstructScheme)
        {
            this.controlConstructScheme = controlConstructScheme;
            constructModels = new List<IConstruct>();
            constructs = new ObservableCollection<ConstructVM>();
            //モデルでは、質問文、説明文、分岐それぞれの配列として保持している。
            //VMでは全てを抽象クラスのコレクションとして保持している。
            //このため他の画面のようにModelSyncherは使えない。操作するごとにUpdateModelを呼び出す。
        }

        public void Init()
        {
            List<string> ids = controlConstructScheme.Sequence.ControlConstructIds;
            foreach (string id in ids)
            {
                QuestionConstruct questionConstructModel = controlConstructScheme.FindQuestionConstruct(id);
                if (questionConstructModel != null)
                {
                    QuestionVM question = StudyUnit.FindQuestion(questionConstructModel.QuestionId);
                      Debug.Assert(question != null, "Question not found id=" + questionConstructModel.QuestionId);
                    QuestionConstructVM questionConstruct = new QuestionConstructVM(questionConstructModel, question);
                    InitConstruct(questionConstruct);
                    constructModels.Add(questionConstructModel);
                    constructs.Add(questionConstruct);
                    continue;
                }
                QuestionGroupConstruct questionGroupConstructModel = controlConstructScheme.FindQuestionGroupConstruct(id);
                if (questionGroupConstructModel != null) 
                {
                    QuestionGroupVM questionGroup = StudyUnit.FindQuestionGroup(questionGroupConstructModel.QuestionGroupId);
                    QuestionGroupConstructVM questionGroupConstruct = new QuestionGroupConstructVM(questionGroupConstructModel, questionGroup);
                    InitConstruct(questionGroupConstruct);
                    constructModels.Add(questionGroupConstructModel);
                    constructs.Add(questionGroupConstruct);
                    continue;
                }
                Statement statementModel = controlConstructScheme.FindStatement(id);
                if (statementModel != null)
                {
                    StatementVM statement = new StatementVM(statementModel);
                    InitConstruct(statement);
                    constructModels.Add(statementModel);
                    constructs.Add(statement);
                    continue;
                }
                IfThenElse ifThenElseModel = controlConstructScheme.FindIfThenElse(id);
                if (ifThenElseModel != null)
                {
                    IfThenElseVM ifThenElse = new IfThenElseVM(ifThenElseModel);
                    InitConstruct(ifThenElse);
                    constructModels.Add(ifThenElseModel);
                    constructs.Add(ifThenElse);
                }
            }

            List<QuestionConstructVM> questionConstructs = QuestionConstructs;
            foreach (ConstructVM construct in constructs)
            {
                if (construct is IfThenElseVM)
                {
                    IfThenElseVM ifThenElse = (IfThenElseVM)construct;
                    ifThenElse.ThenConstructs = ThenConstructs;
                }
            }
            modelSyncher = new ModelSyncher<ConstructVM, IConstruct>(this, constructs, constructModels);
            InitTitle();
        }

        #region モデル関連

        private ControlConstructScheme controlConstructScheme;
        public ControlConstructScheme ControlConstructScheme { get { return controlConstructScheme; } }
        public override object Model
        {
            get
            {
                return controlConstructScheme;
            }
        }
        public string Id
        {
            get { return controlConstructScheme.Id; }
        }

        public void InitTitle()
        {
            if (string.IsNullOrEmpty(controlConstructScheme.Title))
            {
                controlConstructScheme.Title = EDOUtils.OrderTitle(this);
            }
        }

        public string Title
        {
            get
            {
                return controlConstructScheme.Title;
            }
            set
            {
                if (controlConstructScheme.Title != value)
                {
                    controlConstructScheme.Title = value;
                    InitTitle();
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        private void UpdateModel(bool memorize)
        {
            controlConstructScheme.QuestionConstructs.Clear();
            controlConstructScheme.QuestionGroupConstructs.Clear();
            controlConstructScheme.Statements.Clear();
            controlConstructScheme.IfThenElses.Clear();
            controlConstructScheme.Sequence.ControlConstructIds.Clear();
            foreach (IConstruct construct in constructModels)
            {
                if (construct is QuestionConstruct)
                {
                    controlConstructScheme.QuestionConstructs.Add((QuestionConstruct)construct);
                }
                else if (construct is QuestionGroupConstruct)
                {
                    controlConstructScheme.QuestionGroupConstructs.Add((QuestionGroupConstruct)construct);
                }
                else if (construct is Statement)
                {
                    controlConstructScheme.Statements.Add((Statement)construct);
                }
                else if (construct is IfThenElse)
                {
                    controlConstructScheme.IfThenElses.Add((IfThenElse)construct);
                }
                controlConstructScheme.Sequence.ControlConstructIds.Add(construct.Id);
            }
            if (memorize)
            {
                Memorize();
            }
        }

        #endregion

        #region 子供VM関連

        private List<IConstruct> constructModels;
        private ObservableCollection<ConstructVM> constructs;
        public ObservableCollection<ConstructVM> Constructs { get { return constructs; } }
        private ModelSyncher<ConstructVM, IConstruct> modelSyncher;
        private void InitConstruct(ConstructVM construct)
        {
            construct.Parent = this;
        }

        public List<QuestionConstructVM> QuestionConstructs
        {
            get
            {
                List<QuestionConstructVM> questionConstructs = new List<QuestionConstructVM>();
                foreach (ConstructVM construct in constructs)
                {
                    if (construct is QuestionConstructVM)
                    {
                        questionConstructs.Add((QuestionConstructVM)construct);
                    }
                }
                return questionConstructs;
            }
        }

        public List<ConstructVM> ThenConstructs
        {
            get
            {
                List<ConstructVM> thenConstructs = new List<ConstructVM>();
                foreach (ConstructVM construct in constructs)
                {
                    if (construct is QuestionConstructVM || construct is  QuestionGroupConstructVM || construct is StatementVM)
                    {
                        thenConstructs.Add(construct);
                    }
                }
                return thenConstructs;
            }
        }

        private void InsertConstruct(ConstructVM construct, bool manualOperation)
        {
            InitConstruct(construct);
            if (manualOperation)
            {
                //画面から追加した場合選択された行の下に追加
                int index = SelectedConstructIndex + 1;
                constructs.Insert(index, construct);
            }
            else
            {
                //質問追加時に自動生成する場合末尾に追加
                constructs.Add(construct);
            }
            //画面から追加した場合記憶してUndoできるようにする
            UpdateModel(manualOperation);
        }

        public void InsertQuestionConstruct(QuestionVM question, bool manualOperation)
        {
            ConstructVM construct = ConstructVM.FindByQuestionId(constructs, question.Id);
            if (construct != null)
            {
                if (manualOperation)
                {
                    //画面から追加した場合エラーメッセージを表示する
                    MessageBox.Show(Resources.AlreadySelectedQuestion); //選択済みの質問です
                }
                return;
            }
            QuestionConstruct questionConstructModel = new QuestionConstruct();
            questionConstructModel.QuestionId = question.Id;
            questionConstructModel.No = ControlConstructScheme.QUESTION_NO_PREFIX + (ConstructUtils.QuestionConstructCount(constructs) + 1);
            QuestionConstructVM questionConstruct = new QuestionConstructVM(questionConstructModel, question);
            InsertConstruct(questionConstruct, manualOperation);
        }


        public void InsertQuestionGroupConstruct(QuestionGroupVM questionGroup, bool manualOperation)
        {
            ConstructVM construct = ConstructVM.FindByQuestionGroupId(constructs, questionGroup.Id);
            if (construct != null)
            {
                if (manualOperation)
                {
                    MessageBox.Show(Resources.AlreadySelectedQuestionGroup); //選択済みの質問グループです
                }
                return;
            }
            QuestionGroupConstruct questionGroupConstructModel = new QuestionGroupConstruct();
            questionGroupConstructModel.QuestionGroupId = questionGroup.Id;
            questionGroupConstructModel.No = ControlConstructScheme.QUESTION_GROUP_NO_PREFIX + (ConstructUtils.QuestionGroupConstructCount(constructs) + 1);
            QuestionGroupConstructVM questionGroupConstruct = new QuestionGroupConstructVM(questionGroupConstructModel, questionGroup);
            InsertConstruct(questionGroupConstruct, manualOperation);
        }

        private void InsertStatementConstruct(Statement statementModel)
        {
            statementModel.No = ControlConstructScheme.STATEMENT_NO_PREFIX + (ConstructUtils.StatementCount(constructs) + 1);
            StatementVM statement = new StatementVM(statementModel);
            InsertConstruct(statement, true);
        }

        public void InsertIfThenElseConstruct(IfThenElse ifThenElseModel)
        {
            if (ifThenElseModel == null)
            {
                return;
            }
            ifThenElseModel.No = ControlConstructScheme.IFTHENELSE_NO;
            IfThenElseVM ifThenElse = new IfThenElseVM(ifThenElseModel);
            ifThenElse.ThenConstructs = ThenConstructs;
            InsertConstruct(ifThenElse, true);
        }

        #endregion

        #region IOrderedObject メンバー

        public int OrderNo { get; set; }
        public string OrderPrefix { get; set; }

        #endregion

        #region コマンド関連
        private object selectedConstructItem;
        public object SelectedConstructItem
        {
            get
            {
                return selectedConstructItem;
            }
            set
            {
                if (selectedConstructItem != value)
                {
                    selectedConstructItem = value;
                    NotifyPropertyChanged("SelectedConstructItem");
                }
            }
        }

        public ConstructVM SelectedConstruct
        {
            get
            {
                return SelectedConstructItem as ConstructVM;
            }
        }

        private ICommand addQuestionCommand;
        public ICommand AddQuestionCommand
        {
            get
            {
                if (addQuestionCommand == null)
                {
                    addQuestionCommand = new RelayCommand(param => AddQuestion(), param => CanAddQuestion);
                }
                return addQuestionCommand;
            }
        }

        private bool CanAddQuestion
        {
            get
            {
                return true;
            }
        }


        private void AddQuestion()
        {
            ObservableCollection<QuestionVM> questions = StudyUnit.AllQuestions;
            SelectObjectWindowVM<QuestionVM> vm = new SelectObjectWindowVM<QuestionVM>(questions, "Content");
            QuestionVM question = SelectObjectWindow.Select(Resources.SelectQuestion, vm) as QuestionVM;//質問の選択
            if (question != null)
            {
                InsertQuestionConstruct(question, true);
            }
        }

        private ICommand addQuestionGroupCommand;
        public ICommand AddQuestionGroupCommand
        {
            get
            {
                if (addQuestionGroupCommand == null)
                {
                    addQuestionGroupCommand = new RelayCommand(param => AddQuestionGroup(), param => CanAddQuestionGroup);
                }
                return addQuestionGroupCommand;
            }
        }

        private bool CanAddQuestionGroup
        {
            get
            {
                return true;
            }
        }

        private void AddQuestionGroup()
        {
            ObservableCollection<QuestionGroupVM> questionGroups = StudyUnit.QuestionGroups;
            SelectObjectWindowVM<QuestionGroupVM> vm = new SelectObjectWindowVM<QuestionGroupVM>(questionGroups);
            QuestionGroupVM questionGroup = SelectObjectWindow.Select(Resources.SelectQuestionGroup, vm) as QuestionGroupVM;//質問グループの選択
            if (questionGroup != null)
            {
                InsertQuestionGroupConstruct(questionGroup, true);
            }
        }

        private ICommand addSentenceCommand;
        public ICommand AddSentenceCommand
        {
            get
            {
                if (addSentenceCommand == null)
                {
                    addSentenceCommand = new RelayCommand(param => AddSentence(), param => CanAddSentence);
                }
                return addSentenceCommand;
            }
        }

        public bool CanAddSentence
        {
            get
            {
                return true;
            }
        }

        public void AddSentence()
        {
            CreateSentenceWindowVM vm = new CreateSentenceWindowVM(this, new Statement());
            CreateSentenceWindow window = new CreateSentenceWindow(vm);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                InsertStatementConstruct(vm.Statement);
            }
        }

        private ICommand addBranchCommand;
        public ICommand AddBranchCommand
        {
            get
            {
                if (addBranchCommand == null)
                {
                    addBranchCommand = new RelayCommand(param => AddBranch(), param => CanAddBranch);
                }
                return addBranchCommand;
            }
        }

        public bool CanAddBranch
        {
            get
            {
                return (SelectedConstruct is QuestionConstructVM);
            }
        }

        public void AddBranch()
        {
            CreateBranchWindowVM vm = new CreateBranchWindowVM(this);
            CreateBranchWindow window = new CreateBranchWindow(vm);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
            {
                InsertIfThenElseConstruct(vm.IfThenElse);
            }
        }

        private ICommand editConstructCommand;
        public ICommand EditConstructCommand
        {
            get
            {
                if (editConstructCommand == null)
                {
                    editConstructCommand = new RelayCommand(param => EditConstruct(), param => CanEditConstruct);
                }
                return editConstructCommand;
            }
        }

        public bool CanEditConstruct
        {
            get
            {
                return (SelectedConstruct is StatementVM || SelectedConstruct is IfThenElseVM);
            }
        }

        public void EditConstruct()
        {
            ConstructVM construct = SelectedConstruct;
            if (construct is StatementVM)
            {
                StatementVM statement = (StatementVM)construct;
                CreateSentenceWindowVM vm = new CreateSentenceWindowVM(this,  (Statement)statement.Model);
                CreateSentenceWindow window = new CreateSentenceWindow(vm);
                window.Owner = Application.Current.MainWindow;
                if (window.ShowDialog() == true && vm.Statement != null)
                {
                    StatementVM newStatement = new StatementVM(vm.Statement);
                    InitConstruct(newStatement);
                    int index = constructs.IndexOf(construct);
                    constructs.RemoveAt(index);
                    constructs.Insert(index, newStatement);
                    UpdateModel(true);
                    SelectedConstructItem = newStatement;
                }
            }
            else if (construct is IfThenElseVM)
            {
                EditBranchExternal((IfThenElseVM)construct, Application.Current.MainWindow);
            }
        }


        public bool EditBranchExternal(IfThenElseVM ifThenElse, Window ownerWindow)
        {
            CreateBranchWindowVM vm = new CreateBranchWindowVM(this, (IfThenElse)ifThenElse.Model);
            CreateBranchWindow window = new CreateBranchWindow(vm);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true && vm.IfThenElse != null)
            {
                IfThenElseVM newIfThenElse = new IfThenElseVM(vm.IfThenElse);
                InitConstruct(newIfThenElse);
                newIfThenElse.ThenConstructs = ThenConstructs;
                int index = constructs.IndexOf(ifThenElse);
                constructs.RemoveAt(index);
                constructs.Insert(index, newIfThenElse);
                UpdateModel(true);
                SelectedConstructItem = newIfThenElse;
                return true;
            }

            return false;
        }

        private ICommand removeConstructCommand;
        public ICommand RemoveConstructCommand
        {
            get
            {
                if (removeConstructCommand == null)
                {
                    removeConstructCommand = new RelayCommand(param => RemoveConstruct(), param => CanRemoveConstruct);
                }
                return removeConstructCommand;
            }
        }

        public bool CanRemoveConstruct
        {
            get
            {
                if (SelectedConstruct == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveConstruct()
        {
            Constructs.Remove(SelectedConstruct);
            UpdateModel(true);
        }

        private int SelectedConstructIndex
        {
            get
            {
                if (SelectedConstruct == null)
                {
                    return -1;
                }
                return Constructs.IndexOf(SelectedConstruct);
            }
        }

        private bool CanReorderVariable()
        {
            //ビューに問い合わせて、ソートされていたら順序変更不可能と判断する。
            SequenceFormVM parent = (SequenceFormVM)Parent;
            return parent.CanReorderVariable();
        }

        private void FocusCell()
        {
            SequenceFormVM parent = (SequenceFormVM)Parent;
            parent.FocusCell();
        }

        private ICommand upConstructCommand;
        public ICommand UpConstructCommand
        {
            get
            {
                if (upConstructCommand == null)
                {
                    upConstructCommand = new RelayCommand(param => UpConstruct(), param => CanUpConstruct);
                }
                return upConstructCommand;
            }
        }

        public bool CanUpConstruct
        {
            get
            {
                if (SelectedConstruct == null)
                {
                    return false;
                }
                if (!CanReorderVariable())
                {
                    return false;
                }
                if (SelectedConstructIndex == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public void UpConstruct()
        {
            ConstructVM construct = SelectedConstruct;
            int index = SelectedConstructIndex;
            Constructs.Move(index, index - 1);
            UpdateModel(true);
            FocusCell();
        }

        private ICommand downConstructCommand;
        public ICommand DownConstructCommand
        {
            get
            {
                if (downConstructCommand == null)
                {
                    downConstructCommand = new RelayCommand(param => DownConstruct(), param => CanDownConstruct);
                }
                return downConstructCommand;
            }
        }

        public bool CanDownConstruct
        {
            get
            {
                if (SelectedConstruct == null)
                {
                    return false;
                }
                if (!CanReorderVariable())
                {
                    return false;
                }
                if (SelectedConstructIndex == Constructs.Count - 1)
                {
                    return false;
                }
                return true;
            }
        }



        public void DownConstruct()
        {
            ConstructVM construct = SelectedConstruct;
            int index = SelectedConstructIndex;
            Constructs.Move(index, index + 1);
            UpdateModel(true);
            FocusCell();
        }


        #endregion


        public void RemoveQuestion(QuestionVM question)
        {
            bool removed = false;
            for (int i = constructs.Count - 1; i >= 0; i--)
            {
                ConstructVM construct = constructs[i];
                if (construct is QuestionConstructVM )
                {
                    QuestionConstructVM questionConstruct = (QuestionConstructVM)construct;
                    if (questionConstruct.Question == question)
                    {
                        constructs.RemoveAt(i);
                        removed = true;
                    }
                }
            }
            if (removed)
            {
                UpdateModel(false);
            }
        }


        private ICommand previewCommand;
        public ICommand PreviewCommand
        {
            get
            {
                if (previewCommand == null)
                {
                    previewCommand = new RelayCommand(param => Preview(), param => CanPreview);
                }
                return previewCommand;
            }
        }

        public bool CanPreview
        {
            get
            {
                return true;
            }
        }


        public void Preview()
        {
            ChartWindowVM vm = new ChartWindowVM(this);
            ChartWindow window = new ChartWindow(vm);
            window.Owner = Application.Current.MainWindow; 
            window.ShowDialog();
        }
    }
}
