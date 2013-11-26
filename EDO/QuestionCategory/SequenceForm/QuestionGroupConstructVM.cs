using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using System.Diagnostics;
using EDO.QuestionCategory.QuestionGroupForm;
using EDO.Properties;

namespace EDO.QuestionCategory.SequenceForm
{
    public class QuestionGroupConstructVM :ConstructVM        
    {
        public QuestionGroupConstructVM(QuestionGroupConstruct questionGroupConstruct, QuestionGroupVM questionGroup)
            : base(questionGroupConstruct)
        {
            Debug.Assert(questionGroupConstruct != null);
            Debug.Assert(questionGroup != null);
            this.questionGroupConstruct = questionGroupConstruct;
            this.questionGroup = questionGroup;
        }

        private QuestionGroupConstruct questionGroupConstruct;
        private QuestionGroupVM questionGroup;

        public override string Id { get { return questionGroupConstruct.Id; } }

        public override string QuestionGroupId { get { return questionGroup.Id; } }

        public QuestionGroupVM QuestionGroup { get { return questionGroup; } }

        public override string Title
        {
            get
            {
                return questionGroup.Title;
            }
        }


        public override string TypeString
        {
            get
            {
                return Resources.QuestionGroup;
            }
        }

    }
}
