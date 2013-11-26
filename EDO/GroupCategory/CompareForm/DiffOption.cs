using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;

namespace EDO.GroupCategory.CompareForm
{
    public class DiffOption : Option
    {
        public DiffOption(Option option)
            : this(option.Code, option.Label, null)
        {
        }

        public DiffOption(string code, string label) :this(code, label, null)
        {
        }

        public DiffOption(string code, string label, string targetTitle) :base(code, label)
        {
            this.TargetTitle = targetTitle;
        }

        public string TargetTitle { get; set; } //コンボで表示する比較対象のアイテムのタイトル

        public string DetailLabel
        {
            get
            {
                if (string.IsNullOrEmpty(TargetTitle)) 
                {
                    return Label;
                }
                return Label + " " + TargetTitle;
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Code, Label, TargetTitle);
        }

        public bool IsMatchOrNotMatch
        {
            get
            {
                return Options.IsMatch(Code) || Options.IsNotMatch(Code);
            }
        }

        public bool IsPartialMatch
        {
            get
            {
                //Diffオプションのコードとして部分一致の場合、コードにはRowIdが保存されている。
                //そこで完全一致または部分一致ではないという判断を使う。
                return !IsMatchOrNotMatch;
            }
        }
    }
}
