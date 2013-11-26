using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using EDO.Core.Util;
using System.Collections.Specialized;
using EDO.Core.View;
using EDO.Properties;

namespace EDO.StudyCategory.FundingInfoForm
{
    public class FundingInfoFormVM :FormVM
    {
        private static readonly string PREFIX = Resources.FundMoney; //助成金

        public FundingInfoFormVM(StudyUnitVM studyUnitVM) :base(studyUnitVM)
        {
            fundingInfos = new ObservableCollection<FundingInfoVM>();
            int i = 1;
            foreach (FundingInfo fundingInfoModel in studyUnitVM.FundingInfoModels)
            {
                FundingInfoVM fundingInfo = new FundingInfoVM(fundingInfoModel)
                {
                    Parent = this,
                    OrderNo = i++,
                    OrderPrefix = PREFIX
                };
                fundingInfo.InitTitle();
                fundingInfos.Add(fundingInfo);
            }
            modelSyncher = new ModelSyncher<FundingInfoVM, FundingInfo>(this, fundingInfos, studyUnitVM.FundingInfoModels);
        }

        private ModelSyncher<FundingInfoVM, FundingInfo> modelSyncher;
        private ObservableCollection<FundingInfoVM> fundingInfos;
        public ObservableCollection<FundingInfoVM> FundingInfos { get {return fundingInfos;} }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedItem = EDOUtils.Find(fundingInfos, state.State1);
            }
            if (SelectedItem == null)
            {
                SelectedItem = EDOUtils.GetFirst(fundingInfos);
            }
        }

        public override VMState SaveState()
        {
            VMState state = new VMState();
            state.State1 = SelectedFundingInfo.Id;
            return state;
        }

        private object selectedItem;
        public object SelectedItem {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    NotifyPropertyChanged("SelectedItem");
                }
            }
        }

        public FundingInfoVM SelectedFundingInfo
        {
            get
            {
                return this.selectedItem as FundingInfoVM;
            }
        }

        public ICommand addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                {
                    addCommand = new RelayCommand(param => AddFundingInfo(), param => CanAddFundingInfo);
                }
                return addCommand;
            }
        }

        public bool CanAddFundingInfo
        {
            get
            {
                return true;
            }
        }

        public void AddFundingInfo()
        {
            int count = fundingInfos.Count + 1;
            FundingInfoVM fundingInfo = new FundingInfoVM()
            {
                Parent = this,
                OrderNo = EDOUtils.GetMaxOrderNo<FundingInfoVM>(fundingInfos) + 1,
                OrderPrefix = PREFIX
            };
            fundingInfo.InitTitle();
            fundingInfos.Add(fundingInfo);
            SelectedItem = fundingInfo;
            Memorize();
        }

        public ICommand removeCommand;
        public ICommand RemoveCommand
        {
            get
            {
                if (removeCommand == null)
                {
                    removeCommand = new RelayCommand(param => RemoveFundingInfo(), param => CanRemoveFundingInfo);
                }
                return removeCommand;
            }
        }

        public bool CanRemoveFundingInfo
        {
            get
            {
                if (SelectedFundingInfo == null)
                {
                    return false;
                }
                return fundingInfos.Count > 1;
            }
        }

        public void RemoveFundingInfo()
        {
            fundingInfos.Remove(SelectedFundingInfo);
            SelectedItem = fundingInfos.Last();
//            Memorize();
        }
    }    
}
