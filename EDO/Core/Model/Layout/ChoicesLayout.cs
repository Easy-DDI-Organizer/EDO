using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model.Layout
{
    public class ChoicesLayout :ResponseLayout
    {
        public ChoicesLayoutMesurementMethod MeasurementMethod { get; set; }

        public int? MaxSelectionCount { get; set; }

        public bool Surround { get; set; }

        public ChoicesLayoutDirection Direction { get; set; }

        public int? ColumnCount { get; set; }
    }
}
