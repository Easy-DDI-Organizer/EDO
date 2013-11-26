using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using EDO.Core.ViewModel;
using System.Diagnostics;

namespace EDO.StudyCategory.CoverageForm
{
    public class CheckBoxTemplateSelector :DataTemplateSelector
    {
        public CheckBoxTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null && item is CheckOptionVM)
            {
                CheckOptionVM option = item as CheckOptionVM;
                Window window = Application.Current.MainWindow;
                if (option.HasMemo)
                {
                    return MemoTemplate;
                }
                return NormalTemplate;
            }

            return null;
        }

        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate MemoTemplate { get; set; }
    }
}
