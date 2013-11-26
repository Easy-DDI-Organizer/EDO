using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Main;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.Core.IO;

namespace EDO.Core.View
{
    public class ImportOptionWindowVM
    {
        public ImportOptionWindowVM(DDIImportOption importOption)
        {
            menuItems = new ObservableCollection<CheckMenuItemVM>();

            CheckMenuItemVM curCategory = null;
            foreach (MenuElem elem in importOption.MenuElems)
            {
                CheckMenuItemVM menuItem = null;
                if (elem.IsCategory) {
                    menuItem = new CheckMenuItemVM(elem);
                    menuItems.Add(menuItem);
                    curCategory = menuItem;
                }
                else
                {
                    menuItem = new CheckMenuItemVM(elem);
                    curCategory.Add(menuItem);
                }
                menuItem.UpdateSelfCheckedStatus(true);
            }

            List<CheckMenuItemVM> leafMenuItems = LeafMenuItems;
            foreach (CheckMenuItemVM menuItem in leafMenuItems)
            {
                List<CheckMenuItemVM> relatedMenuItems = CheckMenuItemVM.FindByMenuElems(leafMenuItems, importOption.GetRelatedMenuElems(menuItem.MenuElem));
                menuItem.RelatedMenuItems.AddRange(relatedMenuItems);
            }
        }


        private List<CheckMenuItemVM> LeafMenuItems
        {
            get{
                List<CheckMenuItemVM> results = new List<CheckMenuItemVM>();
                foreach (CheckMenuItemVM menuItem in menuItems)
                {
                    results.AddRange(menuItem.MenuItems);
                }
                return results;
            }
        }


        private ObservableCollection<CheckMenuItemVM> menuItems;
        public ObservableCollection<CheckMenuItemVM> MenuItems { get { return menuItems; } }

        public List<MenuElem> CheckMenuElems
        {
            get
            {
                List<MenuElem> menuElems = new List<MenuElem>();
                foreach (CheckMenuItemVM checkMenuItem in MenuItems)
                {
                    if (checkMenuItem.IsChecked)
                    {
                        menuElems.Add(checkMenuItem.MenuElem);
                    }
                    foreach (CheckMenuItemVM childCheckMenuItem in checkMenuItem.MenuItems)
                    {
                        if (childCheckMenuItem.IsChecked)
                        {
                            menuElems.Add(childCheckMenuItem.MenuElem);
                        }
                    }
                }
                return menuElems;
            }
        }


    }
}
