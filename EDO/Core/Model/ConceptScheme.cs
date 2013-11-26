using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class ConceptScheme :ICloneable, IIDPropertiesProvider
    {

        public static Concept FindConcept(List<ConceptScheme> conceptSchemes, string conceptId)
        {
            List<Concept> concepts = GetConcepts(conceptSchemes);
            foreach (Concept concept in concepts)
            {
                if (concept.Id == conceptId)
                {
                    return concept;
                }
            }
            return null;
        }

        public static ConceptScheme FindConceptScheme(List<ConceptScheme> conceptSchemes, string conceptSchemeId)
        {
            foreach (ConceptScheme conceptScheme in conceptSchemes)
            {
                if (conceptScheme.Id == conceptSchemeId)
                {
                    return conceptScheme;
                }

            }
            return null;
        }

        public static ConceptScheme FindConceptSchemeByConceptId(List<ConceptScheme> conceptSchemes, string conceptId)
        {
            foreach (ConceptScheme conceptScheme in conceptSchemes)
            {
                if (conceptScheme.Contains(conceptId))
                {
                    return conceptScheme;
                }
            }
            return null;
        }

        public static List<Concept> GetConcepts(List<ConceptScheme> conceptSchemes)
        {
            List<Concept> concepts = new List<Concept>();
            foreach (ConceptScheme conceptScheme in conceptSchemes)
            {
                concepts.AddRange(conceptScheme.Concepts);
            }
            return concepts;
        }

        public static List<string> GetTitles(List<ConceptScheme> conceptSchemes)
        {
            List<string> titles = new List<string>();
            foreach (ConceptScheme conceptScheme in conceptSchemes)
            {
                if (!string.IsNullOrEmpty(conceptScheme.Title))
                {
                    titles.Add(conceptScheme.Title);
                }
            }
            return titles;
        }

        public static List<string> GetMemos(List<ConceptScheme> conceptSchemes)
        {
            List<string> memos = new List<string>();
            foreach (ConceptScheme conceptScheme in conceptSchemes)
            {
                if (!string.IsNullOrEmpty(conceptScheme.Memo))
                {
                    memos.Add(conceptScheme.Memo);
                }
            }
            return memos;
        }

        public static List<string> GetConceptTitles(List<ConceptScheme> conceptSchemes)
        {
            return Concept.GetTitles(GetConcepts(conceptSchemes));
        }

        public static List<string> GetConceptContents(List<ConceptScheme> conceptSchemes)
        {
            return Concept.GetContents(GetConcepts(conceptSchemes));
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public ConceptScheme()
        {
            Id = IDUtils.NewGuid();
            Concepts = new List<Concept>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }

        public List<Concept> Concepts { get; set; }
        public bool Contains(string conceptId)
        {
            return Concept.Find(Concepts, conceptId) != null;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
