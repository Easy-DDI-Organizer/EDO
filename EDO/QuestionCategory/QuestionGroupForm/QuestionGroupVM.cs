using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.Windows.Input;
using EDO.Core.Util;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.QuestionForm;
using EDO.Core.View;
using EDO.Properties;

namespace EDO.QuestionCategory.QuestionGroupForm
{
    public class QuestionGroupVM :BaseVM, IStringIDProvider, ITitleProvider
    {
        public QuestionGroupVM(QuestionGroup questionGroup) :this(questionGroup, new ObservableCollection<QuestionVM>())
        {
        }

        public QuestionGroupVM(QuestionGroup questionGroup, ObservableCollection<QuestionVM> allQuestions)
        {
            this.questionGroup = questionGroup;
            this.questions = new ObservableCollection<QuestionVM>();
            foreach (string questionId in questionGroup.QuestionIds)
            {
                QuestionVM question = EDOUtils.Find(allQuestions, questionId);
                if (question != null) //管理下にないVMを流用しているのでParentは変更しないこと。
                {
                    questions.Add(question);
                }
            }
        }

        private QuestionGroup questionGroup;

        private ObservableCollection<QuestionVM> questions;
        public ObservableCollection<QuestionVM> Questions { get { return questions; } }

        public override object Model { get { return questionGroup; } }
        public string Id { get { return questionGroup.Id; } }

        private void FocusCell()
        {
            QuestionGroupFormVM parent = (QuestionGroupFormVM)Parent;
            parent.FocusCell();
        }

        public string Title
        {
            get
            {
                return questionGroup.Title;
            }
            set
            {
                if (questionGroup.Title != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = Resources.EmptyQuestionGroup;
                    }
                    questionGroup.Title = value;
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public string Memo
        {
            get
            {
                return questionGroup.Memo;
            }
            set
            {
                if (questionGroup.Memo != value)
                {
                    questionGroup.Memo = value;
                    NotifyPropertyChanged("Memo");
                    Memorize();
                }
            }
        }

        public QuestionGroupOrientation Orientation
        {
            get
            {
                return questionGroup.Orientation;
            }
            set
            {
                if (questionGroup.Orientation != value)
                {
                    questionGroup.Orientation = value;
                    NotifyPropertyChanged("Orientation");
                    Memorize();
                }
            }
        }

        public QuestionGroupSentence Sentence
        {
            get
            {
                return questionGroup.Sentence;
            }
            set
            {
                if (questionGroup.Sentence != value)
                {

                    questionGroup.Sentence = value;
                    NotifyPropertyChanged("Sentence");
                    Memorize();
                }
            }
        }


        private object selectedQuestionItem;
        public object SelectedQuestionItem
        {
            get
            {
                return selectedQuestionItem;
            }
            set
            {
                if (selectedQuestionItem != value)
                {
                    selectedQuestionItem = value;
                    NotifyPropertyChanged("SelectedQuestionItem");
                }
            }
        }

        public QuestionVM SelectedQuestion
        {
            get
            {
                return SelectedQuestionItem as QuestionVM;
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
        private void UpdateModel()
        {
            questionGroup.QuestionIds.Clear();
            foreach (QuestionVM question in questions)
            {
                questionGroup.QuestionIds.Add(question.Id);
            }
            Memorize();
        }


        public bool CanAddQuestion
        {
            get
            {
                return true;
            }
        }

        public void AddQuestion()
        {
            ObservableCollection<QuestionVM> allQuestions = StudyUnit.AllChoicesQuestions;
            SelectObjectWindowVM<QuestionVM> vm = new SelectObjectWindowVM<QuestionVM>(allQuestions, "Content");
            QuestionVM question = SelectObjectWindow.Select(Resources.SelectQuestion, vm) as QuestionVM;//質問の選択
            if (question != null)
            {
                Questions.Add(question);
                UpdateModel();
            }
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
                return SelectedQuestion != null;
            }
        }

        public void RemoveQuestion()
        {
            Questions.Remove(SelectedQuestion);
            UpdateModel();
        }


        private int SelectedQuestionIndex
        {
            get
            {
                if (SelectedQuestion == null)
                {
                    return -1;
                }
                return Questions.IndexOf(SelectedQuestion);
            }
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
                if (SelectedQuestion == null)
                {
                    return false;
                }
                if (SelectedQuestionIndex == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public void UpQuestion()
        {
            QuestionVM question = SelectedQuestion;
            int index = SelectedQuestionIndex;
            Questions.Move(index, index - 1);
            UpdateModel();
            FocusCell();
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
                if (SelectedQuestion == null)
                {
                    return false;
                }
                if (SelectedQuestionIndex == Questions.Count - 1)
                {
                    return false;
                }
                return true;
            }
        }

        public void DownQuestion()
        {
            QuestionVM question = SelectedQuestion;
            int index = SelectedQuestionIndex;
            Questions.Move(index, index + 1);
            UpdateModel();
            FocusCell();
        }
    }
}
