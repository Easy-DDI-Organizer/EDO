using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public interface IConstruct: IStringIDProvider, IIDPropertiesProvider
    {
        string No { get; set;  }
    }
}
