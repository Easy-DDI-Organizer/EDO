using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDO.QuestionCategory.CategoryForm
{
    public class CategoryVM : BaseVM, IEditableObject
    {
        public static CategoryVM Find(ICollection<CategoryVM> categories, string categoryId)
        {
            foreach (CategoryVM category in categories)
            {
                if (category.Category.Id == categoryId)
                {
                    return category;
                }
            }
            return null;
        }

        private Category category;

        private Category bakCategory;

        public CategoryVM()
            : this(new Category())
        {
        }

        public CategoryVM(Category category)
        {
            this.category = category;
        }

        public Category Category { get { return category; } }

        public bool IsValid
        {
            get
            {
                return IsNotEmpty(Title);
            }
        }

        public string Id { get { return category.Id; } }

        public string CategorySchemeId
        {
            get
            {
                return category.CategorySchemeId;
            }
            set
            {
                category.CategorySchemeId = value;
            }
        }

        public override object Model { get { return category; } }

        public string Title
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
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Memo
        {
            get
            {
                return category.Memo;
            }
            set
            {
                if (category.Memo != value)
                {
                    category.Memo = value;
                    NotifyPropertyChanged("Memo");
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

            bakCategory = category.Clone() as Category;
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            this.Title = bakCategory.Title;
            this.Memo = bakCategory.Memo;
        }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            bakCategory = null;
            Memorize();
        }

        protected override void PrepareValidation()
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = EMPTY_VALUE;
            }
        }

        #endregion

    }
}
