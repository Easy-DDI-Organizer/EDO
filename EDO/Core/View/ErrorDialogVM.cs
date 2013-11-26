using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using EDO.Main;
using System.Diagnostics;

namespace EDO.Core.View
{
    public class ErrorDialogVM :BaseVM
    {
        public ErrorDialogVM()
        {
            errorInfos = new ObservableCollection<ErrorInfoVM>();
        }

        private MainWindowVM mainWindow;
        private ObservableCollection<ErrorInfoVM> errorInfos;
        public ObservableCollection<ErrorInfoVM> ErrorInfos { get { return errorInfos; } }

        public void SetErrorInfos(MainWindowVM mainWindow, List<WriteError> errorInfos)
        {
            this.mainWindow = mainWindow;
            this.errorInfos.Clear();
            foreach (WriteError errorInfoModel in errorInfos)
            {
                MenuItemVM menuItem = errorInfoModel.EDOUnit.FindMenuItem(errorInfoModel.Form);
                Debug.Assert(menuItem != null);
                ErrorInfoVM errorInfo = new ErrorInfoVM(errorInfoModel, menuItem);
                this.errorInfos.Add(errorInfo);
            }
        }

        private ErrorInfoVM selectedErrorInfo;
        public ErrorInfoVM SelectedErrorInfo
        {
            get
            {
                return selectedErrorInfo;
            }
            set
            {
                if (selectedErrorInfo != value)
                {
                    selectedErrorInfo = value;
                    if (selectedErrorInfo != null)
                    {
                        mainWindow.ShowMenuItem(selectedErrorInfo.ErrorInfo.EDOUnit, selectedErrorInfo.MenuItem);
                    }
                    NotifyPropertyChanged("SelectedErrorInfo");
                }
            }
        }        
    }
}
