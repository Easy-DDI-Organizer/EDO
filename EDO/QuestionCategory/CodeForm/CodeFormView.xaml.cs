using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EDO.Core.ViewModel;
using EDO.Core.View;
using EDO.Core.Util;
using EDO.Core.Model;

namespace EDO.QuestionCategory.CodeForm
{
    /// <summary>
    /// CodeFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class CodeFormView : FormView
    {
        public CodeFormView()
        {
            InitializeComponent();
        }

        private CodeFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<CodeFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid> { codeDataGrid };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(IB(VM.RemoveCodeCommand, EDOConstants.KEY_DELETE))
                };
            }
        }
    }
}

