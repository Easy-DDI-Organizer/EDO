using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace EDO.Core.View
{
    public class DataGridEx : DataGrid
    {
        protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCommitEdit(e);
        }
    }
}
