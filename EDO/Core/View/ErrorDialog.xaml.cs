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
    /// ErrorDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ErrorDialog : Window
    {
        private static ErrorDialog instance = null;

        public static ErrorDialog Instance()
        {
            if (instance == null)
                instance = new ErrorDialog();
            return instance;
        }

        private ErrorDialogVM viewModel;

        public ErrorDialog()
        {
            InitializeComponent();
            viewModel = new ErrorDialogVM();
            DataContext = viewModel;
        }

        private void ok_Clicked(object sender, RoutedEventArgs e)
        {
            Close();
            instance = null;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            instance = null;
        }

        public void ShowError(MainWindowVM main, List<WriteError> errorInfos)
        {
            viewModel.SetErrorInfos(main, errorInfos);
            Owner = Application.Current.MainWindow;
            Show();
        }

    }
}
