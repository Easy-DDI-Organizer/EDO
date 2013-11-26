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
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace EDO.Main
{
    /// <summary>
    /// AboutWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            string path = Environment.GetCommandLineArgs()[0];

            // メモ帳のICON情報の取得。
            System.Drawing.Icon iconObject = System.Drawing.Icon.ExtractAssociatedIcon(
                                         path);

            //// WPFアプリケーション用にデータを変換してImageコントロールに登録
            //icon.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
            //               iconObject.Handle, Int32Rect.Empty,
            //               BitmapSizeOptions.FromEmptyOptions());

            this.DataContext = new AboutWindowVM();
        }

        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
