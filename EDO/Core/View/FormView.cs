using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using EDO.Core.Model;
using System.Diagnostics;
using System.Windows;
using EDO.Core.ViewModel;
using EDO.Core.Util;
using System.Windows.Input;
using System.Windows.Data;
using System.Reflection;

namespace EDO.Core.View
{
    public class FormView :UserControl, IValidatable
    {
        public static InputBinding IB(ICommand command, KeyGesture key)
        {
            return new InputBinding(command, key);
        }

        public static InputBindingCollection IBC(params InputBinding[] inputBindings)
        {
            InputBindingCollection collection = new InputBindingCollection(inputBindings);
            return collection;
        }

        public FormView()
        {
            Loaded += new RoutedEventHandler(form_Loaded);
            DataContextChanged += new DependencyPropertyChangedEventHandler(form_DataContextChanged);
        }

        protected FormVM FormVM { get {return EDOUtils.GetVM<FormVM>(this); }}

        private void form_Loaded(object sender, RoutedEventArgs e)
        {
            List<DataGrid> dataGrids = DataGrids;
            foreach (DataGrid dataGrid in DataGrids)
            {
                if (dataGrid != null)
                {
                    dataGrid.InitializingNewItem += dataGrid_InitializingNewItem;
                }
            }
            SetupDataGridInputBindings();
            OnFormLoaded();
        }

        protected virtual void SetupDataGridInputBindings()
        {
            DataGridHelper.SetInputBindings(DataGrids, DataGridInputBindingCollections);
        }

        protected virtual void OnFormLoaded() { }

        private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            FormVM.InitRow(e.NewItem);
        }

        protected virtual List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>();
            }
        }

        protected virtual List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>();
            }
        }

        private void form_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            EDOUtils.UpdateViewOfVM(this);
            if (FormVM != null)
            {
//                Debug.WriteLine("IsLoaded=" + IsLoaded);
                if (IsLoaded)
                {
                    // ・大部分の場合form_Loadedが呼び出されるのでそこでInputBindingを設定すれば良い。
                    // ・しかしEventFormViewで複数の調査ファイルを開き調査ファイル間をタブで切り替えた時、
                    // ２つ目のタブでform_Loadedが呼ばれず、結果としてInputBindingが設定さないことがある事を確認。
                    // ・上記の場合、IsLoaded=trueのまま、DataContextが変更されているようなのでここでInputBindingsを設定するようにした
                    // ・通常form_DataContextChanged(IsLoaded=false)→form_Loadedの順に呼ばれるので、二回InputBindingsが設定されることはない
                    // ・ちなみにIsLoaded=falseだと、DataGridHelper.FindDataGrid(this, "universeDataGrid")などがnullを返すような気がする。
                    SetupDataGridInputBindings();
                }
            }
            OnFormDataContextChanged();
        }

        protected virtual void OnFormDataContextChanged() { }

        public void FinalizeDataGrid()
        {
            DataGridHelper.Finalize(DataGrids);
        }

        public virtual bool Validate()
        {
            List<DataGrid> dataGrids = DataGrids;
            DataGridHelper.CommitEdit(dataGrids);
            if (!Validator.Validate(this))
            {
                return false;
            }
            DataGridHelper.Finalize(dataGrids);
            FormVM.Complete(null);
            return true;
        }
    }
}
