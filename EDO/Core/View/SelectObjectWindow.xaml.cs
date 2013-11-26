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
using EDO.Core.ViewModel;

namespace EDO.Core.View
{
    /// <summary>
    /// SelectObjectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectObjectWindow : Window
    {
        public static object Select(string title, ISelectObjectWindowVM vm)
        {
            SelectObjectWindow dlg = new SelectObjectWindow(vm);
            dlg.Title = title;
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                return dlg.SelectedObject;
            }
            return null;
        }

        private ISelectObjectWindowVM viewModel;
        public SelectObjectWindow(ISelectObjectWindowVM viewModel)
        {
            InitializeComponent();
            listBox.DisplayMemberPath = viewModel.DisplayMemberPath;
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedObject == null)
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

        public object SelectedObject { get {return viewModel.SelectedObject; } }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Filter(textBox.Text);
            //ここでセットしなおさないとリフレッシュされない
            listBox.ItemsSource = viewModel.Objects;
        }

    }
}
