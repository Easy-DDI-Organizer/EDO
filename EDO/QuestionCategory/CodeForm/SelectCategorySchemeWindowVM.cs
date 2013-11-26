using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using EDO.Main;
using EDO.QuestionCategory.CategoryForm;

namespace EDO.QuestionCategory.CodeForm
{
    public class SelectCategorySchemeWindowVM :FormVM
    {
        public SelectCategorySchemeWindowVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            categorySchemes = new ObservableCollection<CategorySchemeVM>();
            selectedCategorySchemes = new ObservableCollection<CategorySchemeVM>();
            Filter("");
        }

        private ObservableCollection<CategorySchemeVM> categorySchemes;
        public ObservableCollection<CategorySchemeVM> CategorySchemes { get { return categorySchemes; } }

        private ObservableCollection<CategorySchemeVM> selectedCategorySchemes;
        public ObservableCollection<CategorySchemeVM> SelectedCategorySchemes { get { return selectedCategorySchemes; } }

        public void Filter(string text)
        {
            StudyUnitVM studyUnit = (StudyUnitVM)Parent;

            string lowerText = text.ToLower();
            categorySchemes.Clear();
            foreach (CategorySchemeVM categoryScheme in studyUnit.CategoryForm.CategorySchemes)
            {
                if (string.IsNullOrEmpty(lowerText) || categoryScheme.Title.ToLower().Contains(lowerText))
                {
                    categorySchemes.Add(categoryScheme);
                }
            }
        }
    }
}
