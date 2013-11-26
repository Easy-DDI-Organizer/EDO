using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public class MissingValue :ICloneable
    {
        public static string JoinContent(List<MissingValue> missingValues, string separator)
        {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < missingValues.Count; i++)
            {
                MissingValue mv = missingValues[i];
                buf.Append(mv.Content);
                if (i != missingValues.Count - 1)
                {
                    buf.Append(separator);
                }
            }
            return buf.ToString();
        }

        public string Content { get; set; }

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
