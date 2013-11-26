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
using System.Windows.Shapes;
using EDO.Core.View;
using EDO.Core.Util;
using EDO.Core.Model;

namespace EDO.DataCategory.BookForm
{
    /// <summary>
    /// BookFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class BookFormView : FormView
    {
        public BookFormView()
        {
            InitializeComponent();
        }

        private BookFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<BookFormVM>(this);
            }
        }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>
                {
                    bookDataGrid
                };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>()
                {
                    IBC(
                    IB(VM.RemoveBookCommand, EDOConstants.KEY_DELETE)
                    )
                };
            }
        }
        private void bookDataGrid_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (VM == null)
            {
                return;
            }
            VM.EditBook();
        }

    }
}
