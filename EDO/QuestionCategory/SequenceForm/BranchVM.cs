using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Util;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;
using EDO.Core.Model;
using EDO.Properties;

namespace EDO.QuestionCategory.SequenceForm
{
    // IF / ELSE IF / ELSE のそれぞれに対応。
    public class BranchVM : BaseVM
    {
        public const string TYPE_IF_CODE = "1";
        public const string TYPE_ELSE_IF_CODE = "2";
        public const string TYPE_ELSE_CODE = "3";
        public static Option TYPE_IF = new Option(TYPE_IF_CODE, "IF");
        public static Option TYPE_ELSE_IF = new Option(TYPE_ELSE_IF_CODE, "ELSE IF");
        public static Option TYPE_ELSE = new Option(TYPE_ELSE_CODE, "ELSE");

        public string TypeNameOf(string typeCode)
        {
            List<Option> types = new List<Option>()
            {
                TYPE_IF, TYPE_ELSE_IF, TYPE_ELSE
            };
            return Option.FindLabel(types, typeCode);
        }

        public BranchVM(string typeCode)
        {
            this.typeCode = typeCode;
            condGroups = new ObservableCollection<CondGroupVM>();
            changeableTypes = new ObservableCollection<Option>();
            changeableTypes.Add(TYPE_ELSE_IF);
            changeableTypes.Add(TYPE_ELSE);
            IsNew = true;
            IsIgnoreValidation = true;
        }

        #region メソッド

        public void Init()
        {
            ResetType();
        }

        private CondGroupVM CreateDefaultCondGroup()
        {
            CondGroupVM condGroup = new CondGroupVM() { Parent = this };
            CondVM cond = new CondVM() { Parent = condGroup };
            CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
            cond.SelectedQuestionConstruct = parent.TargetQuestionConstruct;
            cond.SelectedOperatorCode = Options.OPERATOR_EQUALS_CODE;
            condGroup.Conds.Add(cond);
            return condGroup;
        }

        private void ResetType()
        {
            if (IsTypeElse)
            {
                CondGroups.Clear();
            }
            else
            {
                CondGroupVM condGroup = CreateDefaultCondGroup();
                condGroups.Add(condGroup);

                CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
                ThenConstruct = parent.NextConstruct;
            }
        }

        private CreateBranchWindowVM WindowVM
        {
            get
            {
                return (CreateBranchWindowVM)Parent;
            }
        }

        public IBranchEditor Editor
        {
            get
            {
                return WindowVM.Editor;
            }
        }

        public bool IsFirst(CondGroupVM condGroup)
        {
            return condGroups.First() == condGroup;
        }

        public bool IsLast(CondGroupVM condGroup)
        {
            return condGroups.Last() == condGroup;
        }

        public BranchVM DeepCopy()
        {
            BranchVM newBranch = new BranchVM(TypeCode);
            newBranch.Parent = Parent;
            newBranch.Init();
            newBranch.IsNew = IsNew;
            newBranch.ThenConstruct = ThenConstruct;
            newBranch.CondGroups.Clear();
            foreach (CondGroupVM condGroup in condGroups)
            {
                CondGroupVM newGroup = condGroup.DeepCopy(newBranch);
                newBranch.CondGroups.Add(newGroup);
            }
            return newBranch;
        }

        private void OnCondGroupsOrderChanged()
        {
            foreach (CondGroupVM condGroup in CondGroups)
            {
                condGroup.OnOrderChanged();
            }
        }

        public bool CanRemoveCondGroup(CondGroupVM condGroup)
        {
            return CondGroups.Count > 1;
        }

        public void RemoveCondGroup(CondGroupVM condGroup)
        {
            condGroups.Remove(condGroup);
            OnCondGroupsOrderChanged();
        }

        public void AddCondGroup(CondGroupVM condGroup)
        {
            CondGroupVM newCondGroup = CreateDefaultCondGroup();
            condGroups.Add(newCondGroup);
            OnCondGroupsOrderChanged();
        }

        public bool CanUpCondGroup(CondGroupVM condGroup)
        {
            int index = CondGroups.IndexOf(condGroup);
            if (index == 0)
            {
                return false;
            }
            return true;
        }

