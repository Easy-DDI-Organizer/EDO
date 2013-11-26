using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Main;
using EDO.QuestionCategory.ConceptForm;
using EDO.QuestionCategory.CodeForm;
using EDO.VariableCategory.VariableForm;
using EDO.Core.Model;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using EDO.Core.Util;
using EDO.QuestionCategory.CategoryForm;
using EDO.Core.View;
using EDO.Properties;

namespace EDO.QuestionCategory.QuestionForm
{
    public class QuestionFormVM :FormVM
    {
        private Dictionary<string, ObservableCollection<QuestionVM>> questionsDict;

        public QuestionFormVM(StudyUnitVM studyUnit)
            : base(studyUnit)
        {
            concepts = new ObservableCollection<ConceptVM>();
            questionsDict = new Dictionary<string, ObservableCollection<QuestionVM>>();
            foreach (Question questionModel in studyUnit.QuestionModels)
            {
                //QuestionVMの生成
                ObservableCollection<QuestionVM> questions = RelatedQuestions(questionModel.ConceptId);
                QuestionVM question = new QuestionVM(questionModel);
                InitQuestion(question);
                //ResponseVMの生成
                question.Response = CreateResponse(questionModel.Response);
                //配列に追加
                questions.Add(question);
            }            
        }

        private void UpdateConcepts()
        {
            concepts.Clear();
            concepts.AddRange(StudyUnit.AllConcepts);
        }

        public void code_EndEdit(CodeVM code)
        {
            StudyUnit.CompleteResponse(SelectedQuestion.Response);
        }

        protected override void Reload(VMState state)
        {
            UpdateConcepts();
            if (state != null)
            {
                SelectedConcept = EDOUtils.Find(Concepts, state.State1);
                if (SelectedConcept != null)
                {
                    SelectedQuestionItem = EDOUtils.Find(SelectedQuestions, state.State2);
                    if (selectedQuestionItem != null)
                    {
                        SelectedQuestion.Response.SelectedIndex = Convert.ToInt32(state.State3);
                    }
                }
            }
            if (SelectedConcept == null)
            {
                SelectedConcept = EDOUtils.GetFirst<ConceptVM>(Concepts);
            }
        }

        public override VMState SaveState()
        {
            if (SelectedConcept == null)
            {
                //ありえないかもしれないが一応チェックが必要?
                return null; 
            }
            VMState state = new VMState();
            state.State1 = SelectedConcept.Id;
            if (SelectedQuestion != null)
            {
                state.State2 = SelectedQuestion.Id;
                ResponseVM response = SelectedQuestion.Response;
                state.State3 = response.SelectedIndex;
            }
            return state;
        }

        public override void InitRow(object newItem)
        {
            if (newItem is QuestionVM)
            {
                QuestionVM question = (QuestionVM)newItem;
                InitQuestion(question);
                if (question.Question.ConceptId == null && SelectedConcept != null)
                {
                    //グリッドから追加した場合のConceptIdの設定
                    question.Question.ConceptId = SelectedConcept.Id;
                }
                UpdateModel(false);
            }
        }

        public void InitQuestion(QuestionVM question)
        {
            question.Parent = this;
            question.ResponseTypes = Options.ResponseTypes;
        }

        public ResponseVM CreateResponse(Response responseModel)
        {
            CodeSchemeVM codeScheme = null;
            if (responseModel.IsTypeChoices)
            {
                codeScheme = StudyUnit.FindCodeScheme(responseModel);
            }
            ResponseVM response = new ResponseVM(responseModel, codeScheme);
            return response;
        }

        private ObservableCollection<ConceptVM> concepts;
        public ObservableCollection<ConceptVM> Concepts { get { return concepts; } }

        private ObservableCollection<QuestionVM> RelatedQuestions(string conceptId)
        {
            if (questionsDict.ContainsKey(conceptId))
            {
                return questionsDict[conceptId];
            }
            var questions = new ObservableCollection<QuestionVM>();
            questionsDict[conceptId] = questions;
            return questions;
        }

        public ObservableCollection<QuestionVM> QuestionsFor(ConceptVM concept)
        {
            return RelatedQuestions(concept.Id);
        }

        private ConceptVM selectedConcept;
        public ConceptVM SelectedConcept
        {
            get
            {
                return selectedConcept;
            }
            set
            {
                if (selectedConcept != value)
                {
                    Window.FinalizeDataGrid();
                    selectedConcept = value;
                    NotifyPropertyChanged("SelectedConcept");
                    NotifyPropertyChanged("SelectedQuestions");
                }
            }
        }

