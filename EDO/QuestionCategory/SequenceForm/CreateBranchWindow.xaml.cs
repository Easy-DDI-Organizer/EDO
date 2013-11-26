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
using System.Collections.ObjectModel;

namespace EDO.QuestionCategory.SequenceForm
{
    /// <summary>
    /// CreateBranchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CreateBranchWindow : Window, IBranchEditor
    {
        private CreateBranchWindowVM viewModel;

        public CreateBranchWindow(CreateBranchWindowVM viewModel)
        {
            InitializeComponent();
            viewModel.Editor = this;
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (!ValidateAll())
            {
                return;
            }
            viewModel.Save();
            this.DialogResult = true;
        }

        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        public bool ValidateEditingBranch()
        {
            ItemsControl condGroupControl = VisualTreeFinder.FindChild<ItemsControl>(this, "condGroupControl");
            return Validator.Validate(condGroupControl);
        }

        public bool ValidateAll()
        {
            ObservableCollection<BranchVM> branches = viewModel.Branches;
            foreach (BranchVM branch in branches)
            {
                if (!branch.IsValid)
                {
                    MessageBox.Show(string.Format(Properties.Resources.InvalidItem, branch.TypeName) + ": " + branch.Expression);
                    return false;
                }
            }
            return true;
        }
    }
}
