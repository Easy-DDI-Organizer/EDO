using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using System.Windows.Input;
using EDO.Core.Util;
using System.Diagnostics;

namespace EDO.QuestionCategory.SequenceForm
{
    public class CondGroupVM :BaseVM
    {
        public static  CondGroupVM CreateDefault(BranchVM parent)
        {
            CondGroupVM condGroup = new CondGroupVM() { Parent = parent };
            CondVM cond = CondVM.CreateDefault(condGroup);
            condGroup.Conds.Add(cond);
            return condGroup;
        }


        public CondGroupVM()
        {
            selectedConnectionCode = Options.CONNECTION_AND_CODE;
            conds = new ObservableCollection<CondVM>();
            IsIgnoreValidation = true;
        }

        #region メソッド

        public void OnOrderChanged()
        {
            NotifyPropertyChanged("IsFirst");
            NotifyPropertyChanged("IsLast");
            NotifyPropertyChanged("CanRemoveCondGroup");
            NotifyPropertyChanged("CanUpCondGroup");
            NotifyPropertyChanged("CanDownCondGroup");
        }

        public bool IsLastCond(CondVM cond)
        {
            return Conds.Last() == cond;
        }

        public CondGroupVM DeepCopy(BaseVM parent)
        {
            CondGroupVM newCondGroup = new CondGroupVM();
            newCondGroup.Parent = parent;
            foreach (CondVM cond in Conds)
            {
                CondVM newCond = cond.DeepCopy(newCondGroup);
                newCondGroup.Conds.Add(newCond);
            }
            return newCondGroup;
        }

        private void OnCondsOrderChanged()
        {
            foreach (CondVM cond in Conds)
            {
                cond.OnOrderChanged();
            }
        }


        public bool CanAddCond(CondVM cond)
        {
            return cond.IsLast;
        }

        public void AddCond(CondVM cond)
        {
            CondVM newCond = CondVM.CreateDefault(this);
            Conds.Add(newCond);
            OnCondsOrderChanged();
        }

        public bool CanRemoveCond(CondVM cond)
        {
            return Conds.Count > 1;
        }

        public void RemoveCond(CondVM cond)
        {
            Conds.Remove(cond);
            OnCondsOrderChanged();
        }

        public bool CanUpCond(CondVM cond)
        {
            int index = Conds.IndexOf(cond);
            if (index == 0)
            {
                return false;
            }
            return true;
        }

        public void UpCond(CondVM cond)
        {
            int index = Conds.IndexOf(cond);
            Conds.Move(index, index - 1);
            OnCondsOrderChanged();
        }

        public bool CanDownCond(CondVM cond)
        {
            int index = Conds.IndexOf(cond);
            if (index == Conds.Count - 1)
            {
                return false;
            }
            return true;
        }

        public void DownCond(CondVM cond)
        {
            int index = Conds.IndexOf(cond);
            Conds.Move(index, index + 1);
            OnCondsOrderChanged();
        }
        #endregion

        #region プロパティ
        public string Expression()
        {
            return Expression(null);
        }

        public string Expression(string emptyValue)
        {
            StringBuilder buf = new StringBuilder();
            if (!IsFirst)
            {
                buf.Append(" ");
                buf.Append(Options.FindLabel(Connections, selectedConnectionCode));
                buf.Append(" ");
            }
            if (Conds.Count > 1)
            {
                buf.Append(" (");
            }
            foreach (CondVM cond in conds)
            {
                if (cond != conds.First())
                {
                    buf.Append(" OR ");
                }
                buf.Append(cond.Expression(emptyValue));
            }
            if (Conds.Count > 1)
            {
                buf.Append(")");
            }
            return buf.ToString();
        }

        public QuestionConstructVM FirstQuestionConstruct
        {
            get
            {
                foreach (CondVM cond in Conds)
                {
                    if (cond.IsValid)
                    {
                        return cond.SelectedQuestionConstruct;
                    }
                }
                return null;
            }
        }

