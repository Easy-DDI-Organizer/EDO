using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace EDO.Core.Util
{
    public class ApplicationDetails
    {
        /// 
        /// バージョン情報を返す
        /// 
        public static string ProductVersion
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                AssemblyName assemblyName = assembly.GetName();
                Version version = assemblyName.Version;
                return version.ToString();
            }
        }

        /// 
        /// アセンブリ情報からプロダクト名を返す
        /// 
        public static string ProductName
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (customAttributes == null || customAttributes.Length == 0) 
                {
                    return string.Empty;
                }
                return ((AssemblyProductAttribute)customAttributes[0]).Product;
            }
        }

        /// 
        /// コピーライトを返す
        /// 
        public static string CopyRight
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (customAttributes == null || customAttributes.Length == 0) 
                {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
            }
        }

        static string companyName = string.Empty;

        /// 
        /// Get the name of the system provider name from the assembly
        /// 
        public static string CompanyName
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes
                                        (typeof(AssemblyCompanyAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        companyName =
                            ((AssemblyCompanyAttribute)customAttributes[0]).Company;
                    }
                    if (string.IsNullOrEmpty(companyName))
                    {
                        companyName = string.Empty;
                    }
                }

                return companyName;
            }

        }
        static string productVersion = string.Empty;



        static string copyRightsDetail = string.Empty;



        static string productTitle = string.Empty;

        /// 
        /// Get the Product tile from the assembly
        /// 
        public static string ProductTitle
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes
                            (typeof(AssemblyTitleAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        productTitle =
                            ((AssemblyTitleAttribute)customAttributes[0]).Title;
                    }
                    if (string.IsNullOrEmpty(productTitle))
                    {
                        productTitle = string.Empty;
                    }
                }
                return productTitle;
            }
        }

        static string productDescription = string.Empty;

        /// 
        /// Get the description of the product from the assembly
        /// 
        public static string ProductDescription
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes
                            (typeof(AssemblyDescriptionAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        productDescription =
                          ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
                    }
                    if (string.IsNullOrEmpty(productDescription))
                    {
                        productDescription = string.Empty;
                    }
                }
                return productDescription;
            }
        }
    }
}
