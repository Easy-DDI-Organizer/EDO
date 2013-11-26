using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Reflection;
using EDO.Core.Model;
using EDO.Core.Util;

namespace EDO.Core.View
{
    public class TextBoxHelper
    {
        public static object GetValue(BindingExpression expression)
        {
            Type currentType = null;
            if (expression == null)
            {
                return null;
            }
            object dataItem = expression.DataItem;
            if (dataItem == null)
            {
                return null;
            }
            string bindingPath = expression.ParentBinding.Path.Path;
            string[] properties = bindingPath.Split('.');

            object currentObject = dataItem;
            for (int i = 0; i < properties.Length; i++)
            {
                currentType = currentObject.GetType();
                PropertyInfo property = currentType.GetProperty(properties[i]);
                if (property == null)
                {
                    currentObject = null;
                    break;
                }
                currentObject = property.GetValue(currentObject, null);
                if (currentObject == null)
                {
                    break;
                }
            }

            return currentObject;
        }

        private static void GetTargetElement(TextBox sender, out DependencyObject element, out DependencyProperty dp)
        {
            element = null;
            dp = null;
            if (sender is TextBox)
            {
                if ((sender.Tag as string) == EDOConstants.TAG_UNDOABLE)
                {
                    element = sender;
                    dp = TextBox.TextProperty;
                }
                else
                {
                    ComboBox combo = VisualTreeFinder.FindParentControl<ComboBox>(sender);
                    if (combo != null && (combo.Tag as string) == EDOConstants.TAG_UNDOABLE)
                    {
                        element = combo;
                        dp = ComboBox.TextProperty;
                    }
                }
            }
        }

        private static bool IsSame(string lhv, string rhv)
        {
            if (string.IsNullOrEmpty(lhv) && string.IsNullOrEmpty(rhv))
            {
                return true;
            }
            if (lhv == null && rhv != null)
            {
                return false;
            }
            if (lhv != null && rhv == null)
            {
                return false;
            }
            return lhv == rhv;
        }

        //バインドされているソースのプロパティとテキストボックスの現在の値を比較し
        //変更されているならばtrueを。それ以外はfalseを返す。
        public static bool IsChanged(TextBox sender)
        {
            DependencyObject element = null;
            DependencyProperty dp = null;
            GetTargetElement(sender, out element, out dp);
            if (element == null || dp == null) {
                return false;
            }
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(element, dp);
            //現在のBindingExpressionを使って、ソースの値をターゲットのテキストプロパティにセットしたときの
            //文字列を取得。こうしないとコンバーターやStringFormatがセットされているときやプロパティの型がstringじゃないとき
            //どうやって変換するかという問題が発生する。
            string propertyValue = PropertyPathHelper.GetTargetValue(bindingExpression);
            string elementValue = element.GetValue(dp) as string;
            return !IsSame(propertyValue, elementValue);
        }

        public static bool UpdateSource(TextBox sender) 
        {
            DependencyObject element = null;
            DependencyProperty dp = null;
            GetTargetElement(sender, out element, out dp);
            if (element == null || dp == null)
            {
                return true;
            }
            BindingExpression bindingExpression = BindingOperations.GetBindingExpression(element, dp);
            if (bindingExpression == null)
            {
                return true;
            }
            string elementValue = element.GetValue(dp) as string;
            bindingExpression.UpdateSource();
            string propertyValue = PropertyPathHelper.GetTargetValue(bindingExpression);
            return IsSame(elementValue, propertyValue);
        }

    }

}
