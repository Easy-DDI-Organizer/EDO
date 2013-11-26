using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EDO.Core.Model;

namespace EDO.Core.ViewModel
{
    public class KeywordVM :BaseVM, IEditableObject
    {
        private Keyword keyword;
        private Keyword bakKeyword;
    
        public KeywordVM() :this(new Keyword())
        {
        }

        public KeywordVM(Keyword keyword)
        {
            this.keyword = keyword;
        }

        public Keyword Keyword { get { return keyword; } }

        public override object Model
        {
            get
            {
                return keyword;
            }
        }

        public string Content
        {
            get
            {
                return keyword.Content;
            }
            set
            {
                if (keyword.Content != value)
                {
                    keyword.Content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }


        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Content))
            {
                Content = EMPTY_VALUE;
            }
        }


        #region IEditableObject メンバー

        private bool inEdit = false;
        public bool InEdit { get { return inEdit; } }
        public void BeginEdit()
        {
            if (inEdit)
            {
                return;
            }
            inEdit = true;
            bakKeyword = keyword.Clone() as Keyword;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            this.Content = bakKeyword.Content;

        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            bakKeyword = null;
            Memorize();   
        }

        #endregion
    }
}
