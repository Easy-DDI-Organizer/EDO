using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.CategoryForm;
using EDO.QuestionCategory.CodeForm;
using EDO.Core.ViewModel;
using EDO.DataCategory.DataSetForm;
using EDO.DataCategory.DataFileForm;
using EDO.Core.Util;
using EDO.Core.Model;
using EDO.QuestionCategory.QuestionGroupForm;

namespace EDO.Main
{
    //StudyUnitのメニューを表すViewModel
    public class MenuItemVM :BaseVM
    {
        public MenuItemVM(MenuElem elem, FormVM content)
        {
            this.elem = elem;
            this.content = content;
            this.MenuItems = new ObservableCollection<MenuItemVM>();
        }

        private MenuElem elem;

        //idはLoadState/SaveStateで使われる
        public int Id { get { return elem.Id; } }

        public string Title
        {
            get
            {
                return elem.Title;
            }
        }

        private FormVM content;
        public FormVM Content
        {
            get
            {
                return content;
            }
            set
            {
                if (content != value)
                {
                    this.content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected != value)
                {
                    this.isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public ObservableCollection<MenuItemVM> MenuItems { get; set; }

        public void Add(MenuItemVM childMenuItem)
        {
            this.MenuItems.Add(childMenuItem);
        }

        public override string ToString()
        {
            return Title;
        }

        #region リボンメニュー制御用
        public bool IsCategory {
            get
            {
                return (this.Content is CategoryFormVM);
            }
        }
        public bool IsCode 
        {
            get
            {
                return (this.Content is CodeFormVM);
            }
        }

        public bool IsDataSet
        {
            get
            {
                return (this.Content is DataSetFormVM);
            }
        }

        public bool IsDataFile
        {
            get
            {
                return (this.Content is DataFileFormVM);
            }
        }

        public bool IsQuestionGroup
        {
            get
            {
                return (Content is QuestionGroupFormVM);
            }
        }

        #endregion リボンメニュー制御用

        public bool validate()
        {
            return content.Validate();
        }
    }
}
