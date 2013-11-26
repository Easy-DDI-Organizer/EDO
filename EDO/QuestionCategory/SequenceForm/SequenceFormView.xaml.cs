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
using System.ComponentModel;
using System.Diagnostics;
using EDO.Core.Model;

namespace EDO.QuestionCategory.SequenceForm
{
    /// <summary>
    /// SequenceFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class SequenceFormView : FormView
    {
        public SequenceFormView()
        {
            InitializeComponent();
            tabControl.Items.CurrentChanging += Items_CurrentChanging;
        }

        private SequenceFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<SequenceFormVM>(this);
            }
        }


        protected override void OnFormDataContextChanged()
        {
            Debug.WriteLine("DataContextChanged");
        }
        private DataGrid ConstructDataGrid
        {
            get
            {
                return DataGridHelper.FindDataGrid(this, "constructDataGrid");
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>
                {
                   ConstructDataGrid
                };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(
                    IB(VM.RemoveConstructCommand, EDOConstants.KEY_DELETE),
                    IB(VM.UpConstructCommand,  EDOConstants.KEY_PAGE_UP),
                    IB(VM.DownConstructCommand,  EDOConstants.KEY_PAGE_DOWN)
                    )
                };
            }
        }

        void Items_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (!e.IsCancelable)
            {
                return;
            }
            if (Validate())
            {
                return;
            }
            e.Cancel = true;
            tabControl.SelectedItem = ((ICollectionView)sender).CurrentItem;
        }

        public bool IsDataGridSorting
        {
            get
            {
                DataGrid dataGrid = ConstructDataGrid;
                if (dataGrid == null)
                {
                    return false;
                }
                return dataGrid.Items.SortDescriptions.Count != 0;
            }
        }

        public void FocusCell()
        {
            DataGrid dataGrid = ConstructDataGrid;
            if (dataGrid == null)
            {
                return;
            }
            DataGridHelper.FocusCell(dataGrid, dataGrid.SelectedIndex, 0);
        }

        private void constructDataGrid_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (VM == null)
            {
                return;
            }
            VM.EditCurrentRow();
        }
    }
}
