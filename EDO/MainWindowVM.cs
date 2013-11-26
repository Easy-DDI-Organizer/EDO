using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows;
using EDO.QuestionCategory.CategoryForm;
using EDO.QuestionCategory.CodeForm;
using EDO.Main;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using EDO.EventCategory.EventForm;
using EDO.StudyCategory.MemberForm;
using EDO.StudyCategory.CoverageForm;
using EDO.StudyCategory.FundingInfoForm;
using EDO.SamplingCategory.SamplingForm;
using EDO.QuestionCategory.ConceptForm;
using EDO.VariableCategory.VariableForm;
using EDO.DataCategory.DataSetForm;
using EDO.DataCategory.DataFileForm;
using EDO.StudyCategory.AbstractForm;
using EDO.Core.ViewModel;
using EDO.Core.View;
using EDO.Core.Util;
using System.Diagnostics;
using System.Xml.Serialization;
using EDO.Core.Model;
using System.IO;
using EDO.Core.IO;
using System.Xml.Linq;
using EDO.Properties;
using System.Threading;
using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows.Threading;

namespace EDO
{
    public class MainWindowVM :BaseVM, IStatefullVM
    {
        private const string DEFAULT_TITLE = "Easy DDI Organizer";

        public MainWindowVM(EDOConfig config) {

//手動で英語環境のテストをしたい場合以下を有効にする

            Options.Init();
            this.config = config;
            recentFileList = new RecentFileList();
            List<string> files = recentFileList.RecentFiles;
            foreach (string file in files)
            {
                Debug.WriteLine(file);
            }

            edoUnits = new ObservableCollection<EDOUnitVM>();
            undoManager = new UndoManager();
            undoManager.UndoBufferCount = config.UndoBufferCount;
            undoManager.ModelChangeHandler += OnModelChanged;
            if (config.ReopenLastFile && File.Exists(config.LastFile))
            {
                DoOpen(false, config.LastFile);
            }
            if (edoModel == null)
            {
                edoModel = EDOModel.createDefault();
                createViewModels(edoModel);
                undoManager.Init(edoModel, this);
                UpdateRecentFiles();
            }

            this.HelpCommand = new HelpCommandImpl(this);
            this.AddCommand = new AddCommandImpl(this);
            this.StatusMessage = "Ready";
            this.IsErrorMessage = true;
            this.Title = DEFAULT_TITLE;
            this.IsTextBoxChanged = false;
//            var xdoc = XDocument.Parse(Resources.Options);
//            Debug.WriteLine(xdoc.ToString());
            this.isRestarting = false;
        }

        public bool IsTextBoxChanged { get; set; }
        public MainWindow Window { get; set; }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (title != value)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private void UpdateTitle()
        {
            string title = DEFAULT_TITLE;
            if (IsModified)
            {
                title += " *";
            }
            Title = title;
        }

        public void OnModelChanged(object sender, EventArgs e)
        {
            UpdateTitle();
        }

        public void StudyUnitPropertyChangedEventHandler(object source, PropertyChangedEventArgs arg)
        {
            if (arg.PropertyName == "Title")
            {
                NotifyPropertyChanged("CloneStudyUnitCommandTitle");
            }
        }

        #region フィールド・プロパティ

        private EDOConfig config;

        private RecentFileList recentFileList;

        private string loadHash;

        private UndoManager undoManager;

        private EDOModel edoModel;

        private ObservableCollection<EDOUnitVM> edoUnits;

        public ObservableCollection<EDOUnitVM> EDOUnits { 
            get { 
                return edoUnits; 
            } 
        }

        public override UndoManager UndoManager
        {
            get
            {
                return undoManager;
            }
        }

        public GroupVM GroupVM
        {
            get
            {
                foreach (EDOUnitVM unit in edoUnits) 
                {
                    GroupVM group = unit as GroupVM;
                    if (group != null) {
                        return group;
                    }
                }
                return null;
            }
        }

        public List<StudyUnitVM> StudyUnits
        {
            get
            {
                List<StudyUnitVM> studyUnits = new List<StudyUnitVM>();
                foreach (EDOUnitVM unit in edoUnits)
                {
                    StudyUnitVM studyUnit = unit as StudyUnitVM;
                    if (studyUnit != null)
                    {
                        studyUnits.Add(studyUnit);
                    }
                }
                return studyUnits;
            }
        }

        public List<StudyUnitVM> GetOtherStudyUnits(StudyUnitVM excludeStudyUnit)
        {
            List<StudyUnitVM> studyUnits = StudyUnits;
            List<StudyUnitVM> results = new List<StudyUnitVM>();
            foreach (StudyUnitVM studyUnit in studyUnits)
            {
                if (studyUnit != excludeStudyUnit)
                {
                    results.Add(studyUnit);
                }
            }
            return results;
        }

        public string StudyUnitNames
        {
            get
            {
                StringBuilder buf = new StringBuilder();
                List<StudyUnitVM> studyUnits = this.StudyUnits;
                int i = 0; 
                foreach (StudyUnitVM studyUnit in studyUnits)
                {
                    buf.Append(studyUnit.Title);
                    if (i < studyUnits.Count - 1)
                    {
                        buf.Append(",");
                    }
                    i++;
                }
                return buf.ToString();
            }
        }

        private string statusMessage;
        public string StatusMessage {
            get
            {
                return statusMessage;
            }
            set
            {
                if (this.statusMessage != value)
                {
                    this.statusMessage = value;
                    NotifyPropertyChanged("StatusMessage");
                }
            }
        }

        private bool isErrorMessage;
        public bool IsErrorMessage 
        {
            get
            {
                return isErrorMessage;
            }
            set
            {
                if (this.isErrorMessage != value) {
                    this.isErrorMessage = value;
                    NotifyPropertyChanged("IsErrorMessage");
                }
            }
        }

