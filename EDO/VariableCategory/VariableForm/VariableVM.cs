using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EDO.QuestionCategory.ConceptForm;
using EDO.QuestionCategory.QuestionForm;
using System.Text.RegularExpressions;
using System.Diagnostics;
using EDO.Core.Util;

namespace EDO.VariableCategory.VariableForm
{
    public class VariableVM :BaseVM, IEditableObject, IStringIDProvider, ITitleProvider
    {
        public static VariableVM FindByQuestionId(ICollection<VariableVM> variables, string questionId)
        {
            foreach (VariableVM variable in variables)
            {
                if (variable.QuestionId == questionId)
                {
                    return variable;
                }
            }
            return null;
        }

        public static VariableVM Find(ICollection<VariableVM> variables, string variableId)
        {
            foreach (VariableVM variable in variables)
            {
                if (variable.Id == variableId)
                {
                    return variable;
                }
            }
            return null;
        }

        public static List<VariableVM> FindByUniverseId(ICollection<VariableVM> Variables, string universeId)
        {
            List<VariableVM> variables = new List<VariableVM>();
            foreach (VariableVM variable in variables)
            {
                if (variable.UniverseId == universeId)
                {
                    variables.Add(variable);
                }
            }
            return variables;
        }

        private Variable variable;
        private Variable bakVariable;
        private string bakResponseTypeCode;

        public VariableVM()
        {
            this.variable = new Variable();
            this.Response = new ResponseVM(variable.Response)
            {
                IsQuestionDesignMode = false
            };
        }

        public VariableVM(Variable variable)
        {
            this.variable = variable;
        }

        public Variable Variable { get { return variable; } }

        public override object Model { get { return variable; } }

        public string Id { get { return variable.Id; } }

        public bool IsCreatedDataSet
        {
            get
            {
                return variable.IsCreatedDataSet;
            }
            set
            {
                variable.IsCreatedDataSet = value;
            }
        }

        public ResponseVM response;
        public ResponseVM Response
        {
            get
            {
                return response;
            }
            set
            {
                if (response != value)
                {
                    response = value;
                    if (response != null)
                    {
                        response.Parent = this;
                        response.ParentId = Id;
                        if (variable.Response != response.Response)
                        {
                            variable.Response = response.Response;
                        }
                    }
                    NotifyPropertyChanged("Response");
                    Memorize();
                }
            }
        }

        public string ResponseTypeCode { get { return response.TypeCode; } }

        public bool IsResponseTypeUnknown { get { return response.IsTypeUnknown; } }
        public bool IsResponseTypeChoices { get { return response.IsTypeChoices; } }
        public bool IsResponseTypeNumber { get { return response.IsTypeNumber; } }
        public bool IsResponseTypeFree { get { return response.IsTypeFree; } }
        public bool IsResponseTypeDateTime { get { return response.IsTypeDateTime; } }

        public string Title
        {
            get
            {
                return variable.Title;
            }
            set
            {
                if (variable.Title != value)
                {
                    variable.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Label
        {
            get
            {
                return variable.Label;
            }
            set
            {
                if (variable.Label != value)
                {
                    variable.Label = value;
                    NotifyPropertyChanged("Label");
                }
            }
        }

        public string ConceptId 
        {
            get
            {
                return variable.ConceptId;
            }
            set
            {
                if (variable.ConceptId != value)
                {
                    variable.ConceptId = value;
                    NotifyPropertyChanged("ConceptId");
                }
            }
        }

        private ConceptVM concept;
        public ConceptVM Concept
        {
            get
            {
                return concept;
            }
            set
            {
                if (concept != value)
                {
                    concept = value;
                    NotifyPropertyChanged("Concept");
                }
            }
        }

        public string QuestionId 
        {
            get
            {
                return variable.QuestionId;
            }
            set
            {
                if (variable.QuestionId != value)
                {
                    variable.QuestionId = value;
                    NotifyPropertyChanged("QuestionId");
                }
            }
        }


        public string UniverseId 
        {
            get
            {
                return variable.UniverseId;                    
            }
            set
            {
                if (variable.UniverseId != value)
                {
                    variable.UniverseId = value;
                    NotifyPropertyChanged("UniverseId");
                }
            }
        }

        public string TitleNo
        {
            get
            {
                return EDOUtils.ToTitleNo(Title);
            }
        }

        public ObservableCollection<Option> ResponseTypes { get; set; }


        #region IEditableObject メンバー

        public bool InEdit { get { return inEdit; } }

        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit)
            {
                return;
            }
            inEdit = true;
            bakVariable = variable.Clone() as Variable;
            bakResponseTypeCode = response.TypeCode;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            this.Title = bakVariable.Title;
            this.Label = bakVariable.Label;
            this.ConceptId = bakVariable.ConceptId;
            this.QuestionId = bakVariable.QuestionId;
            this.UniverseId = bakVariable.UniverseId;
            response.TypeCode = bakResponseTypeCode;
        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;

            bakVariable = null;
            bakResponseTypeCode = null;
            Memorize();
        }

        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = EMPTY_VALUE;
            }
        }
        #endregion
    }
}
