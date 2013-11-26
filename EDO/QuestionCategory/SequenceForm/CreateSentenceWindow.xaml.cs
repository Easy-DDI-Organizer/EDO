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

namespace EDO.QuestionCategory.SequenceForm
{
    /// <summary>
    /// CreateSentenceWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CreateSentenceWindow : Window
    {
        private CreateSentenceWindowVM viewModel;
        public CreateSentenceWindow(CreateSentenceWindowVM viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            viewModel.Save();
            this.DialogResult = true;
        }

        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
