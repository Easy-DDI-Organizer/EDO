using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;
using EDO.Core.ViewModel;

namespace EDO.Core.Model
{
    public class QuestionConstruct :IConstruct, IStringIDProvider, ICloneable
    {
        public static void ChangeQuestionId(List<QuestionConstruct> questionConstructs, string oldId, string newId)
        {
            foreach (QuestionConstruct questionConstruct in questionConstructs)
            {
                if (questionConstruct.QuestionId == oldId)
                {
                    questionConstruct.QuestionId = newId;
                }
            }
        }

        public  string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public QuestionConstruct()
        {
            Id = IDUtils.NewGuid();

        }

        public string Id { get; set; }
        public string QuestionId { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string No { get; set; }

    }
}
