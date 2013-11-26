using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using EDO.Properties;
using System.Diagnostics;

namespace EDO.Core.Model
{
    public static class Options
    {
        # region 初期化

        private static ObservableCollection<Option> LoadOptions(XDocument xdoc, string id)
        {
            ObservableCollection<Option> options = new ObservableCollection<Option>();
            var optionCollection = (
                from p in xdoc.Descendants("Options")
                where p.Attribute("id").Value == id
                select p).Single();
            var optionElems = optionCollection.Elements("Option");
            foreach (var optionElem in optionElems)
            {
                string key = optionElem.Element("Key").Value;
                string value = optionElem.Element("Value").Value;
                Option option = new Option(key, value);
                options.Add(option);
            }
            return options;
        }

        public static void Init()
        {
            var xdoc = XDocument.Parse(Resources.Options);
            eventTypes = LoadOptions(xdoc, "EventTypes");            
            roles = LoadOptions(xdoc, "Roles");
            coverageTopics = LoadOptions(xdoc, "CoverageTopics");
            coverageAreas = LoadOptions(xdoc, "CoverageAreas");
            samplingMethods = LoadOptions(xdoc, "SamplingMethods");
            responseTypes = LoadOptions(xdoc, "ResponseTypes");
            numberTypes = LoadOptions(xdoc, "NumberTypes");
            dateTimeTypes = LoadOptions(xdoc, "DateTimeTypes");
            delimiters = LoadOptions(xdoc, "Delimiters");
            times = LoadOptions(xdoc, "Times");
            instruments = LoadOptions(xdoc, "Instruments");
            panels = LoadOptions(xdoc, "Panels");
            geographies = LoadOptions(xdoc, "Geographies");
            dataSets = LoadOptions(xdoc, "DataSets");
            languages = LoadOptions(xdoc, "Languages");
            compareValues = LoadOptions(xdoc, "CompareValues");
            positions = LoadOptions(xdoc, "Positions");
            operators = LoadOptions(xdoc, "Operators");
            connections = LoadOptions(xdoc, "Connections");
            bookTypes = LoadOptions(xdoc, "BookTypes");
            Dump();
        }

        public static void Dump()
        {
            Debug.WriteLine("EventTypes=" + eventTypes.Count);
            Debug.WriteLine("Roles=" + roles.Count);
            Debug.WriteLine("CoverageTopics=" + coverageTopics.Count);
            Debug.WriteLine("CoverageAreas=" + coverageAreas.Count);
            Debug.WriteLine("SamplingMethods=" + samplingMethods.Count);
            Debug.WriteLine("ResponseTypes=" + responseTypes.Count);
            Debug.WriteLine("NumberTypes=" + numberTypes.Count);
            Debug.WriteLine("DateTimeTypes=" + dateTimeTypes.Count);
            Debug.WriteLine("Delimiters=" + delimiters.Count);
            Debug.WriteLine("Times=" + times.Count);
            Debug.WriteLine("Instruments=" + instruments.Count);
            Debug.WriteLine("Panels=" + panels.Count);
            Debug.WriteLine("Geographies=" + geographies.Count);
            Debug.WriteLine("DataSets=" + dataSets.Count);
            Debug.WriteLine("Languages=" + languages.Count);
            Debug.WriteLine("compareValues=" + compareValues.Count);
            Debug.WriteLine("positions=" + positions.Count);
            Debug.WriteLine("operators=" + operators.Count);
            Debug.WriteLine("connections=" + connections.Count);
        }

        #endregion

        #region ユーティリティメソッド
        public static Option Find(ObservableCollection<Option> options, string code)
        {
            foreach (Option option in options)
            {
                if (option.Code == code)
                {
                    return option;
                }
            }
            return null;
        }

        private static ObservableCollection<Option> FilterIncludes(ObservableCollection<Option> source, string includeCode)
        {
            return FilterIncludes(source, new List<string>{includeCode});
        }

        private static ObservableCollection<Option> FilterIncludes(ObservableCollection<Option> source, List<string> includeCodes)
        {
            return Filter(source, includeCodes, null);
        }

        private static ObservableCollection<Option> FilterExcludes(ObservableCollection<Option> source, string excludeCode)
        {
            return FilterExcludes(source, new List<string>{excludeCode});
        }


        private static ObservableCollection<Option> FilterExcludes(ObservableCollection<Option> source, List<string> excludeCodes)
        {
            return Filter(source, null, excludeCodes);
        }

