using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.ViewModel
{
    public interface IOrderedObject
    {
        int OrderNo { get; set;  }
        string OrderPrefix { get; set;  }
    }
}
