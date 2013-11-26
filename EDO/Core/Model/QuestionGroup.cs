using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class QuestionGroup: IIDPropertiesProvider
    {
        public static bool Contains(List<QuestionGroup> questionGroups, Question question)
        {
            foreach (QuestionGroup questionGroup in questionGroups)
            {
                if (questionGroup.Contains(question.Id))
                {
                    return true;
                }
            }
            return false;
        }

        public static QuestionGroup Find(List<QuestionGroup> questionGroups, string questionId)
        {
            foreach (QuestionGroup questionGroup in questionGroups)
            {
                if (questionGroup.Id == questionId)
                {
                    return questionGroup;
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

        public QuestionGroup()
        {
            Id = IDUtils.NewGuid();
            QuestionIds = new List<string>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public List<string> QuestionIds { get; set; }

        public QuestionGroupOrientation Orientation { get; set; }
        public QuestionGroupSentence Sentence { get; set; }

        public bool Contains(string questionId)
        {
            foreach (string qid in QuestionIds)
            {
                if (qid == questionId)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
