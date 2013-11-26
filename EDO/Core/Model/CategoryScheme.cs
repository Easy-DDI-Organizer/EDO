using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class CategoryScheme :ICloneable, IIDPropertiesProvider
    {
        public static void ChangeCategorySchemeId(List<CategoryScheme> categorySchemes, string oldId, string newId)
        {
            foreach (CategoryScheme categoryScheme in categorySchemes)
            {
                Category.ChangeCategorySchemeId(categoryScheme.Categories, oldId, newId);
            }
        }

        public static List<Category> GetCategories(List<CategoryScheme> categorySchemes)
        {
            List<Category> categories = new List<Category>();
            foreach (CategoryScheme categoryScheme in categorySchemes)
            {
                categories.AddRange(categoryScheme.Categories);
            }
            return categories;
        }

        public static CategoryScheme Find(List<CategoryScheme> categorySchemes, string categorySchemeId)
        {
            foreach (CategoryScheme categoryScheme in categorySchemes)
            {
                if (categoryScheme.Id == categorySchemeId)
                {
                    return categoryScheme;
                }
            }
            return null;
        }

        public static CategoryScheme FindByCategoryId(List<CategoryScheme> CategorySchemes, string categoryId)
        {
            foreach (CategoryScheme categoryScheme in CategorySchemes)
            {
                if (categoryScheme.Contains(categoryId))
                {
                    return categoryScheme;
                }
            }
            return null;
        }

        public bool Contains(string categoryId)
        {
            return Category.Find(Categories, categoryId) != null;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public CategoryScheme()
        {
            Id = IDUtils.NewGuid();
            Categories = new List<Category>();
        }
        public string ResponseId { get; set; } //設問設計画面で作られた場合はセットする
        public string Id { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public List<Category> Categories { get; set; }

        public Category FindCategory(string categoryId)
        {
            foreach (Category category in Categories)
            {
                if (category.Id == category.Id)
                {
                    return category;
                }
            }
            return null;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
