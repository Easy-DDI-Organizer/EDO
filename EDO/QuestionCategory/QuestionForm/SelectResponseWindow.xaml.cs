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
using EDO.QuestionCategory.QuestionForm;

namespace EDO.QuestionCategory.QuestionForm
{
    /// <summary>
    /// SelectResponseWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectResponseWindow : Window
    {
        private SelectResponseWindowVM viewModel;
        public SelectResponseWindow(StudyUnitVM studyUnit, ResponseVM excludeResponse)
        {
            InitializeComponent();
            this.viewModel = new SelectResponseWindowVM(studyUnit, excludeResponse);
            this.DataContext = this.viewModel;
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedResponse == null)
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

        public ResponseVM SelectedResponse
        {
            get
            {
                return viewModel.SelectedResponse;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Filter(textBox.Text);
            //ここでセットしなおさないとリフレッシュされない
            listBox.ItemsSource = viewModel.Responses;
        }

    }
}
