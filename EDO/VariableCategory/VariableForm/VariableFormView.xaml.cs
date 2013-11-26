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
using System.Diagnostics;
using EDO.Core.View;
using EDO.Core.Util;
using EDO.Core.Model;
using EDO.DataCategory.DataSetForm;
using EDO.QuestionCategory.QuestionForm;

namespace EDO.VariableCategory.VariableForm
{
    /// <summary>
    /// VariableFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class VariableFormView : FormView
    {
        public VariableFormView()
        {
            InitializeComponent();
        }

        private VariableFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<VariableFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>()
                {
                    DataGridHelper.FindDataGrid(responsePane, "bookDataGrid"),
                    DataGridHelper.FindDataGrid(responsePane, "codeDataGrid"),
                    DataGridHelper.FindDataGrid(responsePane, "missingValueDataGrid"),
                    variableDataGrid
                };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    null,
                    null,
                    null,
                    IBC(IB(VM.RemoveVariableCommand, EDOConstants.KEY_DELETE)),
                };
            }
        }

        protected override void OnFormDataContextChanged()
        {
            UpdateTemplate();
        }

        private void combo_ResponseTypeChanged(Object sender, RoutedEventArgs e)
        {
            UpdateTemplate();
        }

        public void UpdateTemplate()
        {
            if (VM == null)
            {
                return;
            }
            responsePane.ContentTemplate = EDOUtils.SelectTemplate(VM.SelectedVariable);
        }
    }
}
