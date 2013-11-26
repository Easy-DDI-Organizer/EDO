using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;
using System.Xml.Serialization;
using EDO.Core.Model;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using EDO.Core.Util;
using System.Windows;
using System.Security.Cryptography;
using EDO.Properties;

namespace EDO.Core.IO
{
    public static class EDOSerializer
    {
        #region 保存・読込するファイル名の問い合わせ

        private static readonly string GROUP_FILTER = Resources.EDOGroupFileFilter;
        private static readonly string STUDYUNIT_FILTER = Resources.EDOFileFilter;
        private static readonly string SAMEPATHNAMEERROR = Resources.UsedFileNameAndInput; //他の調査ファイルで使用済みのファイル名です。\n別のファイル名を指定して下さい。

        private static bool QuerySavePathName(EDOUnitVM unit, string filter, bool askPathName)
        {
            if (unit == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(unit.PathName) || askPathName)
            {
                //パスがnullかパス名を問い合わせるフラグが指定されている場合はダイアログを表示する。
                //(名前をつけて保存の場合はaskPathName=trueで呼び出される)
                string title = string.Format(Resources.PleaseSave, unit.Title);
                string path = IOUtils.QuerySavePathName(title, unit.PathName, filter, askPathName); //～を保存してください。
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }
                unit.PathName = path;
            }
            return true;
        }


        private static bool QuerySavePathNamesUniq(EDOUnitVM unit, string filter, bool askPathName, List<string> usingPathNames)
        {
            while (true)
            {
                if (!QuerySavePathName(unit, STUDYUNIT_FILTER, askPathName))
                {
                    return false;
                }
                if (usingPathNames.Contains(unit.PathName))
                {
                    MessageBox.Show(SAMEPATHNAMEERROR);
                    unit.PathName = null;
                    continue;
                }
                usingPathNames.Add(unit.PathName);
                break;
            }
            return true;
        }


        private static string QueryOpenPathNameUniq(string filter, List<string> usingPathNames)
        {
            string pathName = null;
            while (true)
            {
                pathName = IOUtils.QueryOpenPathName(filter);
                if (string.IsNullOrEmpty(pathName)) 
                {
                    return null;
                }
                if (usingPathNames.Contains(pathName))
                {
                    MessageBox.Show(SAMEPATHNAMEERROR);
                    continue;
                }
                usingPathNames.Add(pathName);
                break;
            }
            return pathName;
        }
        #endregion

        #region ファイルの読み込み
        private static bool IsGroupFile(string pathName)        
        {
            string ext = Path.GetExtension(pathName);
            return ext == ".egr";
        }

        private static EDOModel LoadAll(string pathName)
        {
            Group group = Load<Group>(pathName);
            if (group == null)
            {
                return null;
            }

            List<StudyUnit> studyUnits = new List<StudyUnit>();
            List<string> absPaths = group.StudyUnitAbsPathNames;
            List<string> usingPaths = new List<string>();
            foreach (string absPath in absPaths)
            {
                if (usingPaths.Contains(absPath))
                {
                    //他の調査ファイルで使用済みのファイル名なのでスキップします
                    MessageBox.Show(Resources.UsedFileNameAndSkip);
                    continue;
                }
                StudyUnit studyUnit = DoLoadStudyUnit(absPath);
                if (studyUnit != null)
                {
                    studyUnits.Add(studyUnit);
                    usingPaths.Add(absPath);
                }
            }
            if (studyUnits.Count == 0)
            {
                return null;
            }
            EDOModel edoModel = new EDOModel();
            edoModel.Group = group;
            foreach (StudyUnit studyUnit in studyUnits)
            {
                edoModel.StudyUnits.Add(studyUnit);
            }
            return edoModel;
        }

        private static StudyUnit DoLoadStudyUnit(string pathName)
        {
            StudyUnit studyUnit = Load<StudyUnit>(pathName);
            if (studyUnit == null)
            {
                return null;
            }

            // 古いデータファイル(データの収集方法がタブ化されていないデータファイルの読み込み対応)
            if (studyUnit.Sampling != null && studyUnit.Samplings.Count == 0)
            {
                Sampling sampling = studyUnit.Sampling;
                sampling.Universes.AddRange(studyUnit.Universes);
                studyUnit.Samplings.Add(studyUnit.Sampling);
            }
            studyUnit.Sampling = null;
            studyUnit.Universes.Clear();
            return studyUnit;
        }

        private static EDOModel LoadStudyUnit(string pathName)
        {
            StudyUnit studyUnit = DoLoadStudyUnit(pathName);

            EDOModel newEdoModel = new EDOModel();
            newEdoModel.StudyUnits.Add(studyUnit);
            return newEdoModel;
        }

        public static EDOModel LoadFile()
        {
            string pathName = IOUtils.QueryOpenPathName(STUDYUNIT_FILTER + "|" + GROUP_FILTER);
            return LoadFile(pathName);
        }

        public static EDOModel LoadFile(string pathName)
        {
            if (string.IsNullOrEmpty(pathName))
            {
                return null;
            }
            EDOModel edoModel = null;
            try {
                if (IsGroupFile(pathName))
                {
                    edoModel = LoadAll(pathName);
                }
                else
                {
                    edoModel = LoadStudyUnit(pathName);
                }
            }
            catch (Exception ex)
            {
                //エラーが発生しました。
                MessageBox.Show(Resources.ErrorOccurred + ":" + ex.Message);
            }
            return edoModel;
        }