        public void UpCondGroup(CondGroupVM condGroup)
        {
            int index = CondGroups.IndexOf(condGroup);
            CondGroups.Move(index, index - 1);
            OnCondGroupsOrderChanged();
        }

        public bool CanDownCondGroup(CondGroupVM condGroup)
        {
            int index = CondGroups.IndexOf(condGroup);
            if (index == CondGroups.Count - 1)
            {
                return false;
            }
            return true;        
        }

        public void DownCondGroup(CondGroupVM condGroup)
        {
            int index = CondGroups.IndexOf(condGroup);
            CondGroups.Move(index, index + 1);
            OnCondGroupsOrderChanged();            
        }

        public void OnOrderChanged()
        {
            NotifyPropertyChanged("CanAddBranch");
            NotifyPropertyChanged("CanChangeType");
        }

        public void OnEditStarted()
        {
            //NotifyPropertyChanged("CanEditBranch");
            //NotifyPropertyChanged("CanAddBranch");
            //NotifyPropertyChanged("CanRemoveBranch");
        }

        #endregion

        #region フィールド・プロパティ

        public int No { get; set; }
        public bool IsNew { get; set; }
        public bool CanChangeType
        {
            get
            {
                CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
                return parent.CanChangeType(this);
            }
        }

        private string typeCode;
        public string TypeCode 
        { 
            get 
            { 
                return typeCode; 
            }
            set
            {
                if (typeCode != value)
                {
                    typeCode = value;
                    ResetType();
                    NotifyPropertyChanged("TypeCode");
                    NotifyPropertyChanged("IsTypeElse");
                }
            }
        }

        public string TypeName
        {
            get
            {
                return TypeNameOf(typeCode);
            }
        }

        public bool IsTypeIf
        {
            get
            {
                return typeCode == TYPE_IF_CODE;
            }
        }

        public bool IsTypeElseIf
        {
            get
            {
                return typeCode == TYPE_ELSE_IF_CODE;
            }
        }

        public bool IsTypeElse
        {
            get
            {
                return TypeCode == TYPE_ELSE_CODE;
            }
        }

        private string CreateIfOrElseIfExpression(string typeName)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(typeName);
            buf.Append(" ");
            foreach (CondGroupVM condGorup in CondGroups)
            {
                buf.Append(condGorup.Expression());
            }
            buf.Append(" THEN ");
            if (thenConstructNo != null)
            {
                buf.Append(thenConstructNo);
            }
            return buf.ToString();
        }

