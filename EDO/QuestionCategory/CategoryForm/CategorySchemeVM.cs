using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using EDO.Core.Util;
using EDO.QuestionCategory.CodeForm;
using EDO.Properties;

namespace EDO.QuestionCategory.CategoryForm
{
    public class CategorySchemeVM :BaseVM, IStringIDProvider
    {
        public static ObservableCollection<CategoryVM> GetAllCategories(ICollection<CategorySchemeVM> categorySchemes)
        {
            ObservableCollection<CategoryVM> categories = new ObservableCollection<CategoryVM>();
            foreach (CategorySchemeVM categoryScheme in categorySchemes)
            {
                foreach (CategoryVM category in categoryScheme.Categories)
                {
                    categories.Add(category);
                }
            }
            return categories;
        }

        public static CategoryVM FindCategory(ICollection<CategoryVM> categories, string categoryId)
        {
            foreach (CategoryVM category in categories)
            {
                if (category.Id == categoryId)
                {
                    return category;
                }
            }
            return null;
        }

        public CategorySchemeVM() :this(new CategoryScheme())
        {
        }

        public CategorySchemeVM(CategoryScheme categoryScheme)
        {
            this.categoryScheme = categoryScheme;
            categories = new ObservableCollection<CategoryVM>();
            foreach (Category categoryModel in categoryScheme.Categories)
            {
                CategoryVM category = new CategoryVM(categoryModel);
                category.Parent = this;
                categories.Add(category);
            }
            modelSyncher = new ModelSyncher<CategoryVM, Category>(this, categories, categoryScheme.Categories);
            modelSyncher.AddActionHandler = (param) => { 
                param.CategorySchemeId = Id; 
            };
        }

        private CategoryScheme categoryScheme;
        private ModelSyncher<CategoryVM, Category> modelSyncher;
        public CategoryScheme CategoryScheme { get { return categoryScheme; } }

        public override object Model {get {return categoryScheme; }}

        public string Id { get { return categoryScheme.Id; } }

        public string ResponseId {
            get
            {
                return categoryScheme.ResponseId;
            }
            set 
            { 
                categoryScheme.ResponseId = value; 
            } 
        }

        private ObservableCollection<CategoryVM> categories;
        public ObservableCollection<CategoryVM> Categories {get {return categories; }}

        public CategoryVM FindCategory(string categoryId)
        {
            return CategoryVM.Find(categories, categoryId);
        }

        public string Title
        {
            get
            {
                return categoryScheme.Title;
            }
            set
            {
                if (categoryScheme.Title != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = Resources.UntitledCategoryScheme; // 無題の選択肢群"
                    }
                    categoryScheme.Title = value;
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public string Memo
        {
            get
            {
                return categoryScheme.Memo;
            }
            set
            {
                if (categoryScheme.Memo != value)
                {
                    categoryScheme.Memo = value;
                    NotifyPropertyChanged("Memo");
                    Memorize();
                }
            }
        }

        private object selectedCategoryItem;
        public object SelectedCategoryItem
        {
            get
            {
                return selectedCategoryItem;
            }
            set
            {
                if (selectedCategoryItem != value)
                {
                    selectedCategoryItem = value;
                    NotifyPropertyChanged("SelectedCategoryItem");
                }
            }
        }

        public CategoryVM SelectedCategory
        {
            get
            {
                return SelectedCategoryItem as CategoryVM;
            }
        }
    }
}