        private static bool SaveStudyUnit(StudyUnitVM studyUnit, bool queryPathName, List<string> usingPaths)
        {
            if (!QuerySavePathNamesUniq(studyUnit, STUDYUNIT_FILTER, queryPathName, usingPaths))
            {
                return false;
            }
            EDOSerializer.DoSave<StudyUnit>(studyUnit.StudyUnitModel);
            return true;
        }

        private static bool SaveGroup(GroupVM group, List<string> studyUnitPathNames, bool queryPathName)
        {
            if (!QuerySavePathName(group, GROUP_FILTER, queryPathName))
            {
                return false;
            }
            string baseDir = Path.GetDirectoryName(group.PathName) + Path.DirectorySeparatorChar;
            group.GroupModel.StudyUnitRelPathNames.Clear();
            foreach (string absPath in studyUnitPathNames)
            {
                string relPath = EDOUtils.AbsToRel(absPath, baseDir);
                group.GroupModel.StudyUnitRelPathNames.Add(relPath);
            }
            EDOSerializer.DoSave<Group>(group.GroupModel);
            return true;
        }


        public static bool SaveFile(GroupVM group, List<StudyUnitVM> studyUnits, bool queryPathName)
        {
            List<string> usingPathNames = new List<string>();
            foreach (StudyUnitVM studyUnit in studyUnits)
            {
                if (!SaveStudyUnit(studyUnit, queryPathName, usingPathNames))
                {
                    return false;
                }
            }
            if (group == null)
            {
                return true;
            }
            return SaveGroup(group, usingPathNames, queryPathName);
        }

        #endregion

        #region 調査ファイルの保存

        private static void DoSave<T>(T data) where T : class, IFile
        {
            try
            {
                Debug.Assert(!string.IsNullOrEmpty(data.PathName));
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream fs = new FileStream(data.PathName, FileMode.Create);
                serializer.Serialize(fs, data);
                fs.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw ex;
            }
        }

        public static bool SaveStudyUnit(StudyUnitVM studyUnit, bool queryPathName, List<StudyUnitVM> otherStudyUnits)
        {
            List<string> usingPathNames = StudyUnitVM.GetAllPathNames(otherStudyUnits);
            if (!QuerySavePathNamesUniq(studyUnit, STUDYUNIT_FILTER, queryPathName, usingPathNames))
            {
                return false;
            }
            EDOSerializer.DoSave<StudyUnit>(studyUnit.StudyUnitModel);
            return true;
        }

        #endregion 調査ファイルの保存

        #region 調査ファイルの読み込み

        public static T Load<T>(string pathName) where T :class, IFile
        {
            Debug.Assert(!string.IsNullOrEmpty(pathName));

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                FileStream fs = new FileStream(pathName, FileMode.Open);
                object result = serializer.Deserialize(fs);
                fs.Close();
                T unit = result as T;
                if (unit == null)
                {
                    return null;
                }
                unit.PathName = pathName;
                return unit;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw ex;
            }
        }

        public static StudyUnit LoadStudyUnit()
        {
            return LoadStudyUnitInGroup(new List<StudyUnitVM>());
        }

        public static StudyUnit LoadStudyUnitInGroup(List<StudyUnitVM> otherStudyUnits)
        {
            List<string> usingPathNames = StudyUnitVM.GetAllPathNames(otherStudyUnits);
            string pathName = QueryOpenPathNameUniq(STUDYUNIT_FILTER, usingPathNames);
            if (string.IsNullOrEmpty(pathName))
            {
                return null;
            }
            return Load<StudyUnit>(pathName);
        }

        #endregion

        public static string ComputeHash(EDOModel model)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EDOModel));
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, model);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            ms.Position = 0;
            var bs =  md5.ComputeHash(ms);
            ms.Close();
            return BitConverter.ToString(bs).ToLower().Replace("-", "");
        }

        public static bool IsNotSame(EDOModel model1, EDOModel model2)
        {
            string hash1 = ComputeHash(model1);
            string hash2 = ComputeHash(model2);
            return (hash1 != hash2);
        }

        private static void CopyPath(EDOModel fromModel, EDOModel toModel)
        {
            if (fromModel != null && toModel != null)
            {
                if (fromModel.Group != null && toModel.Group != null)
                {
                    toModel.Group.PathName = fromModel.Group.PathName;
                }
                for (int i = 0; i < fromModel.StudyUnits.Count && i < toModel.StudyUnits.Count; i++)
                {
                    StudyUnit fromStudyUnit = fromModel.StudyUnits[i];
                    StudyUnit toStudyUnit = toModel.StudyUnits[i];
                    toStudyUnit.PathName = fromStudyUnit.PathName;
                }
            }
        }

        public static EDOModel Clone(EDOModel edoModel)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(EDOModel));
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, edoModel);
            ms.Position = 0;
            object result = serializer.Deserialize(ms);
            EDOModel newEdoModel = (EDOModel)result;
            CopyPath(edoModel, newEdoModel);
            return newEdoModel;
        }

        public static void SaveDebug(string path, EDOModel model)        
        {
            try
            {
                Debug.Assert(!string.IsNullOrEmpty(path));
                XmlSerializer serializer = new XmlSerializer(typeof(EDOModel));
                FileStream fs = new FileStream(path, FileMode.Create);
                serializer.Serialize(fs, model);
                fs.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                throw ex;
            }
        }
    }
}