        private string CreateElseExpression()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(TYPE_ELSE.Label);
            buf.Append(" ");
            if (thenConstructNo != null)
            {
                buf.Append(thenConstructNo);
            }
            return buf.ToString();
        }

        public string Expression
        {
            get
            {
                if (IsTypeIf)
                {
                    return CreateIfOrElseIfExpression(TYPE_IF.Label);
                }
                else if (IsTypeElseIf)
                {
                    return CreateIfOrElseIfExpression(TYPE_ELSE_IF.Label);
                }
                return CreateElseExpression();
            }
        }

        private ObservableCollection<Option> changeableTypes;
        public ObservableCollection<Option> ChangeableTypes { get { return changeableTypes; } }

        private ObservableCollection<CondGroupVM> condGroups;
        public ObservableCollection<CondGroupVM> CondGroups { get { return condGroups; } }


        private ObservableCollection<ConstructVM> ThenConstructs
        {
            get
            {
                CreateBranchWindowVM window = (CreateBranchWindowVM)Parent;
                return window.ThenConstructs;
            }
        }

        //Then節で移動できるのはStatementとQuestionなのでConstructVMで保管する
        private bool IsValidThenConstructNo(string no)
        {
            return ConstructVM.FindByNo(ThenConstructs, no) != null;
        }

        private string thenConstructNo;
        public string ThenConstructNo
        {
            get
            {
                return thenConstructNo;
            }
            set
            {
                if (thenConstructNo != value && IsValidThenConstructNo(value))
                {
                    thenConstructNo = value;
                    NotifyPropertyChanged("ThenConstructNo");
                }
            }
        }

        public ConstructVM ThenConstruct
        {
            get
            {
                return ConstructVM.FindByNo(ThenConstructs, thenConstructNo);
            }
            set
            {
                if (value != null)
                {
                    thenConstructNo = value.No;
                }
                else
                {
                    thenConstructNo = null;
                }
            }
        }

        //private ConstructVM thenConstruct;
        //public ConstructVM ThenConstruct
        //{
        //    get
        //    {
        //        return thenConstruct;
        //    }
        //    set
        //    {
        //        if (thenConstruct != value)
        //        {
        //            thenConstruct = value;
        //            NotifyPropertyChanged("ThenConstruct");
        //        }
        //    }
        //}

        public List<CondGroupVM> ValidCondGroups
        {
            get
            {
                List<CondGroupVM> validCondGroups = new List<CondGroupVM>();
                foreach (CondGroupVM condGroup in CondGroups)
                {
                    if (condGroup.IsValid)
                    {
                        validCondGroups.Add(condGroup);
                    }
                }
                return validCondGroups;
            }
        }

        public bool IsValid
        {
            get
            {
                if (thenConstructNo == null)
                {
                    return false;
                }
                if (IsTypeElse)
                {
                    return true;
                }
                if (ValidCondGroups.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region コマンド

        private ICommand editBranchCommand;
        public ICommand EditBranchCommand
        {
            get
            {
                if (editBranchCommand == null)
                {
                    editBranchCommand = new RelayCommand(param => EditBranch(), param => CanEditBranch);
                }
                return editBranchCommand;
            }
        }

        public bool CanEditBranch
        {
            get
            {
                CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
                return parent.CanEditBranch(this);
            }
        }

        public void EditBranch()
        {
            CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
            parent.EditBranch(this);
        }

        private ICommand removeBranchCommand;
        public ICommand RemoveBranchCommand
        {
            get
            {
                if (removeBranchCommand == null)
                {
                    removeBranchCommand = new RelayCommand(param => RemoveBranch(), param => CanRemoveBranch);
                }
                return removeBranchCommand;
            }
        }

        public bool CanRemoveBranch
        {
            get
            {
                CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
                return parent.CanRemoveBranch(this);
            }
        }

        public void RemoveBranch()
        {
            CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
            parent.RemoveBranch(this);
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
                CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
                bool result = parent.CanAddBranch(this);
                return result;
            }
        }

        public void AddBranch()
        {
            CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
            parent.AddBranch(this);
        }

        private ICommand submitCommand;
        public ICommand SubmitCommand
        {
            get
            {
                if (submitCommand == null)
                {
                    submitCommand = new RelayCommand(param => Submit(), param => CanSubmit);
                }
                return submitCommand;
            }
        }

        private bool CanSubmit
        {
            get
            {
                return true;
            }
        }


        private void IgnoreValidation(bool ignore)
        {
            IsIgnoreValidation = ignore;
            foreach (CondGroupVM condGroup in condGroups)
            {
                condGroup.IgnoreValidation(ignore);
            }
        }

        public void Submit()
        {
            IgnoreValidation(false);
            bool valid = Editor.ValidateEditingBranch();
            IgnoreValidation(true);
            if (!valid)
            {
                return;
            }
            CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
            parent.SubmitEditingBranch();
        }

        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {
                    cancelCommand = new RelayCommand(param => Cancel(), param => CanCancel);
                }
                return cancelCommand;
            }
        }

        private bool CanCancel
        {
            get
            {
                return true;
            }
        }

        public void Cancel()
        {
            CreateBranchWindowVM parent = (CreateBranchWindowVM)Parent;
            parent.CancelEditingBranch();
        }

        public override string this[string columnName]
        {
            get
            {
                if (IsIgnoreValidation)
                {
                    return null;
                }
                if (columnName == "ThenConstructNo")
                {
                    if (string.IsNullOrEmpty(ThenConstructNo))
                    {
                        return Resources.SelectQuestionOrSentence; //質問または説明文を選択してください
                    }
                }
                return null;
            }
        }

        #endregion

    }
}
