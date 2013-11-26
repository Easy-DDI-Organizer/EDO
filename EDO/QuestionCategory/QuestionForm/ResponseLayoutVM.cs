using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;

namespace EDO.QuestionCategory.QuestionForm
{
    public class ResponseLayoutVM :BaseVM
    {
        public ResponseLayoutVM(ResponseLayout layout)
        {
            this.layout = layout;
        }

        private ResponseLayout layout;
        public ResponseLayout Layout { get { return layout; } }
    }
}
