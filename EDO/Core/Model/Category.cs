using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class Category :ICloneable, IIDPropertiesProvider
    {
        public static Category Find(List<Category> categories, string categoryId)
        {
            foreach (Category category in categories)
            {
                if (category.Id == categoryId)
                {
                    return category;
                }
            }
            return null;
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public static void ChangeCategorySchemeId(List<Category> categories, string oldId, string newId)
        {
            foreach (Category category in categories)
            {
                if (category.CategorySchemeId == oldId)
                {
                    category.CategorySchemeId = newId;
                }
            }
        }

        public Category()
        {
            Id = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string Title {get; set; }
        public string Memo { get; set; }
        public string CategorySchemeId { get; set; }
        public bool IsMissingValue { get; set; }
        public bool IsExcludeValue { get; set; }

        #region ICloneable メンバー

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion


    }
}