        public override void SetStatusMessage(string message, bool isError)
        {
            this.StatusMessage = message;
            this.IsErrorMessage = isError;
        }

        private EDOUnitVM selectedItem;
        public EDOUnitVM SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    NotifyPropertyChanged("SelectedItem");
                    NotifyPropertyChanged("CloneStudyUnitCommandTitle");
                }
            }
        }

        public StudyUnitVM SelectedStudyUnit
        {
            get
            {
                //コマンドで使用する(Viewへバインディングするわけではない)
                return SelectedItem as StudyUnitVM;
            }
        }

        public GroupVM SelectedGroup
        {
            get
            {
                return SelectedItem as GroupVM;
            }

        }

        public MenuItemVM SelectedMenuItem
        {
            get
            {
                //コマンドで使用する(Viewへバインディングするわけではない)
                if (SelectedItem == null)
                {
                    return null;
                }
                return SelectedItem.SelectedMenuItem;
            }
        }

        #endregion フィールド。プロパティ

        #region 初期化

        private EDOUnitVM DefaultSelectedItem
        {
            get 
            {
                if (edoUnits.Count == 0)
                {
                    return null;
                }
                else if (edoUnits.Count == 1)
                {
                    return edoUnits[0];
                }
                return edoUnits[1];
            }
        }

        private void createViewModels(EDOModel newEdoModel)
        {
            if (newEdoModel == null)
            {
                return;
            }
            edoUnits.Clear();
            this.edoModel = newEdoModel;
            if (edoModel.Group != null)
            {
                edoUnits.Add(createGroup(edoModel.Group));
            }
            foreach (StudyUnit studyUnit in edoModel.StudyUnits)
            {
                edoUnits.Add(createStudyUnit(studyUnit));
            }
            this.SelectedItem = this.DefaultSelectedItem;
            this.IsCategory = false;
            this.IsCode = false;

            //createStudyUnitでStudyUnitのタイトルが変わったりするので念のため
            //ハッシュを保存するのはこのメソッドの最後で行う。
            loadHash = EDOSerializer.ComputeHash(newEdoModel);
        }

        private GroupVM createGroup(Group group)
        {
            return new GroupVM(this, group);
        }

        private StudyUnitVM createStudyUnit(StudyUnit studyUnitModel)
        {
            StudyUnitVM studyUnit = new StudyUnitVM(this, studyUnitModel) {
                OrderNo = EDOUtils.GetMaxOrderNo<StudyUnitVM>(StudyUnits) + 1,
                OrderPrefix = "StudyUnit"
            };
            studyUnit.InitTitle();
            studyUnit.PropertyChanged += new PropertyChangedEventHandler(StudyUnitPropertyChangedEventHandler);
            return studyUnit;
        }

        private StudyUnitVM AddStudyUnit(StudyUnit studyUnitModel)
        {
            edoModel.StudyUnits.Add(studyUnitModel);
            StudyUnitVM studyUnit = createStudyUnit(studyUnitModel);
            edoUnits.Add(studyUnit);
            Memorize();
            return studyUnit;
        }

        private void RemoveStudyUnit(StudyUnitVM studyUnit)
        {
            edoModel.StudyUnits.Remove(studyUnit.StudyUnitModel);
            edoUnits.Remove(studyUnit);
            Memorize();
        }

        #endregion 初期化

        #region 状況依存のリボンメニュー制御

        // メニューを表示するかどうかと、選択状態(アクティブ)にするかどうかの２つの状態が必要
        // そうしないと(ホームメニューが選択できなくなる)

        private bool isCategory;
        public bool IsCategory
        {
            get
            {
                return isCategory;
            }
            set
            {
                if (isCategory != value)
                {
                    isCategory = value;
                    NotifyPropertyChanged("IsCategory");
                }
            }
        }

        private bool isCategorySelected;
        public bool IsCategorySelected
        {
            get
            {
                return isCategorySelected;
            }
            set
            {
                if (isCategorySelected != value)
                {
                    isCategorySelected = value;
                    NotifyPropertyChanged("IsCategorySelected");
                }
            }
        }


        private bool isCode;
        public bool IsCode
        {
            get
            {
                return isCode;
            }
            set
            {
                if (isCode != value)
                {
                    isCode = value;
                    NotifyPropertyChanged("IsCode");
                }
            }
        }

        private bool isCodeSelected;
        public bool IsCodeSelected
        {
            get
            {
                return isCodeSelected;
            }
            set
            {
                if (isCodeSelected != value)
                {
                    isCodeSelected = value;
                    NotifyPropertyChanged("IsCodeSelected");
                }
            }
        }

        private bool isDataSet;
        public bool IsDataSet
        {
            get
            {
                return isDataSet;
            }
            set
            {
                if (isDataSet != value)
                {
                    isDataSet = value;
                    NotifyPropertyChanged("IsDataSet");
                }
            }
        }

        private bool isDataSetSelected;
        public bool IsDataSetSelected
        {
            get
            {
                return isDataSetSelected;
            }
            set
            {
                if (isDataSetSelected != value)
                {
                    isDataSetSelected = value;
                    NotifyPropertyChanged("IsDataSetSelected");
                }
            }
        }

        private bool isQuestionGroup;
        public bool IsQuestionGroup
        {
            get
            {
                return isQuestionGroup;
            }
            set
            {
                if (isQuestionGroup != value)
                {
                    isQuestionGroup = value;
                    NotifyPropertyChanged("IsQuestionGroup");
                }
            }
        }

        private bool isQuestionGroupSelected;
        public bool IsQuestionGroupSelected
        {
            get
            {
                return isQuestionGroupSelected;
            }
            set
            {
                if (isQuestionGroupSelected != value)
                {
                    isQuestionGroupSelected = value;
                    NotifyPropertyChanged("IsQuestionGroupSelected");
                }
            }
        }

        #endregion
        #region テストコマンド

        private ICommand testCommand;
        public ICommand TestCommand
        {
            get
            {
                if (testCommand == null)
                {
                    testCommand = new RelayCommand(param => Test(), param => CanTest);
                }
                return testCommand;
            }
        }

        public bool CanTest
        {
            get
            {
                return true;
            }
        }

        public void Test()
        {
            //ImportOptionWindowVM vm = new ImportOptionWindowVM();
            //ImportOptionWindow window = new ImportOptionWindow(vm);
            //window.ShowDialog();
        }

        #endregion


        #region ファイルコマンド

        private ICommand configCommand;
        public ICommand ConfigCommand
        {
            get
            {
                if (configCommand == null)
                {
                    configCommand = new RelayCommand(param => Config(), param => CanConfig);
                }
                return configCommand;
            }
        }

        public bool CanConfig
        {
            get
            {
                return true;
            }
        }

        public void Config()
        {
            ConfigWindow configWindow = new ConfigWindow(config);
            configWindow.Owner = Application.Current.MainWindow;
            if (configWindow.ShowDialog() != true)
            {
                return;
            }
            undoManager.UndoBufferCount = config.UndoBufferCount;
            if (configWindow.ShouldRestart)
            {
                Restart();
            }
        }

        private ICommand exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                {
                    exitCommand = new RelayCommand(param => Exit(), param => CanExit);
                }
                return exitCommand;
            }
        }

        public bool CanExit
        {
            get
            {
                return true;
            }
        }

        private bool IsModified
        {
            get
            {
                string curHash = EDOSerializer.ComputeHash(edoModel);
                return loadHash != curHash;
            }
        }

        public bool ConfirmModified()
        {
            if (isRestarting)
            {
                //再起動時、Restart()中のConfirumModified()で"NO"が選ばれた場合、window_Closingから再度よばれてしまう。
                //それを避けるための処理。
                isRestarting = false; //念のためフラグを戻す。
                return true;
            }
            if (SelectedItem != null && !SelectedItem.ValidateCurrentItem())
            {
                return false;
            }

            //終了前にユーザーに保存するかどうかをYES / NO / CANCELで問い合わせる。
            if (!IsModified)
            {
                //変更されていない場合は問い合わせない
                return true;
            }
            bool continueProcess = false;
            //変更内容を保存しますか?
            MessageBoxResult dialogResult = MessageBox.Show(Resources.SaveChanges, ApplicationDetails.ProductTitle, MessageBoxButton.YesNoCancel);
            if (dialogResult == MessageBoxResult.Yes)
            {
                //保存して終了する。
                Save();
                continueProcess = true;
            }
            else if (dialogResult == MessageBoxResult.No)
            {
                //保存せずに終了する。
                continueProcess = true;
            } else if (dialogResult == MessageBoxResult.Cancel)
            {
                //終了しない
                continueProcess = false;
            }
            return continueProcess;
        }

        public bool ConfirmRestart()
        {
            MessageBoxResult dialogResult = MessageBox.Show(Properties.Resources.ConfirmRestart, ApplicationDetails.ProductTitle, MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                return true;
            }
            return false;
        }

        public void Exit()
        {
            if (!ConfirmModified())
            {
                return;
            }
            Application.Current.Shutdown();
        }

        private bool isRestarting = false;

        public void Restart()
        {
            if (!ConfirmRestart())
            {
                return;
            }
            if (!ConfirmModified())
            {
                return;
            }
            isRestarting = true;
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        } 

        private ICommand createStudyUnitCommand;
        public ICommand CreateStudyUnitCommand
        {
            get
            {
                if (createStudyUnitCommand == null)
                {
                    createStudyUnitCommand = new RelayCommand(param => CreateStudyUnit(), param => CanCreateStudyUnit);
                }
                return createStudyUnitCommand;
            }
        }

        public bool CanCreateStudyUnit
        {
            get
            {
                return true;
            }
        }

        public void CreateStudyUnit()
        {
            if (!ConfirmModified())
            {
                return;
            }
            createViewModels(EDOModel.createDefault());
            undoManager.Init(edoModel, this);
            //edoModel.Group = Group.createDefault();
            //GroupVM group = createGroup(edoModel.Group);
            //this.EDOUnits.Insert(0, group);
            //edoUnit = group;
        }

        private ICommand addEmptyStudyUnitCommand;
        public ICommand AddEmptyStudyUnitCommand
        {
            get
            {
                if (addEmptyStudyUnitCommand == null)
                {
                    addEmptyStudyUnitCommand = new RelayCommand(param => AddEmptyStudyUnit(), param => CanAddEmptyStudyUnit);
                }
                return addEmptyStudyUnitCommand;
            }
        }

        public bool CanAddEmptyStudyUnit
        {
            get
            {
                return edoModel.IsExistGroup;
            }
        }

        public void AddEmptyStudyUnit()
        {
            StudyUnit newStudyUnit = EDO.Core.Model.StudyUnit.CreateDefault();
            AddStudyUnit(newStudyUnit);
        }

        private ICommand addGroupCommand;
        public ICommand AddGroupCommand
        {
            get
            {
                if (addGroupCommand == null)
                {
                    addGroupCommand = new RelayCommand(param => AddGroup(), param => CanAddGroup);
                }
                return addGroupCommand;
            }
        }

        public bool CanAddGroup
        {
            get
            {
                return !edoModel.IsExistGroup;
            }
        }

        public void AddGroup()
        {
            edoModel.Group = Group.CreateDefault();
            GroupVM group = createGroup(edoModel.Group);
            this.EDOUnits.Insert(0, group);
            Memorize();
        }

        private ICommand openCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (openCommand == null)
                {
                    openCommand = new RelayCommand(param => this.Open(), param => this.CanOpen);
                }
                return openCommand;
            }
        }

        public bool CanOpen
        {
            get
            {
                return true;
            }
        }

        private void DoOpen(bool confirmModified, string path)
        {
            if (confirmModified && !ConfirmModified())
            {
                return;
            }
            EDOModel newEdoModel = null;
            if (string.IsNullOrEmpty(path))
            {
                newEdoModel = EDOSerializer.LoadFile();
            }
            else
            {
                newEdoModel = EDOSerializer.LoadFile(path);
            }

            if (newEdoModel == null)
            {
                return;
            }
            string pathName = null;
            if (newEdoModel.IsExistGroup)
            {
                pathName = newEdoModel.Group.PathName;
            }
            else if (newEdoModel.StudyUnits.Count > 0)
            {
                pathName = newEdoModel.StudyUnits[0].PathName;
            }
            recentFileList.InsertFile(pathName);
            UpdateRecentFiles();
            createViewModels(newEdoModel);
            undoManager.Init(newEdoModel, this);
        }

        public void Open()
        {
            DoOpen(true, null);
        }

        private void DoSave(bool query)
        {
            if (SelectedItem == null || !SelectedItem.ValidateCurrentItem())
            {
                return;
            }
            GroupVM group = GroupVM;
            if (group != null)
            {
                group.SyncModel();
            }
            EDOSerializer.SaveFile(group, StudyUnits, query);
            loadHash = EDOSerializer.ComputeHash(edoModel);
            UpdateTitle();
        }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (saveCommand == null)
                {
                    saveCommand = new RelayCommand(param => this.Save(), param => this.CanSave);
                }
                return saveCommand;
            }
        }

        public bool CanSave
        {
            get
            {
                return true;
            }
        }

        public void Save()
        {
            DoSave(false);
        }

        private ICommand saveAsCommand;
        public ICommand SaveAsCommand
        {
            get
            {
                if (saveAsCommand == null)
                {
                    saveAsCommand = new RelayCommand(param => this.SaveAs(), param => this.CanSaveAs);
                }
                return saveAsCommand;
            }
        }

        public bool CanSaveAs
        {
            get
            {
                return true;
            }
        }

        public void SaveAs()
        {
            DoSave(true);
        }

        private ICommand undoCommand;
        public ICommand UndoCommand
        {
            get
            {
                if (undoCommand == null)
                {
                    undoCommand = new RelayCommand(param => Undo(), param => CanUndo);
                }
                return undoCommand;
            }
        }

        private TextBox FocusedTextBox
        {
            get
            {
                return Keyboard.FocusedElement as TextBox;
            }
        }

        private bool FocusedTextBoxChanged
        {
            get
            {
                TextBox textBox = FocusedTextBox;
                if (textBox == null)
                {
                    return false;
                }
                return TextBoxHelper.IsChanged(textBox);
                ////if ((element.Tag as string) != EDOConstants.TAG_UNDOABLE)
                ////{
                ////    return false;
                ////}
                //bool changed = TextBoxHelper.IsChanged(element);
                //return changed;
            }
        }

        private bool CanRedoFocusedTextBox
        {
            get
            {
                TextBox textBox = FocusedTextBox;
                if (textBox == null)
                {
                    return false;
                }
                return textBox.CanRedo;
                ////if ((element.Tag as string) != EDOConstants.TAG_UNDOABLE)
                ////{
                ////    return false;
                ////}
                //bool changed = TextBoxHelper.IsChanged(element);
                //return changed;
            }
        }

        private bool CommitFocusedTextBox()
        {
            bool result = false;
            TextBox textBox = FocusedTextBox;
            if (textBox == null)
            {
                return false;
            }
            result = TextBoxHelper.UpdateSource(textBox);
            return result;
        }

        public bool CanUndo
        {
            get
            {
                //TextBox textBox = FocusedTextBox;
                //if (textBox != null && string.IsNullOrEmpty(textBox.Text))
                //{
                //    return textBox.CanUndo;
                //}
                //return UndoManager.CanUndo;
                return FocusedTextBoxChanged || UndoManager.CanUndo;
            }
        }

        public void LoadState(VMState state)
        {
            string edoUnitId = (string)state.State1;
            VMState childState = (VMState)state.State2;
            foreach (EDOUnitVM edoUnit in edoUnits)
            {
                if (edoUnit.Id == edoUnitId)
                {
                    SelectedItem = edoUnit;
                    SelectedItem.LoadState(childState);
                    break;
                }
            }
        }

        public void Complete(VMState state) 
        {
            string edoUnitId = (string)state.State1;
            VMState childState = (VMState)state.State2;
            foreach (EDOUnitVM edoUnit in edoUnits)
            {
                if (edoUnit.Id == edoUnitId)
                {
                    SelectedItem.Complete(childState);
                    break;
                }
            }
        }

        public VMState SaveState()
        {
            if (SelectedItem == null)
            {
                return null;
            }
            return new VMState(SelectedItem.Id, SelectedItem.SaveState());
        }

        private void DoUndoOrRedo(Func<UndoInfo> func)
        {
            //if (SelectedItem != null)
            //{
            //    SelectedItem.ValidateCurrentItem();
            //}

            //ここで一時的にUndoManagerを無効にしておかないと
            //イベントフォームで日付入力中にUndoを押したとき、最後の編集がコミットされたあと記憶されてしまう。
            //(本当は最後の編集は記憶せずそのまえに戻りたい)。
            UndoManager.IsEnabled = false;

            VMState state = SaveState();

            //string edoUnitId = SelectedItem.Id;
            //int id = SelectedMenuItem.Id;

            string orgLoadHash = loadHash;
            UndoInfo undoInfo = func();
            if (undoInfo != null)
            {
                //何らかの理由で失敗した場合以下の処理はとばす(現状のまま)。
                createViewModels(undoInfo.Model);
                loadHash = orgLoadHash;
                LoadState(state);
                Complete(undoInfo.State);
            }
            //foreach (EDOUnitVM edoUnit in edoUnits)
            //{
            //    if (edoUnit.Id == edoUnitId)
            //    {
            //        SelectedItem = edoUnit;
            //        break;
            //    }
            //}
            //SelectedItem.SelectMenuItem(id);

            UndoManager.IsEnabled = true;
            UpdateTitle();
        }

        public void Undo()
        {
            if (FocusedTextBoxChanged)
            {
                if (!CommitFocusedTextBox())
                {
                    return;
                }
            }
            Debug.WriteLine("Undo Start");
            DoUndoOrRedo(() => { return UndoManager.Undo(); });
            Debug.WriteLine("Undo Finish");
        }

        private ICommand redoCommand;
        public ICommand RedoCommand
        {
            get
            {
                if (redoCommand == null)
                {
                    redoCommand = new RelayCommand(param => Redo(), param => CanRedo);
                }
                return redoCommand;
            }
        }

        public bool CanRedo
        {
            get
            {
                return !FocusedTextBoxChanged && UndoManager.CanRedo;
            }
        }

        public void Redo()
        {
            Debug.WriteLine("Redo Start");
            DoUndoOrRedo(() => { return UndoManager.Redo(); });
            Debug.WriteLine("Undo Finish");
        }

        #endregion

        #region グループコマンド

        #region 新規グループコマンド

        //private ICommand createGroupCommand;
        //public ICommand CreateGroupCommand {
        //    get
        //    {
        //        if (createGroupCommand == null)
        //        {
        //            createGroupCommand = new RelayCommand(param => this.CreateGroup(), param => this.CanCreateGroup);
        //        }
        //        return createGroupCommand;
        //    }
        //}

        //public bool CanCreateGroup
        //{
        //    get
        //    {
        //        return !edoModel.IsExistGroup;
        //    }
        //}

        //public void CreateGroup()
        //{
        //    edoModel.Group = Group.createDefault();
        //    this.EDOUnits.Insert(0, createGroup(edoModel.Group));      
        //}
        #endregion

        #region グループを開くコマンド
        private ICommand openGroupCommand;
        public ICommand OpenGroupCommand
        {
            get
            {
                if (openGroupCommand == null)
                {
                    openGroupCommand = new RelayCommand(param => this.OpenGroup(), param => this.CanOpenGroup);
                }
                return openGroupCommand;
            }
        }

        public bool CanOpenGroup
        {
            get
            {
                return true;                    
            }
        }

        public void OpenGroup()
        {
//            EDOModel edoModel = EDOSerializer.LoadAll();
            createViewModels(edoModel);
        }
        #endregion

        #region グループを保存するコマンド
        private ICommand saveGroupCommand;
        public ICommand SaveGroupCommand
        {
            get
            {
                if (saveGroupCommand == null)
                {
                    saveGroupCommand = new RelayCommand(param => this.SaveGroup(), param => this.CanSaveGroup);
                }
                return saveGroupCommand;
            }
        }

        public bool CanSaveGroup
        {
            get
            {
                return edoModel.IsExistGroup;
            }
        }

        public void SaveGroup()
        {
            GroupVM group = GroupVM;
            if (!group.ValidateCurrentItem())
            {
                return;
            }
            group.SyncModel();
//            EDOSerializer.SaveAll(group, StudyUnits, false);
        }
        #endregion

        #region グループを名前をつけて保存するコマンド
        private ICommand saveAsGroupCommand;
        public ICommand SaveAsGroupCommand
        {
            get
            {
                if (saveAsGroupCommand == null)
                {
                    saveAsGroupCommand = new RelayCommand(param => this.SaveAsGroup(), param => this.CanSaveAsGroup);
                }
                return saveAsGroupCommand;
            }
        }

        public bool CanSaveAsGroup
        {
            get
            {
                return edoModel.IsExistGroup;
            }
        }

        public void SaveAsGroup()
        {
//            EDOSerializer.SaveAll(this.GroupVM, this.StudyUnits, true);
        }
        #endregion 

        #endregion グループコマンド

        #region StudyUnitコマンド

             #region StudyUnitの新規作成コマンド

        //private ICommand createStudyUnitCommand;
        //public ICommand CreateStudyUnitCommand {
        //    get
        //    {
        //        if (createStudyUnitCommand == null) {
        //            createStudyUnitCommand = new RelayCommand(param => this.CreateStudyUnit(), param => this.CanCreateStudyUnit);
        //        }
        //        return createStudyUnitCommand;
        //    }
        //}

        //public bool CanCreateStudyUnit
        //{
        //    get
        //    {
        //        return edoModel.IsExistGroup;
        //    }
        //}

        //public void CreateStudyUnit()
        //{
        //    StudyUnit newStudyUnit = EDO.Core.Model.StudyUnit.createDefault();
        //    AddStudyUnit(newStudyUnit);
        //}

            #endregion

        #region StudyUnitの複製コマンド
        public string CloneStudyUnitCommandTitle
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return Resources.CloneSelectedStudyUnit; // 選択された調査ファイルを複製して追加
                }
                return string.Format(Resources.CloneAndAdd, SelectedStudyUnit.Title); //を複製して追加
            }
        }

        private ICommand cloneStudyUnitCommand;
        public ICommand CloneStudyUnitCommand
        {
            get
            {
                if (cloneStudyUnitCommand == null)
                {
                    cloneStudyUnitCommand = new RelayCommand(param => this.CloneStudyUnit(), param => this.CanCloneStudyUnit);
                }
                return cloneStudyUnitCommand;
            }
        }

        public bool CanCloneStudyUnit
        {
            get
            {
                return edoModel.IsExistGroup && SelectedStudyUnit != null;
            }
        }

        public void CloneStudyUnit()
        {
            StudyUnit newStudyUnit = DeepCopyUtils.DeepCopy<StudyUnit>(SelectedStudyUnit.StudyUnitModel);
            AddStudyUnit(newStudyUnit);
        }
        #endregion

        #region StuUnitを開くコマンド
        private ICommand openStudyUnitCommand;
        public ICommand OpenStudyUnitCommand
        {
            get
            {
                if (openStudyUnitCommand == null)
                {
                    openStudyUnitCommand = new RelayCommand(param => this.OpenStudyUnit(), param => this.CanOpenStudyUnit);
                }
                return openStudyUnitCommand;
            }
        }

        public bool CanOpenStudyUnit
        {
            get
            {
                return true;
            }
        }

        public void OpenStudyUnit()
        {
            StudyUnit studyUnit = EDOSerializer.LoadStudyUnit();
            if (studyUnit == null) {
                return;
            }
            EDOModel newEdoModel = new EDOModel();
            newEdoModel.StudyUnits.Add(studyUnit);
            createViewModels(newEdoModel);
        }
        #endregion

        #region StudyUnitを追加するコマンド(既存ファイルより)
        private ICommand addStudyUnitCommand;
        public ICommand AddStudyUnitCommand
        {
            get
            {
                if (addStudyUnitCommand == null)
                {
                    addStudyUnitCommand = new RelayCommand(param => AddStudyUnit(), param => CanAddStudyUnit);
                }
                return addStudyUnitCommand;                
            }
        }

        public bool CanAddStudyUnit
        {
            get
            {
                return edoModel.IsExistGroup;
            }
        }

        public void AddStudyUnit()
        {
            StudyUnit studyUnit = EDOSerializer.LoadStudyUnitInGroup(StudyUnits);
            if (studyUnit == null)
            {
                return;
            }
            AddStudyUnit(studyUnit);
        }
        #endregion

        #region StudyUnitを閉じるコマンド
        private ICommand closeStudyUnitCommand;
        public ICommand CloseStudyUnitCommand {
            get
            {
                if (closeStudyUnitCommand == null)
                {
                    closeStudyUnitCommand = new RelayCommand(param => this.CloseStudyUnit(), param => this.CanCloseStudyUnit);
                }
                return closeStudyUnitCommand;
            }
        }

        public bool CanCloseStudyUnit
        {
            get
            {
                return edoModel.IsExistGroup && edoModel.StudyUnits.Count > 1 && this.SelectedStudyUnit != null;
            }
        }

        public void CloseStudyUnit()
        {
            RemoveStudyUnit(SelectedStudyUnit);
        }
        #endregion 

        #region StudyUnitを保存するコマンド
        private ICommand saveStudyUnitCommand;
        public ICommand SaveStudyUnitCommand
        {
            get
            {
                if (saveStudyUnitCommand == null)
                {
                    saveStudyUnitCommand = new RelayCommand(param => this.SaveStudyUnit(), param => this.CanSaveStudyUnit);
                }
                return saveStudyUnitCommand;
            }
        }

        public bool CanSaveStudyUnit
        {
            get
            {
                return SelectedStudyUnit != null;
            }
        }

        public void SaveStudyUnit()
        {
            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            List<StudyUnitVM> otherStudyUnits = GetOtherStudyUnits(SelectedStudyUnit);
            EDOSerializer.SaveStudyUnit(SelectedStudyUnit, false, otherStudyUnits);
        }

        #endregion

        #region StudyUnitを名前をつけて保存するコマンド
        private ICommand saveAsStudyUnitCommand;
        public ICommand SaveAsStudyUnitCommand
        {
            get
            {
                if (saveAsStudyUnitCommand == null)
                {
                    saveAsStudyUnitCommand = new RelayCommand(param => this.SaveAsStudyUnit(), param => this.CanSaveAsStudyUnit);
                }
                return saveAsStudyUnitCommand;
            }
        }

        public bool CanSaveAsStudyUnit
        {
            get
            {
                return SelectedStudyUnit != null;
            }
        }

        public void SaveAsStudyUnit()
        {
            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            List<StudyUnitVM> otherStudyUnits = GetOtherStudyUnits(SelectedStudyUnit);
            EDOSerializer.SaveStudyUnit(SelectedStudyUnit, true, otherStudyUnits);
        }
        #endregion


        private ICommand addCategorySchemeCommand;
        public ICommand AddCategorySchemeCommand
        {
            get
            {
                if (addCategorySchemeCommand == null)
                {
                    addCategorySchemeCommand = new RelayCommand(param => this.AddCategoryScheme(), param => this.CanAddCategoryScheme);
                }
                return addCategorySchemeCommand;
            }
        }

        public bool CanAddCategoryScheme
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return SelectedStudyUnit.CanAddCategoryScheme;
            }
        }

        public void AddCategoryScheme()
        {
            SelectedStudyUnit.AddCategoryScheme();
        }

        private ICommand addCodeSchemeCommand;
        public ICommand AddCodeSchemeCommand
        {
            get
            {
                if (addCodeSchemeCommand == null)
                {
                    addCodeSchemeCommand = new RelayCommand(param => this.AddCodeScheme(), param => this.CanAddCodeScheme);
                }
                return addCodeSchemeCommand;
            }
        }

        public bool CanAddCodeScheme
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return SelectedStudyUnit.CanAddCodeScheme;
            }
        }

        public void AddCodeScheme()
        {
            SelectedStudyUnit.AddCodeScheme();
        }


        private ICommand addFromCategorySchemeCommand;
        public ICommand AddFromCategorySchemeCommand
        {
            get
            {
                if (addFromCategorySchemeCommand == null)
                {
                    addFromCategorySchemeCommand = new RelayCommand(param => this.AddFromCategoryScheme(), param => this.CanAddFromCategoryScheme);
                }
                return addFromCategorySchemeCommand;
            }
        }

        public bool CanAddFromCategoryScheme
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return SelectedStudyUnit.CanAddFromCategoryScheme;
            }
        }

        public void AddFromCategoryScheme()
        {
            SelectedStudyUnit.AddFromCategoryScheme();
        }


        private ICommand addFromCategoryCommand;
        public ICommand AddFromCategoryCommand
        {
            get
            {
                if (addFromCategoryCommand == null)
                {
                    addFromCategoryCommand = new RelayCommand(param => this.AddFromCategory(), param => this.CanAddFromCategory);
                }
                return addFromCategoryCommand;
            }
        }

        public bool CanAddFromCategory
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return SelectedStudyUnit.CanAddFromCategory;
            }
        }

        public void AddFromCategory()
        {
            SelectedStudyUnit.AddFromCategory();
        }


        private ICommand addDataSetCommand;
        public ICommand AddDataSetCommand
        {
            get
            {
                if (addDataSetCommand == null)
                {
                    addDataSetCommand = new RelayCommand(param => this.AddDataSet(), param => this.CanAddDataSet);
                }
                return addDataSetCommand;
            }
        }

        public bool CanAddDataSet
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return SelectedStudyUnit.CanAddDataSet;
            }
        }

        public void AddDataSet()
        {
            SelectedStudyUnit.AddDataSet();
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

        public bool CanAddQuestionGroup
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return SelectedStudyUnit.CanAddQuestionGroup;
            }
        }

        public void AddQuestionGroup()
        {
            SelectedStudyUnit.AddQuestionGroup();
        }

        #endregion

        #region エクスポートコマンド

        private ICommand exportGroupCommand;
        public ICommand ExportGroupCommand {
            get
            {
                if (exportGroupCommand == null)
                {
                    exportGroupCommand = new RelayCommand(param => this.ExportGroup(), param => this.CanExportGroup);
                }
                return exportGroupCommand;
            }
        }

        public bool CanExportGroup
        {
            get
            {
                return edoModel.IsExistGroup;
            }
        }

        public void ExportGroup()
        {
            if (!SelectedItem.ValidateCurrentItem())
            {
                return;
            }
//            Debug.WriteLine("Before IsModified=" + IsModified);
            GroupVM.SyncModel();
//            Debug.WriteLine("After IsModified=" + IsModified);
            try
            {
                DDISerializer.ExportGroup(config, GroupVM, StudyUnits);
            }
            catch (EDOException ex)
            {
                ErrorDialog.Instance().ShowError(this, ex.WriteErrors);
            }
        }

        private ICommand exportStudyUnitCommand;
        public ICommand ExportStudyUnitCommand
        {
            get
            {
                if (exportStudyUnitCommand == null)
                {
                    exportStudyUnitCommand = new RelayCommand(param => ExportStudyUnit(), param => CanExportStudyUnit);
                }
                return exportStudyUnitCommand;
            }
        }

        public bool CanExportStudyUnit
        {
            get
            {
                return SelectedStudyUnit != null;
            }
        }

        public void ExportStudyUnit()
        {
            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            try
            {
                DDISerializer.ExportStudyUnit(config, SelectedStudyUnit);
            }
            catch (EDOException ex)
            {
                ErrorDialog.Instance().ShowError(this, ex.WriteErrors);
            }
        }

        private ICommand exportQuestionnaireCommand;
        public ICommand ExportQuestionnaireCommand
        {
            //調査票のエクスポート
            get
            {
                if (exportQuestionnaireCommand == null)
                {
                    exportQuestionnaireCommand = new RelayCommand(param => ExportQuestionnaire(), param => CanExportQuestionnaire);
                }
                return exportQuestionnaireCommand;
            }
        }

        public bool CanExportQuestionnaire
        {
            get
            {
                return SelectedStudyUnit != null;
            }
        }


        public void ExportQuestionnaire()
        {
            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            try
            {
                MiscSerializer.ExportQuestionnaire(config, SelectedStudyUnit);
            }
            catch (EDOException ex)
            {
                ErrorDialog.Instance().ShowError(this, ex.WriteErrors);
            }

        }

        private ICommand exportCodebookCommand;
        public ICommand ExportCodebookCommand
        {
            get
            {
                if (exportCodebookCommand == null)
                {
                    exportCodebookCommand = new RelayCommand(param => ExportCodebook(), param => CanExportCodebook);
                }
                return exportCodebookCommand;
            }
        }

        public bool CanExportCodebook
        {
            get
            {
                return SelectedStudyUnit != null;
            }
        }

        public void ExportCodebook()
        {
            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            try
            {
                MiscSerializer.ExportCodebook(SelectedStudyUnit);
            }
            catch (EDOException ex)
            {
                ErrorDialog.Instance().ShowError(this, ex.WriteErrors);
            }
        }

        private ICommand exportSetupSyntaxCommand;
        public ICommand ExportSetupSyntaxCommand
        {
            get
            {
                if (exportSetupSyntaxCommand == null)
                {
                    exportSetupSyntaxCommand = new RelayCommand(param => ExportSetupSyntax(), param => CanExportSetupSyntax);
                }
                return exportSetupSyntaxCommand;
            }
        }

        public bool CanExportSetupSyntax
        {
            get
            {
                return SelectedStudyUnit != null;
            }
        }

        public void ExportSetupSyntax()
        {
            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            try
            {
                MiscSerializer.ExportSetupSyntax(SelectedStudyUnit);
            }
            catch (EDOException ex)
            {
                ErrorDialog.Instance().ShowError(this, ex.WriteErrors);
            }
        }

        #endregion エクスポートコマンド

        #region インポートコマンド

        private ICommand importDDICommand;
        public ICommand ImportDDICommand
        {
            get
            {
                if (importDDICommand == null)
                {
                    importDDICommand = new RelayCommand(param => ImportDDI(), param => CanImportDDI);
                }
                return importDDICommand;
            }
        }

        public bool CanImportDDI
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return true;
            }
        }


        private void RecreateViewModels()
        {
            //現在のEDOModelの内容でViewModelを作り直す。
            //・Undoバッファは初期化する
            //・loadHashは以前のものをキープする。
            //DDI/SPSSのインポート後に行われる処理
            undoManager.IsEnabled = false;
            string orgLoadHash = loadHash; // ファイルを読み込んだときのハッシュをキープする(変更されたかどうかのチェックを表示するため)
            VMState state = SaveState();
            createViewModels(edoModel);
            SelectedStudyUnit.CompleteVariables(); //データセットなどを作った状態を最初の状態にしないとグリッド編集後にUndoStackにつまれてしまう。
            undoManager.Init(edoModel, this);
            loadHash = orgLoadHash;
            undoManager.IsEnabled = true;
        }

        public void ImportDDI()
        {
            //これが必要かどうか今ひとつ自身なし
            //テキストボックス編集中にインポートしたときただしく戻すために必要かも。

            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            if (DDISerializer.ImportDDI(SelectedStudyUnit, edoModel))
            {
                RecreateViewModels();
            }
        }

        private ICommand importSpssCommand;
        public ICommand ImportSpssCommand
        {
            get
            {
                if (importSpssCommand == null)
                {
                    importSpssCommand = new RelayCommand(param => ImportSpss(), param => CanImportSpss);
                }
                return importSpssCommand;
            }
        }

        public bool CanImportSpss
        {
            get
            {
                if (SelectedStudyUnit == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void ImportSpss()
        {
            //これが必要かどうか今ひとつ自身なし
            //テキストボックス編集中にインポートしたときただしく戻すために必要かも。
            if (!SelectedStudyUnit.ValidateCurrentItem())
            {
                return;
            }
            StudyUnitVM selectedStudyUnit = SelectedStudyUnit;
            if (MiscSerializer.ImportSpss(selectedStudyUnit))
            {
                RecreateViewModels();
            }
        }

        #endregion 

        public AddCommandImpl AddCommand { get; set; }
        public HelpCommandImpl HelpCommand { get; set; }
        public class HelpCommandImpl : ICommand
        {
            private MainWindowVM viewModel;
            public HelpCommandImpl(MainWindowVM viewModel)
            {
                this.viewModel = viewModel;
            }

            #region ICommand メンバー

            public bool CanExecute(object parameter)
            {
                return true;
            }

#pragma warning disable
            public event EventHandler CanExecuteChanged;
#pragma warning restore


            public void Execute(object parameter)
            {
                AboutWindow aboutWindow = new AboutWindow();
                aboutWindow.ShowDialog();

            }
            #endregion
        }



        public class AddCommandImpl : ICommand
        {
            private MainWindowVM viewModel;
            public AddCommandImpl(MainWindowVM viewModel)
            {
                this.viewModel = viewModel;
            }

            #region ICommand メンバー

            public bool CanExecute(object parameter)
            {
                return true;
            }

#pragma warning disable
            public event EventHandler CanExecuteChanged;
#pragma warning restore

            public void Execute(object parameter)
            {
//                viewModel.AddStudyUnit();
            }
            #endregion
        }

        public void ShowMenuItem(EDOUnitVM edoUnit, MenuItemVM menuItem)
        {
            SelectedItem = edoUnit;
            Window window = Application.Current.MainWindow;
            //こうしないと現在表示中のStudyUnit以外でエラーが発生したときに指定メニューが表示されない。
            window.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                edoUnit.SelectMenuItem(menuItem);
            }));
        }

        public void SaveConfig()
        {
            string lastFile = null;
            if (edoModel.Group != null)
            {
                lastFile = edoModel.Group.PathName;
            }
            else if (edoModel.StudyUnits.Count > 0)
            {
                lastFile = edoModel.StudyUnits[0].PathName;
            }
            config.LastFile = lastFile;

            config.Save();
        }

        private string  selectedFile;
        public string SelectedFile
        {
            get
            {
                return selectedFile;
            }
            set
            {
                if (selectedFile != value)
                {
                    selectedFile = value;                    
                    if (selectedFile != null)
                    {
                        Window window = Application.Current.MainWindow;
                        window.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            DoOpen(true, selectedFile);
                        }));
                    }
                    NotifyPropertyChanged("SelectedFile");
                }
            }
        }

        private ObservableCollection<Option> recentFiles = new ObservableCollection<Option>();
        public ObservableCollection<Option> RecentFiles
        {
            get
            {
                return recentFiles;
            }
        }

        private void UpdateRecentFiles()
        {
            SelectedFile = null;
            recentFiles.Clear();
            List<string> files = recentFileList.RecentFiles;
            List<string> shortFiles = recentFileList.ShortFiles(files);
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];
                string label = shortFiles[i];
                recentFiles.Add(new Option(file, label));
            }
        }
    }

}
