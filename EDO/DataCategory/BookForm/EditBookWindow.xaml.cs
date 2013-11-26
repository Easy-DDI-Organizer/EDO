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
using System.Collections.ObjectModel;

namespace EDO.DataCategory.BookForm
{
    /// <summary>
    /// EditBookWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditBookWindow : Window
    {
        private EditBookWindowVM viewModel;

        public EditBookWindow(EditBookWindowVM viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
            viewModel.Window = this;
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.Validate();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            this.DialogResult = true;
        }

        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void relationDataGrid_MouseDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            viewModel.EditRelation();
        }
    }
}
