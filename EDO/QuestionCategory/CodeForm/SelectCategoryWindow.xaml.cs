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
using EDO.QuestionCategory.CategoryForm;
using EDO.Main;
using System.Collections.ObjectModel;

namespace EDO.QuestionCategory.CodeForm
{
    /// <summary>
    /// SelectCategoryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectCategoryWindow : Window
    {
        private SelectCategoryWindowVM viewModel;
        public SelectCategoryWindow(StudyUnitVM studyUnit)
        {
            InitializeComponent();
            this.viewModel = new SelectCategoryWindowVM(studyUnit);
            this.DataContext = this.viewModel;
        }

        private void UpdateSelectedItems()
        {
            viewModel.SelectedCategories.Clear();
            foreach (var item in listBox.SelectedItems)
            {
                viewModel.SelectedCategories.Add((CategoryVM)item);
            }
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            UpdateSelectedItems();
            if (viewModel.SelectedCategories.Count == 0)
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

        public ObservableCollection<CategoryVM> SelectedCategories
        {
            get
            {
                return viewModel.SelectedCategories;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Filter(textBox.Text);
            //ここでセットしなおさないとリフレッシュされない
            listBox.ItemsSource = viewModel.Categories;
        }
    }
}
