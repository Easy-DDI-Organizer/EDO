using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;
using EDO.Core.ViewModel;

namespace EDO.Core.Model
{
    public class Statement :IConstruct, IStringIDProvider, ICloneable
    {
        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public Statement()
        {
            Id = IDUtils.NewGuid();
        }

        public string Id { get; set; }
        public string Text { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public string No { get; set; }
    }
}
