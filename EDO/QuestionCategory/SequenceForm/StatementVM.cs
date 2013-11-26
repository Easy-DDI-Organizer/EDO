using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Properties;

namespace EDO.QuestionCategory.SequenceForm
{
    public class StatementVM :ConstructVM
    {
        public StatementVM(Statement statement) :base(statement)
        {
            this.statement = statement;
        }

        private Statement statement;

        public override string Id { get { return statement.Id; } }

        public override string Title
        {
            get
            {
                return statement.Text;
            }
        }

        public string Text
        {
            get
            {
                return statement.Text;
            }
        }

        public override string TypeString
        {
            get
            {
                return Resources.Sentence;
            }
        }
    }
}
