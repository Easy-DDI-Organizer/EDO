using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Main;
using EDO.Core.Model;
using System.Collections.Specialized;
using EDO.Core.View;
using System.Windows;
using System.Windows.Input;
using EDO.Core.Util;
using EDO.Properties;

namespace EDO.QuestionCategory.CategoryForm
{
    public class CategoryFormVM :FormVM
    {
        public CategoryFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            categorySchemes = new ObservableCollection<CategorySchemeVM>();
            foreach (CategoryScheme categorySchemeModel in studyUnit.CategorySchemeModels)
            {
                CategorySchemeVM categoryScheme = new CategorySchemeVM(categorySchemeModel) {
                    Parent = this
                };
                categorySchemes.Add(categoryScheme);
            }
            modelSyncher = new ModelSyncher<CategorySchemeVM, CategoryScheme>(this, categorySchemes, studyUnit.CategorySchemeModels);
        }

        private CategoryFormView Window { get { return (CategoryFormView)View; } }

        private ObservableCollection<CategorySchemeVM> categorySchemes;
        private ModelSyncher<CategorySchemeVM, CategoryScheme> modelSyncher;

        public ObservableCollection<CategorySchemeVM> CategorySchemes { get { return categorySchemes; } }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedCategoryScheme = EDOUtils.Find(categorySchemes, state.State1);
            }
            if (SelectedCategoryScheme == null)
            {
                SelectedCategoryScheme = EDOUtils.GetFirst(categorySchemes);
            }
        }

        public override VMState SaveState()
        {
            if (SelectedCategoryScheme == null)
            {
                return null;
            }
            return new VMState(SelectedCategoryScheme.Id);
        }

        private CategorySchemeVM selectedCategoryScheme;
        public CategorySchemeVM SelectedCategoryScheme
        {
            get
            {
                return selectedCategoryScheme;
            }
            set
            {
                if (selectedCategoryScheme != value)
                {
                    Window.FinalizeDataGrid();
                    selectedCategoryScheme = value;
                    NotifyPropertyChanged("SelectedCategoryScheme");
                }
            }
        }

        public void AddCategoryScheme()
        {
            InputDialog dlg = new InputDialog();
            dlg.Title = Resources.InputCategorySchemeName; //選択肢群の名前を入力してください
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                CategoryScheme categorySchemeModel = new CategoryScheme() { Title = dlg.textBox.Text };
                CategorySchemeVM categoryScheme = new CategorySchemeVM(categorySchemeModel);
                categorySchemes.Add(categoryScheme);
                if (SelectedCategoryScheme == null)
                {
                    SelectedCategoryScheme = categoryScheme;
                }
                Memorize();
            }
        }


        private ICommand removeCategorySchemeCommand;
        public ICommand RemoveCategorySchemeCommand
        {
            get
            {
                if (removeCategorySchemeCommand == null)
                {
                    removeCategorySchemeCommand = new RelayCommand(param => this.RemoveCategoryScheme(), param => this.CanRemoveCategoryScheme);
                }
                return removeCategorySchemeCommand;
            }
        }

        public bool CanRemoveCategoryScheme
        {
            get
            {
                if (SelectedCategoryScheme == null)
                {
                    return false;
                }
                return true;
            }
        }

        private bool CheckRemovable(CategoryVM category)
        {
            ObservableCollection<CategoryVM> categories = new ObservableCollection<CategoryVM>();
            categories.Add(category);
            return CheckRemovable(categories);
        }

        private bool CheckRemovable(Collection<CategoryVM> categories)
        {
            List<CategoryVM> usedCategories = new List<CategoryVM>();
            foreach (CategoryVM category in categories)
            {
                if (StudyUnit.ContainsCodeByCategoryId(category.Id))
                {
                    usedCategories.Add(category);
                }
            }
            if (usedCategories.Count > 0)
            {
                string msg = EDOUtils.CannotDeleteError<CategoryVM>(Resources.Code, usedCategories, param => param.Title);
                MessageBox.Show(msg);
                return false;
            }
            return true;
        }

        public void RemoveCategoryScheme()
        {
            if (!CheckRemovable(SelectedCategoryScheme.Categories))
            {
                return;
            }
            categorySchemes.Remove(SelectedCategoryScheme);
            SelectedCategoryScheme = null;
        }

        private ICommand removeCategoryCommand;
        public ICommand RemoveCategoryCommand
        {
            get
            {
                if (removeCategoryCommand == null)
                {
                    removeCategoryCommand = new RelayCommand(param => this.RemoveCategory(), param => this.CanRemoveCategory);
                }
                return removeCategoryCommand;
            }
        }

        public bool CanRemoveCategory
        {
            get
            {
                if (SelectedCategoryScheme == null)
                {
                    return false;
                }
                if (SelectedCategoryScheme.SelectedCategory == null)
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveCategory()
        {
            CategoryVM category = SelectedCategoryScheme.SelectedCategory;
            if (!CheckRemovable(category))
            {
                return;
            }
            SelectedCategoryScheme.Categories.Remove(SelectedCategoryScheme.SelectedCategory);
            SelectedCategoryScheme.SelectedCategoryItem = null;
        }

        public ObservableCollection<CategoryVM> AllCategories
        {
            get
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
        }

        public CategorySchemeVM FindCategoryScheme(string categorySchemeId)
        {
            foreach (CategorySchemeVM categoryScheme in categorySchemes)
            {
                if (categoryScheme.Id == categorySchemeId)
                {
                    return categoryScheme;
                }
            }
            return null;
        }

        public CategorySchemeVM FindCategorySchemeByResponseId(string responseId)
        {
            foreach (CategorySchemeVM categoryScheme in categorySchemes)
            {
                if (categoryScheme.ResponseId == responseId)
                {
                    return categoryScheme;
                }
            }
            return null;
        }
        
        public CategoryVM FindCategory(string categoryId)
        {
            foreach (CategorySchemeVM categoryScheme in categorySchemes)
            {
                CategoryVM category = categoryScheme.FindCategory(categoryId);
                if (category != null)
                {
                    return category;
                }
            }
            return null;
        }

        public bool Contains(string categoryId)
        {
            return FindCategory(categoryId) != null;
        }
    }
}
