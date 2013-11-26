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
using System.ComponentModel;

namespace EDO.SamplingCategory.SamplingForm
{
    /// <summary>
    /// SamplingView.xaml の相互作用ロジック
    /// </summary>
    public partial class SamplingFormView : FormView
    {
        public SamplingFormView()
        {
            InitializeComponent();
            tabControl.Items.CurrentChanging += Items_CurrentChanging;
        }

        private SamplingFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<SamplingFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>() { 
                    DataGridHelper.FindDataGrid(this, "universeDataGrid"),
                };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(IB(VM.RemoveUniverseDelegateCommand, EDOConstants.KEY_DELETE))
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

    }
}
