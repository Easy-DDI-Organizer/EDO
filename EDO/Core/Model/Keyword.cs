using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public class Keyword :ICloneable
    {
        public string Content { get; set; }

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
