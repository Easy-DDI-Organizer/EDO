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
using EDO.StudyCategory.MemberForm;
using EDO.Main;

namespace EDO.SamplingCategory.SamplingForm
{
    /// <summary>
    /// SelectMemberWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectMemberWindow : Window
    {
        private SelectMemberWindowVM viewModel;
        public SelectMemberWindow(StudyUnitVM studyUnitVM)
        {
            InitializeComponent();
            this.viewModel = new SelectMemberWindowVM(studyUnitVM);
            this.DataContext = this.viewModel;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            viewModel.Filter(textBox.Text);
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedMember == null)
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


        public MemberVM SelectedMember
        {
            get
            {
                return viewModel.SelectedMember;
            }
        }

    }
}
