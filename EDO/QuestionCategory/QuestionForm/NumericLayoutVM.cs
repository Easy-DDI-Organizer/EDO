using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model.Layout;
using EDO.Core.Model;

namespace EDO.QuestionCategory.QuestionForm
{
    public class NumericLayoutVM :ResponseLayoutVM
    {
        public NumericLayoutVM(NumericLayout numericLayout) :base(numericLayout)
        {
        }

        public NumericLayout NumericLayout { get { return (NumericLayout)Layout;  } }

        public NumericLayoutMeasurementMethod MeasurementMethod
        {
            get
            {
                return NumericLayout.MeasurementMethod;
            }
            set
            {
                if (NumericLayout.MeasurementMethod != value)
                {
                    NumericLayout.MeasurementMethod = value;
                    NotifyPropertyChanged("MeasurementMethod");
                    Memorize();
                }
            }
        }

        public string Unit
        {
            get
            {
                return NumericLayout.Unit;
            }
            set
            {
                if (NumericLayout.Unit != value)
                {
                    NumericLayout.Unit = value;
                    NotifyPropertyChanged("Unit");
                    Memorize();
                }
            }
        }

        public LayoutStyle Style
        {
            get
            {
                return NumericLayout.Style;
            }
            set
            {
                if (NumericLayout.Style != value)
                {
                    NumericLayout.Style = value;
                    NotifyPropertyChanged("Style");
                    Memorize();
                }
            }
        }

        public string LeftStatement
        {
            get
            {
                return NumericLayout.LeftStatement;
            }
            set
            {
                if (NumericLayout.LeftStatement != value)
                {
                    NumericLayout.LeftStatement = value;
                    NotifyPropertyChanged("LeftStatement");
                    Memorize();
                }
            }
        }

        public string RightStatement
        {
            get
            {
                return NumericLayout.RightStatement;
            }
            set
            {
                if (NumericLayout.RightStatement != value)
                {
                    NumericLayout.RightStatement = value;
                    NotifyPropertyChanged("RightStatement");
                    Memorize();
                }
            }
        }

        public int? Length
        {
            get
            {
                return NumericLayout.Length;
            }
            set
            {
                if (NumericLayout.Length != value)
                {
                    NumericLayout.Length = value;
                    NotifyPropertyChanged("Length");
                    Memorize();
                }
            }
        }
    }
}
