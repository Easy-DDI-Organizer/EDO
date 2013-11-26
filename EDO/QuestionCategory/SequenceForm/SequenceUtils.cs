using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.Util;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Diagnostics;
using EDO.Core.ViewModel;

namespace EDO.QuestionCategory.SequenceForm
{
    public static class SequenceUtils
    {

        private static IfCondition CreateIfCondition(List<CondGroupVM> condGroups)
        {
            StringBuilder buf = new StringBuilder();
            foreach (CondGroupVM condGroup in condGroups)
            {
                buf.Append(condGroup.Expression(BaseVM.EMPTY_VALUE));
            }
            IfCondition ifCondition = new IfCondition();
            ifCondition.Code = buf.ToString();
            ifCondition.QuestionId = condGroups[0].FirstQuestionConstruct.Id;    
            return ifCondition;
        }

        private static IfThenElse CreateIfThenElse(BranchVM branch)
        {
            IfThenElse ifThenElse = new IfThenElse();
            ifThenElse.No = "-";
            ifThenElse.IfCondition = CreateIfCondition(branch.ValidCondGroups);
            ifThenElse.ThenConstructId = branch.ThenConstruct.Id;
            return ifThenElse;
        }

        private static ElseIf CreateElseIf(BranchVM branch)
        {
            ElseIf elseIf = new ElseIf();
            elseIf.IfCondition = CreateIfCondition(branch.ValidCondGroups);
            elseIf.ThenConstructId = branch.ThenConstruct.Id;
            return elseIf;
        }

        public static IfThenElse CreateIfThenElse(List<BranchVM> branches)
        {
            if (branches.Count == 0)
            {
                return null;
            }
            IfThenElse ifThenElse = CreateIfThenElse(branches[0]);
            for (int i = 1; i < branches.Count; i++)
            {
                BranchVM branch = branches[i];
                if (branch.IsTypeElseIf)
                {
                    ElseIf elseIf = CreateElseIf(branch);
                    if (elseIf != null)
                    {
                        ifThenElse.ElseIfs.Add(elseIf);
                    }
                }
                else if (branch.IsTypeElse)
                {
                    ifThenElse.ElseConstructId = branch.ThenConstruct.Id;
                }                
            }
            return ifThenElse;
        }


        private static void CreateCondGroups(string code, BranchVM branch, ObservableCollection<QuestionConstructVM> questionConstructs)
        {
            CondParser parser = new CondParser(code);
            List<CondGroup> condGroupElems  = parser.Parse();

            foreach (CondGroup condGroupElem in condGroupElems)
            {
                CondGroupVM condGroup = new CondGroupVM()
                {
                    Parent = branch
                };
                condGroup.SelectedConnectionCode = Option.FindCodeByLabel(Options.Connections, condGroupElem.Connector);
                branch.CondGroups.Add(condGroup);

                foreach (Cond condElem in condGroupElem.Conds)
                {
                    CondVM cond = new CondVM()
                    {
                        Parent = condGroup
                    };
                    condGroup.Conds.Add(cond);
                    cond.SelectedQuestionConstruct = QuestionConstructVM.FindQuestionConstructByNo(questionConstructs, condElem.LeftValue);
                    cond.SelectedOperatorCode = Option.FindCodeByLabel(Options.Operators, condElem.Operator);
                    cond.CondValue = condElem.RightValue;
                }
            }
        }

        private static BranchVM CreateIfBranch(IfThenElse ifThenElse, CreateBranchWindowVM window)
        {
            BranchVM branch = new BranchVM(BranchVM.TYPE_IF_CODE)
            {
                Parent = window
            };
            branch.Init();
            branch.CondGroups.Clear();
            IfCondition ifCondition = ifThenElse.IfCondition;
            CreateCondGroups(ifCondition.Code, branch, window.QuestionConstructs);
            branch.ThenConstruct = EDOUtils.Find(window.ThenConstructs, ifThenElse.ThenConstructId);
            return branch;
        }

        private static List<BranchVM> CreateElseIfBranches(IfThenElse ifThenElse, CreateBranchWindowVM window)
        {
            List<BranchVM> branches = new List<BranchVM>();
            List<ElseIf> elseIfs = ifThenElse.ElseIfs;
            foreach (ElseIf elseIf in elseIfs)
            {
                BranchVM branch = new BranchVM(BranchVM.TYPE_ELSE_IF_CODE)
                {
                    Parent = window
                };
                branch.Init();
                branch.CondGroups.Clear();
                CreateCondGroups(elseIf.IfCondition.Code, branch, window.QuestionConstructs);
                branch.ThenConstruct = EDOUtils.Find(window.ThenConstructs, elseIf.ThenConstructId);
                branches.Add(branch);
            }
            return branches;
        }

        private static BranchVM CreateElseBranch(IfThenElse ifThenElse, CreateBranchWindowVM window)
        {
            if (ifThenElse.ElseConstructId == null)
            {
                return null;
            }
            BranchVM branch = new BranchVM(BranchVM.TYPE_ELSE_CODE)
            {
                Parent = window
            };
            branch.Init();
            branch.CondGroups.Clear();
            branch.ThenConstruct = EDOUtils.Find(window.ThenConstructs, ifThenElse.ElseConstructId);
            return branch;
        }

        public static ObservableCollection<BranchVM> CreateBranches(IfThenElse ifThenElse, CreateBranchWindowVM window)
        {
            ObservableCollection<BranchVM> branches = new ObservableCollection<BranchVM>();
            BranchVM ifBranch = CreateIfBranch(ifThenElse, window);
            branches.Add(ifBranch);
            List<BranchVM> elseIfBranches = CreateElseIfBranches(ifThenElse, window);
            branches.AddRange(elseIfBranches);
            BranchVM elseBranch = CreateElseBranch(ifThenElse, window);
            if (elseBranch != null)
            {
                branches.Add(elseBranch);
            }
            return branches;
        }
    }
}
