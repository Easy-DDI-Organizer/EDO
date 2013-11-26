using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;
using EDO.Core.ViewModel;
using EDO.VariableCategory.VariableForm;
using System.Collections.ObjectModel;

namespace EDO.DataCategory.DataSetForm
{
    public class SelectVariableWindowVM : FormVM
    {
        public SelectVariableWindowVM(StudyUnitVM studyUnit)
            : base(studyUnit)
        {
            variables = new ObservableCollection<VariableVM>();
            selectedVariables = new ObservableCollection<VariableVM>();
            Filter("");
        }

        private ObservableCollection<VariableVM> variables;
        public ObservableCollection<VariableVM> Variables { get { return variables; } }

        private ObservableCollection<VariableVM> selectedVariables;
        public ObservableCollection<VariableVM> SelectedVariables { get { return selectedVariables; } }

        public void Filter(string text)
        {
            StudyUnitVM studyUnit = (StudyUnitVM)this.Parent;

            string lowerText = text.ToLower();
            variables.Clear();
            foreach (VariableVM variable in studyUnit.VariableForm.Variables)
            {
                if (string.IsNullOrEmpty(lowerText) || variable.Title.ToLower().Contains(lowerText))
                {
                    variables.Add(variable);
                }
            }
        }
    }


}
