using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public class IfCondition :ICloneable
    {
        public IfCondition()
        {
        }

        public string Code { get; set; }
        public string QuestionId { get; set; }
        public string Text { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
