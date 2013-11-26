using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public class ElseIf :ICloneable
    {
        public ElseIf()
        {
            IfCondition = new IfCondition();
        }
        public IfCondition IfCondition { get; set; }
        public string ThenConstructId { get; set; }
        public string ElseConstructId { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