        private static ObservableCollection<Option> Filter(ObservableCollection<Option> source, List<string> includeCodes, List<string> excludeCodes)
        {
            ObservableCollection<Option> dest = new ObservableCollection<Option>();
            foreach (Option option in source)
            {
                bool match = true;
                if (includeCodes != null)
                {
                    if (!includeCodes.Contains(option.Code))
                    {
                        match = false;
                    }
                }
                if (!match)
                    continue;

                if (excludeCodes != null)
                {
                    if (excludeCodes.Contains(option.Code))
                    {
                        match = false;
                    }
                }
                if (!match)
                    continue;

                dest.Add(option);
            }
            return dest;
        }

        public static string FindLabel(IEnumerable<Option> options, string code)
        {
            return Option.FindLabel(options, code);
        }

        public static string FindCodeByLabel(IEnumerable<Option> options, string label)
        {
            return Option.FindCodeByLabel(options, label);
        }

        #endregion

        #region イベントタイプ

        private static ObservableCollection<Option> eventTypes;
        public static ObservableCollection<Option> EventTypes 
        { 
            get 
            {
                return eventTypes; 
            }
        }

        #endregion

        #region メンバーの役割

        public static string ROLE_DAIHYOSHA_CODE = "1";
        public static string ROLE_OTHER_CODE = "4";

        private static ObservableCollection<Option> roles;
        public static ObservableCollection<Option> Roles {
            get
            {
                return roles;
            }
        }

        public static ObservableCollection<Option> FirstRoles
        {
            get
            {
                return FilterIncludes(Roles, ROLE_DAIHYOSHA_CODE);
            }
        }

        public static ObservableCollection<Option> OtherRoles
        {
            get
            {
                return FilterExcludes(Roles, ROLE_DAIHYOSHA_CODE);
            }
        }

        public static string RoleLabel(string roleCode)
        {
            return FindLabel(Roles, roleCode);
        }

        public static string RoleCodeByLabel(string roleLabel)
        {
            return FindCodeByLabel(Roles, roleLabel);
        }

        #endregion 

        #region 調査のトピック

        public static string COVERAGE_TOPIC_OTHER_CODE = "13";

        private static ObservableCollection<Option> coverageTopics;
        public static ObservableCollection<Option> CoverageTopics
        {
            get
            {
                return coverageTopics;
            }
        }

        public static string CoverageTopicLabel(string code)
        {
            return FindLabel(CoverageTopics, code);
        }

        #endregion 

        #region 調査の地域

        private static ObservableCollection<Option> coverageAreas;
        public static ObservableCollection<Option> CoverageAreas
        {
            get
            {
                return coverageAreas;
            }
        }

        #endregion

        #region サンプリング方法

        private static ObservableCollection<Option> samplingMethods;
        public static ObservableCollection<Option> SamplingMethods
        {
            get
            {
                return samplingMethods;
            }
        }

        public static string SamplingMethodLabel(string code)
        {
            return FindLabel(SamplingMethods, code);
        }

        #endregion

        #region 回答方法の種類

        public static string RESPONSE_TYPE_UNKNOWN_CODE = "0";
        public static string RESPONSE_TYPE_CHOICES_CODE = "1";
        public static string RESPONSE_TYPE_NUMBER_CODE = "2";
        public static string RESPONSE_TYPE_FREE_CODE = "3";
        public static string RESPONSE_TYPE_DATETIME_CODE = "4";

        public static Option RESPONSE_TYPE_UNKNOWN = new Option(RESPONSE_TYPE_UNKNOWN_CODE, "");

        private static ObservableCollection<Option> responseTypes;
        public static ObservableCollection<Option> ResponseTypes
        {
            get
            {
                return responseTypes;
            }
        }


        #endregion

        #region 数値の型

        public static string NUMBER_TYPE_BIGINTEGER = "1";
        public static string NUMBER_TYPE_INTEGER = "2";
        public static string NUMBER_TYPE_LONG = "3";
        public static string NUMBER_TYPE_SHORT = "4";
        public static string NUMBER_TYPE_DECIMAL = "5";
        public static string NUMBER_TYPE_FLOAT = "6";
        public static string NUMBER_TYPE_DOUBLE = "7";
        public static string NUMBER_TYPE_COUNT = "8";
        public static string NUMBER_TYPE_INCREMENTAL = "9";

        private static ObservableCollection<Option> numberTypes;
        public static ObservableCollection<Option> NumberTypes
        {
            get
            {
                return numberTypes;
            }
        }

