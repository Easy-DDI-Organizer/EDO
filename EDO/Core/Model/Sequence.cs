using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Sequence :ICloneable, IIDPropertiesProvider
    {
        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public Sequence()
        {
            Id = IDUtils.NewGuid();
            ControlConstructIds = new List<string>();
        }

        public string Id { get; set; }

        public List<string> ControlConstructIds { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
