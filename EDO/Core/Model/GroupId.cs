using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class GroupId
    {
        public static bool ContainsAll(List<GroupId> groupIds, List<GroupId> targetGroupIds)
        {
            foreach (GroupId groupId in targetGroupIds)
            {
                if (!groupIds.Contains(groupId))
                {
                    return false;
                }
            }
            return true;
        }

        public GroupId()
            : this(IDUtils.NewGuid(), IDUtils.NewGuid())
        {
        }

        public GroupId(string studyUnitId, string id)
        {
            StudyUnitId = studyUnitId;
            Id = id;
        }

        public string StudyUnitId { get; set; }
        public string Id { get; set; }

        public override bool Equals(object obj)
        {
            GroupId other = obj as GroupId;
            if (other == null)
            {
                return false;
            }
            return StudyUnitId == other.StudyUnitId && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return StudyUnitId.GetHashCode() ^ Id.GetHashCode();
        }
    }
}
