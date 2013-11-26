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
using System.Collections.ObjectModel;
using EDO.Core.Util;
using EDO.Core.View;
using EDO.Core.Model;
using System.ComponentModel;

namespace EDO.StudyCategory.FundingInfoForm
{
    /// <summary>
    /// FundingInfoView.xaml の相互作用ロジック
    /// </summary>
    public partial class FundingInfoFormView : FormView
    {
        public FundingInfoFormView()
        {
            InitializeComponent();
        }

        private FundingInfoFormVM VM
        {
            get
            {
                return EDOUtils.GetVM<FundingInfoFormVM>(this);
            }
        }

        protected override void OnFormLoaded()
        {
            tabControl.Items.CurrentChanging += Items_CurrentChanging;
        }

        void Items_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (!e.IsCancelable)
            {
                return;
            }
            Validate();
        }          
    }
}
