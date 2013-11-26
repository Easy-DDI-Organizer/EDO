using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;

namespace EDO.Core.View
{
    public class Validator
    {
        public static bool Validate(DependencyObject parent)
        {
            //Debug.WriteLine("parent=" + parent.ToString());
            //if (parent is TextBox)
            //{
            //    Debug.WriteLine("parent is TextBox");
            //}
            // Validate all the bindings on the parent        
            bool valid = true;
            var infos = parent.GetType().GetFields(
                            BindingFlags.Public
                            | BindingFlags.FlattenHierarchy
                            | BindingFlags.Instance
                            | BindingFlags.Static).Where(f => f.FieldType == typeof(DependencyProperty));
            foreach (FieldInfo field in infos)
            {
                var dp = (DependencyProperty)field.GetValue(null);
                if (BindingOperations.IsDataBound(parent, dp))
                {
                    Binding binding = BindingOperations.GetBinding(parent, dp);
                    if (binding != null) {
                        if (binding.ValidationRules.Count > 0 || binding.ValidatesOnDataErrors)
                        {
                            BindingExpression expression = BindingOperations.GetBindingExpression(parent, dp);
                            expression.UpdateSource();
                            if (expression.HasError)
                            {
                                valid = false;
                            }
                        }
                    }
                }
            }
            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!Validate(child))
                {
                    valid = false;
                }
            }
            return valid;
        }
    }
}
