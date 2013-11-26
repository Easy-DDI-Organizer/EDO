using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core;
using EDO.Core.Model;
using EDO.Core.ViewModel;
using EDO.Core.Util;

namespace EDO.StudyCategory.FundingInfoForm
{
    public class FundingInfoVM : BaseVM, IOrderedObject, IStringIDProvider
    {
        public FundingInfoVM() :this(new FundingInfo())
        {
        }

        public FundingInfoVM(FundingInfo fundingInfo)
        {
            this.fundingInfo = fundingInfo;
        }

        private FundingInfo fundingInfo;

        public FundingInfo FundingInfo { get { return fundingInfo; } }

        public override object Model
        {
            get
            {
                return fundingInfo;
            }
        }

        public string Id
        {
            get { return fundingInfo.Id;}
        }

        #region IOrderedObject メンバー

        public int OrderNo { get; set; }
        public string OrderPrefix { get; set; }

        #endregion
        public void InitTitle()
        {
            if (string.IsNullOrEmpty(fundingInfo.Title))
            {
                fundingInfo.Title = EDOUtils.OrderTitle(this);
            }
        }

        public string Title 
        {
            get
            {
                return fundingInfo.Title;
            }
            set
            {
                if (fundingInfo.Title != value)
                {
                    fundingInfo.Title = value;
                    InitTitle();
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public string OrganizationName 
        {
            get
            {
                return fundingInfo.Organization.OrganizationName;
            }
            set
            {
                if (fundingInfo.Organization.OrganizationName != value)
                {
                    fundingInfo.Organization.OrganizationName = value;
                    NotifyPropertyChanged("OrganizationName");
                    Memorize();
                }
            }
        }

        public decimal? Money 
        {
            get
            {
                return fundingInfo.Money;
            }
            set
            {
                if (fundingInfo.Money != value)
                {
                    fundingInfo.Money = value;
                    NotifyPropertyChanged("Money");
                    Memorize();
                }
            }
        }

        public string Number 
        {
            get
            {
                return fundingInfo.Number;
            }
            set
            {
                if (fundingInfo.Number != value)
                {
                    fundingInfo.Number = value;
                    NotifyPropertyChanged("Number");
                    Memorize();
                }
            }
        }

        public DateRange DateRange {
            get
            {
                return fundingInfo.DateRange;
            }
            set
            {
                if (!DateRange.EqualsDateRange(fundingInfo.DateRange, value))
                {
                    fundingInfo.DateRange = value;
                    NotifyPropertyChanged("DateRange");
                    Memorize();
                }
            }
        }
    }
}
