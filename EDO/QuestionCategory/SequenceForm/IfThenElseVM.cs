using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.Util;
using EDO.Properties;

namespace EDO.QuestionCategory.SequenceForm
{
    public class IfThenElseVM :ConstructVM
    {
        public IfThenElseVM(IfThenElse ifThenElse)
            : base(ifThenElse)
        {
            this.ifThenElse = ifThenElse;
        }

        private IfThenElse ifThenElse;

        public List<ConstructVM> ThenConstructs { get; set; }

        public override string Id { get { return ifThenElse.Id; } }


        public ConstructVM FindConstruct(string id)
        {
            return EDOUtils.Find(ThenConstructs, id);
        }

        public override string Title
        {
            get
            {
                StringBuilder buf = new StringBuilder();

                buf.Append("IF ");
                buf.Append(ifThenElse.IfCondition.Code);
                ConstructVM questionConstruct = FindConstruct(ifThenElse.ThenConstructId);
                buf.Append(" THEN ");
                if (questionConstruct != null)
                {
                    buf.Append(questionConstruct.No);
                }
                foreach (ElseIf elseIf in ifThenElse.ElseIfs)
                {
                    buf.Append("\nELSEIF ");
                    buf.Append(elseIf.IfCondition.Code);
                    questionConstruct = FindConstruct(elseIf.ThenConstructId);
                    buf.Append(" THEN ");
                    if (questionConstruct != null)
                    {
                        buf.Append(questionConstruct.No);
                    }
                }
                questionConstruct = FindConstruct(ifThenElse.ElseConstructId);
                if (questionConstruct != null)
                {
                    buf.Append("\nELSE ");
                    buf.Append(questionConstruct.No);
                }
                return buf.ToString();

            }
        }

        public int GetCondCount()
        {
            int count = 0;
            ConstructVM construct = FindConstruct(ifThenElse.ThenConstructId);
            if (construct != null)
            {
                count++;
            }
            foreach (ElseIf elseIf in ifThenElse.ElseIfs)
            {
                construct = FindConstruct(elseIf.ThenConstructId);
                if (construct != null)
                {
                    count++;
                }
            }
            construct = FindConstruct(ifThenElse.ElseConstructId);
            if (construct != null)
            {
                count++;
            }
            return count;
        }

        public override string TypeString
        {
            get { return Resources.Branch; }
        }


        public override string No
        {
            get
            {
                return base.No;
            }
            set
            {
                base.No = value;
            }
        }
    }
}
