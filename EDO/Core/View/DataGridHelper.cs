using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using EDO.Properties;

namespace EDO.Core.View
{
    class DataGridHelper
    {
        #region GetCellValue
        public static string GetCellText(DataGridCell cell)
        {
            Object content = cell.Content;
            string text = ""; //こうするしかないのか?
            if (content is TextBlock)
            {
                TextBlock tb = (TextBlock)content;
                text = tb.Text;
            }
            return text;
        }

        public static string GetCellText(DataGrid dataGrid, int row, int column)
        {
            DataGridCell cell = GetCell(dataGrid, row, column);
            return GetCellText(cell);
        }

        public static bool IsEmptyRow(DataGrid dataGrid, DataGridRow row)
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                DataGridCell cell = GetCell(dataGrid, row, i);            
                string text = GetCellText(cell);
                if (!string.IsNullOrEmpty(text))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion GetCellValue


        #region GetCell
        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                // try to get the cell but it may possibly be virtualized
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    // now try to bring into view and retreive the cell
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);

                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }

                return cell;
            }

            return null;
        }

        public static DataGridCell GetCell(DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);
            return GetCell(dataGrid, rowContainer, column);
        }

        public static void FocusCell(DataGrid dataGrid, int row, int column)
        {
            DataGridCell cell = GetCell(dataGrid, row, column);
            if (cell != null)
            {
                cell.Focus();
            }
        }

        #endregion GetCell

        #region GetRow

        /// <summary>
        /// Gets the DataGridRow based on the given index
        /// </summary>
        /// <param name="index">the index of the container to get</param>
        public static DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // may be virtualized, bring into view and try again
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                dataGrid.UpdateLayout();

                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return row;
        }

        #endregion GetRow

        #region GetRowHeader

        /// <summary>
        /// Gets the DataGridRowHeader based on the row index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static DataGridRowHeader GetRowHeader(DataGrid dataGrid, int index)
        {
            return GetRowHeader(GetRow(dataGrid, index));
        }

        /// <summary>
        /// Returns the DataGridRowHeader based on the given row.
        /// </summary>
        /// <param name="row">Uses reflection to access and return RowHeader</param>
        public static DataGridRowHeader GetRowHeader(DataGridRow row)
        {
            if (row != null)
            {
                return GetVisualChild<DataGridRowHeader>(row);
            }
            return null;
        }

        #endregion GetRowHeader

        #region GetColumnHeader

        public static DataGridColumnHeader GetColumnHeader(DataGrid dataGrid, int index)
        {
            DataGridColumnHeadersPresenter presenter = GetVisualChild<DataGridColumnHeadersPresenter>(dataGrid);

            if (presenter != null)
            {
                return (DataGridColumnHeader)presenter.ItemContainerGenerator.ContainerFromIndex(index);
            }

            return null;
        }

        #endregion GetColumnHeader

        #region GetVisualChild

        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        public static T GetVisualChild<T>(Visual parent, int index) where T : Visual
        {
            T child = default(T);

            int encounter = 0;
            Queue<Visual> queue = new Queue<Visual>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Visual v = queue.Dequeue();
                child = v as T;
                if (child != null)
                {
                    if (encounter == index)
                        break;
                    encounter++;
                }
                else
                {
                    int numVisuals = VisualTreeHelper.GetChildrenCount(v);
                    for (int i = 0; i < numVisuals; i++)
                    {
                        queue.Enqueue((Visual)VisualTreeHelper.GetChild(v, i));
                    }
                }
            }

            return child;
        }

        public static bool VisualChildExists(Visual parent, DependencyObject visualToFind)
        {
            Queue<Visual> queue = new Queue<Visual>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                Visual v = queue.Dequeue();
                DependencyObject child = v as DependencyObject;
                if (child != null)
                {
                    if (child == visualToFind)
                        return true;
                }
                else
                {
                    int numVisuals = VisualTreeHelper.GetChildrenCount(v);
                    for (int i = 0; i < numVisuals; i++)
                    {
                        queue.Enqueue((Visual)VisualTreeHelper.GetChild(v, i));
                    }
                }
            }

            return false;
        }

        #endregion GetVisualChild

        #region FindPartByName

        public static DependencyObject FindPartByName(DependencyObject ele, string name)
        {
            DependencyObject result;
            if (ele == null)
            {
                return null;
            }
            if (name.Equals(ele.GetValue(FrameworkElement.NameProperty)))
            {
                return ele;
            }

            int numVisuals = VisualTreeHelper.GetChildrenCount(ele);
            for (int i = 0; i < numVisuals; i++)
            {
                DependencyObject vis = VisualTreeHelper.GetChild(ele, i);
                if ((result = FindPartByName(vis, name)) != null)
                {
                    return result;
                }
            }
            return null;
        }

        #endregion FindPartByName

        #region FindVisualParent

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element;
            while (parent != null)
            {
                T correctlyTyped = parent as T;
                if (correctlyTyped != null)
                {
                    return correctlyTyped;
                }

                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }

            return null;
        }

        #endregion FindVisualParent

        #region WaitTillQueueItemsProcessed

        public static void WaitTillQueueItemsProcessed()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(
                delegate(object arg)
                {
                    frame.Continue = false;
                    return null;
                }), null);

            // Keep the thread busy processing events until the timeout has expired.
            Dispatcher.PushFrame(frame);
        }

        #endregion WaitTillQueueItemsProcessed

        public static GroupBox CreateColumnCustomizer(DataGridColumn columnSource)
        {
            TextBox displayIndexTextBox = new TextBox();
            Binding tbBinding = new Binding("DisplayIndex");
            tbBinding.Source = columnSource;
            displayIndexTextBox.SetBinding(TextBox.TextProperty, tbBinding);

            GroupBox gb_DisplayIndex = new GroupBox();
            gb_DisplayIndex.Header = "DisplayIndex:";
            gb_DisplayIndex.Content = displayIndexTextBox;

            CheckBox canSortCB = new CheckBox();
            canSortCB.Content = "CanUserSort";
            Binding binding = new Binding("CanUserSort");
            binding.Source = columnSource;
            canSortCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox canReorderCB = new CheckBox();
            canReorderCB.Content = "CanUserReorder";
            binding = new Binding("CanUserReorder");
            binding.Source = columnSource;
            canReorderCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox canResizeCB = new CheckBox();
            canResizeCB.Content = "CanUserResize";
            binding = new Binding("CanUserResize");
            binding.Source = columnSource;
            canResizeCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox isReadOnlyCB = new CheckBox();
            isReadOnlyCB.Content = "IsReadOnly";
            Binding binding1 = new Binding("IsReadOnly");
            binding1.Source = columnSource;
            isReadOnlyCB.SetBinding(CheckBox.IsCheckedProperty, binding1);

            CheckBox isFrozenCB = new CheckBox();
            isFrozenCB.Content = "IsFrozen";
            isFrozenCB.IsEnabled = false;
            binding = new Binding("IsFrozen");
            binding.Source = columnSource;
            binding.Mode = BindingMode.OneWay;
            isFrozenCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            CheckBox isAutoGeneratedCB = new CheckBox();
            isAutoGeneratedCB.Content = "IsAutoGenerated";
            isAutoGeneratedCB.IsEnabled = false;
            binding = new Binding("IsAutoGenerated");
            binding.Source = columnSource;
            binding.Mode = BindingMode.OneWay;
            isAutoGeneratedCB.SetBinding(CheckBox.IsCheckedProperty, binding);

            ComboBox sortDirectionComboBox = new ComboBox();
            sortDirectionComboBox.Items.Add("null");
            sortDirectionComboBox.Items.Add("Ascending");
            sortDirectionComboBox.Items.Add("Descending");
            Binding sortDirBinding = new Binding("SortDirection");
            sortDirBinding.Source = columnSource;
            //            sortDirBinding.Converter = new SortDirectionConverter();
            sortDirectionComboBox.SetBinding(ComboBox.SelectedItemProperty, sortDirBinding);
            sortDirectionComboBox.SelectedIndex = 0;

            GroupBox gb_SortDirection = new GroupBox();
            gb_SortDirection.Header = "SortDirection:";
            gb_SortDirection.Content = sortDirectionComboBox;

            ComboBox cb_Visibility = new ComboBox();
            cb_Visibility.ItemsSource = Enum.GetValues(typeof(Visibility));
            binding = new Binding("Visibility");
            binding.Source = columnSource;
            cb_Visibility.SetBinding(ComboBox.SelectedItemProperty, binding);
            cb_Visibility.SelectedIndex = 0;

            GroupBox gb_Visibility = new GroupBox();
            gb_Visibility.Header = "Visibility:";
            gb_Visibility.Content = cb_Visibility;

            TextBox widthTextBox = new TextBox();
            Binding widthBinding = new Binding("Width");
            widthBinding.Source = columnSource;
            widthTextBox.SetBinding(TextBox.TextProperty, widthBinding);

            GroupBox gb_Width = new GroupBox();
            gb_Width.Header = "Width:";
            gb_Width.Content = widthTextBox;

            TextBox actualWidthTextBox = new TextBox();
            Binding actualWidthBinding = new Binding("ActualWidth");
            actualWidthBinding.Source = columnSource;
            actualWidthBinding.Mode = BindingMode.OneWay;
            actualWidthBinding.NotifyOnTargetUpdated = true;
            actualWidthTextBox.SetBinding(TextBox.TextProperty, actualWidthBinding);

            GroupBox gbActualWidth = new GroupBox();
            gbActualWidth.Header = "ActualWidth:";
            gbActualWidth.Content = actualWidthTextBox;

            TextBox desiredWidthTextBox = new TextBox();
            Binding desiredWidthBinding = new Binding("Width.DesiredValue");
            desiredWidthBinding.Source = columnSource;
            desiredWidthBinding.Mode = BindingMode.OneWay;
            desiredWidthTextBox.SetBinding(TextBox.TextProperty, desiredWidthBinding);

            GroupBox gb_DesiredWidth = new GroupBox();
            gb_DesiredWidth.Header = "DesiredWidth:";
            gb_DesiredWidth.Content = desiredWidthTextBox;

            TextBox displayWidthTextBox = new TextBox();
            Binding displayWidthBinding = new Binding("Width.DisplayValue");
            displayWidthBinding.Source = columnSource;
            displayWidthBinding.Mode = BindingMode.OneWay;
            displayWidthTextBox.SetBinding(TextBox.TextProperty, displayWidthBinding);

            GroupBox gb_DisplayWidth = new GroupBox();
            gb_DisplayWidth.Header = "DisplayWidth:";
            gb_DisplayWidth.Content = displayWidthTextBox;

            TextBox minWidthTextBox = new TextBox();
            Binding minWidthBinding = new Binding("MinWidth");
            minWidthBinding.Source = columnSource;
            minWidthTextBox.SetBinding(TextBox.TextProperty, minWidthBinding);

            GroupBox gb_MinWidth = new GroupBox();
            gb_MinWidth.Header = "MinWidth:";
            gb_MinWidth.Content = minWidthTextBox;

            TextBox maxWidthTextBox = new TextBox();
            Binding maxWidthBinding = new Binding("MaxWidth");
            maxWidthBinding.Source = columnSource;
            maxWidthTextBox.SetBinding(TextBox.TextProperty, maxWidthBinding);

            GroupBox gb_MaxWidth = new GroupBox();
            gb_MaxWidth.Header = "MaxWidth:";
            gb_MaxWidth.Content = maxWidthTextBox;

            TextBox sortMemberPathTextBox = new TextBox();
            binding = new Binding("SortMemberPath");
            binding.Source = columnSource;
            sortMemberPathTextBox.SetBinding(TextBox.TextProperty, binding);

            GroupBox gb_SortMemberPath = new GroupBox();
            gb_SortMemberPath.Header = "SortMemberPath:";
            gb_SortMemberPath.Content = sortMemberPathTextBox;

            StackPanel sp = new StackPanel();
            sp.Children.Add(gb_DisplayIndex);
            sp.Children.Add(canSortCB);
            sp.Children.Add(canReorderCB);
            sp.Children.Add(canResizeCB);
            sp.Children.Add(isReadOnlyCB);
            sp.Children.Add(isFrozenCB);
            sp.Children.Add(isAutoGeneratedCB);
            sp.Children.Add(gb_SortDirection);
            sp.Children.Add(gb_Width);
            //sp.Children.Add(gb_DesiredWidth);
            //sp.Children.Add(gb_DisplayWidth);
            //sp.Children.Add(gbActualWidth);
            sp.Children.Add(gb_MinWidth);
            sp.Children.Add(gb_MaxWidth);
            sp.Children.Add(gb_SortMemberPath);
            sp.Children.Add(gb_Visibility);

            GroupBox gb_all = new GroupBox();
            gb_all.Header = columnSource.Header + ":";
            gb_all.Content = sp;

            return gb_all;
        }

        public static DataGrid FindDataGrid(DependencyObject parent, string name)
        {
            return VisualTreeFinder.FindChild<DataGrid>(parent, name);
        }

        public static void Finalize(DependencyObject parent, string name)
        {
            DataGrid dataGrid = FindDataGrid(parent, name);
            if (dataGrid == null)
            {
                Debug.WriteLine("Finalize: dataGrid is not found name=" + name);
                return;
            }
            Finalize(dataGrid);
        }

        public static void Finalize(DataGrid grid)        
        {
            if (grid == null)
            {
                return;
            }
            grid.CommitEdit(); // まずCommitを試みる。
            IEditableCollectionView collection = grid.Items as IEditableCollectionView;
            if (collection.IsEditingItem)
            {
                collection.CancelEdit();
            }
            else if (collection.IsAddingNew)
            {
                collection.CancelNew();
                //編集中に保存ボタンを押した場合、新規アイテム入力用の行が消えてしまう。
                //一端非表示にして表示しなおし防ぐ。
                grid.CanUserAddRows = false;
                grid.CanUserAddRows = true;
            }
        }

        public static void Finalize(List<DataGrid> grids)
        {
            foreach (DataGrid grid in grids)
            {
                Finalize(grid);
            }
        }


        public static bool FinalizeConfirm(DataGrid grid)
        {
            if (grid == null)
            {
                return false;
            }
            //            grid.CommitEdit(); // まずCommitを試みる。
            IEditableCollectionView collection = grid.Items as IEditableCollectionView;
            if (collection.IsEditingItem || collection.IsAddingNew)
            {
                //編集中の行を確定して下さい
                MessageBox.Show(Resources.ConfirmEditingRow);
                return false;
            }
            return true;
        }

        public static void SetRemoveCommand(DataGrid dataGrid, ICommand command)
        {
            dataGrid.InputBindings.Clear();
            dataGrid.InputBindings.Add(new InputBinding(command, new KeyGesture(Key.Delete)));
        }

        public static void SetRemoveCommand(DependencyObject parent, string dataGridName, ICommand command)
        {
            DataGrid dataGrid = VisualTreeFinder.FindChild<DataGrid>(parent, dataGridName);
            if (dataGrid == null)
            {
                Debug.WriteLine("SetRemoveCommand: dataGrid is not found name=" + dataGridName);
                return;
            }
            SetRemoveCommand(dataGrid, command);
        }

        public static void SetRemoveCommand(List<DataGrid> grids, List<ICommand> commands)
        {
            Debug.Assert(grids.Count == commands.Count, "grid count <> command count");
            for (int i = 0; i < grids.Count; i++)
            {
                DataGrid grid = grids[i];
                ICommand command = commands[i];
                if (grid != null && command != null)
                {
                    SetRemoveCommand(grid, command);
                }
            }
        }

        private static void SetInputBindings(DataGrid grid, InputBindingCollection inputBindingCollection)
        {
            if (grid == null || inputBindingCollection == null || inputBindingCollection.Count == 0)
            {
                return;
            }
            grid.InputBindings.Clear();
            grid.InputBindings.AddRange(inputBindingCollection);
        }

        public static void SetInputBindings(List<DataGrid> grids, List<InputBindingCollection> inputBindingCollections)
        {
//            Debug.Assert(grids.Count == inputBindingCollections.Count, "grid count <> inputBindingCollections count");
            for (int i = 0; i < grids.Count; i++)
            {
                DataGrid grid = grids[i];
                InputBindingCollection inputBindingCollection = inputBindingCollections[i];
                SetInputBindings(grid, inputBindingCollection);
            }
        }

        public static void CommitEdit(List<DataGrid> grids)
        {
            foreach (DataGrid grid in grids)
            {
                if (grid != null)
                {
                    grid.CommitEdit();
                }
            }
        }

    }
}
