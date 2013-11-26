using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class IfThenElse :IConstruct, ICloneable
    {
        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public IfThenElse()
        {
            Id = IDUtils.NewGuid();
            IfCondition = new IfCondition();
            ElseIfs = new List<ElseIf>();
        }

        public string Id { get; set; }
        public IfCondition IfCondition {get; set; }
        public string ThenConstructId { get; set; }
        public string ElseConstructId { get; set; }
        public List<ElseIf> ElseIfs { get; set; }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public string No { get; set; }
    }
}
