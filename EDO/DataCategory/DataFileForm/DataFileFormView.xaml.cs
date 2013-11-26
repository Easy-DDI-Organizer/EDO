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
using EDO.VariableCategory.VariableForm;
using EDO.Core.Util;
using EDO.Core.View;
using EDO.Core.Model;

namespace EDO.DataCategory.DataFileForm
{
    /// <summary>
    /// DataFileFormView.xaml の相互作用ロジック
    /// </summary>
    public partial class DataFileFormView : FormView
    {
        public DataFileFormView()
        {
            InitializeComponent();
        }

        private DataFileFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<DataFileFormVM>(this);
            }
        }
    }
}
