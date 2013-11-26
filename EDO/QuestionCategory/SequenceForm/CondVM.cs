using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using System.Windows.Input;
using EDO.Core.Util;
using EDO.QuestionCategory.QuestionForm;
using System.Text.RegularExpressions;
using EDO.Properties;

namespace EDO.QuestionCategory.SequenceForm
{
    public class CondVM :BaseVM
    {
        public static CondVM CreateDefault(CondGroupVM parent)
        {
            CondVM cond = new CondVM() { Parent = parent };
            CreateBranchWindowVM window = parent.Window;
            cond.SelectedQuestionConstruct = window.TargetQuestionConstruct;
            cond.SelectedOperatorCode = Options.OPERATOR_EQUALS_CODE;
            return cond;
        }

        public CondVM()
        {
            IsIgnoreValidation = true;
        }

        #region メソッド

        public CondVM DeepCopy(BaseVM parent)
        {
            CondVM newCond = new CondVM();
            newCond.Parent = parent;
            newCond.SelectedQuestionConstruct = SelectedQuestionConstruct;
            newCond.selectedOperatorCode = SelectedOperatorCode;
            newCond.CondValue = CondValue;
            return newCond;
        }

        public bool IsLast
        {
            get
            {
                bool result = CondGroup.IsLastCond(this);
                return result;
            }
        }


        public void OnOrderChanged()
        {
            NotifyPropertyChanged("CanRemoveCond");
            NotifyPropertyChanged("CanAddCond");
            NotifyPropertyChanged("CanUpCond");
            NotifyPropertyChanged("CanDownCond");
        }

        #endregion

        #region プロパティ

        public int No { get; set; }

        public string Expression(string emptyValue)
        {
            StringBuilder buf = new StringBuilder();
            if (selectedQuestionConstructNo != null)
            {
                buf.Append(selectedQuestionConstructNo);
                buf.Append(" ");
            }
            if (selectedOperatorCode != null)
            {
                buf.Append(Option.FindLabel(Operators, selectedOperatorCode));
                buf.Append(" ");
            }
            string value = CondValue;
            if (string.IsNullOrEmpty(value))
            {
                value = emptyValue;
            }
            buf.Append(value);
            return buf.ToString();
        }

        private CondGroupVM CondGroup
        {
            get
            {
                return (CondGroupVM)Parent;
            }
        }

        private BranchVM Branch
        {
            get
            {
                return (BranchVM)CondGroup.Parent;
            }
        }

        private CreateBranchWindowVM Window
        {
            get
            {
                return (CreateBranchWindowVM)Branch.Parent;
            }
        }

        public ObservableCollection<QuestionConstructVM> QuestionConstructs 
        { 
            get 
            {
                return Window.QuestionConstructs;
            } 
        }

        //private QuestionConstructVM selectedQuestionConstruct;
        //public QuestionConstructVM SelectedQuestionConstruct
        //{
        //    get
        //    {
        //        return selectedQuestionConstruct;
        //    }
        //    set
        //    {
        //        if (selectedQuestionConstruct != value)
        //        {
        //            selectedQuestionConstruct = value;
        //            NotifyPropertyChanged("SelectedQuestionConstruct");
        //        }
        //    }
        //}

        private bool IsValidQuestionConstructNo(string no)
        {
            QuestionConstructVM questin = QuestionConstructVM.FindQuestionConstructByNo(QuestionConstructs, no);
            return questin != null;
        }

        private string selectedQuestionConstructNo;
        public string SelectedQuestionConstructNo
        {
            get
            {
                return selectedQuestionConstructNo;
            }
            set
            {
                if (selectedQuestionConstructNo != value && IsValidQuestionConstructNo(value))
                {

                    selectedQuestionConstructNo = value;
                    NotifyPropertyChanged("SelectedQuestionConstructNo");
                }
            }
        }

        public QuestionConstructVM SelectedQuestionConstruct
        {
            get
            {
                return QuestionConstructVM.FindQuestionConstructByNo(QuestionConstructs, selectedQuestionConstructNo);
            }
            set
            {
                if (value != null)
                {
                    selectedQuestionConstructNo = value.No;
                }
                else
                {
                    selectedQuestionConstructNo = null;
                }
            }
        }


