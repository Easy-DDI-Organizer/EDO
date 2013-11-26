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
using EDO.Core.View;
using EDO.Core.Util;
using EDO.Core.Model;

namespace EDO.QuestionCategory.QuestionGroupForm
{
    /// <summary>
    /// QuestionGroupFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class QuestionGroupFormView : FormView
    {
        public QuestionGroupFormView()
        {
            InitializeComponent();
        }

        private QuestionGroupFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<QuestionGroupFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>() { questionDataGrid };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(
                    IB(VM.RemoveQuestionCommand, EDOConstants.KEY_DELETE),
                    IB(VM.UpQuestionCommand, EDOConstants.KEY_PAGE_UP),
                    IB(VM.DownQuestionCommand, EDOConstants.KEY_PAGE_DOWN)
                    )
                };
            }
        }

        private DataGrid QuestionDataGrid
        {
            get
            {
                return questionDataGrid;
            }
        }

        public void FocusCell()
        {
            DataGrid dataGrid = QuestionDataGrid;
            if (dataGrid == null)
            {
                return;
            }
            DataGridHelper.FocusCell(dataGrid, dataGrid.SelectedIndex, 0);
        }
    }
}
