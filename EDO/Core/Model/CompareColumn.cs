using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class CompareColumn
    {
        public CompareColumn()
        {
            Id = IDUtils.NewGuid();
            GroupIds = new List<GroupId>();
        }

        public string Id { get; set; }
        public List<GroupId> GroupIds { get; set; }
    }
}
