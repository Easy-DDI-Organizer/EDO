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
using System.Diagnostics;
using System.Collections.ObjectModel;
using EDO.Core.View;
using EDO.Core.Model;
using EDO.Core.Util;
using System.ComponentModel;

namespace EDO.StudyCategory.MemberForm
{
    /// <summary>
    /// MemberView.xaml の相互作用ロジック
    /// </summary>
    public partial class MemberFormView : FormView
    {
        public MemberFormView()
        {
            InitializeComponent();
        }

        private MemberFormVM VM { get {return EDOUtils.GetVM<MemberFormVM>(this);} }

        protected override List<DataGrid> DataGrids
        {
            get
            {
                return new List<DataGrid>() { 
                    memberDataGrid, 
                    organizationDataGrid 
                };
            }
        }

        protected override List<InputBindingCollection> DataGridInputBindingCollections
        {
            get
            {
                return new List<InputBindingCollection>() {
                    IBC(IB(VM.RemoveMemberCommand, EDOConstants.KEY_DELETE)),
                    IBC(IB(VM.RemoveOrganizationCommand, EDOConstants.KEY_DELETE)),
                };
            }
        }
    }
}
