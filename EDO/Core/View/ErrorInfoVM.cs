using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Main;

namespace EDO.Core.View
{
    public class ErrorInfoVM :BaseVM
    {

        public ErrorInfoVM(WriteError errorInfo, MenuItemVM menuItem)
        {
            this.errorInfo = errorInfo;
            this.menuItem = menuItem;
        }
        private WriteError errorInfo;
        public WriteError ErrorInfo { get { return errorInfo; } }
        private MenuItemVM menuItem;
        public MenuItemVM MenuItem { get { return menuItem; } }

        public string UnitTitle { get { return errorInfo.EDOUnit.Title; } }
        public string MenuTitle { get { return menuItem.Title; } }
        public string Message { get { return errorInfo.Message; } }
    }
}
