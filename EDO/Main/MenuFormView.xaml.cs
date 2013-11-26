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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Diagnostics;
using EDO.Core.ViewModel;
using EDO.Core.Model;

namespace EDO.Main
{
    /// <summary>
    /// TreeMenu.xaml の相互作用ロジック
    /// </summary>
    public partial class MenuFormView : UserControl
    {
        public MenuFormView()
        {
            InitializeComponent();
        }

        private bool ProcessChanged(EDOUnitVM edoUnit, object newValue, object oldValue)
        {
            MenuItemVM newMenuItem = newValue as MenuItemVM;
            MenuItemVM oldMenuItem = oldValue as MenuItemVM;
            if (newMenuItem == null)
            {
                return false;
            }
            //同じアイテムが選択された場合は以降の処理は行わない(エラーが発生して戻す場合に無限ループになるのを防ぐ)
            if (newMenuItem == edoUnit.SelectedMenuItem)
            {
                return false;
            }

            if (oldMenuItem != null)
            {
                if (!oldMenuItem.validate())
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        oldMenuItem.IsSelected = true;
                    }));
                    return false;
                }
            }

            edoUnit.SelectedMenuItem = newMenuItem;
            edoUnit.SetStatusMessage(newMenuItem.Title, false);
            return true;
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            EDOUnitVM edoUnit = DataContext as EDOUnitVM;
            if (!ProcessChanged(edoUnit, e.NewValue, e.OldValue)) 
            {
                return;
            }
            if (edoUnit is StudyUnitVM)
            {
                StudyUnitVM studyUnitVM = (StudyUnitVM)edoUnit;
                MenuItemVM menuItem = studyUnitVM.SelectedMenuItem;
                studyUnitVM.Main.IsDataSet = menuItem.IsDataSet;
                if (menuItem.IsDataSet)
                {
                    studyUnitVM.Main.IsDataSetSelected = true;
                }
                studyUnitVM.Main.IsCategory = menuItem.IsCategory;
                if (menuItem.IsCategory)
                {
                    studyUnitVM.Main.IsCategorySelected = true;
                }
                studyUnitVM.Main.IsCode = menuItem.IsCode;
                if (menuItem.IsCode)
                {
                    studyUnitVM.Main.IsCodeSelected = true;
                }
                studyUnitVM.Main.IsQuestionGroup = menuItem.IsQuestionGroup;
                if (menuItem.IsQuestionGroup)
                {
                    studyUnitVM.Main.IsQuestionGroupSelected = true;
                }
            } else if (edoUnit is GroupVM) {
                
            }

        }
    }
}
