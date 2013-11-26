using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;

namespace EDO.QuestionCategory.SequenceForm.Chart
{
    public class CondInfo
    {
        public static List<CondInfo> Create(IfThenElseVM ifThenElse)
        {
            List<CondInfo> condInfos = new List<CondInfo>();
            IfThenElse ifThenElseModel = (IfThenElse)ifThenElse.Model;
            ConstructVM construct = ifThenElse.FindConstruct(ifThenElseModel.ThenConstructId);
            if (construct != null)
            {
                CondInfo condInfo = new CondInfo();
                condInfo.TargetConstructId = construct.Id;
                condInfo.Code = ifThenElseModel.IfCondition.Code;
                condInfos.Add(condInfo);
            }

            foreach (ElseIf elseIf in ifThenElseModel.ElseIfs)
            {
                construct = ifThenElse.FindConstruct(elseIf.ThenConstructId);
                if (construct != null)
                {
                    CondInfo condInfo = new CondInfo();
                    condInfo.TargetConstructId = construct.Id;
                    condInfo.Code = elseIf.IfCondition.Code;
                    condInfos.Add(condInfo);
                }
            }

            construct = ifThenElse.FindConstruct(ifThenElseModel.ElseConstructId);
            if (construct != null)
            {
                CondInfo condInfo = new CondInfo();
                condInfo.TargetConstructId = ifThenElseModel.ElseConstructId;
                condInfo.Code = "ELSE";
                condInfos.Add(condInfo);
            }
            return condInfos;
        }

        public string TargetConstructId { get; set; }
        public string Code { get; set; }
    }
}
