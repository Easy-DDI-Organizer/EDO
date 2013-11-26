using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class CompareRow
    {
        public CompareRow()
        {
            Id = IDUtils.NewGuid();
            RowGroupIds = new List<GroupId>();
            Cells = new List<CompareCell>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public List<GroupId> RowGroupIds { get; set; }
        public List<CompareCell> Cells { get; set; }

        public bool ContainsAll(List<GroupId> rowGroupIds)
        {
            return GroupId.ContainsAll(RowGroupIds, rowGroupIds);
        }

        public bool IsExistStudyUnit(string studyUnitId)
        {
            //この行に属する変数・概念などが指定されたStudyUnitに属するものかどうかを判定。
            foreach (GroupId groupId in RowGroupIds)
            {
                if (groupId.StudyUnitId == studyUnitId)
                {
                    return true;
                }
            }
            return false;
        }

        public CompareCell FindCell(string studyUnitId)
        {
            foreach (CompareCell cell in Cells)
            {
                if (cell.ColumnStudyUnitId == studyUnitId)
                {
                    return cell;
                }
            }
            return null;
        }

        public List<GroupId> RelatedGroupIds(CompareCell cell)
        {
            return RelatedGroupIds(cell.ColumnStudyUnitId);
        }

        public List<GroupId> RelatedGroupIds(string studyUnitId)
        {
            List<GroupId> groupIds = new List<GroupId>();
            foreach (GroupId groupId in RowGroupIds)
            {
                if (groupId.StudyUnitId == studyUnitId)
                {
                    groupIds.Add(groupId);
                }
            }
            return groupIds;
        }

        public List<CompareCell> ValidCells()
        {
            List<CompareCell> validCells = new List<CompareCell>();
            foreach (CompareCell cell in Cells)
            {
                if (IsExistStudyUnit(cell.ColumnStudyUnitId))
                {
                    validCells.Add(cell);
                }
            }
            return validCells;
        }

        public List<CompareCell> MatchValidCells()
        {
            List<CompareCell> matchCells = new List<CompareCell>();
            List<CompareCell> validCells = ValidCells();
            foreach (CompareCell cell in validCells)
            {
                if (cell.IsMatch)
                {
                    matchCells.Add(cell);
                }
            }
            return matchCells;
        }

        public List<CompareCell> PartialMatchValidCells()
        {
            List<CompareCell> partialMatchCells = new List<CompareCell>();
            List<CompareCell> validCells = ValidCells();
            foreach (CompareCell cell in validCells)
            {
                if (cell.IsPartialMatch)
                {
                    partialMatchCells.Add(cell);
                }
            }
            return partialMatchCells;
        }

        public List<GroupId> GetMatchIds()
        {
            List<GroupId> matchIds = new List<GroupId>();
            List<CompareCell> matchCells = MatchValidCells();
            if (matchCells.Count > 1)
            {
                //各行で一致したとみなす最初のセルのIDをかえす(現状では各行で実質一つしか一致は表現できないはず
                foreach (CompareCell cell in matchCells)
                {
                    List<GroupId> ids = RelatedGroupIds(cell);
                    matchIds.AddRange(ids);
                }
            }
            return matchIds;
        }
    }
}