        public ObservableCollection<QuestionVM> SelectedQuestions
        {
            get
            {
                if (selectedConcept == null)
                {
                    return null;
                }
                return RelatedQuestions(selectedConcept.Id);
            }
        }


        private QuestionFormView Window
        {
            get
            {
                return (QuestionFormView)View;
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
                    //選択された質問が変わるタイミングでコードスキームなどを完成させるために、CompleteResponseを呼び出す。
                    if (SelectedQuestion != null)
                    {
                        StudyUnit.CompleteResponse(SelectedQuestion.Response);
                    }
                    selectedQuestionItem = value;
                    Window.UpdateTemplate();
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

        private ICommand selectResponseCommand;
        public ICommand SelectResponseCommand
        {
            get
            {
                if (selectResponseCommand == null)
                {
                    selectResponseCommand = new RelayCommand(param => SelectResponse(), param => CanSelectResponse);
                }
                return selectResponseCommand;
            }
        }

        public bool CanSelectResponse
        {
            get
            {
                return SelectedQuestion != null;
            }
        }


        public ObservableCollection<ResponseVM> GetCandidateResponses()
        {
            //選択の対象となるレスポンスを返す
            ObservableCollection<ResponseVM> responses = new ObservableCollection<ResponseVM>();
            foreach (ResponseVM response in AllResponses)
            {
                responses.Add(response);
            }
            return responses;
        }


        public ResponseVM SelectAndCreateResponse(ResponseVM sourceResponse)
        {
            ResponseVM newResponse = null;
            if (sourceResponse.IsTypeChoices)
            {
                StudyUnit.CompleteResponse(sourceResponse);
                CodeSchemeVM sourceCodeScheme = sourceResponse.CodeScheme;

                ObservableCollection<CodeSchemeVM> codeSchemes = new ObservableCollection<CodeSchemeVM>(StudyUnit.CodeSchemes);

                SelectObjectWindowVM<CodeSchemeVM> vm = new SelectObjectWindowVM<CodeSchemeVM>(codeSchemes);
                CodeSchemeVM codeScheme = SelectObjectWindow.Select(Resources.SelectResponse, vm) as CodeSchemeVM; //回答の選択
                if (codeScheme != null && sourceCodeScheme != codeScheme)
                {
                    Response responseModel = new Response();
                    responseModel.TypeCode = Options.RESPONSE_TYPE_CHOICES_CODE;
                    responseModel.Title = codeScheme.Title;
                    newResponse = new ResponseVM(responseModel, codeScheme);
                }
            }
            else
            {
                ObservableCollection<ResponseVM> responses = GetCandidateResponses();
                SelectObjectWindowVM<ResponseVM> vm = new SelectObjectWindowVM<ResponseVM>(responses);
                ResponseVM selectedResponse = SelectObjectWindow.Select(Resources.SelectResponse, vm) as ResponseVM;
                if (selectedResponse != null && sourceResponse != selectedResponse)
                {
                    newResponse = selectedResponse.Dup();
                }
            }
            return newResponse;
        }


        public void SelectResponse()
        {
            ResponseVM newResponse = SelectAndCreateResponse(SelectedQuestion.Response);
            if (newResponse != null)
            {
                SelectedQuestion.Response = newResponse;
                //質問が選択されたとき、回答方法名を質問グリッドの回答方法名に反映するために以下の通知が必要
                SelectedQuestion.NotifyPropertyChanged("ResponseTitle");
                Window.UpdateTemplate();
            }
        }

        public ObservableCollection<QuestionVM> AllQuestions
        {
            get
            {
                var allQuestions = new ObservableCollection<QuestionVM>();
                allQuestions.Clear();
                foreach (KeyValuePair<string, ObservableCollection<QuestionVM>> pair in questionsDict)
                {
                    foreach (QuestionVM q in pair.Value)
                    {
                        allQuestions.Add(q);
                    }
                }
                return allQuestions;
            }
        }

        public ObservableCollection<QuestionVM> AllChoicesQuestions
        {
            get
            {
                ObservableCollection<QuestionVM> allQuestions = AllQuestions;
                ObservableCollection<QuestionVM> choicesQuestions = new ObservableCollection<QuestionVM>();
                foreach (QuestionVM question in allQuestions)
                {
                    if (question.Response.IsTypeChoices)
                    {
                        choicesQuestions.Add(question);
                    }
                }
                return choicesQuestions;
            }
        }


        public List<ResponseVM> AllResponses
        {
            get
            {
                List<ResponseVM> responses = new List<ResponseVM>();
                foreach (KeyValuePair<string, ObservableCollection<QuestionVM>> pair in questionsDict)
                {
                    foreach (QuestionVM q in pair.Value)
                    {
                        responses.Add(q.Response);
                    }
                }
                return responses;
            }
        }

        public List<ResponseVM> FindResponses(CodeSchemeVM codeScheme)
        {
            List<ResponseVM> responses = new List<ResponseVM>();
            List<ResponseVM> allResponses = AllResponses;
            foreach (ResponseVM response in allResponses)
            {
                if (response.CodeScheme == codeScheme)
                {
                    responses.Add(response);
                }
            }
            return responses;
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
                if (SelectedQuestion == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveQuestion()
        {
            using (UndoTransaction tx = new UndoTransaction(UndoManager))
            {
                StudyUnit.OnRemoveQuestion(SelectedQuestion);
                SelectedQuestions.Remove(SelectedQuestion);
                SelectedQuestionItem = null;
                ConceptVM concept = SelectedConcept;
                UpdateModel(false);
                SelectedConcept = concept;
                tx.Commit();
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
                if (SelectedQuestion == null)
                {
                    return false;
                }
                return SelectedQuestion.Response.CanRemoveMissingValue;
            }
        }

        public void RemoveMissingValue()
        {
            SelectedQuestion.Response.RemoveMissingValue();
        }

        public QuestionVM FindQuestion(string questionId)
        {
            ObservableCollection<QuestionVM> questions = AllQuestions;
            foreach (QuestionVM question in questions)
            {
                if (question.Id == questionId)
                {
                    return question;
                }
            }
            return null;
        }

        private ICommand changeImageCommand;
        public ICommand ChangeImageCommand
        {
            get
            {
                if (changeImageCommand == null)
                {
                    changeImageCommand = new RelayCommand(param => ChangeImage(), param => CanChangeImage);
                }
                return changeImageCommand;
            }
        }

        public bool CanChangeImage
        {
            get
            {
                if (SelectedConcept == null)
                {
                    //ありえないかもしれないが一応チェックが必要?
                    return false;
                }
                if (SelectedQuestion == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void ChangeImage()
        {
            Debug.WriteLine("Change Image");
            ConceptVM selectedConcept = SelectedConcept;
            QuestionVM selectedQuestion = SelectedQuestion;
            SelectObjectWindowVM<ConceptVM> vm = new SelectObjectWindowVM<ConceptVM>(Concepts);
            ConceptVM newConcept = SelectObjectWindow.Select(Resources.SelectImage, vm) as ConceptVM;
            if (newConcept != null && newConcept != selectedConcept)
            {
                SelectedQuestions.Remove(selectedQuestion);
                SelectedQuestionItem = null;

                ObservableCollection<QuestionVM> questions = RelatedQuestions(newConcept.Id);
                questions.Add(selectedQuestion);
                selectedQuestion.Question.ConceptId = newConcept.Id;

                UpdateModel(true);
            }
        }

        private void UpdateModel(bool memorize)
        {
            StudyUnit.QuestionModels.Clear();
            foreach (ConceptVM concept in Concepts)
            {
                ObservableCollection<QuestionVM> questions = RelatedQuestions(concept.Id);
                foreach (QuestionVM question in questions)
                {
                    StudyUnit.QuestionModels.Add(question.Question);
                }
            }
            if (memorize)
            {
                Memorize();
            }
        }

        protected override Action GetCompleteAction(VMState state)
        {
            return () => { StudyUnit.CompleteQuestions(); };
        }

        public void OnRemoveConcept(ConceptVM concept)
        {
            if (SelectedConcept == concept)
            {
                SelectedConcept = null;
            }
        }

        private ICommand removeConceptCommand;
        public ICommand RemoveConceptCommand
        {
            get
            {
                if (removeConceptCommand == null)
                {
                    removeConceptCommand = new RelayCommand(param => RemoveConcept(), param => CanRemoveConcept);
                }
                return removeConceptCommand;
            }
        }

        public bool CanRemoveConcept
        {
            get
            {
                if (SelectedConcept == null)
                {
                    return false;
                }
                return StudyUnit.CanRemoveConcept(SelectedConcept);
            }
        }

        public void RemoveConcept()
        {
            StudyUnit.RemoveConcept(SelectedConcept);
            UpdateConcepts();
        }
    }
}
