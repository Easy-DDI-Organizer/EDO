using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls.Ribbon;
using System.Collections;
using EDO.Main;
using EDO.EventCategory.EventForm;
using System.ComponentModel;
using EDO.Core.View;
using EDO.Core.Model;
using System.Windows.Controls;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Globalization;
using WPFLocalization;
using System.Configuration;

namespace EDO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private EDOConfig config;
        private MainWindowVM viewModel;
        public MainWindow()
        {
//            MessageBox.Show(System.Environment.CurrentDirectory);
//            System.Configuration.Configuration  configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
//            MessageBox.Show("Local user config path: " + configuration.FilePath); 

            config = new EDOConfig();
            ChangeLanguage();
            InitializeComponent();

            viewModel = new MainWindowVM(config);
            viewModel.Window = this;
            this.DataContext = viewModel;
        }

        private void Ribbon_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void MergeResource(string resourceName)
        {
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri(resourceName, UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public void ChangeLanguage()
        {
//            Debug.WriteLine(LocalizationManager.UICulture.Name);
//            MessageBox.Show("config.Language=" + config.Language + " LocalizationManager.UICulture.Name=" + LocalizationManager.UICulture.Name);
            if (string.IsNullOrEmpty(config.Language))
            {
                config.InitLanguage(LocalizationManager.UICulture.Name);
            }
            LocalizationManager.UICulture = new CultureInfo(config.Language);

            if (config.IsLanguageEn)
            {
                MergeResource("Resources_en.xaml");
            }
            else
            {
                MergeResource("Resources_ja.xaml");
            }
            MergeResource("Resources.xaml");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tabControl.Items.CurrentChanging += this.Items_CurrentChanging;
        }

        void Items_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (!e.IsCancelable)
            {
                return;
            }
            var item = ((ICollectionView)sender).CurrentItem;
            if (!(item is IValidatableCollection))
            {
                return;
            }
            IValidatableCollection validatable = (IValidatableCollection)item;
            if (validatable.ValidateCurrentItem())
            {
                return;
            }

            e.Cancel = true;
            tabControl.SelectedItem = validatable;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !viewModel.ConfirmModified();
            viewModel.SaveConfig();
        }

    }
}
