using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Core.Model.Layout;

namespace EDO.QuestionCategory.QuestionForm
{
    public class FreeLayoutVM : ResponseLayoutVM
    {
        public FreeLayoutVM(FreeLayout freeLayout)
            : base(freeLayout)
        {
        }

        public FreeLayout FreeLayout { get { return (FreeLayout)Layout; } }

        public LayoutStyle Style
        {
            get
            {
                return FreeLayout.Style;
            }
            set
            {
                if (FreeLayout.Style != value)
                {
                    FreeLayout.Style = value;
                    NotifyPropertyChanged("Style");
                    Memorize();
                }
            }
        }

        public int? ColumnCount
        {
            get
            {
                return FreeLayout.ColumnCount;
            }
            set
            {
                if (FreeLayout.ColumnCount != value)
                {
                    FreeLayout.ColumnCount = value;
                    NotifyPropertyChanged("ColumnCount");
                    Memorize();
                }
            }
        }

        public int? RowCount
        {
            get
            {
                return FreeLayout.RowCount;                
            }
            set
            {
                if (FreeLayout.RowCount != value)
                {
                    FreeLayout.RowCount = value;
                    NotifyPropertyChanged("RowCount");
                    Memorize();
                }
            }
        }
    }
}
