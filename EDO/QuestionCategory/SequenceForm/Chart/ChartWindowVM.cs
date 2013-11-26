using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.QuestionForm;
using System.Windows;
using EDO.Core.ViewModel;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    public class ChartWindowVM :BaseVM
    {
        public ChartWindowVM(ControlConstructSchemeVM controlConstructScheme)
        {
            this.controlConstructScheme = controlConstructScheme;
        }

        private ControlConstructSchemeVM controlConstructScheme;

        public ObservableCollection<ConstructVM> Constructs
        {
            get
            {
                return controlConstructScheme.Constructs;
            }
        }

        public bool EditBranch(IfThenElseVM ifThenElse, Window ownerWindow)
        {
            return controlConstructScheme.EditBranchExternal(ifThenElse, ownerWindow);
        }

        public string Title
        {
            get
            {
                return controlConstructScheme.Title;
            }
        }
    }
}
