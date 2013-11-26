using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;

namespace EDO.Core.IO
{
    // DDI書き出し用の一時クラス
    public class CompareItem
    {
        public static CompareTable CreateCompareTable(List<CompareItem> compareItems)
        {
            return null;
        }

        private static string ToId(GroupId sourceId, GroupId targetId)
        {
            return sourceId.Id + "_" + targetId.Id;
        }

        private static string ToItemId(GroupId id)
        {
            return ITEM_PREFIX + id.Id;
        }

        public static string ToOrigId(string id)
        {
            string result = id;
            if (id.StartsWith(ITEM_PREFIX)) {
                result = id.Substring(ITEM_PREFIX.Length);
            }
            return result;
        }

        private const string ITEM_PREFIX = "Item_";

        private const string MATCH_WEIGHT = "1";

        private const string PARTIAL_MATCH_WEIGHT = "0.5";

        public static CompareItem CreateMatch(GroupId sourceId, GroupId targetId, string memo)
        {
            return new CompareItem(sourceId, targetId, memo, MATCH_WEIGHT);
        }

        public static CompareItem CreatePartialMatch(GroupId sourceId, GroupId targetId, string memo)
        {
            return new CompareItem(sourceId, targetId, memo, PARTIAL_MATCH_WEIGHT);
        }

        public CompareItem(GroupId sourceId, GroupId targetId, string memo, string weight)
        {
            this.SourceId = sourceId;
            this.TargetId = targetId;
            Memo = memo;
            Weight = weight;
        }

        public string IdPrefix { get; set; }
        public string Id { get { return ToId(SourceId, TargetId); } }
        public string IdWithPrefix(string prefix) { return prefix + "_" + ToId(SourceId, TargetId); }
        public GroupId SourceId { get; set; }
        public GroupId TargetId { get; set; }
        public string Memo { get; set; }
        public string Weight { get; set; }

        //SourceItem/TargetItemに格納するため先頭にItem_を追加
        public string SourceItemId { get { return ToItemId(SourceId); } }
        public string TargetItemId { get { return ToItemId(TargetId); } }

        public GroupId ParentSourceId { get; set; }
        public GroupId ParentTargetId { get; set; }
        public string ParentIdPrefix { get; set; }
        public string ParentId { get { return ToId(ParentSourceId, ParentTargetId); } }

        public string SourceTitle { get; set;  } //読込時にだけ使用するプロパティ
        public string TargetTitle { get; set; }

        public bool IsMatch
        {
            get
            {
                return MATCH_WEIGHT == Weight;
            }
        }

        public bool IsPartialPatch
        {
            get
            {
                return PARTIAL_MATCH_WEIGHT == Weight;
            }
        }
    }
}
