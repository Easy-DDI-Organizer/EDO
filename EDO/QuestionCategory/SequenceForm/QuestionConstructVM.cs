using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.QuestionCategory.QuestionForm;
using System.Diagnostics;
using EDO.Properties;

namespace EDO.QuestionCategory.SequenceForm
{
    public class QuestionConstructVM :ConstructVM
    {
        public static QuestionConstructVM FindQuestionConstructByNo(ICollection<QuestionConstructVM> questionConstructs, string no)
        {
            foreach (QuestionConstructVM construct in questionConstructs)
            {
                if (construct.No == no)
                {
                    return construct;
                }
            }
            return null;
        }

        public QuestionConstructVM(QuestionConstruct questionConstruct, QuestionVM question)
            : base(questionConstruct)
        {
            Debug.Assert(questionConstruct != null);
            Debug.Assert(question != null);
            this.questionConstruct = questionConstruct;
            this.question = question;
        }

        private QuestionConstruct questionConstruct;

        private QuestionVM question;

        public override string Id { get { return questionConstruct.Id; } }

        public override string QuestionId { get { return question.Id; } }

        public QuestionVM Question { get { return question; } }

        public ResponseVM Response { get { return question.Response; } }

        public override string Title
        {
            get
            {
                return question.Content;
            }
        }

        public string Text
        {
            get
            {
                return question.Text;
            }
        }

        public override string TypeString
        {
            get
            {
                return Resources.Question;
            }
        }
    }
}
