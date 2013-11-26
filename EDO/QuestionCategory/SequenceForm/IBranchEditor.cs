using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace EDO.QuestionCategory.SequenceForm
{
    public interface IBranchEditor
    {
        bool ValidateEditingBranch();
        bool ValidateAll();
        Dispatcher Dispatcher {get; }
    }
}
