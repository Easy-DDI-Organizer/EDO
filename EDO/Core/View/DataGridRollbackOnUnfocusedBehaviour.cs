using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace EDO.Core.View
{
    public class DataGridRollbackOnUnfocusedBehaviour
    {
        #region DataGridRollbackOnUnfocusedBehaviour

        public static bool GetDataGridRollbackOnUnfocused(DataGrid datagrid)
        {
            return (bool)datagrid.GetValue(DataGridRollbackOnUnfocusedProperty);
        }

        public static void SetDataGridRollbackOnUnfocused(
         DataGrid datagrid, bool value)
        {
            datagrid.SetValue(DataGridRollbackOnUnfocusedProperty, value);
        }

        public static readonly DependencyProperty DataGridRollbackOnUnfocusedProperty =
            DependencyProperty.RegisterAttached(
            "DataGridRollbackOnUnfocused",
            typeof(bool),
            typeof(DataGridRollbackOnUnfocusedBehaviour),
            new UIPropertyMetadata(false, OnDataGridRollbackOnUnfocusedChanged));

        static void OnDataGridRollbackOnUnfocusedChanged(
         DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid datagrid = depObj as DataGrid;
            if (datagrid == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                //※このイベントハンドラ必要かどうか確認する。有効なままだとタブコントロールの上にDataGridがあるとき中身がきえる
                //これがなくても外部にキーフォーカスを移しても問題はなさそうだけど。
                datagrid.LostKeyboardFocus += RollbackDataGridOnLostFocus; //やっぱりだめ。これがないと質問設計がめんでこける。
                datagrid.DataContextChanged += RollbackDataGridOnDataContextChanged;
            }
            else
            {
                datagrid.LostKeyboardFocus -= RollbackDataGridOnLostFocus;
                datagrid.DataContextChanged -= RollbackDataGridOnDataContextChanged;
            }
        }

        static void RollbackDataGridOnLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            DataGrid senderDatagrid = sender as DataGrid;

            if (senderDatagrid == null)
                return;

            UIElement focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement == null)
                return;

            DataGrid focusedDatagrid = GetParentDatagrid(focusedElement); //let's see if the new focused element is inside a datagrid
            if (focusedDatagrid == senderDatagrid)
            {
                return;
                //if the new focused element is inside the same datagrid, then we don't need to do anything;
                //this happens, for instance, when we enter in edit-mode: the DataGrid element loses keyboard-focus, which passes to the selected DataGridCell child
            }
            senderDatagrid.CommitEdit();
            //otherwise, the focus went outside the datagrid; in order to avoid exceptions like ("DeferRefresh' is not allowed during an AddNew or EditItem transaction")
            //or ("CommitNew is not allowed for this view"), we undo the possible pending changes, if any
            IEditableCollectionView collection = senderDatagrid.Items as IEditableCollectionView;

            if (collection.IsEditingItem)
            {
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
            }
        }

        public static DataGrid GetParentDatagrid(UIElement element)
        {
            UIElement childElement; //element from which to start the tree navigation, looking for a Datagrid parent

            if (element is ComboBoxItem) //since ComboBoxItem.Parent is null, we must pass through ItemsPresenter in order to get the parent ComboBox
            {
                ItemsPresenter parentItemsPresenter = VisualTreeFinder.FindParentControl<ItemsPresenter>((element as ComboBoxItem));
                ComboBox combobox = parentItemsPresenter.TemplatedParent as ComboBox;
                childElement = combobox;
            }
            else
            {
                childElement = element;
            }

            DataGrid parentDatagrid = VisualTreeFinder.FindParentControl<DataGrid>(childElement); //let's see if the new focused element is inside a datagrid
            return parentDatagrid;
        }

        static void RollbackDataGridOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid senderDatagrid = sender as DataGrid;

            if (senderDatagrid == null)
                return;

            senderDatagrid.CommitEdit();
            IEditableCollectionView collection = senderDatagrid.Items as IEditableCollectionView;

            if (collection.IsEditingItem)
            {
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
            }
        }

        #endregion DataGridRollbackOnUnfocusedBehaviour
    }
}
