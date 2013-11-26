using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class FundingInfo :IIDPropertiesProvider
    {
        public static List<Organization> GetOrganizations(List<FundingInfo> fundingInfos)
        {
            List<Organization> organizations = new List<Organization>();
            foreach (FundingInfo fundingInfo in fundingInfos)
            {
                organizations.Add(fundingInfo.Organization);
            }
            return organizations;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public FundingInfo()
        {
            Id = IDUtils.NewGuid();
            DateRange = new DateRange();
            Organization = new Organization();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public Organization Organization { get; set; }
        public decimal? Money { get; set; }
        public string Number { get; set; }
        public DateRange DateRange { get; set; }
    }
}
