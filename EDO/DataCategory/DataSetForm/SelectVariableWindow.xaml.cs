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
using EDO.Main;
using EDO.VariableCategory.VariableForm;
using System.Collections.ObjectModel;

namespace EDO.DataCategory.DataSetForm
{
    /// <summary>
    /// SelectVariableWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectVariableWindow : Window
    {
        private SelectVariableWindowVM viewModel;
        public SelectVariableWindow(StudyUnitVM studyUnit)
        {
            InitializeComponent();
            this.viewModel = new SelectVariableWindowVM(studyUnit);
            this.DataContext = this.viewModel;
        }


        private void updateSelectedItems()
        {
            viewModel.SelectedVariables.Clear();
            foreach (var item in listBox.SelectedItems)
            {
                viewModel.SelectedVariables.Add((VariableVM)item);
            }
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            updateSelectedItems();
            if (viewModel.SelectedVariables.Count == 0)
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

        public ObservableCollection<VariableVM> SelectedVariables
        {
            get
            {
                return viewModel.SelectedVariables;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Filter(textBox.Text);
            //ここでセットしなおさないとリフレッシュされない
            listBox.ItemsSource = viewModel.Variables;
        }
    }
}
