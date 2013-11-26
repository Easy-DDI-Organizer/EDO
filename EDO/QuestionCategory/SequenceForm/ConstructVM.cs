using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.QuestionCategory.QuestionForm;
using EDO.Core.Model;

namespace EDO.QuestionCategory.SequenceForm
{
    public abstract class ConstructVM :BaseVM, IStringIDProvider
    {
        public static ConstructVM FindByQuestionId(ICollection<ConstructVM> constructs, string id)
        {
            foreach (ConstructVM construct in constructs)
            {
                if (construct.QuestionId == id)
                {
                    return construct;
                }
            }
            return null;
        }

        public static ConstructVM FindByQuestionGroupId(ICollection<ConstructVM> constructs, string id)
        {
            foreach (ConstructVM construct in constructs)
            {
                if (construct.QuestionGroupId == id)
                {
                    return construct;
                }
            }
            return null;
        }

        public static ConstructVM FindByNo(ICollection<ConstructVM> constructs, string no)
        {
            foreach (ConstructVM construct in constructs)
            {
                if (construct.No == no)
                {
                    return construct;
                }
            }
            return null;
        }

        public ConstructVM(IConstruct construct)
        {
            this.construct = construct;
        }

        private IConstruct construct;

        public abstract string Id { get; }

        public override object Model
        {
            get
            {
                return construct;
            }
       }

        public virtual string No
        {
            get
            {
                return construct.No;
            }
            set
            {
                if (construct.No != value)
                {
                    construct.No = value;
                    NotifyPropertyChanged("No");
                }
            }
        }

        public abstract string Title
        {
            get;
        }

        public abstract string TypeString
        {
            get;
        }

        public virtual string QuestionId { get { return null; } }

        public virtual string QuestionGroupId { get { return null; } }
   
    }
}
