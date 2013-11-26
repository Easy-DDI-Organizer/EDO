using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class ControlConstructScheme :ICloneable, IIDPropertiesProvider
    {
        public static void ChangeQuestionId(List<ControlConstructScheme> controlConstructSchemes, string oldId, string newId)
        {
            foreach (ControlConstructScheme controlConstructScheme in controlConstructSchemes)
            {
                QuestionConstruct.ChangeQuestionId(controlConstructScheme.QuestionConstructs, oldId, newId);
            }
        }

        public static void ChangeControlConstructId(List<ControlConstructScheme> controlConstructSchemes, string oldId, string newId)
        {
            foreach (ControlConstructScheme controlConstructScheme in controlConstructSchemes)
            {
                Sequence sequence = controlConstructScheme.Sequence;
                for (int i = 0; i < sequence.ControlConstructIds.Count; i++)
                {
                    if (sequence.ControlConstructIds[i] == oldId)
                    {
                        sequence.ControlConstructIds[i] = newId;
                    }
                }
            }
        }

        public static List<Sequence> GetSequences(List<ControlConstructScheme> controlConstructSchemes)
        {
            List<Sequence> sequences = new List<Sequence>();
            foreach (ControlConstructScheme controlConstructScheme in controlConstructSchemes)
            {
                sequences.Add(controlConstructScheme.Sequence);
            }
            return sequences;
        }

        public static List<IConstruct> GetConstructs(List<ControlConstructScheme> controlConstructSchemes)
        {
            List<IConstruct> constructs = new List<IConstruct>();
            foreach (ControlConstructScheme controlConstructScheme in controlConstructSchemes)
            {
                constructs.AddRange(controlConstructScheme.QuestionConstructs);
                constructs.AddRange(controlConstructScheme.Statements);
                constructs.AddRange(controlConstructScheme.IfThenElses);
            }
            return constructs;
        }

        public  string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public const string QUESTION_NO_PREFIX = "Q";
        public const string QUESTION_GROUP_NO_PREFIX = "QG";
        public const string STATEMENT_NO_PREFIX = "S";
        public const string IFTHENELSE_NO = "-";

        public ControlConstructScheme()
        {
            Id = IDUtils.NewGuid();
            QuestionConstructs = new List<QuestionConstruct>();
            QuestionGroupConstructs = new List<QuestionGroupConstruct>();
            Statements = new List<Statement>();
            IfThenElses = new List<IfThenElse>();
            Sequence = new Sequence();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public List<QuestionConstruct> QuestionConstructs { get; set; }
        public List<QuestionGroupConstruct> QuestionGroupConstructs { get; set; }
        public List<Statement> Statements { get; set; }
        public List<IfThenElse> IfThenElses { get; set; }
        public Sequence Sequence { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public QuestionConstruct FindQuestionConstruct(string id)
        {
            return EDOUtils.Find(QuestionConstructs, id);
        }

        public QuestionGroupConstruct FindQuestionGroupConstruct(string id)
        {
            return EDOUtils.Find(QuestionGroupConstructs, id);
        }

        public Statement FindStatement(string id)
        {
            return EDOUtils.Find(Statements, id);
        }

        public IfThenElse FindIfThenElse(string id)
        {
            return EDOUtils.Find(IfThenElses, id);
        }

        public bool HasConstruct
        {
            get
            {
                int count = QuestionConstructs.Count + QuestionGroupConstructs.Count + Statements.Count + IfThenElses.Count;
                return (count > 0);
            }
        }
    }
}
