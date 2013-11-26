using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Properties;
using System.IO;

namespace EDO.Core.Model
{
    public class EDOConfig
    {
        public const string LANGUAGE_JA = "ja";
        public const string LANGUAGE_EN = "en";

        public bool ReopenLastFile
        {
            get
            {
                return Settings.Default.ReopenLastFile;
            }
            set
            {
                Settings.Default.ReopenLastFile = value;
            }
        }

        public string LastFile
        {
            get
            {
                return Settings.Default.LastFile;
            }
            set
            {
                Settings.Default.LastFile = value;
            }
        }

        public string UserId
        {
            get
            {
                return Settings.Default.UserId;
            }
            set
            {
                Settings.Default.UserId = value;
            }
        }

        public int UndoBufferCount
        {
            get
            {
                return Settings.Default.UndoBufferCount;
            }
            set
            {
                Settings.Default.UndoBufferCount = value;
            }
        }

        public string Language
        {
            get
            {
                return Settings.Default.Language;
            }
            set
            {
                Settings.Default.Language = value;
            }
        }

        public void InitLanguage(string cultureName)
        {
            if (cultureName == "ja-JP")
            {
                Language = EDOConfig.LANGUAGE_JA;
            }
            else
            {
                Language = EDOConfig.LANGUAGE_EN;
            }
        }

        public bool IsLanguageJa
        {
            get
            {
                return Language == LANGUAGE_JA;
            }
        }

        public bool IsLanguageEn
        {
            get
            {
                return Language == LANGUAGE_EN;
            }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        public static string AppPath
        {
            get
            {
                string appPath = "";
                try
                {
                    appPath = Path.GetFullPath(".");
                    appPath = appPath.Replace(@"\bin\Debug", "");
                    appPath = appPath.Replace(@"\bin\Release", "");
                    if (appPath.EndsWith(@"\") == false) { appPath += @"\"; }
                }
                catch (Exception) { throw; }
                return appPath;
            }
        }

        public static string DDI31Path
        {
            get
            {
                return AppPath + @"DDI\3.1\";            
            }
        }

        public static string DDI25Path
        {
            get
            {
                return AppPath + @"DDI\2.5\";
            }
        }
    }
}
