using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace EDO.QuestionCategory.SequenceForm
{

    public class CondGroup
    {
        public CondGroup()
        {
            Connector = "AND";
            Conds = new List<Cond>();
        }
        public string Connector { get; set; }
        public List<Cond> Conds { get; set; }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(Connector);
            buf.Append(" (");
            foreach (Cond cond in Conds)
            {
                buf.Append(cond.ToString());
                if (Conds.Last() != cond)
                {
                    buf.Append(" ");
                }
            }
            buf.Append(")");
            return buf.ToString();
        }
    }

    public class Cond
    {
        public Cond()
        {
            Connector = "OR";
        }
        public string Connector { get; set; }
        public string LeftValue { get; set; }
        public string Operator { get; set; }
        public string RightValue { get; set; }

        public override string ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(Connector);
            buf.Append(" ");
            buf.Append(LeftValue);
            buf.Append(" ");
            buf.Append(Operator);
            buf.Append(" ");
            buf.Append(RightValue);
            return buf.ToString();
        }
    }

    //順序のConditionに記述される文字列形式の論理式を解析する
    public class CondParser
    {
        const string PLACEHOLDER = "__GROUP_PLACEHOLDER__ = __GROUP_PLACEHOLDER__";

        private string code;
        public CondParser(string code)
        {
            this.code = code;
        }

        private List<string> childCondStrings = new List<string>();

        private string ReplaceByPlaceholder(Match m)
        {
            childCondStrings.Add(m.Groups[1].Value);
            return PLACEHOLDER;
        }

        public List<CondGroup> Parse()
        {
            childCondStrings.Clear();
            string escapeedCode = "AND " + Regex.Replace(code, @"\((.*?)\)", ReplaceByPlaceholder);
            Regex regex = new Regex(@"(AND|OR)\s+(\S+)\s+(\S+)\s+(\S+)\s*");
            Regex placeHolderRegex = new Regex(@"^(AND|OR)\s+" + PLACEHOLDER);
            List<CondGroup> condGroups = new List<CondGroup>();
            MatchCollection mc = regex.Matches(escapeedCode);
            int placeHolderIndex = 0;
            foreach (Match m in mc)
            {
                CondGroup condGroup = new CondGroup();
                condGroups.Add(condGroup);
                condGroup.Connector = m.Groups[1].Value;
                if (placeHolderRegex.IsMatch(m.Groups[0].Value))
                {
                    string childCode = "OR " + childCondStrings[placeHolderIndex++];
                    MatchCollection childMatchCollection = regex.Matches(childCode);
                    foreach (Match childMatch in childMatchCollection)
                    {
                        Cond cond = new Cond();
                        condGroup.Conds.Add(cond);
                        cond.Connector = childMatch.Groups[1].Value;
                        cond.LeftValue = childMatch.Groups[2].Value;
                        cond.Operator = childMatch.Groups[3].Value;
                        cond.RightValue = childMatch.Groups[4].Value;
                    }
                }
                else
                {
                    Cond cond = new Cond();
                    condGroup.Conds.Add(cond);
                    cond.LeftValue = m.Groups[2].Value;
                    cond.Operator = m.Groups[3].Value;
                    cond.RightValue = m.Groups[4].Value;
                }
            }
            return condGroups;
        }
    }
}
