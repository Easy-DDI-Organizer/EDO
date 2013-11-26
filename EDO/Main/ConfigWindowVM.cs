using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;

namespace EDO.Main
{
    public class ConfigWindowVM :BaseVM
    {
        static bool IsValidUndoBufferCount(string count)
        {
            int n;
            bool result = Int32.TryParse(count, out n);
            if (!result)
            {
                return false;
            }
            return (n >= 1 && n <= 200);
        }

        static string SelectLanguage(ObservableCollection<Option> languages, String language)
        {
            foreach (Option opt in languages)
            {
                if (opt.Code == language)
                {
                    return language;
                }
            }
            return languages.First().Code;
        }

        private EDOConfig config;
        public ConfigWindowVM(EDOConfig config)
        {
            this.config = config;
            ReopenLastFile = config.ReopenLastFile;
            UndoBufferCount = config.UndoBufferCount;

            Languages = new ObservableCollection<Option>() {
                new Option("ja", "日本語"),
                new Option("en", "English")
            };
            language = SelectLanguage(Languages, config.Language);
            ShouldRestart = false;
        }
        public bool ShouldRestart {get; set; }

        private bool reopenLastFile;
        public bool ReopenLastFile
        {
            get
            {
                return reopenLastFile;
            }
            set
            {
                if (reopenLastFile != value)
                {
                    reopenLastFile = value;
                    NotifyPropertyChanged("ReopenLastFile");
                }
            }
        }

        private string undoBufferCountText;
        public string UndoBufferCountText
        {
            get
            {
                return undoBufferCountText;
            }
            set
            {
                if (undoBufferCountText != value && IsValidUndoBufferCount(value))
                {
                    undoBufferCountText = value;
                    NotifyPropertyChanged("UndoBufferCountText");
                }
            }
        }

        private int UndoBufferCount
        {
            get
            {
                int i = 0;
                try {
                    i = Int32.Parse(undoBufferCountText);
                } catch {
                }
                return i;
            }
            set
            {
                undoBufferCountText = value.ToString();
            }
        }

        private string language;
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                if (language != value)
                {
                    language = value;
                    NotifyPropertyChanged("Language");
                }
            }
        }
        public ObservableCollection<Option> Languages { get; set; }

        public void Save()
        {
            config.ReopenLastFile = ReopenLastFile;
            config.UndoBufferCount = UndoBufferCount;
            ShouldRestart = (config.Language != Language);
            config.Language = Language;
        }
    }
}
