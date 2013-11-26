using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace EDO.Core.Util
{
    public class PropertyPathHelper
    {
        public static Binding CloneBinding(BindingBase bindingBase, object source)
        {
            var binding = bindingBase as Binding;
            if (binding != null)
            {
                var result = new Binding
                {
                    Source = source,
                    AsyncState = binding.AsyncState,
                    BindingGroupName = binding.BindingGroupName,
                    BindsDirectlyToSource = binding.BindsDirectlyToSource,
                    Converter = binding.Converter,
                    ConverterCulture = binding.ConverterCulture,
                    ConverterParameter = binding.ConverterCulture,
                    //ElementName = binding.ElementName,
                    FallbackValue = binding.FallbackValue,
                    IsAsync = binding.IsAsync,
                    Mode = binding.Mode,
                    NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
                    NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
                    NotifyOnValidationError = binding.NotifyOnValidationError,
                    Path = binding.Path,
                    //RelativeSource = binding.RelativeSource,
                    StringFormat = binding.StringFormat,
                    TargetNullValue = binding.TargetNullValue,
                    UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
                    UpdateSourceTrigger = binding.UpdateSourceTrigger,
                    ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
                    ValidatesOnExceptions = binding.ValidatesOnExceptions,
                    XPath = binding.XPath,

                };
                foreach (var validationRule in binding.ValidationRules)
                {
                    result.ValidationRules.Add(validationRule);
                }
                return result;
            }
            return null;
        }

        public static object GetValue(object obj, string propertyPath)
        {
            Binding binding = new Binding(propertyPath);
            binding.Mode = BindingMode.OneTime;
            binding.Source = obj;
            Dummy _dummy = new Dummy();
            BindingOperations.SetBinding(_dummy, Dummy.ValueProperty, binding);
            return _dummy.GetValue(Dummy.ValueProperty);
        }

        public static void SetValue(object obj, string propertyPath, object value)
        {
            Binding binding = new Binding(propertyPath);
            binding.Mode = BindingMode.OneWayToSource;
            binding.Source = obj;
            Dummy _dummy = new Dummy();
            BindingOperations.SetBinding(_dummy, Dummy.ValueProperty, binding);
            _dummy.SetValue(Dummy.ValueProperty, value);
        }

        public static string GetTargetValue(BindingExpression expression)
        {
            Binding binding = CloneBinding(expression.ParentBinding, expression.DataItem);
            binding.Mode = BindingMode.OneTime;
            Dummy _dummy = new Dummy();
            BindingOperations.SetBinding(_dummy, Dummy.TextProperty, binding);
            BindingExpression newExpression = BindingOperations.GetBindingExpression(_dummy, Dummy.TextProperty);
            string text = _dummy.GetValue(Dummy.TextProperty) as string;
            return text;
        }

        private class Dummy : TextBox
        {
            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(object), typeof(Dummy), new UIPropertyMetadata(null));
        }
    }
}
