using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Universe :ICloneable, IIDPropertiesProvider
    {
        public static Universe FindMainUniverse(List<Universe> universes)
        {
            if (universes.Count == 0)
            {
                return null;
            }
            foreach (Universe universe in universes)
            {
                if (universe.IsMain)
                {
                    return universe;
                }
            }
            return universes[0];
        }

        public static Universe Find(List<Universe> universes, string universeId)
        {
            foreach (Universe universe in universes)
            {
                if (universe.Id == universeId)
                {
                    return universe;
                }
            }
            return null;
        }


        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id", "SamplingProcedureId" };
            }
        }

        public Universe()
        {
            IsMain = false;
            Id = IDUtils.NewGuid();
            SamplingProcedureId = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string Title {get; set; }
        public string Memo { get; set; }
        public string Method { get; set; }
        public bool IsMain { get; set; }
        public string SamplingProcedureId { get; set; }

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
