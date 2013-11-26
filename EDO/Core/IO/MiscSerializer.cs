using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Main;
using EDO.Core.View;
using EDO.QuestionCategory.SequenceForm;
using EDO.DataCategory.DataFileForm;
using System.Windows;
using EDO.Core.Util;
using EDO.Properties;

namespace EDO.Core.IO
{
    public static class MiscSerializer
    {

        private static readonly string QUESTIONNAIRE_FILTER = Resources.QuestionnaireFileFilter;
        public static void ExportQuestionnaire(EDOConfig config, StudyUnitVM studyUnit)
        {
            ControlConstructSchemeVM controlConstructScheme = null;
            if (studyUnit.ControlConstructSchemes.Count > 1)
            {
                SelectObjectWindowVM<ControlConstructSchemeVM> vm = new SelectObjectWindowVM<ControlConstructSchemeVM>(studyUnit.ControlConstructSchemes);
                SelectObjectWindow window = new SelectObjectWindow(vm);
                controlConstructScheme = SelectObjectWindow.Select(Resources.SelectOrder, vm) as ControlConstructSchemeVM; //順序の選択
            }
            else if (studyUnit.ControlConstructSchemes.Count == 1)
            {
                controlConstructScheme = studyUnit.ControlConstructSchemes[0];
            }
            if (controlConstructScheme == null)
            {
                return;
            }
            string path = IOUtils.QuerySavePathName(Resources.ExportQuestionnair + ": " + controlConstructScheme.Title , null, QUESTIONNAIRE_FILTER, true);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                QuestionnaireWriter writer = new QuestionnaireWriter(config, controlConstructScheme);
                writer.Write(path);
            }
            catch (Exception ex)
            {
                EDOUtils.ShowUnexpectedError(ex);
            }
        }

        private static readonly string CODEBOOK_FILTER = Resources.CodeBookFileFilter;
        public static void ExportCodebook(StudyUnitVM studyUnit)
        {
            string path = IOUtils.QuerySavePathName(Resources.ExportCodebook, null, CODEBOOK_FILTER, true); //コードブックのエクスポート
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                CodebookWriter writer = new CodebookWriter(studyUnit);
                writer.Write(path);
            }
            catch (Exception ex)
            {
                EDOUtils.ShowUnexpectedError(ex);
            }
        }

        private static readonly string SPSS_FILTER = Resources.SPSSFileFilter;
        public static bool ImportSpss(StudyUnitVM studyUnit)
        {            
            string path = IOUtils.QueryOpenPathName(SPSS_FILTER);
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                SpssReader reader = new SpssReader();
                return reader.Read(path, studyUnit);
            }
            catch (Exception ex)
            {
                EDOUtils.ShowUnexpectedError(ex);
            }
            return false;
        }

        private static readonly string SETUPSYNTAX_FILTER = Resources.SetupSyntaxFileFilter;
        public static void ExportSetupSyntax(StudyUnitVM studyUnit)
        {
            DataFileVM dataFile = null;
            if (studyUnit.DataFiles.Count > 1)
            {
                SelectObjectWindowVM<DataFileVM> vm = new SelectObjectWindowVM<DataFileVM>(studyUnit.DataFiles);
                SelectObjectWindow window = new SelectObjectWindow(vm);
                dataFile = SelectObjectWindow.Select(Resources.SelectDataFile, vm) as DataFileVM; //データファイルの選択
            }
            else if (studyUnit.DataFiles.Count == 1)
            {
                dataFile = studyUnit.DataFiles[0];
            }
            if (dataFile == null)
            {
                return;
            }
            string path = IOUtils.QuerySavePathName(Resources.ExportSetupSyntax, null, SETUPSYNTAX_FILTER, true); //セットアップシンタックスのエクスポート
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                SetupSyntaxWriter writer = new SetupSyntaxWriter(studyUnit, dataFile);
                writer.Write(path);
            }
            catch (Exception ex)
            {
                EDOUtils.ShowUnexpectedError(ex);
            }
        }
    }
}
