using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model.Layout
{
    public class NumericLayout :ResponseLayout
    {
        public NumericLayoutMeasurementMethod MeasurementMethod { get; set; }

        public string Unit { get; set; }

        public LayoutStyle Style { get; set; }

        public string LeftStatement { get; set; }

        public string RightStatement { get; set; }

        public int? Length { get; set; }

    }
}
