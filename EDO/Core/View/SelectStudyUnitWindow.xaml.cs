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
using EDO.Core.IO;

namespace EDO.Core.View
{
    /// <summary>
    /// SelectStudyUnitWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectStudyUnitWindow : Window
    {
        private SelectStudyUnitWindowVM viewModel;

        public SelectStudyUnitWindow(SelectStudyUnitWindowVM viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            viewModel.Window = this;
            DataContext = viewModel;

            DDIImportOption importOption = viewModel.ImportOption;
            if (!importOption.CanSelectFromStudyUnit)
            {
                grid.RowDefinitions[0].Height = new GridLength(0);
            }
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
