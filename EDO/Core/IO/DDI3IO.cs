using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Windows;
using EDO.Properties;
using EDO.Core.Util;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace EDO.Core.IO
{
    public class DDI3IO :DDIIO
    {
        #region for DDI 3.1

        public static readonly XNamespace ddi = "ddi:instance:3_1";
        public static readonly XNamespace s = "ddi:studyunit:3_1";
        public static readonly XNamespace r = "ddi:reusable:3_1";
        public static readonly XNamespace a = "ddi:archive:3_1";
        public static readonly XNamespace c = "ddi:conceptualcomponent:3_1";
        public static readonly XNamespace d = "ddi:datacollection:3_1";
        public static readonly XNamespace l = "ddi:logicalproduct:3_1";
        public static readonly XNamespace p = "ddi:physicaldataproduct:3_1";
        public static readonly XNamespace pi = "ddi:physicalinstance:3_1";
        public static readonly XNamespace g = "ddi:group:3_1";
        public static readonly XNamespace cm = "ddi:comparative:3_1";
        public static readonly XNamespace dce = "ddi:dcelements:3_1";
        public static readonly XNamespace dc = "http://purl.org/dc/elements/1.1/";

        public const string AGENCY = "SSJDA";
        public const string VERSION = "1.0.0";

        public const string TAG_USER_ID = "UserID";
        public const string TAG_ID = "ID";
        public const string TAG_CONTENT = "Content";
        public const string TAG_IDENTIFYING_AGENCY = "IdentifyingAgency";
        public const string TAG_VERSION = "Version";
        public const string TAG_LABEL = "Label";
        public const string TAG_DESCRIPTION = "Description";
        public const string TAG_SIMPLE_DATE = "SimpleDate";
        public const string TAG_START_DATE = "StartDate";
        public const string TAG_END_DATE = "EndDate";
        public const string TAG_AGENCY_ORGANIZATION_REFERENCE = "AgencyOrganizationReference";
        public const string TAG_DATE = "Date";
        public const string TAG_LIFECYCLE_EVENT = "LifecycleEvent";
        public const string TAG_INDIVIDUAL = "Individual";
        public const string TAG_INDIVIDUAL_NAME = "IndividualName";
        public const string TAG_FIRST = "First";
        public const string TAG_LAST = "Last";
        public const string TAG_POSITION = "Position";
        public const string TAG_TITLE = "Title";
        public const string TAG_RELATION = "Relation";
        public const string TAG_ORGANIZATION_REFERENCE = "OrganizationReference";
        public const string TAG_ORGANIZATION = "Organization";
        public const string TAG_ORGANIZATION_NAME = "OrganizationName";
        public const string TAG_ARCHIVE = "Archive";
        public const string TAG_ARCHIVE_SPECIFIC = "ArchiveSpecific";
        public const string TAG_ARCHIVE_ORGANIZATION_REFERENCE = "ArchiveOrganizationReference";
        public const string TAG_ORGANIZATION_SCHEME = "OrganizationScheme";
        public const string TAG_LIFECYCLE_INFORMATION = "LifecycleInformation";
        public const string TAG_TOPICAL_COVERAGE = "TopicalCoverage";
        public const string TAG_SUBJECT = "Subject";
        public const string TAG_KEYWORD = "Keyword";
        public const string TAG_TEMPORAL_COVERAGE = "TemporalCoverage";
        public const string TAG_REFERENCE_DATE = "ReferenceDate";
        public const string TAG_SPATIAL_COVERAGE = "SpatialCoverage";
        public const string TAG_GEOGRAPHIC_STRUCTURE_REFERENCE = "GeographicStructureReference";
        public const string TAG_TOP_LEVEL_REFERENCE = "TopLevelReference";
        public const string TAG_LEVEL_REFERENCE = "LevelReference";
        public const string TAG_LEVEL_NAME = "LevelName";
        public const string TAG_LOWEST_LEVEL_REFERENCE = "LowestLevelReference";
        public const string TAG_COVERAGE = "Coverage";
        public const string TAG_FUNDING_INFORMATION = "FundingInformation";
        public const string TAG_GRANT_NUMBER = "GrantNumber";
        public const string TAG_CONCEPTUAL_COMPONENT = "ConceptualComponent";
        public const string TAG_CONCEPT_SCHEME = "ConceptScheme";
        public const string TAG_CONCEPT = "Concept";
        public const string TAG_CONCEPTS = "Concepts";
        public const string TAG_UNIVERSE_SCHEME = "UniverseScheme";
        public const string TAG_UNIVERSE = "Universe";
        public const string TAG_HUMAN_READABLE = "HumanReadable";
        public const string TAG_SUB_UNIVERSE = "SubUniverse";
        public const string TAG_GEOGRAPHIC_STRUCTURE_SCHEME = "GeographicStructureScheme";
        public const string TAG_GEOGRAPHIC_STRUCTURE = "GeographicStructure";
        public const string TAG_GEOGRAPHY = "Geography";
        public const string TAG_LEVEL = "Level";
        public const string TAG_NAME = "Name";
        public const string TAG_CODE_SCHEME_REFERENCE = "CodeSchemeReference";
        public const string TAG_NUMBER_RANGE = "NumberRange";
        public const string TAG_LOW = "Low";
        public const string TAG_HIGH = "High";
        public const string TAG_QUESTION_SCHEME = "QuestionScheme";
        public const string TAG_MULTIPLE_QUESTION_ITEM = "MultipleQuestionItem";
        public const string TAG_MULTIPLE_QUESTION_ITEM_NAME = "MultipleQuestionItemName";
        public const string TAG_SUB_QUESTIONS = "SubQuestions";
        public const string TAG_QUESTION_ITEM = "QuestionItem";
        public const string TAG_QUESTION_ITEM_NAME = "QuestionItemName";
        public const string TAG_QUESTION_TEXT = "QuestionText";
        public const string TAG_LITERAL_TEXT = "LiteralText";
        public const string TAG_TEXT = "Text";
        public const string TAG_CODE_DOMAIN = "CodeDomain";
        public const string TAG_NUMERIC_DOMAIN = "NumericDomain";
        public const string TAG_TEXT_DOMAIN = "TextDomain";
        public const string TAG_DATE_TIME_DOMAIN = "DateTimeDomain";
        public const string TAG_CONCEPT_REFERENCE = "ConceptReference";
        public const string TAG_DATA_COLLECTION = "DataCollection";
        public const string TAG_METHODOLOGY = "Methodology";
        public const string TAG_SAMPLING_PROCEDURE = "SamplingProcedure";
        public const string TAG_COLLECTION_EVENT = "CollectionEvent";
        public const string TAG_DATA_COLLECTOR_ORGANIZATION_REFERENCE = "DataCollectorOrganizationReference";
        public const string TAG_DATA_COLLECTION_DATE = "DataCollectionDate";
        public const string TAG_MODE_OF_COLLECTION = "ModeOfCollection";
        public const string TAG_COLLECTION_SITUATION = "CollectionSituation";
        public const string TAG_CATEGORY_SCHEME = "CategoryScheme";
        public const string TAG_CATEGORY = "Category";
        public const string TAG_CODE_SCHEME = "CodeScheme";
        public const string TAG_CODE = "Code";
        public const string TAG_CATEGORY_REFERENCE = "CategoryReference";
        public const string TAG_VALUE = "Value";
        public const string TAG_VARIABLE_SCHEME = "VariableScheme";
        public const string TAG_VARIABLE = "Variable";
        public const string TAG_VARIABLE_NAME = "VariableName";
        public const string TAG_UNIVERSE_REFERENCE = "UniverseReference";
        public const string TAG_QUESTION_REFERENCE = "QuestionReference";
        public const string TAG_REPRESENTATION = "Representation";
        public const string TAG_CODE_REPRESENTATION = "CodeRepresentation";
        public const string TAG_NUMERIC_REPRESENTATION = "NumericRepresentation";
        public const string TAG_TEXT_REPRESENTATION = "TextRepresentation";
        public const string TAG_DATE_TIME_REPRESENTATION = "DateTimeRepresentation";
        public const string TAG_DATA_RELATIONSHIP = "DataRelationship";
        public const string TAG_LOGICAL_RECORD = "LogicalRecord";
        public const string TAG_LOGICAL_RECORD_NAME = "LogicalRecordName";
        public const string TAG_VARIABLES_IN_RECORD = "VariablesInRecord";
        public const string TAG_VARIABLE_SCHEME_REFERENCE = "VariableSchemeReference";
        public const string TAG_VARIABLE_USED_REFERENCE = "VariableUsedReference";
        public const string TAG_LOGICAL_PRODUCT = "LogicalProduct";
        public const string TAG_PHYSICAL_DATA_PRODUCT = "PhysicalDataProduct";
        public const string TAG_PHYSICAL_STRUCTURE_SCHEME = "PhysicalStructureScheme";
        public const string TAG_PHYSICAL_STRUCTURE = "PhysicalStructure";
        public const string TAG_LOGICAL_PRODUCT_REFERENCE = "LogicalProductReference";
        public const string TAG_FORMAT = "Format";
        public const string TAG_DEFAULT_DELIMITER = "DefaultDelimiter";
        public const string TAG_GROSS_RECORD_STRUCTURE = "GrossRecordStructure";
        public const string TAG_LOGICAL_RECORD_REFERENCE = "LogicalRecordReference";
        public const string TAG_PHYSICAL_RECORD_SEGMENT = "PhysicalRecordSegment";
        public const string TAG_RECORD_LAYOUT_SCHEME = "RecordLayoutScheme";
        public const string TAG_PHYSICAL_STRUCTURE_REFERENCE = "PhysicalStructureReference";
        public const string TAG_PHYSICAL_RECORD_SEGMENT_USED = "PhysicalRecordSegmentUsed";
        public const string TAG_RECORD_LAYOUT = "RecordLayout";
        public const string TAG_CHARACTER_SET = "CharacterSet";
        public const string TAG_ARRAY_BASE = "ArrayBase";
        public const string TAG_DATA_ITEM = "DataItem";
        public const string TAG_VARIABLE_REFERENCE = "VariableReference";
        public const string TAG_PHYSICAL_LOCATION = "PhysicalLocation";
        public const string TAG_ARRAY_POSITION = "ArrayPosition";
        public const string TAG_PHYSICAL_INSTANCE = "PhysicalInstance";
        public const string TAG_RECORD_LAYOUT_REFERENCE = "RecordLayoutReference";
        public const string TAG_DATA_FILE_IDENTIFICATION = "DataFileIdentification";
        public const string TAG_GROSS_FILE_STRUCTURE = "GrossFileStructure";
        public const string TAG_CASE_QUANTITY = "CaseQuantity";
        public const string TAG_OVERALL_RECORD_COUNT = "OverallRecordCount";
        public const string TAG_URI = "URI";
        public const string TAG_STUDY_UNIT = "StudyUnit";
        public const string TAG_CITATION = "Citation";
        public const string TAG_ABSTRACT = "Abstract";
        public const string TAG_PURPOSE = "Purpose";
        public const string TAG_CONCEPT_MAP = "ConceptMap";
        public const string TAG_SOURCE_SCHEME_REFERENCE = "SourceSchemeReference";
        public const string TAG_TARGET_SCHEME_REFERENCE = "TargetSchemeReference";
        public const string TAG_CORRESPONDENCE = "Correspondence";
        public const string TAG_COMMONALITY = "Commonality";
        public const string TAG_DIFFERENCE = "Difference";
        public const string TAG_COMMONALITY_WEIGHT = "CommonalityWeight";
        public const string TAG_ITEM_MAP = "ItemMap";
        public const string TAG_SOURCE_ITEM = "SourceItem";
        public const string TAG_TARGET_ITEM = "TargetItem";
        public const string TAG_VARIABLE_MAP = "VariableMap";
        public const string TAG_GROUP = "Group";
        public const string TAG_COMPARISON = "Comparison";
        public const string TAG_DDI_INSTANCE = "DDIInstance";
        public const string TAG_CONTROL_CONSTRUCT_SCHEME = "ControlConstructScheme";
        public const string TAG_QUESTION_CONSTRUCT = "QuestionConstruct";
        public const string TAG_CONTROL_CONSTRUCT_SCHEME_NAME = "ControlConstructSchemeName";
        public const string TAG_STATEMENT_ITEM = "StatementItem";
        public const string TAG_DISPLAY_TEXT = "DisplayText";
        public const string TAG_SEQUENCE = "Sequence";
        public const string TAG_CONTROL_CONSTRUCT_REFERENCE = "ControlConstructReference";
        public const string TAG_IF_THEN_ELSE = "IfThenElse";
        public const string TAG_IF_CONDITION = "IfCondition";
        public const string TAG_SOURCE_QUESTION_REFERENCE = "SourceQuestionReference";
        public const string TAG_THEN_CONSTRUCT_REFERENCE = "ThenConstructReference";
        public const string TAG_ELSE_CONSTRUCT_REFERENCE = "ElseConstructReference";
        public const string TAG_ELSE_IF = "ElseIf";
        public const string TAG_SCHEME = "Scheme";
        public const string TAG_MODULE = "Module";
        public const string TAG_OTHER_MATERIAL = "OtherMaterial";
        public const string TAG_CREATOR = "Creator";
        public const string TAG_PUBLISHER = "Publisher";
        public const string TAG_CONTRIBUTOR = "Contributor";
        public const string TAG_PUBLICATION_DATE = "PublicationDate";
        public const string TAG_LANGUAGE = "Language";
        public const string TAG_DCELEMENTS = "DCElements";
        public const string TAG_EXTERNAL_URL_REFERENCE = "ExternalURLReference";
        public const string TAG_RELATIONSHIP = "Relationship";
        public const string TAG_RELATED_TO_REFERENCE = "RelatedToReference";
        public const string TAG_DC_IDENTIFIER = "identifier";
        public const string TAG_DC_DESCRIPTION = "description";

        public const string ATTR_ID = "id";
        public const string ATTR_TYPE = "type";
        public const string ATTR_MAX_LENGTH = "maxLength";
        public const string ATTR_MIN_LENGTH = "minLength";
        public const string ATTR_HAS_LOCATOR = "hasLocator";
        public const string ATTR_ALL_VARIABLES_IN_LOGICAL_PRODUCT = "allVariablesInLogicalProduct";
        public const string ATTR_DATA_SET = "dataSet";
        public const string ATTR_GEOGRAPHY = "geography";
        public const string ATTR_INSTRUMENT = "instrument";
        public const string ATTR_LANGUAGE = "language";
        public const string ATTR_PANEL = "panel";
        public const string ATTR_TIME = "time";
        public const string ATTR_VERSION = "version";
        public const string ATTR_AGENCY = "agency";
        public const string ATTR_MISSING = "missing";
        public const string ATTR_MISSING_VALUE = "missingValue";

        public const string DDI_BOOK = "Book"; //文献種類
        public const string DDI_BOOK_CHAPTER = "Book Section"; //書籍(章など)
        public const string DDI_TREATISE_WITH_PEER_REVIEW = "Peer Reviewed Journal Article"; //査読付き学術論文
        public const string DDI_TREATISE_WITHOUT_PEER_REVIEW = "Journal Article"; //査読なし学術論文
        public const string DDI_SOCIETY_ABSTRACT = "Conference Proceedings"; //学会抄録
        public const string DDI_REPORT = "Report"; //報告書
        public const string DDI_THESIS = "Thesis"; //学位論文
        public const string DDI_WEBPAGE = "Web Page"; //ウェブページ
        public const string DDI_OTHER = "Other"; //その他

        #endregion

        const string DATERANGE_SEPARATOR = ",";

        private static readonly string LABEL_FUNDING_INFO_MONEY = Resources.FundMoneyValue; //助成金額

        private static readonly  string LABEL_FUNDING_INFO_TITLE = Resources.FundTitle; //助成タイトル

        private static readonly string LABEL_FUNDING_INFO_DATE_RANGE = Resources.FundPeriod; //助成期間

        public static string ToString(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return "";
            }
            DateTime dt = dateTime.Value;
            return dt.ToString("yyyy-MM-ddzzz");
        }

        public static string ToString(DateUnit dateUnit)
        {
            if (dateUnit == null)
            {
                return "";
            }
            DateUnitType type = dateUnit.DateUnitType;
            if (type == DateUnitType.YearMonthDay)
            {
                return ToString(dateUnit.Date);
            }
            else if (type == DateUnitType.YearMonth)
            {
                return dateUnit.ToYearMonthString();
            }
            else if (type == DateUnitType.Year)
            {
                return dateUnit.ToYearString();
            }
            return "";
        }

        public static string ToString(DateRange range)
        {
            return ToString(range.FromDateTime) + DATERANGE_SEPARATOR + ToString(range.ToDateTime);
        }

        public static DateRange ParseDateRange(string str)
        {
            return DateRange.Parse(str, DATERANGE_SEPARATOR[0]);
        }

        public static string ToSimpleDate(string str)
        {
            DateUnit dateUnit = ParseDateUnit(str);
            if (dateUnit == null)
            {
                return "";
            }
            return ToString(dateUnit);
        }


        public DDI3IO()
        {
        }
        private string ToDDIFundingInfoLabel(string label, object obj)
        {
            if (obj == null)
            {
                return null;
            }
            StringBuilder buf = new StringBuilder();
            buf.Append(label);
            buf.Append(":");
            buf.Append(obj.ToString());
            return buf.ToString();
        }

        public string ToDDIFundingInfoMoney(decimal? money)
        {
            return ToDDIFundingInfoLabel(LABEL_FUNDING_INFO_MONEY, money);
        }

        public string ToDDIFundingInfoTitle(string title)
        {
            return ToDDIFundingInfoLabel(LABEL_FUNDING_INFO_TITLE, title);
        }

        public string ToDDIFundingInfoDateRange(DateRange dateRange)
        {
            return ToDDIFundingInfoLabel(LABEL_FUNDING_INFO_DATE_RANGE, ToString(dateRange));
        }

        public static bool IsDDIFundingInfoLabel(string str, string label, ref string value)
        {
            value = null;
            string prefix = label + ":";
            if (!str.StartsWith(prefix)) 
            {
                return false;
            }
            value = str.Substring(prefix.Length);
            return true;        
        }

        public static bool IsDDIFundingInfoTitle(string str, ref string value)
        {
            return IsDDIFundingInfoLabel(str, LABEL_FUNDING_INFO_TITLE, ref value);
        }

        public static bool IsDDIFundingInfoMoney(string str, ref string value)
        {
            return IsDDIFundingInfoLabel(str, LABEL_FUNDING_INFO_MONEY, ref value);
        }

        public static bool IsDDIFundingInfoDateRange(string str, ref string value)
        {
            return IsDDIFundingInfoLabel(str, LABEL_FUNDING_INFO_DATE_RANGE, ref value);
        }

        private static void AddXsd(XmlSchemaSet sc, string name)
        {
            sc.Add(null, EDOConfig.DDI31Path + name);
        }

        public static bool Validate(string xml)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            AddXsd(settings.Schemas, "instance.xsd");
            AddXsd(settings.Schemas, "reusable.xsd");
            AddXsd(settings.Schemas, "dcelements.xsd");
            AddXsd(settings.Schemas, "studyunit.xsd");
            AddXsd(settings.Schemas, "conceptualcomponent.xsd");
            AddXsd(settings.Schemas, "datacollection.xsd");
            AddXsd(settings.Schemas, "logicalproduct.xsd");
            AddXsd(settings.Schemas, "physicaldataproduct.xsd");
            AddXsd(settings.Schemas, "physicalinstance.xsd");
            AddXsd(settings.Schemas, "archive.xsd");
            AddXsd(settings.Schemas, "dcelements.xsd");
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            //settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(xml, settings);
                while (reader.Read())
                {

                }
                return true;
            }
            catch (XmlSchemaValidationException ex)
            {
                //DDIファイルの検証に失敗しました
                string msg = Resources.DDIValidationError + ": " + ex.Message + " " + Resources.LineNumber + ":" + ex.LineNumber + " " + Resources.Position + ":" + ex.LinePosition + " Name:" + reader.Name + " Value:" + reader.Value;
                MessageBox.Show(msg);
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return false;
        }


        public static List<Option> DDIBookTypes
        {
            get {
                List<Option> options = new List<Option>();
                options.Add(new Option(Options.BOOK_TYPE_BOOK, DDI_BOOK)); //1
                options.Add(new Option(Options.BOOK_TYPE_BOOK_CHAPTER, DDI_BOOK_CHAPTER)); //2
                options.Add(new Option(Options.BOOK_TYPE_TREATISE_WITH_PEER_REVIEW, DDI_TREATISE_WITH_PEER_REVIEW)); //3
                options.Add(new Option(Options.BOOK_TYPE_TREATISE_WITHOUT_PEER_REVIEW, DDI_TREATISE_WITHOUT_PEER_REVIEW)); //4
                options.Add(new Option(Options.BOOK_TYPE_SOCIETY_ABSTRACT, DDI_SOCIETY_ABSTRACT)); //5
                options.Add(new Option(Options.BOOK_TYPE_THESIS, DDI_THESIS)); //6
                options.Add(new Option(Options.BOOK_TYPE_REPORT, DDI_REPORT)); //7
                options.Add(new Option(Options.BOOK_TYPE_WEBPAGE, DDI_WEBPAGE)); //8
                options.Add(new Option(Options.BOOK_TYPE_OTHER, DDI_OTHER)); //9
                return options;
            }
        }

        public static string GetDDIBookType(string bookTypeCode)
        {
            List<Option> options = DDIBookTypes;
            foreach (Option option in options)
            {
                if (option.Code == bookTypeCode)
                {
                    return option.Label;
                }
            }
            return null;
        }

        public static string GetBookTypeCode(string ddiBookType)
        {
            List<Option> options = DDIBookTypes;
            foreach (Option option in options)
            {
                if (option.Label == ddiBookType)
                {
                    return option.Code;
                }
            }
            return Options.BOOK_TYPE_BOOK;
        }

    }
}
