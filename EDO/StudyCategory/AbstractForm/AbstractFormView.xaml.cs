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
using EDO.Core.Util;
using EDO.Core.Model;
using EDO.Core.View;

namespace EDO.StudyCategory.AbstractForm
{
    /// <summary>
    /// AbstractView.xaml の相互作用ロジック
    /// </summary>
    public partial class AbstractFormView : FormView
    {
        public AbstractFormView()
        {
            InitializeComponent();
        }

        private AbstractFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<AbstractFormVM>(this);
            }
        }


        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>
                {
                    DataGridHelper.FindDataGrid(this, "bookDataGrid")
                };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>()
                {
                    null
                };
            }
        }
        //protected override void OnFormLoaded()
        //{
        //    FormVM.IsIgnoreValidation = false;
        //}
    }
}
