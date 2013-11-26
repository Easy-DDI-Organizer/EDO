using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Concept :ICloneable, IIDPropertiesProvider
    {
        public static Concept Find(List<Concept> concepts, string conceptId)
        {
            foreach (Concept concept in concepts)
            {
                if (concept.Id == conceptId)
                {
                    return concept;
                }
            }
            return null;
        }

        public static List<Concept> FindAll(List<Concept> concepts, List<string> conceptIds)
        {
            List<Concept> results = new List<Concept>();
            foreach (string conceptId in conceptIds)
            {
                Concept concept = Find(concepts, conceptId);
                if (concept != null)
                {
                    results.Add(concept);
                }
            }
            return results;
        }

        public static List<string> GetTitles(List<Concept> concepts)
        {
            List<string> titles = new List<string>();
            foreach (Concept concept in concepts)
            {
                if (!string.IsNullOrEmpty(concept.Title))
                {
                    titles.Add(concept.Title);
                }
            }
            return titles;
        }

        public static List<string> GetContents(List<Concept> concepts)
        {
            List<string> contents = new List<string>();
            foreach (Concept concept in concepts)
            {
                if (!string.IsNullOrEmpty(concept.Content))
                {
                    contents.Add(concept.Content);
                }
            }
            return contents;
        }

        public static List<string> GetIds(List<Concept> concepts)
        {
            List<string> ids = new List<string>();
            foreach (Concept concept in concepts)
            {
                ids.Add(concept.Id);
            }
            return ids;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public Concept()
        {
            Id = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
