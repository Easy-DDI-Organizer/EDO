﻿using System;
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
using EDO.Core.Model;

namespace EDO.Main
{
    /// <summary>
    /// ConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private ConfigWindowVM viewModel;

        public ConfigWindow(EDOConfig config)
        {
            InitializeComponent();
            viewModel = new ConfigWindowVM(config);
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

        public bool ShouldRestart {get {return viewModel.ShouldRestart; }}
    }
}