using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Code :ICloneable, IIDPropertiesProvider
    {
        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public static void ChangeCategoryId(List<Code> codes, string oldId, string newId)
        {
            foreach (Code code in codes)
            {
                if (code.CategoryId == oldId)
                {
                    code.CategoryId = newId;
                }
            }
        }

        public static void ChangeCodeSchemeId(List<Code> codes, string oldId, string newId)
        {
            foreach (Code code in codes)
            {
                if (code.CodeSchemeId == oldId)
                {
                    code.CodeSchemeId = newId;
                }
            }
        }

        public Code()
        {
            Id = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string Value { get; set; }
        public string CategoryId { get; set; }
        public string CodeSchemeId { get; set; }

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
