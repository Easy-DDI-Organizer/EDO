using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.QuestionForm;
using EDO.Core.Util;
using System.Windows.Input;
using System.Windows;
using EDO.Core.View;
using EDO.Properties;
using EDO.QuestionCategory.SequenceForm.Chart;

namespace EDO.QuestionCategory.SequenceForm
{
    public class SequenceFormVM :FormVM
    {
        private static readonly string PREFIX = Resources.Sequence; //順序

        public SequenceFormVM(StudyUnitVM studyUnit)
            : base(studyUnit)
        {
            controlConstructSchemes = new ObservableCollection<ControlConstructSchemeVM>();
            int i = 1;
            foreach (ControlConstructScheme model in studyUnit.ControlConstructSchemeModels)
            {
                ControlConstructSchemeVM controlConstructScheme = new ControlConstructSchemeVM(model)
                {
                    Parent = this,
                    OrderNo = i++,
                    OrderPrefix = PREFIX
                };
                controlConstructScheme.Init();
                controlConstructSchemes.Add(controlConstructScheme);
            }
            modelSyncher = new ModelSyncher<ControlConstructSchemeVM, ControlConstructScheme>(
                this, controlConstructSchemes, studyUnit.ControlConstructSchemeModels);
        }

        private ModelSyncher<ControlConstructSchemeVM, ControlConstructScheme> modelSyncher;
        private ObservableCollection<ControlConstructSchemeVM> controlConstructSchemes;
        public ObservableCollection<ControlConstructSchemeVM> ControlConstructSchemes { get { return controlConstructSchemes; } }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedControlConstructScheme = EDOUtils.Find(ControlConstructSchemes, state.State1);
            }
            if (SelectedControlConstructScheme == null)
            {
                SelectedControlConstructScheme = EDOUtils.GetFirst(ControlConstructSchemes);
            }
        }


        public override VMState SaveState()
        {
            VMState state = new VMState();
            if (SelectedControlConstructScheme != null)
            {
                state.State1 = SelectedControlConstructScheme.Id;
            }
            return state;
        }


        private ControlConstructSchemeVM selectedControlConstructScheme;
        public ControlConstructSchemeVM SelectedControlConstructScheme
        {
            get
            {
                return selectedControlConstructScheme;
            }
            set
            {
                if (selectedControlConstructScheme != value)
                {
                    selectedControlConstructScheme = value;
                    NotifyPropertyChanged("SelectedControlConstructScheme");
                }
            }
        }

        private ICommand addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(param => AddControlConstructScheme(), param => CanAddControlConstructScheme);
                }
                return addCommand;
            }
        }

        private bool CanAddControlConstructScheme
        {
            get
            {
                return true;
            }
        }

        public void AddControlConstructScheme()
        {
            ControlConstructSchemeVM scheme = new ControlConstructSchemeVM();
            scheme.OrderNo = EDOUtils.GetMaxOrderNo<ControlConstructSchemeVM>(controlConstructSchemes) + 1;
            scheme.OrderPrefix = PREFIX;
            scheme.Init();
            controlConstructSchemes.Add(scheme);
            SelectedControlConstructScheme = scheme;
            Memorize();
        }

        private ICommand removeCommand;
        public ICommand RemoveCommand
        {
            get
            {
                if (removeCommand == null)
                {
                    removeCommand = new RelayCommand(param => RemoveControlConstructScheme(), param => CanRemoveControlConstructScheme);
                }
                return removeCommand;
            }
        }


        private bool CanRemoveControlConstructScheme
        {
            get
            {
                if (SelectedControlConstructScheme == null)
                {
                    return false;
                }
                return controlConstructSchemes.Count > 1;
            }
        }

        public void RemoveControlConstructScheme()
        {
            controlConstructSchemes.Remove(SelectedControlConstructScheme);
            SelectedControlConstructScheme = controlConstructSchemes.Last();
        }

        private ControlConstructSchemeVM FindDefaultControlConstructScheme()
        {
            string id = StudyUnit.DefaultControlConstructSchemeId;
            return FindControlConstructScheme(id);
        }

        public ControlConstructSchemeVM FindControlConstructScheme(string dataSetId)
        {
            return EDOUtils.Find(controlConstructSchemes, dataSetId);
        }

        public void CreateConstructs(ICollection<QuestionVM> questions)
        {
            //質問編集画面の表示終了時によびだされる
            ControlConstructSchemeVM scheme = FindDefaultControlConstructScheme();
            if (scheme == null)
            {
                return;
            }
            foreach (QuestionVM question in questions)
            {
                if (question.IsCreatedConstruct)
                {
                    continue;
                }
                question.IsCreatedConstruct = true;
                scheme.InsertQuestionConstruct(question, false);
            }
        }

        #region コマンド

        public ICommand RemoveConstructCommand
        {
            get
            {
                if (SelectedControlConstructScheme == null)
                {
                    return null;
                }
                return SelectedControlConstructScheme.RemoveConstructCommand;
            }
        }

        public ICommand UpConstructCommand
        {
            get
            {
                //キーバインドから呼び出されるコマンド
                if (SelectedControlConstructScheme == null)
                {
                    return null;
                }
                return SelectedControlConstructScheme.UpConstructCommand;
            }
        }

        public ICommand DownConstructCommand
        {
            get
            {
                if (SelectedControlConstructScheme == null)
                {
                    return null;
                }
                return SelectedControlConstructScheme.DownConstructCommand;
            }
        }

        public bool CanReorderVariable()
        {
            //ビューに問い合わせて、ソートされていたら順序変更不可能と判断する。
            SequenceFormView window = (SequenceFormView)View;
            if (window != null && window.IsDataGridSorting)
            {
                return false;
            }
            return true;
        }

        public void FocusCell()
        {
            SequenceFormView window = (SequenceFormView)View;
            if (window != null)
            {
                window.FocusCell();
            }
        }

        public void EditCurrentRow()
        {
            ControlConstructSchemeVM scheme = SelectedControlConstructScheme;
            if (scheme == null)
            {
                return;
            }
            if (scheme.CanEditConstruct)
            {
                scheme.EditConstruct();
            }
        }

        #endregion 

    
        //質問が削除されたときの処理を行う
        public void OnRemoveQuestion(QuestionVM question)
        {
            foreach (ControlConstructSchemeVM controlConstructScheme in controlConstructSchemes)
            {
                controlConstructScheme.RemoveQuestion(question);
            }
        }
    }
}
