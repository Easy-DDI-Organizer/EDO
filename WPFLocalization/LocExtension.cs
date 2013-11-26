using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Markup;
using System.Resources;
using System.Reflection;
using System.Windows;
using System.ComponentModel;

// Register the extention in the Microsoft's default namespaces
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "WPFLocalization")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2007/xaml/presentation", "WPFLocalization")]
[assembly: System.Windows.Markup.XmlnsDefinition("http://schemas.microsoft.com/winfx/2008/xaml/presentation", "WPFLocalization")]

namespace WPFLocalization
{
    /// <summary>
    /// Represents a localization makrup extension.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    [ContentProperty("Key")]
    public class LocExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the resource key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the formatting string to use.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// The property localized by the instance.
        /// </summary>
        object _targetProperty;

        /// <summary>
        /// The instance that owns the <see cref="DependencyProperty"/> localized by the instance.
        /// </summary>
        WeakReference _targetObject;

        /// <summary>
        /// The list of instances created by a template that own the <see cref="DependencyProperty"/>
        /// localized by the instance.
        /// </summary>
        List<WeakReference> _targetObjects;

        /// <summary>
        /// Gets value indicating if the instance localized by this instance is alive.
        /// </summary>
        internal bool IsAlive
        {
            get
            {
                // Verify if the extension is used in a template

                if (_targetObjects != null)
                {
                    foreach (var item in _targetObjects)
                    {
                        if (item.IsAlive)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                return _targetObject.IsAlive;
            }
        }

        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        public LocExtension() { }

        /// <summary>
        /// Initializes new instance of the class.
        /// </summary>
        /// <param name="key">The resource key.</param>
        public LocExtension(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Returns the object that corresponds to the specified resource key.
        /// </summary>
        /// <param name="serviceProvider">An object that can provide services for the markup extension.</param>
        /// <returns>The object that corresponds to the specified resource key.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (service != null)
            {
                if (service.TargetProperty is DependencyProperty)
                {
                    _targetProperty = service.TargetProperty;

                    if (service.TargetObject is DependencyObject)
                    {
                        var targetObject = new WeakReference(service.TargetObject);

                        // Verify if the extension is used in a template
                        // and has been already registered

                        if (_targetObjects != null)
                        {
                            _targetObjects.Add(targetObject);
                        }
                        else
                        {
                            _targetObject = targetObject;

                            LocalizationManager.AddLocalization(this);
                        }
                    }
                    else
                    {
                        // The extension is used in a template

                        _targetObjects = new List<WeakReference>();

                        LocalizationManager.AddLocalization(this);

                        return this;
                    }
                }
                else if (service.TargetProperty is PropertyInfo)
                {
                    _targetProperty = service.TargetProperty;

                    _targetObject = new WeakReference(service.TargetObject);

                    LocalizationManager.AddLocalization(this);
                }
            }

            return GetValue(Key, Format);
        }

        /// <summary>
        /// Updates the value of the localized object.
        /// </summary>
        internal void UpdateTargetValue()
        {
            var targetProperty = _targetProperty;

            if (targetProperty != null)
            {
                if (targetProperty is DependencyProperty)
                {
                    if (_targetObject != null)
                    {
                        var targetObject = _targetObject.Target as DependencyObject;

                        if (targetObject != null)
                        {
                            targetObject.SetValue((DependencyProperty)targetProperty, GetValue(Key, Format));
                        }
                    }
                    else if (_targetObjects != null)
                    {
                        foreach (var item in _targetObjects)
                        {
                            var targetObject = item.Target as DependencyObject;

                            if (targetObject != null)
                            {
                                targetObject.SetValue((DependencyProperty)targetProperty, GetValue(Key, Format));
                            }
                        }
                    }
                }
                else if (targetProperty is PropertyInfo)
                {
                    var targetObject = _targetObject.Target;

                    if (targetObject != null)
                    {
                        ((PropertyInfo)targetProperty).SetValue(targetObject, GetValue(Key, Format), null);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the object that corresponds to the specified resource key.
        /// </summary>
        /// <param name="key">the resource key.</param>
        /// <returns>The object that corresponds to the specified resource key.</returns>
        static object GetValue(string key, string format)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            var manager = LocalizationManager.ResourceManager;

            object value;

#if DEBUG
            //value = manager == null ? string.Empty : manager.GetObject(key) ?? "[Resource: " + key + "]";

            if (manager == null)
            {
                value = "";
            }
            else
            {
                value = manager.GetObject(key);

                if (value == null)
                {
                    throw new ArgumentOutOfRangeException("key", key, "Resource not found.");
                }
            }
#else
            value = manager == null ? string.Empty : manager.GetObject(key) ?? string.Empty;
#endif

            if (string.IsNullOrEmpty(format))
            {
                return value;
            }
            else
            {
                return string.Format(format, value);
            }
        }
    }
}