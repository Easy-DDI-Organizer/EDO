using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Variable :ICloneable, IIDPropertiesProvider
    {
        public static Variable Find(List<Variable> variables, string variableId)
        {
            foreach (Variable variable in variables)
            {
                if (variable.Id == variableId)
                {
                    return variable;
                }
            }
            return null;
        }

        public static List<Variable> FindByConceptId(List<Variable> variables, string conceptId)
        {
            List<Variable> results = new List<Variable>();
            foreach (Variable variable in variables)
            {
                if (variable.ConceptId == conceptId)
                {
                    results.Add(variable);
                }
            }
            return results;
        }

        public static void ChangeConceptId(List<Variable> variables, string oldId, string newId)
        {
            foreach (Variable variable in variables)
            {
                if (variable.ConceptId == oldId)
                {
                    variable.ConceptId = newId;
                }
            }
        }

        public static void ChangeQuestionId(List<Variable> variables, string oldId, string newId)
        {
            foreach (Variable variable in variables)
            {
                if (variable.QuestionId == oldId)
                {
                    variable.QuestionId = newId;
                }
            }
        }

        public static void ChangeUniverseId(List<Variable> variables, string oldId, string newId)
        {
            foreach (Variable variable in variables)
            {
                if (variable.UniverseId == oldId)
                {
                    variable.UniverseId = newId;
                }
            }
        }

        public static List<Response> GetResponses(List<Variable> variables)
        {
            List<Response> responses = new List<Response>();
            foreach (Variable variable in variables)
            {
                responses.Add(variable.Response);
            }
            return responses;
        }

        public static IEnumerable<string> GetIds(List<Variable> variables)
        {
            return variables.Select(p => p.Id);
        }

        public  string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public Variable()
        {
            this.Id = IDUtils.NewGuid();
            this.Response = new Response();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Label { get; set; }
        public string ConceptId { get; set; }
        public string QuestionId { get; set; }
        public string UniverseId { get; set; }
        public Response Response {get; set; }
        public bool IsCreatedDataSet { get; set; } //この変数からデータセットを生成したかどうか

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion


    }
}
