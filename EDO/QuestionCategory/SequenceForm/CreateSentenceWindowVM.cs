using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Util;
using System.Collections.ObjectModel;
using EDO.Core.Model;

namespace EDO.QuestionCategory.SequenceForm
{
    public class CreateSentenceWindowVM :BaseVM
    {
        public CreateSentenceWindowVM(ControlConstructSchemeVM controlConstructScheme, Statement statement)
        {
            this.controlConstructScheme = controlConstructScheme;
            this.statement = new Statement()
            {
                No = statement.No,
                Text = statement.Text
            };
        }

        private ControlConstructSchemeVM controlConstructScheme;
        private Statement statement;

        public string Text
        {
            get
            {
                return statement.Text;
            }
            set
            {
                if (statement.Text != value)
                {
                    statement.Text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }

        public Statement Statement 
        {
            get
            {
                return statement;
            }
        }

        public void Save()
        {

        }


    }
}
