using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class BookRelation :ICloneable
    {
        public static BookRelation CreateAbstract()
        {
            return new BookRelation() { BookRelationType = BookRelationType.Abstract };
        }

        public static BookRelation CreateConcept(string conceptId)
        {
            return new BookRelation() { BookRelationType = BookRelationType.Concept, MetadataId = conceptId };
        }

        public static BookRelation CreateQuestion(string questionId)
        {
            return new BookRelation() { BookRelationType = BookRelationType.Question, MetadataId = questionId };
        }

        public static BookRelation CreateVariable(string variableId)
        {
            return new BookRelation() { BookRelationType = BookRelationType.Variable, MetadataId = variableId };
        }

        public BookRelation()
        {
        }

        public BookRelationType BookRelationType { get; set; }

        public bool IsBookRelationTypeAbstract {get {return BookRelationType == BookRelationType.Abstract; }}
        public bool IsBookRelationTypeConcept { get { return BookRelationType == BookRelationType.Concept; } }
        public bool IsBookRelationTypeQuestion { get { return BookRelationType == BookRelationType.Question; } }
        public bool IsBookRelationTypeVariable { get { return BookRelationType == BookRelationType.Variable; } }

        public string MetadataId { get; set; }

        public object Clone()
        {
            return DeepCopy(false);
        }


        public BookRelation DeepCopy(Boolean keepId)
        {
            BookRelation newRelation = (BookRelation)MemberwiseClone();
            if (keepId)
            {
//                newRelation.Id = Id;
            }
            return newRelation;
        }

        public bool EqualsValue(BookRelation other)
        {
            bool result = false;
            if (BookRelationType == BookRelationType.Abstract)
            {
                result = BookRelationType == other.BookRelationType;
            }
            else
            {
                result = BookRelationType == other.BookRelationType && MetadataId == other.MetadataId;
            }
            return result;
        }
    }
}
