using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Properties;
using System.Windows.Input;

namespace EDO.Core.Model
{
    public class EDOConstants
    {
        public const string TAG_UNDOABLE = "Undoable";
        public static readonly string ERR_UNEXPECTED = Resources.UnexpectedErrorOccurred; //予期しないエラーが発生しました。
        public static readonly string LABEL_ALL = Resources.All; //全体
        public static readonly KeyGesture KEY_DELETE = new KeyGesture(Key.Delete);
        public static readonly KeyGesture KEY_PAGE_UP = new KeyGesture(Key.PageUp);
        public static readonly KeyGesture KEY_PAGE_DOWN = new KeyGesture(Key.PageDown);
    }
}