        public BranchVM Branch
        {
            get
            {
                return (BranchVM)Parent;
            }
        }


        public CreateBranchWindowVM Window
        {
            get
            {
                return (CreateBranchWindowVM)Branch.Parent;
            }
        }

        private ObservableCollection<CondVM> conds;
        public ObservableCollection<CondVM> Conds { get { return conds; } }

        public ObservableCollection<Option> Connections
        {
            get
            {
                return Options.Connections;
            }
        }

        private string selectedConnectionCode;
        public string SelectedConnectionCode
        {
            get
            {
                return selectedConnectionCode;
            }
            set
            {
                if (selectedConnectionCode != value)
                {
                    selectedConnectionCode = value;
                    NotifyPropertyChanged("SelectedConnectionCode");
                }
            }
        }

        public bool CanSelectConnection
        {
            get
            {
                return !Branch.IsFirst(this);
            }
        }

        public bool IsFirst
        {
            get
            {
                return Branch.IsFirst(this);
            }
        }

        public bool IsLast
        {
            get
            {
                return Branch.IsLast(this);
            }
        }

        private List<CondVM> ValidConds
        {
            get
            {
                List<CondVM> validConds = new List<CondVM>();
                foreach (CondVM cond in Conds)
                {
                    if (cond.IsValid)
                    {
                        validConds.Add(cond);
                    }
                }
                return validConds;
            }
        }

        public bool IsValid
        {
            get
            {
                return ValidConds.Count > 0;
            }
        }

        #endregion

        #region コマンド

        private ICommand removeCondGroupCommand;
        public ICommand RemoveCondGroupCommand
        {
            get
            {
                if (removeCondGroupCommand == null)
                {
                    removeCondGroupCommand = new RelayCommand(param => RemoveCondGroup(), param => CanRemoveCondGroup);
                }
                return removeCondGroupCommand;
            }
        }

        public bool CanRemoveCondGroup
        {
            get
            {
               return Branch.CanRemoveCondGroup(this);
            }
        }

        public void RemoveCondGroup()
        {
            Branch.RemoveCondGroup(this);
        }

        private ICommand addCondGroupCommand;
        public ICommand AddCondGroupCommand
        {
            get
            {
                if (addCondGroupCommand == null)
                {
                    addCondGroupCommand = new RelayCommand(param => AddCondGroup(), param => CanAddCondGroup);
                }
                return addCondGroupCommand;
            }
        }

        private bool CanAddCondGroup
        {
            get
            {
                return true;
            }
        }

        public void AddCondGroup()
        {
            Branch.AddCondGroup(this);
        }

        private ICommand upCondGroupCommand;
        public ICommand UpCondGroupCommand
        {
            get
            {
                if (upCondGroupCommand == null)
                {
                    upCondGroupCommand = new RelayCommand(param => UpCondGroup(), param => CanUpCondGroup);
                }
                return upCondGroupCommand;
            }
        }

        public bool CanUpCondGroup
        {
            get
            {
                return Branch.CanUpCondGroup(this);
            }
        }

        public void UpCondGroup()
        {
            Branch.UpCondGroup(this);
        }

        private ICommand downCondGroupCommand;
        public ICommand DownCondGroupCommand
        {
            get
            {
                if (downCondGroupCommand == null)
                {
                    downCondGroupCommand = new RelayCommand(param => DownCondGroup(), param => CanDownCondGroup);
                }
                return downCondGroupCommand;
            }
        }

        public bool CanDownCondGroup
        {
            get
            {
                return Branch.CanDownCondGroup(this);
            }
        }

        public void DownCondGroup()
        {
            Branch.DownCondGroup(this);
        }

        #endregion

        public void IgnoreValidation(bool ignore)
        {
            IsIgnoreValidation = ignore;
            foreach (CondVM cond in conds)
            {
                cond.IsIgnoreValidation = ignore;
            }
        }

    }
}
