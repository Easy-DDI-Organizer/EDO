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

namespace EDO.Core.View
{
    /// <summary>
    /// InputDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class InputDialog : Window
    {
        public InputDialog()
        {
            InitializeComponent();
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (textBox.Text.Length == 0)
            {
                MessageBox.Show(Properties.Resources.StringIsNotSpecified);
                return;
            }
            this.DialogResult = true;
        }
        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string Info { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Visibility v = Visibility.Visible;
            if (string.IsNullOrEmpty(Info))
            {
                v = Visibility.Collapsed;
            }
            info.Visibility = v;
            info.Text = Info;
        }


    }
}
