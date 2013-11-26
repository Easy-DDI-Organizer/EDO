using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Properties;

namespace EDO.DataCategory.BookForm
{
    public class BookRelationItem
    {
        public static List<BookRelationItem> All
        {
            get
            {
                List<BookRelationItem> items = new List<BookRelationItem>();
                items.Add(new BookRelationItem(BookRelationType.Abstract, Resources.StudyAbstract));
                items.Add(new BookRelationItem(BookRelationType.Concept, Resources.VariableConcept));
                items.Add(new BookRelationItem(BookRelationType.Question, Resources.ConfigQuestion));
                items.Add(new BookRelationItem(BookRelationType.Variable, Resources.VariableInfo));
                return items;
            }
        }

        public static string GetLabel(BookRelationType type)
        {
            List<BookRelationItem> items = All;
            foreach (BookRelationItem item in items)
            {
                if (item.Type == type)
                {
                    return item.Label;
                }
            }
            return null;
        }

        public BookRelationItem(BookRelationType type, string label)
        {
            Type = type;
            Label = label;
        }

        public BookRelationType Type { get; set; }
        public string Label { get; set; }
    }
}
