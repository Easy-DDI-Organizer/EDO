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
using EDO.Core.View;
using System.ComponentModel;
using System.Diagnostics;
using EDO.Core.Model;
using EDO.Core.Util;

namespace EDO.EventCategory.EventForm
{
    /// <summary>
    /// EventView.xaml の相互作用ロジック
    /// </summary>
    public partial class EventFormView : FormView
    {
        public EventFormView()
        {
            InitializeComponent();
        }

        private EventFormVM VM { get { return EDOUtils.GetVM<EventFormVM>(this); } }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>() { dataGrid };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(IB(VM.RemoveEventCommand, EDOConstants.KEY_DELETE))
                };
            }
        }

        protected override void OnFormDataContextChanged() 
        {
            if (VM != null)
            {
                dataGrid.ItemsSource = VM.Events; //ItemsSourceをセットしなおさないと複数のタブを開いたときにソートされない(原因不明)
                dataGrid.Items.SortDescriptions.Clear();
                dataGrid.Items.SortDescriptions.Add(new SortDescription("DateRange", ListSortDirection.Ascending));
                dataGrid.Items.SortDescriptions.Add(new SortDescription("No", ListSortDirection.Ascending));

                //ここで無理矢理コマンドを再設定しないとコンテキストメニューの「削除」コマンドがうまく動作しない。
                MenuItem menuItem = dataGrid.ContextMenu.Items[0] as MenuItem;
                menuItem.Command = VM.RemoveEventCommand;
            } 
        }
    }
}
