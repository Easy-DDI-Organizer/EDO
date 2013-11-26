using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EDO.Core.ViewModel
{
    //DDIインポート時に使われるメニュー
    public class CheckMenuItemVM :BaseVM
    {
        public static CheckMenuItemVM FindByMenuElem(List<CheckMenuItemVM> menuItems, MenuElem menuElem)
        {
            foreach (CheckMenuItemVM menuItem in menuItems)
            {
                if (menuItem.MenuElem == menuElem)
                {
                    return menuItem;
                }
            }
            return null;
        }

        public static List<CheckMenuItemVM> FindByMenuElems(List<CheckMenuItemVM> menuItems, List<MenuElem> menuElems)
        {
            List<CheckMenuItemVM> results = new List<CheckMenuItemVM>();
            foreach (MenuElem menuElem in menuElems)
            {
                CheckMenuItemVM menuItem = FindByMenuElem(menuItems, menuElem);
                if (menuItem == null) {
                    throw new ApplicationException();
                }
                results.Add(menuItem);
            }
            return results;
        }

        public static int CountCheck(IEnumerable<CheckMenuItemVM> menuItems)
        {
            int checkCount = 0;
            foreach (CheckMenuItemVM childMenuItem in menuItems)
            {
                if ((bool)childMenuItem.IsChecked)
                {
                    checkCount++;
                }
            }
            return checkCount;
        }

        public CheckMenuItemVM(MenuElem elem)
        {
            this.elem = elem;
            this.menuItems = new ObservableCollection<CheckMenuItemVM>();
            this.relatedMenuItems = new List<CheckMenuItemVM>();
            isChecked = true;
        }

        private MenuElem elem;
        public MenuElem MenuElem { get { return elem; } }

        private List<CheckMenuItemVM> relatedMenuItems;
        public List<CheckMenuItemVM> RelatedMenuItems { get { return relatedMenuItems; } }

        public string Title { get { return elem.Title; } }

        private CheckMenuItemVM parentMenuItem;
        public CheckMenuItemVM ParentMenuItem {
            get 
            {
                return parentMenuItem; 
            }
            set
            {
                if (parentMenuItem != value) {
                    //最初しか設定されないのでNotifyする必要なし
                    parentMenuItem = value;
                }
            }
        }

        private ObservableCollection<CheckMenuItemVM> menuItems;
        public ObservableCollection<CheckMenuItemVM> MenuItems { get { return menuItems;  } }

        public void Add(CheckMenuItemVM menuItem)
        {
            menuItem.ParentMenuItem = this;
            menuItems.Add(menuItem);
        }


        private void Log(string msg)
        {
            Debug.WriteLine(msg + " [" + Title + "] IsChecked=" + IsChecked);
        }

        private void Log(string msg, CheckMenuItemVM menuItem)
        {
            Debug.WriteLine(msg + " [" + menuItem.Title + "] IsChecked=" + menuItem.IsChecked);
        }

        private bool isChanging = false;

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    //自分のチェック状態を変更
                    UpdateSelfCheckedStatus(value);
                    try
                    {
                        //自部の子供の関連メニューの状態変更時に、自分自身の状態が元に戻ってしまうのを防ぐガード変数。
                        //このガードがないと例えば「設問設計」のカテゴリーチェックが外せなくなる。
                        isChanging = true; 
                        if (elem.IsCategory)
                        {
                            //カテゴリの場合は子供のチェック状態を変更
                            UpdateChildrenCheckedStatus();
                        }
                        else
                        {
                            UpdateParentCheckedStatus();
                        }
                        UpdateRelatedCheckStatus();
                    }
                    finally
                    {
                        isChanging = false;
                    }
                }
            }
        }


        public void UpdateSelfCheckedStatus(bool isChecked)
        {
            if (isChanging)
            {
                return;
            }
            this.isChecked = isChecked;
            NotifyPropertyChanged("IsChecked");

        }

        private void UpdateRelatedCheckStatus()
        {
            if (isChecked)
            {
                //例えば「設問設計」が押されたとき、その子供「質問項目の設計」がチェックされる。
                //「質問項目の設計」に関連する「選択肢」、「コード」などのメニューにチェックを入れるためには、
                // そのメニューのIsChecked=trueをよびださないといけない(UpdateSelfCheckedStatusではだめ)
                foreach (CheckMenuItemVM relatedMenuItem in relatedMenuItems)
                {
                    relatedMenuItem.IsChecked = isChecked;
                }
            }
        }

        private void UpdateChildrenCheckedStatus()
        {
            //子供のチェック状態を変更する
            foreach (CheckMenuItemVM childMenuItem in MenuItems)
            {
                childMenuItem.IsChecked = isChecked;
            }
        }

        private void UpdateParentCheckedStatus()
        {
            //親のチェック状態を変更する
            int checkCount = CountCheck(parentMenuItem.MenuItems);
            bool check = checkCount == 0 ? false : true;
            parentMenuItem.UpdateSelfCheckedStatus(check);
        }

    }
}
