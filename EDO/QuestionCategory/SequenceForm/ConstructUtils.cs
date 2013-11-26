using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.QuestionCategory.SequenceForm
{
    public static class ConstructUtils
    {
        private static int ConstructCount<T>(ICollection<ConstructVM> constructs)
        {
            int count = 0;
            foreach (ConstructVM construct in constructs)
            {
                if (construct is T)
                {
                    count++;
                }
            }
            return count;
        }


        public static int QuestionConstructCount(ICollection<ConstructVM> constructs)
        {
            return ConstructCount<QuestionConstructVM>(constructs);
        }

        public static int QuestionGroupConstructCount(ICollection<ConstructVM> constructs)
        {
            return ConstructCount<QuestionGroupConstructVM>(constructs);
        }

        public static int StatementCount(ICollection<ConstructVM> constructs)
        {
            return ConstructCount<StatementVM>(constructs);
        }

    }
}
