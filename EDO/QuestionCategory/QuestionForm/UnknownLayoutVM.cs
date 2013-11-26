using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model.Layout;

namespace EDO.QuestionCategory.QuestionForm
{
    public class UnknownLayoutVM :ResponseLayoutVM
    {
        public UnknownLayoutVM(UnknownLayout unknownLayout)
            : base(unknownLayout)
        {
        }

        public UnknownLayout UnknownLayout {get {return (UnknownLayout)Layout; }}
    }
}
