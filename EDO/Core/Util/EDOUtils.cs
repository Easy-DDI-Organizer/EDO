using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using System.Windows;
using EDO.QuestionCategory.QuestionForm;
using EDO.VariableCategory.VariableForm;
using System.Windows.Input;
using System.Text.RegularExpressions;
using EDO.Properties;
using EDO.Core.View;

namespace EDO.Core.Util
{
    public static class EDOUtils
    {

        public static T GetVM<T>(Control view) where T: class
        {
            if (view.DataContext is T)
            {
                return (T)view.DataContext;
            }
            return null;
        }

        public static T GetAncestorViewModel<T>(BaseVM viewModel) where T : BaseVM
        {
            BaseVM cur = viewModel;
            while (cur.Parent != null)
            {
                cur = cur.Parent;
                if (cur is T)
                {
                    return (T)cur;
                }
            }
            return null;
        }

        public static void UpdateViewOfVM(UserControl control)
        {
            if (!(control is IValidatable))
            {
                throw new ArgumentException("Invalid control (IValidatable not implemented)");
            }

            FormVM vm = control.DataContext as FormVM;
            if (vm != null)
            {
                vm.View = (IValidatable)control;
                vm.Reload();
            }
        }

        public static string AbsToRel(string absPath, string basePath)
        {
            //basePathの末尾に\がついてないとディレクトリとみなされない。
            Uri baseUri = new Uri(basePath);
            Uri absUri = new Uri(baseUri, absPath);
            string relativePath =  baseUri.MakeRelativeUri(absUri).ToString();
            relativePath = System.Web.HttpUtility.UrlDecode(relativePath).Replace('/', '\\');
            return relativePath;
        }

        public static string RelToAbs(string relPath, string basePath)
        {
            Uri baseUri = new Uri(basePath);
            Uri relUri = new Uri(baseUri, relPath);
            return relUri.LocalPath;
        }

        public static string ToSafeString(this object obj, string defaultValue)
        {
            return (obj ?? string.Empty).ToString();
        }

        public static string ToSafeString(this object obj)
        {
            return ToSafeString(obj, String.Empty);
        }

        public static string ToDebugString(this object obj)
        {
            return "(" + ToSafeString(obj) + ")";
        }


        public static string OrderTitle(IOrderedObject obj) {
            return OrderTitle(obj.OrderPrefix, obj.OrderNo);
        }

        public static string OrderTitle(string prefix, int no)
        {
            return prefix + no;
        }

        public static int UniqOrderNo(HashSet<string> titles, string curTitle, string prefix)
        {
            // 指定されたタイトルに対するユニークな番号を返す
            if (curTitle != null)
            {
                Regex reg = new Regex(prefix + @"\d+");
                Match m = reg.Match(curTitle);
                if (!m.Success)
                {
                    return -1; //prefixで始まらないタイトルがついている場合-1をかえす
                }
            }
            for (int i = 1; i < 9999; i++)
            {
                string title = EDOUtils.OrderTitle(prefix, i);
                if (curTitle != null)
                {
                    // 「収集方法4」のようにタイトルが決まっている場合、4を返す。
                    if (title == curTitle)
                    {
                        return i;
                    }
                }
                else
                {
                    // タイトルがnull(タイトルがモデルに保存されていない場合)、未使用ならばその番号を返す。
                    // タイトル1, タイトル3がtitlesに含まれている場合、2を返す。
                    if (!titles.Contains(title))
                    {
                        return i;
                    }

                }
            }
            return -1;
        }

        public static int GetMaxOrderNo<T>(ICollection<T> objects) where T : IOrderedObject
        {
            int max = 0;
            foreach (T obj in objects)
            {
                if (obj.OrderNo > max)
                {
                    max = obj.OrderNo;
                }
            }
            return max;
        }

        public static T GetFirst<T>(Collection<T> objects) where T : class
        {
            if (objects == null || objects.Count == 0)
            {
                return null;
            }
            return objects[0];
        }

        public static T Find<T>(ICollection<T> objects, string id) where T : class, IStringIDProvider
        {
            foreach (T obj in objects)
            {
                if (obj.Id == id)
                {
                    return obj;
                }
            }
            return null;
        }

