using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using EDO.Core.Model;

namespace EDO.Core.ViewModel
{
    public class CheckOptionVM :BaseVM
    {
        private CheckOption checkOption;

        public CheckOptionVM(BaseVM parent, CheckOption checkOption) :base(parent)
        {
            this.checkOption = checkOption;
        }

        public string Label
        {
            get
            {
                return checkOption.Label;
            }
        }

        public bool IsChecked
        {
            get
            {
                return checkOption.IsChecked;
            }
            set
            {
                if (checkOption.IsChecked != value)
                {
                    checkOption.IsChecked = value;
                    NotifyPropertyChanged("IsChecked");
                    Memorize();
                }
            }
        }

        public string Memo 
        {
            get
            {
                return checkOption.Memo;
            }
            set
            {
                if (checkOption.Memo != value)
                {
                    checkOption.Memo = value;
                    NotifyPropertyChanged("Memo");
                    IsChecked = !string.IsNullOrEmpty(value);
                    Memorize();
                }
            }
        }

        public bool HasMemo 
        {
            get
            {
                return checkOption.HasMemo;
            }
            set
            {
                if (checkOption.HasMemo != value)
                {
                    checkOption.HasMemo = value;
                    NotifyPropertyChanged("HasMemo");
                }
            }
        }

    }
}
