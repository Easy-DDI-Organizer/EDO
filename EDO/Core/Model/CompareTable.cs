using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class CompareTable
    {
        public enum CompareTypes
        {
            ConceptScheme,
            Concept,
            Variable
        };

        public CompareTable()
        {
            Id = IDUtils.NewGuid();
            Rows = new List<CompareRow>();
        }

        public string Id { get; set; }
        public CompareTypes CompareType {get; set; }
        public bool IsCompareTypeConceptScheme { get {return CompareType == CompareTypes.ConceptScheme;} }
        public bool IsCompareTypeConcept { get {return CompareType == CompareTypes.Concept;} }
        public bool IsCompareTypeVariable { get { return CompareType == CompareTypes.Variable; } }
        public List<CompareRow> Rows { get; set; }

        public CompareRow FindRowByTitle(string title)
        {
            foreach (CompareRow row in Rows)
            {
                if (row.Title == title)
                {
                    return row;
                }
            }
            return null;
        }


        public List<GroupId> GetMatchIds()
        {
            List<GroupId> ids = new List<GroupId>();
            foreach (CompareRow row in Rows)
            {
                ids.AddRange(row.GetMatchIds());
            }
            return ids;
        }
    }
}
