using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EDO.Core.Model.Layout;

namespace EDO.Core.Model
{
    [XmlInclude(typeof(ChoicesLayout))]
    [XmlInclude(typeof(DateTimeLayout))]
    [XmlInclude(typeof(FreeLayout))]
    [XmlInclude(typeof(NumericLayout))]
    [XmlInclude(typeof(UnknownLayout))]
    public abstract class ResponseLayout :ICloneable
    {
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
