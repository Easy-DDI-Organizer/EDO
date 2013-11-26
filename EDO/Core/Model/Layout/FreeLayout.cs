using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model.Layout
{
    public class FreeLayout :ResponseLayout
    {
        public LayoutStyle Style { get; set; }

        public int? ColumnCount { get; set; }

        public int? RowCount { get; set; }
    }
}
