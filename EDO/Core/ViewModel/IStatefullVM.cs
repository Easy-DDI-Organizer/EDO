using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.View;

namespace EDO.Core.ViewModel
{
    public interface IStatefullVM
    {
        void LoadState(VMState state);
        VMState SaveState();
    }
}