        public ObservableCollection<Option> Operators
        {
            get
            {
                return Options.Operators;
            }
        }

        private string selectedOperatorCode;
        public string SelectedOperatorCode
        {
            get
            {
                return selectedOperatorCode;
            }
            set
            {
                if (selectedOperatorCode != value)
                {
                    selectedOperatorCode = value;
                    NotifyPropertyChanged("SelectedOperatorCode");
                }
            }
        }

        private string NormalizeCondValue(string value)
        {
            if (value == null)
            {
                return null;
            }
            Regex r = new Regex(@"[ 　\t]+");
            return r.Replace(value, "");
        }

        private string condValue;
        public string CondValue
        {
            get
            {
                return condValue;
            }
            set
            {
                string normalizedValue = NormalizeCondValue(value);
                if (condValue != normalizedValue)
                {
                    condValue = normalizedValue;
                    NotifyPropertyChanged("CondValue");
                }
            }
        }

        public bool IsValid
        {
            get
            {
                if (SelectedQuestionConstruct == null)
                {
                    return false;
                }
                if (SelectedOperatorCode == null)
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region コマンド
        private ICommand addCondCommand;
        public ICommand AddCondCommand
        {
            get
            {
                if (addCondCommand == null)
                {
                    addCondCommand = new RelayCommand(param => AddCond(), param => CanAddCond);
                }
                return addCondCommand;
            }
        }

        public bool CanAddCond
        {
            get
            {
                return CondGroup.CanAddCond(this);
            }
        }

        public void AddCond()
        {
            CondGroup.AddCond(this);
        }

        private ICommand removeCondCommand;
        public ICommand RemoveCondCommand
        {
            get
            {
                if (removeCondCommand == null)
                {
                    removeCondCommand = new RelayCommand(param => RemoveCond(), param => CanRemoveCond);
                }
                return removeCondCommand;
            }
        }

        public bool CanRemoveCond
        {
            get
            {
                return CondGroup.CanRemoveCond(this);
            }
        }

        public void RemoveCond()
        {
            CondGroup.RemoveCond(this);
        }

        private ICommand upCondCommand;
        public ICommand UpCondCommand
        {
            get
            {
                if (upCondCommand == null)
                {
                    upCondCommand = new RelayCommand(param => UpCond(), param => CanUpCond);
                }
                return upCondCommand;
            }
        }

        public bool CanUpCond
        {
            get
            {
                return CondGroup.CanUpCond(this);
            }
        }

        public void UpCond()
        {
            CondGroup.UpCond(this);
        }

        private ICommand downCondCommand;
        public ICommand DownCondCommand
        {
            get
            {
                if (downCondCommand == null)
                {
                    downCondCommand = new RelayCommand(param => DownCond(), param => CanDownCond);
                }
                return downCondCommand;
            }
        }

        public bool CanDownCond
        {
            get
            {
                return CondGroup.CanDownCond(this);
            }
        }

        public void DownCond()
        {
            CondGroup.DownCond(this);
        }

        #endregion

        public override string Error
        {
            get { return null; }
        }

        public override string this[string columnName]
        {
            get
            {
                if (IsIgnoreValidation)
                {
                    return null;
                }
                if (columnName == "SelectedQuestionConstructNo")
                {
                    if (string.IsNullOrEmpty(SelectedQuestionConstructNo))
                    {
                        return Resources.InputQuestionNumber; //質問番号を選択してください"
                    }
                } else if (columnName == "CondValue")
                {
                    if (string.IsNullOrEmpty(CondValue))
                    {
                        return Resources.InputValue; //値を入力してください
                    }
                    QuestionConstructVM questionConstruct = SelectedQuestionConstruct;
                    if (questionConstruct == null)
                    {
                        return Resources.InputQuestionNumber; //質問番号を選択してください
                    }
                }
                return null;
            }
        }
    }
}
