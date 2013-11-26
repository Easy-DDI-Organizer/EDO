using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class VariableScheme: IIDPropertiesProvider
    {
        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }
        public VariableScheme()
        {
            Id = IDUtils.NewGuid();
        }
        public string Id { get; set; }
    }
}
