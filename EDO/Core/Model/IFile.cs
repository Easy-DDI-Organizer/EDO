using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public interface IFile
    {
        string PathName { get; set; }
        string Id { get; set; }
    }
}
