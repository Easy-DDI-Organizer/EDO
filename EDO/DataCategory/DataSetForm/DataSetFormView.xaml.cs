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
using EDO.Core.Model;
using EDO.Core.Util;
using EDO.Core.View;

namespace EDO.DataCategory.DataSetForm
{
    /// <summary>
    /// DataSetFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class DataSetFormView : FormView
    {
        public DataSetFormView()
        {
            InitializeComponent();
        }

        private DataSetFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<DataSetFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>() { dataSetDataGrid };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(
                    IB(VM.RemoveVariableCommand, EDOConstants.KEY_DELETE),
                    IB(VM.UpVariableCommand, EDOConstants.KEY_PAGE_UP),
                    IB(VM.DownVariableCommand, EDOConstants.KEY_PAGE_DOWN)
                    )
                };
            }
        }


        public void FocusCell()
        {
            DataGridHelper.FocusCell(dataSetDataGrid, dataSetDataGrid.SelectedIndex, 0);
        }

        public bool IsDataGridSorting
        {
            get
            {
                return dataSetDataGrid.Items.SortDescriptions.Count != 0;
            }
        }

        //public override bool Validate()
        //{
        //    if (!base.Validate())
        //    {
        //        return false;
        //    }
        //    VM.StudyUnit.CompleteDataSets();
        //    return true;
        //}

    }
}
