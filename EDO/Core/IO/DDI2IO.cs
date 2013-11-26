using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using EDO.Core.Model;
using EDO.StudyCategory.MemberForm;

namespace EDO.Core.IO
{
    public class DDI2IO :DDIIO
    {
        #region for DDI 2.5

        public static readonly XNamespace cb = "ddi:codebook:2_5";
        public static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public static readonly XNamespace dc = "http://purl.org/dc/elements/1.1/";
        public static readonly XNamespace terms = "http://purl.org/dc/terms/";
        public static readonly XNamespace schemaLocation = "ddi:codebook:2_5 codebook.xsd";

        public const string TAG_CODEBOOK = "codeBook";
        public const string TAG_DOC_DSCR = "docDscr";
        public const string TAG_STDY_DSCR = "stdyDscr";
        public const string TAG_CITATION = "citation";
        public const string TAG_TITL_STMT = "titlStmt";
        public const string TAG_TITL = "titl";
        public const string TAG_RSP_STMT = "rspStmt";
        public const string TAG_AUTH_ENTY = "AuthEnty";
        public const string TAG_OTH_ID = "othId";
        public const string TAG_STDY_INFO = "stdyInfo";
        public const string TAG_ABSTRACT = "abstract";
        public const string TAG_SUBJECT = "subject";
        public const string TAG_KEYWORD = "keyword";
        public const string TAG_TOPC_CLAS = "topcClas";
        public const string TAG_SUM_DSCR = "sumDscr";
        public const string TAG_TIME_PRD = "timePrd";
        public const string TAG_GEOG_COVER = "geogCover";
        public const string TAG_PROD_STMT = "prodStmt";
        public const string TAG_FUND_AG = "fundAg";
        public const string TAG_GRANT_NO = "grantNo";
        public const string TAG_UNIVERSE = "universe";
        public const string TAG_METHOD = "method";
        public const string TAG_DATA_COLL = "dataColl";
        public const string TAG_SAMP_PROC = "sampProc";
        public const string TAG_COLL_DATE = "collDate";
        public const string TAG_DATA_COLLECTOR = "dataCollector";
        public const string TAG_COLL_MODE = "collMode";
        public const string TAG_COLL_SITU = "collSitu";
        public const string TAG_DATA_DSCR = "dataDscr";
        public const string TAG_VAR_GRP = "varGrp";
        public const string TAG_CONCEPT = "concept";
        public const string TAG_DEFNTH = "defntn";
        public const string TAG_VAR = "var";
        public const string TAG_QSTN = "qstn";
        public const string TAG_QSTN_LIT = "qstnLit";
        public const string TAG_VAR_FORMAT = "varFormat";
        public const string TAG_CATGRY = "catgry";
        public const string TAG_CAT_VALU = "catValu";
        public const string TAG_LABL = "labl";
        public const string TAG_VALRNG = "valrng";
        public const string TAG_RANGE = "range";
        public const string TAG_CATGRY_GRP = "catgryGrp";
        public const string TAG_TXT = "txt";
        public const string TAG_FILE_DSCR = "fileDscr";
        public const string TAG_FILE_TXT = "fileTxt";
        public const string TAG_FILE_NAME = "fileName";
        public const string TAG_FILE_CONT = "fileCont";
        public const string TAG_FORMAT = "format";
        public const string TAG_OTHR_STDY_MAT = "othrStdyMat";
        public const string TAG_REL_MAT = "relMat";
        public const string TAG_PRODUCER = "producer";
        public const string TAG_PROD_DATE = "prodDate";
        public const string TAG_DATE = "date";
        public const string TAG_MARCURI = "MARCURI";
        public const string TAG_COPYRIGHT = "copyright";
        public const string TAG_DIST_STMT = "distStmt";
        public const string TAG_DIST_DATE = "distDate";
        public const string TAG_BIBL_CIT = "biblCit";
        public const string TAG_HOLDINGS = "holdings";
        public const string TAG_LANGUAGE = "language";
        public const string TAG_PUBLISHER = "publisher";

        public const string ATTR_ID = "ID";
        public const string ATTR_VERSION = "version";
        public const string ATTR_DATE = "date";
        public const string ATTR_EVENT = "event";
        public const string ATTR_AFFILIATION = "affiliation";
        public const string ATTR_VAR_GRP = "varGrp";
        public const string ATTR_VAR = "var";
        public const string ATTR_NAME = "name";
        public const string ATTR_TYPE = "type";
        public const string ATTR_SCHEMA = "schema";
        public const string ATTR_CATEGORY = "category";
        public const string ATTR_MAX = "max";
        public const string ATTR_MIN = "min";
        public const string ATTR_CATGRY = "catgry";
        public const string ATTR_URI = "URI";
        public const string ATTR_RESPONSE_DOMAIN_TYPE = "responseDomainType";
        public const string ATTR_INTRVL = "intrvl";
        public const string ATTR_REPRESENTATION_TYPE = "representationType";
        public const string ATTR_ROLE = "role";
        public const string ATTR_FILES = "files";

