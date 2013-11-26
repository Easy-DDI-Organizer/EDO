using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;

namespace WPFLocalization
{
    /// <summary>
    /// Represents a class that manages the localication features.
    /// </summary>
    public static class LocalizationManager
    {
        /// <summary>
        /// The <see cref="ResourceManager"/> by which resources as accessed.
        /// </summary>
        static ResourceManager _resourceManager;

        /// <summary>
        /// True if an attempt to load the resource manager has been made.
        /// </summary>
        static bool _resourceManagerLoaded;

        /// <summary>
        /// Gets or sets the resource manager to use to access the resources.
        /// </summary>
        public static ResourceManager ResourceManager
        {
            get
            {
                if (_resourceManager == null && !_resourceManagerLoaded)
                {
                    _resourceManager = GetResourceManager();

                    _resourceManagerLoaded = true;
                }

                return _resourceManager;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _resourceManager = value;

                UpdateLocalizations();
            }
        }

        /// <summary>
        /// Gets or sets the current UI culture.
        /// </summary>
        /// <remarks>
        /// This property changes the UI culture of the current thread to the specified value
        /// and updates all localized property to reflect values of the new culture.
        /// </remarks>
        public static CultureInfo UICulture
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                Thread.CurrentThread.CurrentUICulture = value;

                UpdateLocalizations();
            }
        }

        /// <summary>
        /// Holds the list of localization instances.
        /// </summary>
        /// <remarks>
        /// <see cref="WeakReference"/> cannot be used as a localization instance
        /// will be garbage collected on the next garbage collection
        /// as the localizaed object does not hold reference to it.
        /// </remarks>
        static List<LocExtension> _localizations = new List<LocExtension>();

        /// <summary>
        /// Holds the number of localizations added since the last purge of localizations.
        /// </summary>
        static int _localizationPurgeCount;

        /// <summary>
        /// Adds the specified localization instance to the list of manages localization instances.
        /// </summary>
        /// <param name="localization">The localization instance.</param>
        internal static void AddLocalization(LocExtension localization)
        {
            if (localization == null)
            {
                throw new ArgumentNullException("localization");
            }

            if (_localizationPurgeCount > 50)
            {
                // It's really faster to fill a new list instead of removing elements
                // from the existing list when there are a lot of elements to remove.

                var localizatons = new List<LocExtension>(_localizations.Count);

                foreach (var item in _localizations)
                {
                    if (item.IsAlive)
                    {
                        localizatons.Add(item);
                    }
                }

                _localizations = localizatons;

                _localizationPurgeCount = 0;
            }

            _localizations.Add(localization);

            _localizationPurgeCount++;
        }

        /// <summary>
        /// Returns resource manager to access the resources the application's main assembly.
        /// </summary>
        /// <returns></returns>
        static ResourceManager GetResourceManager()
        {
            var assembly = Assembly.GetEntryAssembly();

            // Check if the assembly is the "Expression Blend" executable

            if (assembly != null && string.Compare(assembly.GetName().Name, "Blend", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                assembly = null;
            }

            if (assembly == null)
            {
                // Design time

                // Try to find the main assembly

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var item in assemblies)
                {
                    // Check if the assembly is executable

                    if (item.EntryPoint != null)
                    {
                        // Check if the assembly contains WPF application (e.g. MyApplication.App class
                        // that derives from System.Windows.Application)

                        var applicationType = item.GetType(item.GetName().Name + ".App", false);

                        if (applicationType != null && typeof(System.Windows.Application).IsAssignableFrom(applicationType))
                        {
                            // Check if the assembly is the "Expression Blend" executable

                            if (string.Compare(item.GetName().Name, "Blend", StringComparison.InvariantCultureIgnoreCase) == 0)
                            {
                            }
                            else
                            {
                                assembly = item;

                                break;
                            }
                        }
                    }
                }
            }

            if (assembly != null)
            {
                try
                {
                    // The resoures cannot be found in the manifest of the assembly

                    return new ResourceManager(assembly.GetName().Name + ".Properties.Resources", assembly);
                }
                catch (MissingManifestResourceException) { }
            }

            return null;
        }

        /// <summary>
        /// Updates the localizations.
        /// </summary>
        static void UpdateLocalizations()
        {
            foreach (var item in _localizations)
            {
                item.UpdateTargetValue();
            }
        }
    }
}
