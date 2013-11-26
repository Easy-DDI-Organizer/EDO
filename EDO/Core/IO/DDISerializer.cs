using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using EDO.Core.Model;
using EDO.Properties;

namespace EDO.Core.IO
{
    public static class DDISerializer
    {
        private static readonly string DDI_FILE_FILTER = Resources.DDIFileFilter;
        private static readonly string DDI_GROUP_FILTER = Resources.DDIGroupFilter;
        private const int DDI3_FILTER_INDEX = 1;
        private const int DDI2_FILTER_INDEX = 2;

        public static void ExportStudyUnit(EDOConfig config, StudyUnitVM studyUnit)
        {
            FileDialogResult result = IOUtils.QuerySavePathNameEx(string.Format(Resources.StudyUnitExport, studyUnit.Title), null, DDI_FILE_FILTER, true);
            if (result == null)
            {
                return;
            }
            if (result.FilterIndex == DDI3_FILTER_INDEX)
            {
                DDI3Writer writer = new DDI3Writer(config);
                writer.WriteStudyUnit(result.FileName, studyUnit);
                DDI3Reader.Validate(result.FileName);
            }
            else if (result.FilterIndex == DDI2_FILTER_INDEX)
            {
                DDI2Writer writer = new DDI2Writer(config);
                writer.WriteCodebook(result.FileName, studyUnit);
                DDI2Reader.Validate(result.FileName);
            }
        }

        public static void ExportGroup(EDOConfig config, GroupVM group, List<StudyUnitVM> studyUnits)
        {
            string path = IOUtils.QuerySavePathName(Resources.GroupExport, null, DDI_GROUP_FILTER, true);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            DDI3Writer writer = new DDI3Writer(config);
            writer.WriteGroup(path, group, studyUnits);
            DDI3Reader.Validate(path);
        }

        public static bool ImportDDI(StudyUnitVM studyUnit, EDOModel edoModel)
        {
            FileDialogResult result = IOUtils.QueryOpenPathNameEx(DDI_FILE_FILTER);
            if (result == null)
            {
                return false;
            }
            if (result.FilterIndex == DDI3_FILTER_INDEX)
            {
                DDI3Reader reader = new DDI3Reader();
                return reader.Read(studyUnit, edoModel, result.FileName);
            }
            else if (result.FilterIndex == DDI2_FILTER_INDEX)
            {
                DDI2Reader reader = new DDI2Reader();
                return reader.Read(studyUnit, edoModel, result.FileName);
            }
            return false;
        }
    }
}
