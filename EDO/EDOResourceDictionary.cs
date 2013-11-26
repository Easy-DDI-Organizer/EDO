using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EDO.StudyCategory.AbstractForm;
using System.Diagnostics;
using System.Windows.Input;
using EDO.QuestionCategory.ConceptForm;
using EDO.QuestionCategory.QuestionForm;

namespace EDO
{
    partial class EDOResourceDictionary :ResourceDictionary
    {
        public EDOResourceDictionary()
        {
            InitializeComponent();
        }

        public void bookDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid == null || dataGrid.Name != "bookDataGrid")
            {
                return;
            }
            AbstractFormVM abstractForm = dataGrid.DataContext as AbstractFormVM;
            if (abstractForm != null)
            {
                abstractForm.EditBook();
                return;
            }
            ConceptVM concept = dataGrid.DataContext as ConceptVM;
            if (concept != null)
            {
                concept.EditBook();
                return;
            }
            ResponseVM response = dataGrid.DataContext as ResponseVM;
            if (response != null)
            {
                response.EditBook();
                return;
            }
        }
    }
}
