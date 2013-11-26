using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Coverage :IIDPropertiesProvider
    {
        public static Coverage CreateDefault()
        {
            Coverage coverage = new Coverage();
            foreach (Option option in Options.CoverageTopics)
            {
                CheckOption checkOption = new CheckOption(option.Code, option.Label);
                if (option.Code == Options.COVERAGE_TOPIC_OTHER_CODE)
                {
                    checkOption.HasMemo = true;
                }
                coverage.Topics.Add(checkOption);
            }
            foreach (Option option in Options.CoverageAreas)
            {
                CheckOption checkOption = new CheckOption(option.Code, option.Label);
                coverage.Areas.Add(checkOption);
            }
            return coverage;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id", "TopicalCoverageId", "TemporalCoverageId", "SpatialCoverageId", "GeographicStructureIdSuffix", "GeographicIdSuffix" };
            }
        }

        public Coverage()
        {
            Id = IDUtils.NewGuid();
            TopicalCoverageId = IDUtils.NewGuid();
            TemporalCoverageId = IDUtils.NewGuid();
            SpatialCoverageId = IDUtils.NewGuid();
            //メモとコードをあわせて保存しないといけないので、List<string>で管理することはできない。
            Topics = new List<CheckOption>();
            Keywords = new List<Keyword>();
            DateRange = new DateRange();
            Areas = new List<CheckOption>();
            GeographicStructureIdSuffix = IDUtils.NewGuid();
            GeographicIdSuffix = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string TopicalCoverageId { get; set; } // for DDI
        public string TemporalCoverageId { get; set; } // for DDI
        public string SpatialCoverageId { get; set; } // for DDI
        public List<CheckOption> Topics { get; set; }
        public List<Keyword> Keywords { get; set; }
        public DateRange DateRange { get; set; }
        public List<CheckOption> Areas { get; set; }
        public string Memo { get; set; }
        public string GeographicStructureIdSuffix { get; set; } // for DDI
        public string GeographicIdSuffix { get; set; } // for DDI
        public string GetGeographicStructureId(CheckOption areaOption) { return IDUtils.ToId(areaOption.Code, GeographicStructureIdSuffix); }
        public string GetGeographicId(CheckOption areaOption) { return IDUtils.ToId(areaOption.Code, GeographicIdSuffix); }
        public List<CheckOption> CheckedAreas
        {
            get
            {
                return CheckOption.GetCheckedOptions(Areas);
            }
        }

        public void CheckTopics(List<string> labels)
        {
            //DDIからの読み込みに使う(チェックボックスにチェックをつける処理)
            List<string> otherLabels = new List<string>();
            foreach (string label in labels)
            {
                CheckOption option = CheckOption.FindByLabel<CheckOption>(Topics, label);
                if (option != null)
                {
                    option.IsChecked = true;
                }
                else
                {
                    otherLabels.Add(label);
                }
            }

            if (otherLabels.Count > 0)
            {
                //一致しないラベルが存在した場合は先頭のみをその他に採用する
                CheckOption option = CheckOption.FindMemoOption(Topics);
                option.IsChecked = true;
                option.Memo = otherLabels.First();
            }
        }

        public void CheckAreas(List<string> labels)
        {
            foreach (string label in labels)
            {
                CheckOption option = CheckOption.FindByLabel<CheckOption>(Areas, label);
                if (option != null)
                {
                    option.IsChecked = true;
                }
            }
        }
    }
}
