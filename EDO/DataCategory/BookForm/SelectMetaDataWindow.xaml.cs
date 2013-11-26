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

namespace EDO.DataCategory.BookForm
{
    /// <summary>
    /// SelectMetaDataWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectMetaDataWindow : Window
    {
        private SelectMetaDataWindowVM viewModel;
        public SelectMetaDataWindow(SelectMetaDataWindowVM viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (viewModel.IsSelectable && viewModel.SelectedObject == null)
            {
                MessageBox.Show(Properties.Resources.IsNotSelected);
                return;
            }
            this.DialogResult = true;
        }

        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Filter(textBox.Text);
            //ここでセットしなおさないとリフレッシュされない
            listBox.ItemsSource = viewModel.Objects;
        }
    }
}
