using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using System.ComponentModel;
using EDO.Main;
using EDO.Core.Model;
using EDO.QuestionCategory.ConceptForm;
using EDO.SamplingCategory.SamplingForm;
using System.Diagnostics;
using System.Collections.Specialized;
using EDO.DataCategory.DataSetForm;
using System.Windows.Input;
using EDO.Core.Util;
using System.Windows;
using EDO.QuestionCategory.QuestionForm;
using EDO.QuestionCategory.CodeForm;
using EDO.Core.View;

namespace EDO.VariableCategory.VariableForm
{
    public class VariableFormVM :FormVM
    {
        #region 初期化
        public VariableFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            variables = new ObservableCollection<VariableVM>();
            foreach (Variable variableModel in studyUnit.VariableModels)
            {
                //VariableVMの生成
                VariableVM variable = new VariableVM(variableModel);
                InitVariable(variable);
                //ResponseVMの生成
                variable.Response = CreateResponse(variableModel.Response);
                //配列に追加
                variables.Add(variable);
            }
            modelSyncher = new ModelSyncher<VariableVM, Variable>(this, variables, studyUnit.VariableModels);
        }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedVariableItem = EDOUtils.Find(variables, state.State1);
            }
            if (SelectedVariableItem == null)
            {
                SelectedVariableItem = EDOUtils.GetFirst<VariableVM>(variables);
            }
        }

        public override VMState SaveState()
        {
            if (SelectedVariable == null)
            {
                //ありえないかもしれないが一応チェックが必要?
                return null;
            }
            return new VMState(SelectedVariable.Id);
        }

        private ModelSyncher<VariableVM, Variable> modelSyncher;

        private ResponseVM CreateResponse(Response responseModel)
        {
            ResponseVM response = StudyUnit.CreateResponse(responseModel);
            response.IsQuestionDesignMode = false;
            return response;
        }

        public override void InitRow(object newItem)
        {
            if (newItem is VariableVM)
            {
                InitVariable((VariableVM)newItem);
            }
        }


        public void InitVariable(VariableVM variable)
        {
            variable.Parent = this;
            variable.ResponseTypes = Options.ResponseTypes;
        }

        #endregion

        #region プロパティ

        private DataSetFormVM DataSetForm { get { return this.StudyUnit.DataSetForm; } }

        public ObservableCollection<VariableVM> variables;
        public ObservableCollection<VariableVM> Variables { get { return variables; } }

        public ObservableCollection<QuestionVM> Questions
        {
            get
            {
                return StudyUnit.AllQuestions;
            }
        }

        public ObservableCollection<ConceptVM> Concepts 
        {
            get
            {
                ObservableCollection<ConceptVM> concepts = new ObservableCollection<ConceptVM>(StudyUnit.AllConcepts);
                return concepts;
//                return StudyUnit.AllConcepts;
            }
        }

        public ObservableCollection<UniverseVM> Universes
        {
            get
            {
                ObservableCollection<UniverseVM> universes = new ObservableCollection<UniverseVM>(StudyUnit.Universes);
                return universes;
            }
        }

        private VariableFormView Window
        {
            get
            {
                return (VariableFormView)View;
            }
        }

        private Object selectedVariableItem;
        public Object SelectedVariableItem
        {
            get
            {
                return selectedVariableItem;
            }
            set
            {
                if (selectedVariableItem != value)
                {
                    selectedVariableItem = value;
                    Window.UpdateTemplate();
                    NotifyPropertyChanged("SelectedVariableItem");
                }
            }
        }

        public VariableVM SelectedVariable
        {
            get
            {
                return selectedVariableItem as VariableVM;
            }
        }


        #endregion

        #region メソッド

        private void CreateVariableFor(QuestionVM question)
        {
            if (question.IsCreatedVariable)
            {
                //既に生成済みの場合作らない
                return;
            }
            VariableVM variable = VariableVM.FindByQuestionId(Variables, question.Id);
            if (variable != null)
            { 
                //既に存在している場合は作らない(フラグで処理しているので念のため?)
                return;
            }
            question.IsCreatedVariable = true;

            Variable variableModel = new Variable();
            variableModel.Title = "V" + (Variables.Count + 1);
            variableModel.Label = question.Title;
            variableModel.ConceptId = question.Question.ConceptId;
            variableModel.QuestionId = question.Id;
            variableModel.UniverseId = StudyUnit.DefaultUniverseGuid;
            variableModel.Response = question.DupResponseModel();
            variableModel.Response.Title = null; //変数の回答法にはタイトル設定はできないのでnullにしておく。こうしておくことで変数設計画面で1からコードをいれたときのコード群名の生成が正しくなる。

            VariableVM newVariable = new VariableVM(variableModel);
            InitVariable(newVariable);
            newVariable.Response = CreateResponse(variableModel.Response);
            variables.Add(newVariable);
        }

        public void CreateVariables(ICollection<QuestionVM> questions)
        {
            //質問設計画面の最後で呼び出される。現在の質問に対する変数を自動で生成する
            //(削除された変数とか再生成してよいのか?)
            foreach (QuestionVM question in questions)
            {
                CreateVariableFor(question);
            }
        }

        public VariableVM FindVariable(string variableId)
        {
            return VariableVM.Find(Variables, variableId);
        }

        public List<VariableVM> FindVariablesByUniverseId(string universeId)
        {
            return VariableVM.FindByUniverseId(Variables, universeId);
        }

        #endregion


        #region コマンド

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
                return SelectedVariable != null;
            }
        }

        public void SelectResponse()
        {
            ResponseVM newResponse = StudyUnit.SelectAndCreateResponse(SelectedVariable.Response);
            if (newResponse != null)
            {
                newResponse.IsQuestionDesignMode = false;
                SelectedVariable.Response = newResponse;
                Window.UpdateTemplate();
            }
        }

        #endregion

        public List<ResponseVM> FindResponses(CodeSchemeVM codeScheme)
        {
            List<ResponseVM> responses = new List<ResponseVM>();
            foreach (VariableVM variable in Variables)
            {
                ResponseVM response = variable.Response;
                if (response.IsTypeChoices && response.CodeScheme == codeScheme)
                {
                    responses.Add(response);
                }
            }
            return responses;
        }

        private ICommand removeVariableCommand;
        public ICommand RemoveVariableCommand
        {
            get
            {
                if (removeVariableCommand == null)
                {
                    removeVariableCommand = new RelayCommand(param => RemoveVariable(), param => CanRemoveVariable);
                }
                return removeVariableCommand;
            }
        }

        public bool CanRemoveVariable
        {
            get
            {
                if (SelectedVariable == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveVariable()
        {
            StudyUnit.OnRemoveVariable(SelectedVariable);
            Variables.Remove(SelectedVariable);
            SelectedVariableItem = null;
        }

        public void RemoveUniverse(UniverseVM universe)
        {
            foreach (VariableVM variable in variables)
            {
                if (variable.UniverseId == universe.Id)
                {
                    variable.UniverseId = null;
                }
            }
        }

        public void OnRemoveConcepts(List<ConceptVM> concepts)
        {
            foreach (ConceptVM concept in concepts)
            {
                OnRemoveConcept(concept);
            }
        }

        public void OnRemoveConcept(ConceptVM concept)
        {
            foreach (VariableVM variable in variables)
            {
                if (variable.ConceptId == concept.Id)
                {
                    variable.ConceptId = null;
                    variable.Concept = null;
                }
            }
        }

        public void OnRemoveQuestion(QuestionVM question)
        {
            foreach (VariableVM variable in variables)
            {
                if (variable.QuestionId == question.Id)
                {
                    variable.QuestionId = null;
                }
            }
        }

        protected override Action GetCompleteAction(VMState state)
        {
            return () => { StudyUnit.CompleteVariables(); };
        }
    }
}
