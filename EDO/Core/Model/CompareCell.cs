using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace EDO.Core.Model
{
    public class CompareCell
    {
        public CompareCell()
        {
        }
        public string CompareValue { get; set; }
        public string TargetTitle { get; set; }
        public string ColumnStudyUnitId { get; set; }

        [XmlIgnoreAttribute]
        public bool IsMatch
        {
            get
            {
                return Options.COMPARE_VALUE_MATCH_CODE == CompareValue;
            }
        }
        [XmlIgnoreAttribute]
        public bool IsNotMatch
        {
            get
            {
                return Options.COMPARE_VALUE_NOTMATCH_CODE == CompareValue;
            }
        }
        [XmlIgnoreAttribute]
        public bool IsPartialMatch
        {
            get
            {
                return Options.COMPARE_VALUE_PARTIALMATCH_CODE == CompareValue;
            }
        }
    }
}
