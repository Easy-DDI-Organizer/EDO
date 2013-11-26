using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.ViewModel;
using System.ComponentModel;
using EDO.QuestionCategory.CategoryForm;
using System.ComponentModel.DataAnnotations;
using EDO.Main;

namespace EDO.QuestionCategory.CodeForm
{
    public class CodeVM :BaseVM, IEditableObject
    {
        public static string JoinLabels(ICollection<CodeVM> codes)
        {
            StringBuilder buf = new StringBuilder();
            int i = 0;
            foreach (CodeVM code in codes)
            {
                buf.Append(code.Label);
                if (i < codes.Count - 1)
                {
                    buf.Append(",");
                }
                i++;
            }
            return buf.ToString();
        }

        private Code code;
        private Code bakCode;

        private CategoryVM category;
        private string bakLabel; //バックアップ用。VMのクローンは面倒なので

        public CodeVM()
            : this(new Code(), new CategoryVM())
        {
        }

        public CodeVM(Code code, CategoryVM category)
        {
            this.code = code;
            this.Category = category;
        }

        public bool IsValid
        {
            get
            {
                return Category.IsValid && IsNotEmpty(Value);
            }
        }

        public Code Code { get { return code; } }

        public string CodeId { get { return code.Id; } }

        public string CodeSchemeId
        {
            get
            {
                return code.CodeSchemeId;
            }
            set
            {
                code.CodeSchemeId = value;
            }
        }

        public override object Model { get { return code; } }

        public CategoryVM Category { 
            get 
            { 
                return category; 
            }
            set
            {
                category = value;
                if (category != null)
                {
                    code.CategoryId = category.Id;
                }
            }
        }

        public string CategoryId { get { return Category.Id; } }

        public string CategorySchemeId { get { return Category.CategorySchemeId; } }

        public string Value
        {
            get
            {
                return code.Value;
            }
            set
            {
                if (code.Value != value)
                {
                    code.Value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }

        public string Label
        {
            get
            {
                return category.Title;
            }
            set
            {
                if (category.Title != value)
                {
                    category.Title = value;
                    NotifyPropertyChanged("Label");
                }
            }
        }
        public bool IsMissingValue
        {
            get
            {
                return category.IsMissingValue;
            }
            set
            {
                if (category.IsMissingValue != value)
                {
                    category.IsMissingValue = value;
                    NotifyPropertyChanged("IsMissingValue");
                }
            }
        }

        public bool IsExcludeValue
        {
            get
            {
                return category.IsExcludeValue;
            }
            set
            {
                if (category.IsExcludeValue != value)
                {
                    category.IsExcludeValue = value;
                    NotifyPropertyChanged("IsExcludeValue");
                }
            }
        }


        #region IEditableObject メンバー

        public bool InEdit { get { return inEdit; } }

        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit)
            {
                return;
            }
            inEdit = true;
            bakCode = code.Clone() as Code;
            bakLabel = this.Label;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            this.Value = bakCode.Value;
            this.Label = bakLabel;        
        }

        public override StudyUnitVM StudyUnit
        {
            get
            {
                StudyUnitVM studyUnit = base.StudyUnit;
                if (studyUnit == null)
                {
                    CodeSchemeVM codeScheme = (CodeSchemeVM)Parent;
                    studyUnit = codeScheme.StudyUnit;
                }
                return studyUnit;
            }
        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            bakCode = null;
            bakLabel = null;

            StudyUnit.CompleteResponse();
//            if (EndEditAction != null)
//            {
//                EndEditAction(this);
//            }
            Memorize();
        }

        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Value))
            {
                Value = EMPTY_VALUE;
            }
            if (string.IsNullOrEmpty(Label))
            {
                Label = EMPTY_VALUE;
            }
        }
        #endregion
    }
}
