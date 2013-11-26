using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class QuestionGroupConstruct :IConstruct, IStringIDProvider
    {
        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public QuestionGroupConstruct()
        {
            Id = IDUtils.NewGuid();
        }


        public string Id { get; set; }
        public string QuestionGroupId { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string No { get; set; }
    }
}
