using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class CheckOption :Option
    {
        public static void Merge(List<CheckOption> fromOptions, List<CheckOption> toOptions)
        {
            foreach (CheckOption toOption in toOptions)
            {
                CheckOption fromOption = Option.FindByLabel<CheckOption>(fromOptions, toOption.Label);
                if (fromOption != null)
                {
                    toOption.IsChecked = fromOption.IsChecked;
                    toOption.Memo = fromOption.Memo;
                }
            }
        }

        public static List<CheckOption> GetCheckedOptions(List<CheckOption> options)
        {
            List<CheckOption> checkedOptions = new List<CheckOption>();
            foreach (CheckOption option in options)
            {
                if (option.IsChecked)
                {
                    checkedOptions.Add(option);
                }
            }
            return checkedOptions;
        }

        public static CheckOption FindMemoOption(List<CheckOption> options)
        {
            foreach (CheckOption option in options)
            {
                if (option.HasMemo)
                {
                    return option;
                }
            }
            return null;
        }

        public CheckOption() :this(null, null)
        {
        }

        public CheckOption(string code, string label)
            : base(code, label)
        {
        }
        public bool IsChecked { get; set; }
        public string Memo { get; set; }
        public bool HasMemo { get; set; }
    }
}
