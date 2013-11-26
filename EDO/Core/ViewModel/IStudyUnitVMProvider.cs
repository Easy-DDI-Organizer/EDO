using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;

namespace EDO.Core.ViewModel
{
    public interface IStudyUnitVMProvider
    {
        StudyUnitVM StudyUnit { get; }
    }
}
