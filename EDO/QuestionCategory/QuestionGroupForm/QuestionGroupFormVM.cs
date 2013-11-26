using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Main;
using System.Collections.ObjectModel;
using EDO.Core.Util;
using EDO.Core.Model;
using EDO.Core.View;
using System.Windows;
using System.Windows.Input;
using EDO.QuestionCategory.QuestionForm;
using EDO.Properties;

namespace EDO.QuestionCategory.QuestionGroupForm
{
    public class QuestionGroupFormVM :FormVM
    {
        public QuestionGroupFormVM(StudyUnitVM studyUnit) 
            :base(studyUnit)
        {
            ObservableCollection<QuestionVM> allQuestions = studyUnit.AllQuestions;
            questionGroups = new ObservableCollection<QuestionGroupVM>();
            foreach (QuestionGroup questionGroupModel in studyUnit.QuestionGroupModels)
            {
                QuestionGroupVM questionGroup = new QuestionGroupVM(questionGroupModel, allQuestions)
                {
                    Parent = this
                };
                questionGroups.Add(questionGroup);
            }
            modelSyncher = new ModelSyncher<QuestionGroupVM, QuestionGroup>(this, questionGroups, studyUnit.QuestionGroupModels);
        }

        private ObservableCollection<QuestionGroupVM> questionGroups;
        private ModelSyncher<QuestionGroupVM, QuestionGroup> modelSyncher;
        public ObservableCollection<QuestionGroupVM> QuestionGroups { get { return questionGroups; } }

        public void FocusCell()
        {
            QuestionGroupFormView window = (QuestionGroupFormView)View;
            if (window != null)
            {
                window.FocusCell();
            }
        }

        protected override void Reload(VMState state)
        {
            SelectedQuestionGroup = EDOUtils.FindOrFirst(questionGroups, state);
        }

        public override VMState SaveState()
        {
            if (SelectedQuestionGroup == null)
            {
                return null;
            }
            return new VMState(SelectedQuestionGroup.Id);
        }


        private QuestionGroupVM selectedQuestionGroup;
        public QuestionGroupVM SelectedQuestionGroup
        {
            get
            {
                return selectedQuestionGroup;
            }
            set
            {
                if (selectedQuestionGroup != value)
                {
                    selectedQuestionGroup = value;
                    NotifyPropertyChanged("SelectedQuestionGroup");
                }
            }
        }

        public void AddQuestionGroup()
        {
            InputDialog dlg = new InputDialog();
            dlg.Title = Resources.InputQuestionGroupName; //質問グループの名前を入力してください
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                QuestionGroup questionGroupModel = new QuestionGroup() { Title = dlg.textBox.Text };
                QuestionGroupVM questionGroup = new QuestionGroupVM(questionGroupModel) { Parent = this };
                questionGroups.Add(questionGroup);
                if (SelectedQuestionGroup == null)
                {
                    SelectedQuestionGroup = questionGroup;
                }
                Memorize();
            }
        }

        private ICommand removeQuestionGroupCommand;
        public ICommand RemoveQuestionGroupCommand
        {
            get
            {
                if (removeQuestionGroupCommand == null)
                {
                    removeQuestionGroupCommand = new RelayCommand(param => RemoveQuestionGroup(), param => CanRemoveQuestionGroup);
                }
                return removeQuestionGroupCommand;
            }
        }

        public bool CanRemoveQuestionGroup
        {
            get
            {
                if (SelectedQuestionGroup == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveQuestionGroup()
        {
            QuestionGroups.Remove(SelectedQuestionGroup);
        }

        // QuestionGroupのコマンドを直接呼び出すとnullのときの動作がおかしくなる(しかも選択された後に復帰しない)ので
        // コマンド自体を実装しなおすしかない。
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

        public bool CanAddQuestion
        {
            get
            {
                if (SelectedQuestionGroup == null)
                {
                    return false;
                }
                return SelectedQuestionGroup.CanAddQuestion;
            }
        }

        public void AddQuestion()
        {
            SelectedQuestionGroup.AddQuestion();
        }

        private ICommand removeQuestionCommand;
        public ICommand RemoveQuestionCommand
        {
            get
            {
                if (removeQuestionCommand == null)
                {
                    removeQuestionCommand = new RelayCommand(param => RemoveQuestion(), param => CanRemoveQuestion);
                }
                return removeQuestionCommand;
            }
        }

        public bool CanRemoveQuestion
        {
            get
            {
                if (SelectedQuestionGroup == null)
                {
                    return false;
                }
                return SelectedQuestionGroup.CanRemoveQuestion;
            }
        }

        public void RemoveQuestion()
        {
            SelectedQuestionGroup.RemoveQuestion();
        }

        private ICommand upQuestionCommand;
        public ICommand UpQuestionCommand
        {
            get
            {
                if (upQuestionCommand == null)
                {
                    upQuestionCommand = new RelayCommand(param => UpQuestion(), param => CanUpQuestion);
                }
                return upQuestionCommand;
            }
        }

        public bool CanUpQuestion
        {
            get
            {
                if (SelectedQuestionGroup == null)
                {
                    return false;
                }
                return SelectedQuestionGroup.CanUpQuestion;
            }
        }

        public void UpQuestion()
        {
            SelectedQuestionGroup.UpQuestion();
        }

        private ICommand downQuestionCommand;
        public ICommand DownQuestionCommand
        {
            get
            {
                if (downQuestionCommand == null)
                {
                    downQuestionCommand = new RelayCommand(param => DownQuestion(), param => CanDownQuestion);
                }
                return downQuestionCommand;
            }
        }

        public bool CanDownQuestion
        {
            get
            {
                if (SelectedQuestionGroup == null)
                {
                    return false;
                }
                return SelectedQuestionGroup.CanDownQuestion;
            }
        }

        public void DownQuestion()
        {
            SelectedQuestionGroup.DownQuestion();
        }

        public void OnRemoveQuestion(QuestionVM question)
        {
            foreach (QuestionGroupVM questionGroup in QuestionGroups)
            {
                questionGroup.Questions.Remove(question);
            }
        }

        public void SyncQuestions()
        {

            foreach (QuestionGroupVM questionGroup in QuestionGroups)
            {
                for (int i = questionGroup.Questions.Count - 1; i >= 0; i-- )
                {
                    QuestionVM question = questionGroup.Questions[i];
                    if (!question.IsResponseTypeChoices)
                    {
                        questionGroup.Questions.RemoveAt(i);
                    }
                }
            }
        }

        internal QuestionGroupVM FindQuestionGroup(string questionGroupId)
        {
            foreach (QuestionGroupVM questionGroup in QuestionGroups)
            {
                if (questionGroup.Id == questionGroupId)
                {
                    return questionGroup;
                }
            }
            return null;
        }
    }
}
