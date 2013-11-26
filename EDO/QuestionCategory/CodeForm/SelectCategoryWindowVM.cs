using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using EDO.QuestionCategory.CategoryForm;
using EDO.Main;

namespace EDO.QuestionCategory.CodeForm
{
    public class SelectCategoryWindowVM :FormVM
    {

        public SelectCategoryWindowVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            categories = new ObservableCollection<CategoryVM>();
            selectedCategories = new ObservableCollection<CategoryVM>();
            Filter("");
        }

        private ObservableCollection<CategoryVM> categories;
        public ObservableCollection<CategoryVM> Categories { get { return categories; } }

        private ObservableCollection<CategoryVM> selectedCategories;
        public ObservableCollection<CategoryVM> SelectedCategories { get { return selectedCategories; } }

        public void Filter(string text)
        {
            StudyUnitVM studyUnit = (StudyUnitVM)this.Parent;
            string lowerText = text.ToLower();
            categories.Clear();
            foreach (CategoryVM category in studyUnit.CategoryForm.AllCategories)
            {
                if (string.IsNullOrEmpty(lowerText) || category.Title.ToLower().Contains(lowerText))
                {
                    categories.Add(category);
                }
            }
        }
    }
}