        private static Dictionary<string, string> NumberTypeDict
        {
            get
            {
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {NUMBER_TYPE_BIGINTEGER, "BigInteger"},
                    {NUMBER_TYPE_INTEGER, "Integer"},
                    {NUMBER_TYPE_LONG, "Long"},
                    {NUMBER_TYPE_SHORT, "Short"},
                    {NUMBER_TYPE_DECIMAL, "Decimal"},
                    {NUMBER_TYPE_FLOAT, "Float"},
                    {NUMBER_TYPE_DOUBLE, "Double"},
                    {NUMBER_TYPE_COUNT, "Count"},
                    {NUMBER_TYPE_INCREMENTAL, "Incremental"}
                };
                return dict;
            }
        }

        public static string NumberTypeDDILabel(string code)
        {
            var dict = NumberTypeDict;
            if (code == null || !dict.ContainsKey(code))
            {
                return null;
            }
            return dict[code];
        }

        public static string NumberTypeCode(string label)
        {
            var dict = NumberTypeDict;
            foreach (KeyValuePair<string, string> pair in dict)
            {
                if (pair.Value == label)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        #endregion

        #region 日付の型

        public static string DATETIME_DATETIME = "1";
        public static string DATETIME_DATE = "2";
        public static string DATETIME_YEARMONTH = "3";
        public static string DATETIME_YEAR = "4";
        public static string DATETIME_DURATION = "5";

        private static ObservableCollection<Option> dateTimeTypes;
        public static ObservableCollection<Option> DateTimeTypes
        {
            get
            {
                return dateTimeTypes;
            }
        }

        public static Dictionary<string, string> DateTimeTypeDict
        {
            get
            {
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {DATETIME_DATETIME, "DateTime"}, //年月日+時分
                    {DATETIME_DATE, "Date"}, //年月日
                    {DATETIME_YEARMONTH, "YearMonth"}, //年月
                    {DATETIME_YEAR, "Year"}, //年
                    {DATETIME_DURATION, "Duration"} //期間
                };
                return dict;
            }
        }

        public static string DateTimeTypeDDILabel(string code)
        {
            var dict = DateTimeTypeDict;
            if (code == null || !dict.ContainsKey(code))
            {
                return null;
            }
            return dict[code];
        }

        public static string DateTimeTypeCode(string label)
        {
            var dict = DateTimeTypeDict;
            foreach (KeyValuePair<string, string> pair in dict)
            {
                if (pair.Value == label)
                {
                    return pair.Key;
                }
            }
            return null;
        }

        public static bool IsDateTimeTypeDateTime(string code)
        {
            return DATETIME_DATETIME == code;
        }

        public static bool IsDateTimeTypeDate(string code)
        {
            return DATETIME_DATE == code;
        }

        public static bool IsDateTimeTypeYearMonth(string code)
        {
            return DATETIME_YEARMONTH == code;
        }

        public static bool IsDateTimeTypeYear(string code)
        {
            return DATETIME_YEAR == code;
        }

        public static bool IsDateTimeTypeDuration(string code)
        {
            return DATETIME_DURATION == code;
        }

        #endregion

        #region デリミタ

        public static string DELIMITER_COMMA = "1";
        public static string DELIMITER_TAB = "2";
        public static string DELIMITER_SPACE = "3";
        private static ObservableCollection<Option> delimiters;
        public static ObservableCollection<Option> Delimiters
        {
            get
            {
                return delimiters;
            }
        }

        #endregion

        #region 調査時点

        public static string TIME_0_CODE = "T0";

        private static ObservableCollection<Option> times;
        public static ObservableCollection<Option> Times
        {
            get
            {
                return times;
            }
        }

        public static string TimeLabel(string code)
        {
            return FindLabel(Times, code);
        }

        #endregion

        #region 調査票

        public static string INSTRUMENT_0_CODE = "I0";

        private static ObservableCollection<Option> instruments;
        public static ObservableCollection<Option> Instruments
        {
            get
            {
                return instruments;
            }
        }

        public static string InstrumentLabel(string code)
        {
            return FindLabel(Instruments, code);
        }

        #endregion

        #region 対象者

        public static string PANEL_0_CODE = "P0";

        private static ObservableCollection<Option> panels;
        public static ObservableCollection<Option> Panels
        {
            get
            {
                return panels;
            }
        }

        public static string PanelLabel(string code)
        {
            return FindLabel(Panels, code);
        }

        #endregion

        #region 地理

        public static string GEOGRAPHY_0_CODE = "G0";

        private static ObservableCollection<Option> geographies;
        public static ObservableCollection<Option> Geographies
        {
            get
            {
                return geographies;
            }
        }

        public static string GeographicLabel(string code)
        {
            return FindLabel(Geographies, code);
        }

        #endregion

        #region データセット

        public static string DATASET_0_CODE = "D0";
        private static ObservableCollection<Option> dataSets;
        public static ObservableCollection<Option> DataSets
        {
            get
            {
                return dataSets;
            }
        }

        public static string DataSetLabel(string code)
        {
            return FindLabel(DataSets, code);
        }

        #endregion

        #region 言語

        public static string LANGUAGE_0_CODE = "L0";

        private static ObservableCollection<Option> languages;
        public static ObservableCollection<Option> Languages
        {
            get
            {
                return languages;
            }
        }

        public static string LanguageLabel(string code)
        {
            return FindLabel(Languages, code);
        }

        #endregion

        #region 比較値

        public static string COMPARE_VALUE_MATCH_CODE = "1";
        public static string COMPARE_VALUE_NOTMATCH_CODE = "2";
        public static string COMPARE_VALUE_PARTIALMATCH_CODE = "3";

        private static ObservableCollection<Option> compareValues;
        public static ObservableCollection<Option> CompareValues
        {
            get
            {
                return compareValues;
            }
        }

        public static Option CompareValueMatch
        {
            get
            {
                return Find(CompareValues, COMPARE_VALUE_MATCH_CODE);
            }
        }

        public static Option CompareValueNotMatch
        {
            get
            {
                return Find(CompareValues, COMPARE_VALUE_NOTMATCH_CODE);
            }
        }

        public static Option CompareValuePartialMatch
        {
            get
            {
                return Find(CompareValues, COMPARE_VALUE_PARTIALMATCH_CODE);
            }
        }

        public static bool IsMatch(string value)
        {
            return COMPARE_VALUE_MATCH_CODE == value;
        }

        public static bool IsNotMatch(string value)
        {
            return COMPARE_VALUE_NOTMATCH_CODE == value;
        }

        public static bool IsPartialMatch(string value)
        {
            return COMPARE_VALUE_PARTIALMATCH_CODE == value;
        }

        private static ObservableCollection<Option> positions;
        public static ObservableCollection<Option> Positions { get { return positions; } }
        public static string PositionLabel(string code)
        {
            return FindLabel(Positions, code);
        }

        public static string OPERATOR_EQUALS_CODE = "1";
        private static ObservableCollection<Option> operators;
        public static ObservableCollection<Option> Operators { get { return operators; } }
        public static string OperatorLabel(string code)
        {
            return FindLabel(Operators, code);
        }

        public static string CONNECTION_AND_CODE = "1";
        private static ObservableCollection<Option> connections;
        public static ObservableCollection<Option> Connections { get { return connections; } }
        public static string ConnectionLabel(string code)
        {
            return FindLabel(Connections, code);
        }

        #endregion

        #region

        public static string BOOK_TYPE_BOOK = "1";
        public static string BOOK_TYPE_BOOK_CHAPTER = "2";
        public static string BOOK_TYPE_TREATISE_WITH_PEER_REVIEW = "3";
        public static string BOOK_TYPE_TREATISE_WITHOUT_PEER_REVIEW = "4";
        public static string BOOK_TYPE_SOCIETY_ABSTRACT = "5";
        public static string BOOK_TYPE_REPORT = "6";
        public static string BOOK_TYPE_THESIS = "7";
        public static string BOOK_TYPE_WEBPAGE = "8";
        public static string BOOK_TYPE_OTHER = "9";

        private static ObservableCollection<Option> bookTypes;
        public static ObservableCollection<Option> BookTypes
        {
            get
            {
                return bookTypes;
            }
        }

        public static bool IsBookTypeBook(string value)
        {
            return BOOK_TYPE_BOOK == value;
        }

        public static bool IsBookTypeBookChapter(string value)
        {
            return BOOK_TYPE_BOOK_CHAPTER == value;
        }

        public static bool IsBookTypeTreatiseWithPeerReview(string value)
        {
            return BOOK_TYPE_TREATISE_WITH_PEER_REVIEW == value;
        }

        public static bool IsBookTypeTreatiseWithoutPeerReview(string value)
        {
            return BOOK_TYPE_TREATISE_WITHOUT_PEER_REVIEW == value;
        }

        public static bool IsBookTypeSocietyAbstract(string value)
        {
            return BOOK_TYPE_SOCIETY_ABSTRACT == value;
        }

        public static bool IsBookTypeReport(string value)
        {
            return BOOK_TYPE_REPORT == value;
        }

        public static bool IsBookTypeThesis(string value)
        {
            return BOOK_TYPE_THESIS == value;
        }

        public static bool IsBookTypeWebpage(string value)
        {
            return BOOK_TYPE_WEBPAGE == value;
        }

        public static bool IsBookTypeOther(string value)
        {
            return BOOK_TYPE_OTHER == value;
        }

        #endregion
    }
}
