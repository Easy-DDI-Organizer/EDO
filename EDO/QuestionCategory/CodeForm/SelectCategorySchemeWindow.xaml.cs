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
using EDO.Main;
using EDO.QuestionCategory.CategoryForm;
using System.Collections.ObjectModel;

namespace EDO.QuestionCategory.CodeForm
{
    /// <summary>
    /// SelectCategorySchemeWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectCategorySchemeWindow : Window
    {
        private SelectCategorySchemeWindowVM viewModel;
        public SelectCategorySchemeWindow(StudyUnitVM studyUnit)
        {
            InitializeComponent();
            this.viewModel = new SelectCategorySchemeWindowVM(studyUnit);
            this.DataContext = this.viewModel;
        }

        private void updateSelectedCategorySchemes()
        {
            viewModel.SelectedCategorySchemes.Clear();
            foreach (var item in listBox.SelectedItems)
            {
                viewModel.SelectedCategorySchemes.Add((CategorySchemeVM)item);
            }
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            updateSelectedCategorySchemes();
            if (viewModel.SelectedCategorySchemes.Count == 0)
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
        
        public ObservableCollection<CategorySchemeVM> SelectedCategorySchemes {
            get
            {
                return viewModel.SelectedCategorySchemes;
            }
        }

        public ObservableCollection<CategoryVM> SelectedCategories
        {
            get
            {
                return CategorySchemeVM.GetAllCategories(SelectedCategorySchemes);
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Filter(textBox.Text);
            //ここでセットしなおさないとリフレッシュされない
            listBox.ItemsSource = viewModel.CategorySchemes;
        }
    }
}
