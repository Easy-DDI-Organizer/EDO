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
using EDO.Core.Util;
using EDO.Core.ViewModel;
using EDO.Main;
using System.Collections.Specialized;
using System.Diagnostics;
using EDO.Core.Model;
using EDO.Core.View;
using System.Windows.Markup;

namespace EDO.GroupCategory.CompareForm
{
    /// <summary>
    /// CompareFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class CompareFormView : FormView
    {
        public CompareFormView()
        {
            InitializeComponent();

        }

        private CompareFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<CompareFormVM>(this);
            }
        }

        private DataGridTextColumn CreateTitleColumn()
        {
            DataGridTextColumn titleColumn = new DataGridTextColumn();
            titleColumn.Header = Properties.Resources.List;
            titleColumn.Width = 100;
            titleColumn.Binding = new Binding("Title");
            titleColumn.IsReadOnly = true;
            return titleColumn;
        }

        private DataGridTextColumn CreateMemoColumn()
        {
            DataGridTextColumn titleColumn = new DataGridTextColumn();
            titleColumn.Header = Properties.Resources.Memo;
            titleColumn.Width = 100;
            titleColumn.Binding = new Binding("Memo");
            return titleColumn;
        }

        private DataGridComboBoxColumn CreateStudyUnitColumn(StudyUnitVM studyUnit, int index)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<DataGridComboBoxColumn ");
            sb.Append("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' ");
            sb.Append("xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' ");
            sb.Append("Header='").Append(studyUnit.Title).Append("' Width='100'>");
            sb.Append("<DataGridComboBoxColumn.ElementStyle>");
            sb.Append("<Style TargetType='ComboBox'>");
            sb.Append("<Setter Property='ItemsSource' Value='{Binding Path=DiffOptions}'/>");
            sb.Append("<Setter Property='DisplayMemberPath' Value='DetailLabel'/>");
            sb.Append("<Setter Property='SelectedItem' Value='{Binding SelectedDiffOptions[" + index + "]}'/>");
            sb.Append("</Style>");
            sb.Append("</DataGridComboBoxColumn.ElementStyle>");
            sb.Append("<DataGridComboBoxColumn.EditingElementStyle>");
            sb.Append("<Style TargetType='ComboBox'>");
            sb.Append("<Setter Property='ItemsSource' Value='{Binding Path=DiffOptions}'/>");
            sb.Append("<Setter Property='DisplayMemberPath' Value='DetailLabel'/>");
            sb.Append("<Setter Property='SelectedItem' Value='{Binding SelectedDiffOptions[" + index + "]}'/>");
            sb.Append("</Style>");
            sb.Append("</DataGridComboBoxColumn.EditingElementStyle>");
            sb.Append("</DataGridComboBoxColumn>");
            return (DataGridComboBoxColumn)XamlReader.Parse(sb.ToString());
        }

        protected override void OnFormDataContextChanged()
        {
            if (VM != null)
            {
                dataGrid.ItemsSource = VM.Rows;
                dataGrid.Columns.Clear();
                dataGrid.Columns.Add(CreateTitleColumn());
                int i = 0;
                foreach (StudyUnitVM studyUnit in VM.StudyUnits) 
                {
                    dataGrid.Columns.Add(CreateStudyUnitColumn(studyUnit, i));
                    i++;
                }
                dataGrid.Columns.Add(CreateMemoColumn());
            }
        }

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
                    null
                };
            }
        }
     }
}