        #endregion

        public static string ToString(DateUnit dateUnit)
        {
            if (dateUnit == null)
            {
                return "";
            }
            DateUnitType type = dateUnit.DateUnitType;
            if (type == DateUnitType.YearMonthDay)
            {
                return dateUnit.ToYearMonthDayString();
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
        public static string ToSimpleDate(string str)
        {
            DateUnit dateUnit = ParseDateUnit(str);
            if (dateUnit == null)
            {
                return "";
            }
            return ToString(dateUnit);
        }

        protected static string ToString(MemberVM member)
        {
            return member.LastName + ", " + member.FirstName;
        }


        protected static void SetupMemberName(string str, Member member)
        {
            List<string> elems = str.Split(',').Select(p => p.Trim()).ToList();
            if (elems.Count != 2)
            {
                return;
            }
            string lastName = elems[0];
            string firstName = elems[1];
            if (!string.IsNullOrEmpty(lastName))
            {
                member.LastName = lastName;
            }
            if (!string.IsNullOrEmpty(firstName))
            {
                member.FirstName = firstName;
            }
        }

        protected static string ToString(Universe universe)
        {
            return universe.Title + ": " + universe.Memo;
        }


        protected static void SetupUniverse(string str, Universe universe)
        {
            List<string> elems = str.Split(':').Select(p => p.Trim()).ToList();
            if (elems.Count != 2)
            {
                return;
            }
            string title = elems[0];
            string memo = elems[1];
            if (!string.IsNullOrEmpty(title))
            {
                universe.Title = title;
            }
            if (!string.IsNullOrEmpty(memo))
            {
                universe.Memo = memo;
            }
        }

        protected static List<string> SplitIds(string str)
        {
            return str.Split(' ').Select(p => p.Trim()).ToList();
        }


        // EDOのresponseTypeCodeとDDI2.5の質問のresponseDomainTypeの関連
        public static readonly Option RESPONSE_DOMAIN_TYPE_CHOICES = new Option(Options.RESPONSE_TYPE_CHOICES_CODE, "category");
        public static readonly Option RESPONSE_DOMAIN_TYPE_NUMBER = new Option(Options.RESPONSE_TYPE_NUMBER_CODE, "numeric");
        public static readonly Option RESPONSE_DOMAIN_TYPE_FREE = new Option(Options.RESPONSE_TYPE_FREE_CODE, "text");
        public static readonly Option RESPONSE_DOMAIN_TYPE_DATETIME = new Option(Options.RESPONSE_TYPE_DATETIME_CODE, "datetime");

        public static Option[] ResponseDomainTypes
        {
            get
            {
                return new Option[] {
                    RESPONSE_DOMAIN_TYPE_CHOICES,
                    RESPONSE_DOMAIN_TYPE_NUMBER,
                    RESPONSE_DOMAIN_TYPE_FREE,
                    RESPONSE_DOMAIN_TYPE_DATETIME
                };
            }
        }
    
        public static string GetResponseDomainType(string responseTypeCode)
        {
            return Options.FindLabel(ResponseDomainTypes, responseTypeCode);
        }

        public static string GetTypeFromResponseDomainType(string responseDomainType)
        {
            return Options.FindCodeByLabel(ResponseDomainTypes, responseDomainType);
        }

        public static readonly Option REPRESENTATION_TYPE_CHOICES = new Option(Options.RESPONSE_TYPE_CHOICES_CODE, "code");
        public static readonly Option REPRESENTATION_TYPE_NUMBER = new Option(Options.RESPONSE_TYPE_NUMBER_CODE, "numeric");
        public static readonly Option REPRESENTATION_TYPE_FREE = new Option(Options.RESPONSE_TYPE_FREE_CODE, "text");
        public static readonly Option REPRESENTATION_TYPE_DATETIME = new Option(Options.RESPONSE_TYPE_DATETIME_CODE, "datetime");

        public static Option[] RepresentationTypes
        {
            get
            {
                return new Option[] {
                    REPRESENTATION_TYPE_CHOICES,
                    REPRESENTATION_TYPE_NUMBER,
                    REPRESENTATION_TYPE_FREE,
                    REPRESENTATION_TYPE_DATETIME
                };
            }
        }

        public static string GetRepresentationType(string responseTypeCode)
        {
            return Options.FindLabel(RepresentationTypes, responseTypeCode);
        }


        public static string GetTypeFromRepresentationType(string representationType)
        {
            return Options.FindCodeByLabel(RepresentationTypes, representationType);
        }

    }
}
