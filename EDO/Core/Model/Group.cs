using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using EDO.Core.Util;
using System.Diagnostics;
using EDO.Properties;

namespace EDO.Core.Model
{
    public class Group :IFile
    {
        private static readonly string SHARED_STUDY_UNIT_TITLE = Resources.CommonItem; //共通項目

        public static bool IsSharedStudyUnit(StudyUnit studyUnit)
        {
            return studyUnit.Abstract.Title == SHARED_STUDY_UNIT_TITLE;
        }

        public static Group CreateDefault()
        {
            Group group = new Group();
            group.Title = Resources.Group; //グループ
            group.TimeCode = Options.TIME_0_CODE;
            group.InstrumentCode = Options.INSTRUMENT_0_CODE;
            group.PanelCode = Options.PANEL_0_CODE;
            group.GeographyCode = Options.GEOGRAPHY_0_CODE;
            group.DataSetCode = Options.DATASET_0_CODE;
            group.LanguageCode = Options.LANGUAGE_0_CODE;
            return group;
        }

        public Group()
        {
            InstanceId = IDUtils.NewGuid();
            Id = IDUtils.NewGuid();
            this.studyUnitRelPathNames = new List<string>();
            ConceptSchemeCompareTable = new CompareTable() { CompareType = CompareTable.CompareTypes.ConceptScheme };
            ConceptCompareTable = new CompareTable() { CompareType = CompareTable.CompareTypes.Concept };
            VariableCompareTable = new CompareTable() { CompareType = CompareTable.CompareTypes.Variable };
            PurposeId = IDUtils.NewGuid();
            ComparisonId = IDUtils.NewGuid();
            SharedStudyUnit = new StudyUnit(); //グループ内で共有する変数など
            SharedStudyUnit.Abstract.Title = SHARED_STUDY_UNIT_TITLE;
            Organization organization = new Organization();
            Member member = new Member();
            member.OrganizationId = organization.Id;
            SharedStudyUnit.AddMember(member);
            SharedStudyUnit.AddOrganization(organization);

            SharedStudyUnit.Samplings.Add(new Sampling());
            SharedStudyUnit.Samplings[0].MemberId = member.Id;
            SharedStudyUnit.Samplings[0].DateRange = new DateRange(DateTime.Today, DateTime.Today);

        }

        public string InstanceId { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Purpose { get; set; }
        public string TimeCode { get; set; }
        public string InstrumentCode { get; set; }
        public string PanelCode { get; set; }
        public string GeographyCode { get; set; }
        public string DataSetCode { get; set; }
        public string LanguageCode { get; set; }
        public StudyUnit SharedStudyUnit { get; set; }
        public StudyUnit GetClearedSharedStudyUnit()
        {
            foreach (Sampling sampling in SharedStudyUnit.Samplings)
            {
                sampling.Universes.Clear();
            }
            SharedStudyUnit.ConceptSchemes.Clear();
            SharedStudyUnit.Variables.Clear();
            SharedStudyUnit.CodeSchemes.Clear();
            SharedStudyUnit.CategorySchemes.Clear();
            return SharedStudyUnit;
        }

        public CompareTable ConceptSchemeCompareTable { get; set; }
        public CompareTable ConceptCompareTable { get; set; }
        public CompareTable VariableCompareTable { get; set; }

        private List<string> studyUnitRelPathNames;
        public List<string> StudyUnitRelPathNames {
            get
            {
                return studyUnitRelPathNames;
            }
        }

        public string PurposeId { get; set; }
        public string ComparisonId { get; set; }

        [XmlIgnoreAttribute]
        public string PathName { get; set; }

        [XmlIgnoreAttribute]
        public List<string> StudyUnitAbsPathNames 
        {
            get
            {
                //グループファイルを保存後じゃないとこのメソッドは呼べない
                Debug.Assert(!string.IsNullOrEmpty(PathName));

                string baseDir = Path.GetDirectoryName(PathName) + Path.DirectorySeparatorChar;
                List<string> absPaths = new List<string>();
                foreach (string relPath in StudyUnitRelPathNames) 
                {
                    string absPath = EDOUtils.RelToAbs(relPath, baseDir);
                    absPaths.Add(absPath);
                }
                return absPaths;
            }
        }

    }
}
