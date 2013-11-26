using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using EDO.Core.View;
using System.Diagnostics;

namespace EDO.Main
{
    public abstract class EDOUnitVM : BaseVM, IValidatableCollection, IStatefullVM
    {
        public EDOUnitVM(MainWindowVM mainWindowVM, IFile file) :base(mainWindowVM)
        {
            this.file = file;
            MenuItems = new ObservableCollection<MenuItemVM>();
        }
        private IFile file;
        public ObservableCollection<MenuItemVM> MenuItems { get; set; }
        public string Id { get { return file.Id; } }
        public abstract string Title { get; set; }
        public string PathName
        {
            get
            {
                return file.PathName;
            }
            set
            {
                if (file.PathName != value)
                {
                    file.PathName = value;
                    NotifyPropertyChanged("PathName");
                }
            }
        }

        private MenuItemVM selectedMenuItem;
        public MenuItemVM SelectedMenuItem
        {
            get
            {
                return selectedMenuItem;
            }
            set
            {
                if (selectedMenuItem != value)
                {
                    selectedMenuItem = value;
                    NotifyPropertyChanged("SelectedMenuItem");
                }
            }
        }

        public bool Contains(FormVM form)
        {
            return FindMenuItem(form) != null;
        }

        public MenuItemVM FindMenuItem(FormVM form)
        {
            foreach (MenuItemVM menuItem in MenuItems)
            {
                if (menuItem.Content == form)
                {
                    return menuItem;
                }
                foreach (MenuItemVM childMenuItem in menuItem.MenuItems)
                {
                    if (childMenuItem.Content == form)
                    {
                        return childMenuItem;
                    }
                }
            }
            return null;
        }

        public void SelectMenuItem(MenuItemVM newMenuItem)
        {
            //エラー発生時に外部からメニューを表示するための処理
            foreach (MenuItemVM menuItem in MenuItems)
            {
                menuItem.IsSelected = false;
            }
            newMenuItem.IsSelected = true;
        }

        #region ICollectionValidatable メンバー

        public bool ValidateCurrentItem()
        {
            MenuItemVM menuItem = this.SelectedMenuItem;
            if (menuItem == null)
            {
                return true;
            }
            return menuItem.validate();
        }

        #endregion

        private List<MenuItemVM> AllMenuItems
        {
            get
            {
                List<MenuItemVM> allMenuItems = new List<MenuItemVM>();
                foreach (MenuItemVM menuItem in MenuItems)
                {
                    allMenuItems.Add(menuItem);
                    allMenuItems.AddRange(menuItem.MenuItems);
                }
                return allMenuItems;
            }
        }

        public void LoadState(VMState state)
        {
            int id = (int)state.State1;
            VMState childState = (VMState)state.State2;

            List<MenuItemVM> allMenuItems = AllMenuItems;
            foreach (MenuItemVM menuItem in allMenuItems)
            {
                menuItem.IsSelected = false;
                if (menuItem.Id == id)
                {
                    menuItem.IsSelected = true;
                    menuItem.Content.LoadState(childState);
                }
            }            
        }

        public void Complete(VMState state)
        {
            int id = (int)state.State1;
            VMState childState = (VMState)state.State2;

            List<MenuItemVM> allMenuItems = AllMenuItems;
            foreach (MenuItemVM menuItem in allMenuItems)
            {
                if (menuItem.Id == id)
                {
                    menuItem.Content.Complete(childState);
                }
            }
        }


        public VMState SaveState()
        {
            if (SelectedMenuItem == null)
            {
                return null;
            }
            return new VMState(SelectedMenuItem.Id, SelectedMenuItem.Content.SaveState());
        }
    }
}
