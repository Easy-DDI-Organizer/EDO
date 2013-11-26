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
using System.ComponentModel;
using EDO.Core.View;
using EDO.Core.Util;

namespace EDO.QuestionCategory.ConceptForm
{
    /// <summary>
    /// ConceptView.xaml の相互作用ロジック
    /// </summary>
    public partial class ConceptFormView : FormView
    {
        public ConceptFormView()
        {
            InitializeComponent();
            tabControl.Items.CurrentChanging += Items_CurrentChanging;
        }

        private ConceptFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<ConceptFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>() { DataGridHelper.FindDataGrid(this, "conceptDataGrid") };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(IB(VM.RemoveConceptDelegateCommand, EDOConstants.KEY_DELETE))
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
