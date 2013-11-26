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
    public class DataGridRollbackOnDataContextChangedBehaviour
    {
        public static readonly DependencyProperty DataGridRollbackOnDataContextChangedProperty =
            DependencyProperty.RegisterAttached(
            "DataGridRollbackOnDataContextChanged",
            typeof(bool),
            typeof(DataGridRollbackOnDataContextChangedBehaviour),
            new UIPropertyMetadata(false, OnDataContextChanged));

        public static bool GetDataGridRollbackOnDataContextChanged(DataGrid datagrid)
        {
            return (bool)datagrid.GetValue(DataGridRollbackOnDataContextChangedProperty);
        }

        public static void SetDataGridRollbackOnDataContextChanged(DataGrid datagrid, bool value)
        {
            datagrid.SetValue(DataGridRollbackOnDataContextChangedProperty, value);
        }


        static void OnDataContextChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            DataGrid datagrid = depObj as DataGrid;
            if (datagrid == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
            {
                datagrid.DataContextChanged += RollbackDataGridOnDataContextChanged;
            }
            else
            {
                datagrid.DataContextChanged -= RollbackDataGridOnDataContextChanged;
            }
        }

        static void RollbackDataGridOnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataGrid senderDatagrid = sender as DataGrid;

            if (senderDatagrid == null)
                return;
            senderDatagrid.CommitEdit(); //ここでコミットしても保存されない(?)
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
    }
}
