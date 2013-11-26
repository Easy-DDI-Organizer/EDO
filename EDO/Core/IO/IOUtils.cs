using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;
using System.IO;
using System.Diagnostics;

namespace EDO.Core.IO
{
    public class IOUtils
    {


        public static string QuerySavePathName(string title, string initPathName, string filter, bool askPathName)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = title;
            dlg.Filter = filter;
            if (!string.IsNullOrEmpty(initPathName))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(initPathName);
                dlg.FileName = Path.GetFileName(initPathName);
            }
            bool? result = dlg.ShowDialog();
            string path = null;
            if (result == true)
            {
                path = dlg.FileName;
            }
            return path;
        }

        public static FileDialogResult QuerySavePathNameEx(string title, string initPathName, string filter, bool askPathName)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Title = title;
            dlg.Filter = filter;
            if (!string.IsNullOrEmpty(initPathName))
            {
                dlg.InitialDirectory = Path.GetDirectoryName(initPathName);
                dlg.FileName = Path.GetFileName(initPathName);
            }
            bool? result = dlg.ShowDialog();
            string path = null;
            if (result == true)
            {
                path = dlg.FileName;
            }
            if (path == null)
            {
                return null;
            }
            return new FileDialogResult(path, dlg.FilterIndex);
        }

        public static string QueryOpenPathName(string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = filter;
            bool? result = dlg.ShowDialog();
            if (result != true)
            {
                return null;
            }
            return dlg.FileName;
        }

        public static FileDialogResult QueryOpenPathNameEx(string filter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = filter;
            bool? result = dlg.ShowDialog();
            if (result != true)
            {
                return null;
            }
            if (string.IsNullOrEmpty(dlg.FileName))
            {
                return null;
            }
            return new FileDialogResult(dlg.FileName, dlg.FilterIndex);
        }

    }
}
