using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class CodeScheme :ICloneable, IIDPropertiesProvider
    {
        public static CodeScheme Find(List<CodeScheme> codeSchemes, string codeSchemeId)
        {
            foreach (CodeScheme codeScheme in codeSchemes)
            {
                if (codeScheme.Id == codeSchemeId)
                {
                    return codeScheme;
                }
            }
            return null;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public static List<Code> GetCodes(List<CodeScheme> codeSchemes)
        {
            List<Code> codes = new List<Code>();
            foreach (CodeScheme codeScheme in codeSchemes)
            {
                codes.AddRange(codeScheme.Codes);
            }
            return codes;
        }

        public static void ChangeCategoryId(List<CodeScheme> codeSchemes, string oldId, string newId)
        {
            foreach (CodeScheme codeScheme in codeSchemes)
            {
                Code.ChangeCategoryId(codeScheme.Codes, oldId, newId);
            }
        }

        public static void ChangeCodeSchemeId(List<CodeScheme> codeSchemes, string oldId, string newId)
        {
            foreach (CodeScheme codeScheme in codeSchemes)
            {
                Code.ChangeCodeSchemeId(codeScheme.Codes, oldId, newId);
            }
        }

        public CodeScheme()
        {
            Id = IDUtils.NewGuid();
            Codes = new List<Code>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public List<Code> Codes { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
