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
using EDO.VariableCategory.VariableForm;
using EDO.Core.Model;
using EDO.Core.Util;
using EDO.Core.View;
using System.Diagnostics;

namespace EDO.QuestionCategory.QuestionForm
{
    /// <summary>
    /// QuestionFormWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class QuestionFormView : FormView
    {

        public QuestionFormView()
        {
            InitializeComponent();
        }

        private QuestionFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<QuestionFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>
                {
                    DataGridHelper.FindDataGrid(responsePane, "bookDataGrid"),
                    DataGridHelper.FindDataGrid(responsePane, "codeDataGrid"),
                    DataGridHelper.FindDataGrid(responsePane, "missingValueDataGrid"),
                    questionDataGrid
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
                    IBC(IB(VM.RemoveQuestionCommand, EDOConstants.KEY_DELETE))
                };
            }
        }

        private void combo_ResponseTypeChanged(Object sender, RoutedEventArgs e)
        {
            //DataGridのComboBoxではQuestionVMにバインドされたResponseTypeが追随しない。
            //このイベントハンドラで処理する。
            UpdateTemplate();
        }

        public void UpdateTemplate()
        {
            if (VM == null)
            {
                return;
            }
            responsePane.ContentTemplate = EDOUtils.SelectTemplate(VM.SelectedQuestion);
        }
    }
}
