using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Question :ICloneable, IIDPropertiesProvider
    {
        public static void ChangeConceptId(List<Question> questions, string oldId, string newId)
        {
            foreach (Question question in questions)
            {
                if (question.ConceptId == oldId)
                {
                    question.ConceptId = newId;
                }
            }
        }

        public static List<Response> GetResponses(List<Question> questions)
        {
            List<Response> responses = new List<Response>();
            foreach (Question question in questions)
            {
                responses.Add(question.Response);
            }
            return responses;
        }

        public static Question Find(List<Question> questions, string questionId)
        {
            foreach (Question question in questions)
            {
                if (question.Id == questionId)
                {
                    return question;
                }
            }
            return null;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }
       
        public Question()
        {
            Id = IDUtils.NewGuid();
            Response = new Response();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        private Response response;
        public Response Response { 
            get
            {
                return response;
            }
            set
            {
                if (response != value)
                {
                    response = value;
                }
            }
        }

        public string ConceptId { get; set; }

        public bool IsCreatedVariable { get; set; } //このQuestionからVariableを生成したかどうかのフラグ
        public bool IsCreatedConstruct { get; set; } //このQuestionからConstruct(質問の順番で使用)を生成したかどうかのフラグ

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

    }
}