        private static string GetResponseResourceName(string responseTypeCode)
        {
            Dictionary<string, string> resourceNameDict = new Dictionary<string, string>()
            {
                {Options.RESPONSE_TYPE_CHOICES_CODE, "ChoicesResponse"},
                {Options.RESPONSE_TYPE_DATETIME_CODE, "DateTimeResponse"},
                {Options.RESPONSE_TYPE_FREE_CODE, "FreeResponse"},
                {Options.RESPONSE_TYPE_NUMBER_CODE, "NumberResponse"},
                {Options.RESPONSE_TYPE_UNKNOWN_CODE, "UnknownResponse"},
            };
            if (responseTypeCode == null)
            {
                responseTypeCode = Options.RESPONSE_TYPE_UNKNOWN_CODE;
            }
            if (!resourceNameDict.ContainsKey(responseTypeCode))
            {
                return "UnknownResponse";
            }
            return resourceNameDict[responseTypeCode];
        }

        public static DataTemplate SelectTemplate(QuestionVM question)
        {
            string typeCode = question != null ? question.ResponseTypeCode : null;
            string resourceName = GetResponseResourceName(typeCode);
            return  (DataTemplate)Application.Current.Resources[resourceName];
        }

        public static DataTemplate SelectTemplate(VariableVM variable)
        {
            string typeCode = variable != null ? variable.ResponseTypeCode : null;
            string resourceName = GetResponseResourceName(typeCode);
            return (DataTemplate)Application.Current.Resources[resourceName];
        }


        public static string Join<T>(ICollection<T> items, Func<T, string> valueGetter, string left, string right, string sep) where T : class
        {
            StringBuilder msg = new StringBuilder();
            T lastItem = items.Last();
            foreach (T item in items)
            {
                msg.Append(left);
                msg.Append(valueGetter(item));
                msg.Append(right);
                if (item != lastItem)
                {
                    msg.Append(sep);
                }
            }
            return msg.ToString();
        }


        public static string CannotDeleteError<T>(string message, ICollection<T> items,  Func<T, string> valueGetter) where T :class
        {
            //から参照されているため削除できません。
            return string.Format(Resources.ReferenceDeleteError, message) + Join(items, valueGetter, Resources.StartBracket, Resources.EndBracket, Resources.Comma
                );
        }

        public static string PadZero(string str, int length)
        {
            return str.PadLeft(length, '0');
        }

        public static string ToTitleNo(string title)
        {
            Regex regex = new Regex("^([a-zA-Z]+)([0-9]+)[^0-9]*([0-9]+)?");
            StringBuilder buf = new StringBuilder();
            Match match = regex.Match(title);
            if (match.Success)
            {
                string baseName = PadZero(match.Groups[1].Value, 10);
                buf.Append(baseName);

                string baseNum = PadZero(match.Groups[2].Value, 10);
                buf.Append(baseNum);

                string branchNum = PadZero(match.Groups[3].Value, 10);
                buf.Append(branchNum);
            }
            return buf.ToString();
        }

        public static T Find<T>(ICollection<T> collection, object id) where T : class, IStringIDProvider
        {
            string stringId = id as string;
            if (stringId == null)
            {
                return null;
            }
            foreach (T item in collection)
            {
                if (item.Id == stringId)
                {
                    return item;
                }
            }
            return null;
        }

        public static T FindOrFirst<T>(Collection<T> collection, VMState state) where T : class, IStringIDProvider
        {
            if (state == null)
            {
                return GetFirst(collection);
            }
            return FindOrFirst<T>(collection, state.State1);
        }

        public static T FindOrFirst<T>(Collection<T> collection, object id) where T : class, IStringIDProvider
        {
            T result = Find(collection, id);
            if (result == null)
            {
                result = GetFirst(collection);
            }
            return result;
        }

        public static void ShowUnexpectedError(Exception ex)
        {
            MessageBox.Show(EDOConstants.ERR_UNEXPECTED + ex.Message);
        }


        public static string UniqueLabel(List<string> existLabels, string addingLabel)
        {
            if (!existLabels.Contains(addingLabel))
            {
                return addingLabel;
            }

            int index = 0;
            Regex regex = new Regex("^" + addingLabel + @"\((\d+)");
            foreach (string existLabel in existLabels)
            {
                Match match = regex.Match(existLabel);
                if (match.Success)
                {
                    int v = Int32.Parse(match.Groups[1].Value);
                    if (v > index)
                    {
                        index = v;
                    }
                }
            }
            return addingLabel + "(" + (index + 1) + ")";
        }

    }
}

