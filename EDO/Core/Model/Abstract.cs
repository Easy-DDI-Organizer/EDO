using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Abstract: IIDPropertiesProvider
    {
        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id", "PurposeId", "SummaryId" };
            }
        }

        public Abstract()
        {
            Id = IDUtils.NewGuid();
            PurposeId = IDUtils.NewGuid();
            SummaryId = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string PurposeId { get; set; } //DDI用
        public string Purpose { get; set; }
        public string SummaryId { get; set; } //DDI用
        public string Summary { get; set; }
    }
}
