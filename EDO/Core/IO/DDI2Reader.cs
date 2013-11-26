using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using EDO.Core.Model;
using System.Xml;
using EDO.Properties;
using System.Windows;
using EDO.Main;
using System.Diagnostics;
using System.Xml.Linq;
using EDO.Core.Util;
using EDO.Core.View;

namespace EDO.Core.IO
{
    public class DDI2Reader :DDI2IO
    {
        private class ReaderContext
        {
            public ReaderContext()
            {
                SamplingMethods = new Dictionary<string, List<string>>();
                VarIds = new Dictionary<string, List<string>>();
                DataSetIds = new Dictionary<string, List<string>>();
            }

            // サンプリングIdとサンプリング方法のペア(タブごとに複数のサンプリング方法が存在)
            public Dictionary<string, List<string>> SamplingMethods { get; set; }

            // conceptIdとvariableIdのペア(コンセプトごとに複数の変数が存在)
            public Dictionary<string, List<string>> VarIds {get; set; }

            public Dictionary<string, List<string>> DataSetIds { get; set; }


            public string FindConceptIdByVarId(string varId)
            {
                //変数の所属するコンセプトを決定
                foreach (KeyValuePair<string, List<string>> pair in VarIds)
                {
                    string conceptId = pair.Key;
                    List<string> ids = pair.Value;
                    if (ids.Contains(varId))
                    {
                        return conceptId;
                    }
                }
                return null;
            }
        }

        private class CategorySchemeItem
        {
            public CategorySchemeItem(CategoryScheme categoryScheme, CodeScheme codeScheme)
            {
                CategoryScheme = categoryScheme;
                CodeScheme = codeScheme;
            }
            public CategoryScheme CategoryScheme { get; set; }
            public CodeScheme CodeScheme { get; set; }
        }

        private class VariableItem
        {
            public VariableItem(Variable variable, Question question, CategorySchemeItem categorySchemeItem)
            {
                Variable = variable;
                Question = question;
                CategorySchemeItem = categorySchemeItem;
            }
            public Variable Variable { get; set; }
            public Question Question { get; set; }
            public CategorySchemeItem CategorySchemeItem { get; set; }
        }

        private class DataSetItem
        {
            public DataSetItem(DataSet dataSet, DataFile dataFile)
            {
                DataSet = dataSet;
                DataFile = dataFile;
            }
            public DataSet DataSet { get; set; }
            public DataFile DataFile { get; set; }
        }

        private static void AddXsd(XmlSchemaSet sc, string name)
        {
            sc.Add(null, EDOConfig.DDI25Path + name);
        }

        public static bool Validate(string xml)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            AddXsd(settings.Schemas, "codebook.xsd");
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

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
                //DDIファイルの検証に失敗した場合
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


        private static Member CreateMember(XElement memberElem, string roleCode)
        {
            Member member = new Member();
            SetupMemberName(memberElem.Value, member);
            if (roleCode == null)
            {
                string role = (string)memberElem.Attribute(ATTR_ROLE);
                roleCode = Options.RoleCodeByLabel(role);
            }
            member.RoleCode = roleCode;
            return member;
        }

        private static void CreateOrganization(XElement elem, Member member, List<Organization> organizations)
        {
            string organizationName = (string)elem.Attribute(ATTR_AFFILIATION);
            if (string.IsNullOrEmpty(organizationName))
            {
                organizationName = Resources.UndefinedValue;
            }
            Organization organization = Organization.FindOrganizationByName(organizations, organizationName);
            if (organization == null)
            {
                organization = new Organization() { OrganizationName = organizationName };
                organizations.Add(organization);
            }
            member.OrganizationId = organization.Id;
        }

        private static void CreateMembers(XElement codebookElem, StudyUnit studyUnit)
        {
            List<Member> members = new List<Member>();
            XElement stdyDscrElem = codebookElem.Element(cb + TAG_STDY_DSCR);
            if (stdyDscrElem == null)
            {
                return;
            }
            XElement citationElem = stdyDscrElem.Element(cb + TAG_CITATION);
            if (citationElem == null)
            {
                return;
            }
            XElement rspStmtElem = citationElem.Element(cb + TAG_RSP_STMT);
            if (rspStmtElem == null)
            {
                return;
            }

            List<Organization> organizations = new List<Organization>();
            XElement authEntyElem = rspStmtElem.Element(cb + TAG_AUTH_ENTY);
            if (authEntyElem != null)
            {
                Member member = CreateMember(authEntyElem, Options.ROLE_DAIHYOSHA_CODE);
                members.Add(member);
                CreateOrganization(authEntyElem, member, organizations);
            }
            IEnumerable<XElement> othIdElems = rspStmtElem.Elements(cb + TAG_OTH_ID);
            foreach (XElement othIdElem in othIdElems)
            {
                Member member = CreateMember(othIdElem, null);
                members.Add(member);
                CreateOrganization(othIdElem, member, organizations);
            }

            if (members.Count > 0)
            {
                studyUnit.Members = members;
            }

            if (organizations.Count > 0)
            {
                studyUnit.Organizations = organizations;
            }
        }

        private static void CreateAbstract(XElement codebookElem, StudyUnit studyUnit)
        {
            XElement stdyDscrElem = codebookElem.Element(cb + TAG_STDY_DSCR);
            if (stdyDscrElem == null)
            {
                return;
            }

            Abstract abstractModel = new Abstract();
            XElement citationElem = stdyDscrElem.Element(cb + TAG_CITATION);
            if (citationElem != null)
            {
                XElement titlElem = citationElem.Descendants(cb + TAG_TITL).FirstOrDefault();
                if (titlElem != null)
                {
                    abstractModel.Title = titlElem.Value;
                }
            }

            XElement stdyInfoElem = stdyDscrElem.Element(cb + TAG_STDY_INFO);
            if (stdyInfoElem != null)
            {
                abstractModel.Summary = (string)stdyInfoElem.Element(cb + TAG_ABSTRACT);
            }

            studyUnit.Abstract = abstractModel;
        }


        private static DateRange ReadDateRange(XElement rootElem, XName dateElemName)
        {
            IEnumerable<XElement> timePrdElems = rootElem.Descendants(dateElemName);
            DateUnit fromDate = new DateUnit();
            DateUnit toDate = new DateUnit();
            foreach (XElement timePrdElem in timePrdElems)
            {
                string dateStr = (string)timePrdElem.Attribute(ATTR_DATE);
                string eventStr = (string)timePrdElem.Attribute(ATTR_EVENT);
                if (eventStr == null || eventStr == "start")
                {
                    fromDate = DateParser.Parse(dateStr);
                }
                else if (eventStr == "end")
                {
                    toDate = DateParser.Parse(dateStr);
                }
            }
            if (fromDate == null)
            {
                return null;
            }
            return new DateRange(fromDate, toDate);
        }

        private static void CreateCoverage(XElement codebookElem, StudyUnit studyUnit)
        {
            XElement stdyDscrElem = codebookElem.Element(cb + TAG_STDY_DSCR);
            if (stdyDscrElem == null)
            {
                return;
            }

            //調査のトピック
            Coverage coverageModel = Coverage.CreateDefault();
            XElement stdyInfoElem = stdyDscrElem.Element(cb + TAG_STDY_INFO);
            if (stdyInfoElem != null)
            {
                XElement subjectElem = stdyInfoElem.Element(cb + TAG_SUBJECT);
                if (subjectElem != null)
                {
                    List<string> labels = new List<string>();
                    IEnumerable<XElement> topcClasElems = subjectElem.Elements(cb + TAG_TOPC_CLAS);
                    foreach (XElement topcClasElem in topcClasElems)
                    {
                        labels.Add(topcClasElem.Value);
                    }
                    coverageModel.CheckTopics(labels);

                    //キーワード
                    IEnumerable<XElement> keywordElems = subjectElem.Elements(cb + TAG_KEYWORD);
                    foreach (XElement keywordElem in keywordElems)
                    {
                        Keyword keyword = new Keyword()
                        {
                            Content = keywordElem.Value
                        };
                        coverageModel.Keywords.Add(keyword);
                    }
                }

                XElement sumDscrElem = stdyInfoElem.Element(cb + TAG_SUM_DSCR);
                if (sumDscrElem != null)
                {
                    //カバーする時期
                    DateRange dateRange = ReadDateRange(sumDscrElem, cb + TAG_TIME_PRD);
                    if (dateRange != null)
                    {
                        coverageModel.DateRange = dateRange;
                    }


                    //メモ
                    coverageModel.Memo = (string)sumDscrElem.Element(cb + TAG_GEOG_COVER);
                }

            }
            studyUnit.Coverage = coverageModel;
        }


        private static void CreateFundingInfos(XElement codebookElem, StudyUnit studyUnit)
        {
            XElement stdyDscrElem = codebookElem.Element(cb + TAG_STDY_DSCR);
            if (stdyDscrElem == null)
            {
                return;
            }

            XElement citationElem = stdyDscrElem.Element(cb + TAG_CITATION);
            if (citationElem == null)
            {
                return;
            }

            XElement prodStmtElem = citationElem.Element(cb + TAG_PROD_STMT);
            if (prodStmtElem == null)
            {
                return;
            }

            IEnumerable<XElement> fundAgElems = prodStmtElem.Elements(cb + TAG_FUND_AG);
            IEnumerable<XElement> grantNoElems = prodStmtElem.Elements(cb + TAG_GRANT_NO);


            List<FundingInfo> fundingInfos = new List<FundingInfo>();
            var fundAgEnumerator = fundAgElems.GetEnumerator();
            var grantNoEnumerator = grantNoElems.GetEnumerator();
            while (fundAgEnumerator.MoveNext() && grantNoEnumerator.MoveNext())
            {
                string organizationName = fundAgEnumerator.Current.Value;
                string number = grantNoEnumerator.Current.Value;

                FundingInfo fundingInfo = new FundingInfo();
                fundingInfo.Number = number;
                fundingInfo.Organization.OrganizationName = organizationName;
                fundingInfos.Add(fundingInfo);
            }

            if (fundingInfos.Count > 0)
            {
                studyUnit.FundingInfos = fundingInfos;
            }
        }

        private static List<DateRange> ReadDateRanges(IEnumerable<XElement> dateElems)
        {
            List<DateRange> dateRanges = new List<DateRange>();
            List<XElement> dateElemList = new List<XElement>(dateElems);

            //event属性なし=開始年月日のみ
            //start=開始年月日
            //end=終了年月日
            for (int i = 0; i < dateElemList.Count; i++)
            {
                XElement dateElem = dateElemList[i];
                string eventStr = (string)dateElem.Attribute(ATTR_EVENT);
                string dateStr = (string)dateElem.Attribute(ATTR_DATE);
                if (eventStr == "start" && i < dateElemList.Count - 1)
                {
                    //開始年月日でかつ要素が最後じゃない場合、次の要素を参照
                    XElement nextDateElem = dateElemList[i + 1];
                    string nextEventStr = (string)nextDateElem.Attribute(ATTR_EVENT);
                    string nextDateStr = (string)nextDateElem.Attribute(ATTR_DATE);
                    if (nextEventStr == "end")
                    {
                        //次の要素が終了年月日の場合from, toでdaterangeを作る
                        dateRanges.Add(DateParser.Parse(dateStr, nextDateStr)); //nullがはいってもよい
                        i++;
                    }
                    else
                    {
                        //endじゃない場合(データエラー?)、fromだけで作る
                        dateRanges.Add(DateParser.Parse(dateStr, null));//nullがはいってもよい
                    }
                }
                else if (string.IsNullOrEmpty(eventStr))
                {
                    //正規のの開始年月日のみの場合
                    dateRanges.Add(DateParser.Parse(dateStr, null));//nullがはいってもよい
                }

            }

            return dateRanges;
        }

        private static void CreateSamplings(XElement codebookElem, StudyUnit studyUnit, ReaderContext context)
        {
            XElement stdyDscrElem = codebookElem.Element(cb + TAG_STDY_DSCR);
            if (stdyDscrElem == null)
            {
                return;
            }

            XElement methodElem = stdyDscrElem.Element(cb + TAG_METHOD);
            if (methodElem == null)
            {
                return;
            }

            List<Sampling> samplings = new List<Sampling>();
            IEnumerable<XElement> dataCollElems = methodElem.Elements(cb + TAG_DATA_COLL);
            foreach (XElement dataCollElem in dataCollElems)
            {
                Sampling sampling = new Sampling();
                samplings.Add(sampling);

                //データ収集方法
                string method = (string)dataCollElem.Element(cb + TAG_COLL_MODE);
                sampling.MethodCode = Option.FindCodeByLabel(Options.SamplingMethods, method);
                //データ収集字の状況
                sampling.Situation = (string)dataCollElem.Element(cb + TAG_COLL_SITU);
                //データ収集の責任者
                XElement dataCollectorElem = dataCollElem.Element(cb + TAG_DATA_COLLECTOR);
                if (dataCollectorElem != null)
                {
                    Member newMember = CreateMember(dataCollectorElem, null);
                    Member existMember = Member.FindByName(studyUnit.Members, newMember.LastName, newMember.FirstName);
                    if (existMember != null)
                    {
                        newMember = existMember;
                    }
                    else
                    {
                        CreateOrganization(dataCollectorElem, newMember, studyUnit.Organizations);
                        studyUnit.Members.Add(newMember);
                    }
                    sampling.MemberId = newMember.Id;
                }

                //サンプリング方法の読み込み
                List<string> samplingMethodsPerTab = new List<string>();
                IEnumerable<XElement> sampProcElems = dataCollElem.Elements(cb + TAG_SAMP_PROC);
                foreach (XElement sampProcElem in sampProcElems)
                {
                    samplingMethodsPerTab.Add(sampProcElem.Value);
                }
                //書くタブごとのサンプリング方法を記憶(母集団読み込み時に反映)
                context.SamplingMethods[sampling.Id] = samplingMethodsPerTab;
            }

            //データ収集年月
            XElement stdyInfoElem = stdyDscrElem.Element(cb + TAG_STDY_INFO);
            if (stdyInfoElem != null)
            {
                XElement sumDscrElem = stdyInfoElem.Element(cb + TAG_SUM_DSCR);
                if (sumDscrElem != null)
                {
                    List<DateRange> dateRanges = ReadDateRanges(sumDscrElem.Elements(cb + TAG_COLL_DATE));
                    for (int i = 0; i < samplings.Count && i < dateRanges.Count; i++ )
                    {
                        Sampling sampling = samplings[i];
                        DateRange dateRange = dateRanges[i];
                        if (dateRange != null) //dateRangeは不正な文字列の場合nullになっている場合もありうるのでチェックが必要
                        {
                            sampling.DateRange = dateRange;
                        }
                    }
                }
            }
            if (samplings.Count > 0)
            {
                studyUnit.Samplings = samplings;
            }
        }

        private static List<Universe> CreateUniversesPerTab(List<XElement> universeElemList, ref int index, List<string> samplingMethodsPerTab)
        {
            List<Universe> universes = new List<Universe>();
            foreach (string samplingMethod in samplingMethodsPerTab)
            {
                if (index > universeElemList.Count - 1)
                {
                    break;
                }
                XElement universeElem = universeElemList[index++];
                Universe universe = new Universe() { Method = samplingMethod };
                SetupUniverse(universeElem.Value, universe);
                universes.Add(universe);
            }
            return universes;
        }

        private static void CreateUniverses(XElement codebookElem, StudyUnit studyUnit, ReaderContext context)
        {
            XElement stdyDscrElem = codebookElem.Element(cb + TAG_STDY_DSCR);
            if (stdyDscrElem == null)
            {
                return;
            }
            XElement stdyInfoElem = stdyDscrElem.Element(cb + TAG_STDY_INFO);
            if (stdyInfoElem == null)
            {
                return;
            }
            XElement sumDscrElem = stdyInfoElem.Element(cb + TAG_SUM_DSCR);
            if (sumDscrElem == null)
            {
                return;
            }
            IEnumerable<XElement> universeElems = sumDscrElem.Elements(cb + TAG_UNIVERSE);
            List<XElement> universeElemList = new List<XElement>(universeElems);
            int index = 0;
            foreach (Sampling sampling in studyUnit.Samplings)
            {
                List<string> samplingMethodsPerTab = context.SamplingMethods[sampling.Id];
                List<Universe> universes = CreateUniversesPerTab(universeElemList, ref index, samplingMethodsPerTab);
                sampling.Universes = universes;
            }
        }

        private static void CreateConceptSchemes(XElement codebookElem, StudyUnit studyUnit, ReaderContext context)
        {
            XElement dataDscrElem = codebookElem.Element(cb + TAG_DATA_DSCR);
            if (dataDscrElem == null)
            {
                return;
            }

            Dictionary<string, List<string>> conceptIds = new Dictionary<string, List<string>>();

            List<ConceptScheme> allConceptSchemes = new List<ConceptScheme>();
            List<Concept> allConcepts = new List<Concept>();

            IEnumerable<XElement> varGrpElems = dataDscrElem.Elements(cb + TAG_VAR_GRP);
            foreach (XElement varGrpElem in varGrpElems)
            {
                string varGrpStr = (string)varGrpElem.Attribute(ATTR_VAR_GRP);
                if (!string.IsNullOrEmpty(varGrpStr))
                {
                    //大分類(ConceptScheme)
                    ConceptScheme conceptScheme = new ConceptScheme();
                    allConceptSchemes.Add(conceptScheme);

                    conceptScheme.Title = (string)varGrpElem.Element(cb + TAG_CONCEPT);
                    conceptScheme.Memo = (string)varGrpElem.Element(cb + TAG_DEFNTH);

                    List<string> ids = SplitIds(varGrpStr);
                    conceptIds[conceptScheme.Id] = ids;
                }
                else
                {
                    string id = (string)varGrpElem.Attribute(ATTR_ID);
                    if (!string.IsNullOrEmpty(id))
                    {
                        //イメージ(Concept)
                        Concept concept = new Concept();
                        concept.Id = id;
                        concept.Title = (string)varGrpElem.Element(cb + TAG_CONCEPT);
                        concept.Content = (string)varGrpElem.Element(cb + TAG_DEFNTH);
                        allConcepts.Add(concept);

                        string varStr = (string)varGrpElem.Attribute(ATTR_VAR);
                        List<string> ids = SplitIds(varStr);
                        context.VarIds[concept.Id] = ids; //コンセプトごとの変数IDを記憶(変数読み込み時に変数にconceptIdを設定するのに使う)
                    }
                }
            }

            foreach (ConceptScheme conceptScheme in allConceptSchemes)
            {
                List<string> ids = conceptIds[conceptScheme.Id];
                conceptScheme.Concepts = Concept.FindAll(allConcepts, ids);
            }

            if (allConceptSchemes.Count > 0)
            {
                studyUnit.ConceptSchemes = allConceptSchemes;
            }
        }

        private static void SetupRange(XElement varElem, Response response)
        {
            XElement valrngElem = varElem.Element(cb + TAG_VALRNG);
            if (valrngElem == null) {
                return;
            }

            XElement rangeElem = valrngElem.Element(cb + TAG_RANGE);
            if (rangeElem == null)
            {
                return;
            }

            response.Min = (decimal?)rangeElem.Attribute(ATTR_MIN);
            response.Max = (decimal?)rangeElem.Attribute(ATTR_MAX);
        }

        private static Question CreateQuestion(XElement varElem)
        {
            string id = (string)varElem.Attribute(ATTR_ID);
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            XElement qstnElem = varElem.Element(cb + TAG_QSTN);
            if (qstnElem == null)
            {
                return null;
            }
            string type = (string)qstnElem.Attribute(ATTR_RESPONSE_DOMAIN_TYPE);
            Question question = null;
            if (type == "category")
            {
                //選択肢
                question = new Question();
                question.Response.TypeCode = Options.RESPONSE_TYPE_CHOICES_CODE;
            }
            else if (type == "numeric")
            {
                question = new Question();
                question.Response.TypeCode = Options.RESPONSE_TYPE_NUMBER_CODE;
            }
            else if (type == "text")
            {
                question = new Question();
                question.Response.TypeCode = Options.RESPONSE_TYPE_FREE_CODE;
            }
            else if (type == "datetime")
            {
                question = new Question();
                question.Response.TypeCode = Options.RESPONSE_TYPE_DATETIME_CODE;
            }
            else
            {
                return null;
            }
            question.Id = id;
            question.Title = (string)qstnElem.Attribute(ATTR_NAME);
            question.Text = (string)qstnElem.Element(cb + TAG_QSTN_LIT);
            return question;
        }

        private static CategorySchemeItem CreateCategorySchemeItem(XElement varElem, Variable variable)
        {
            XElement catgryGrpElem = varElem.Element(cb + TAG_CATGRY_GRP);
            if (catgryGrpElem == null)
            {
                return null;
            }
            string title = (string)catgryGrpElem.Element(cb + TAG_LABL);
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }
            string memo = (string)catgryGrpElem.Element(cb + TAG_TXT);
            CategoryScheme categoryScheme = new CategoryScheme()
            {
                Title = title,
                Memo = memo
            };
            CodeScheme codeScheme = new CodeScheme()
            {
                Title = title,
                Memo = memo
            };
            IEnumerable<XElement> catgryElems = varElem.Elements(cb + TAG_CATGRY);
            foreach (XElement catgryElem in catgryElems)
            {
                title = (string)catgryElem.Element(cb + TAG_LABL);
                memo = (string)catgryElem.Element(cb + TAG_TXT);
                string v = (string)catgryElem.Element(cb + TAG_CAT_VALU);

                Category category = new Category() {
                    Title = title,
                    Memo = memo
                };
                category.CategorySchemeId = categoryScheme.Id;
                categoryScheme.Categories.Add(category);

                Code code = new Code()
                {
                    Value = v
                };
                code.CodeSchemeId = codeScheme.Id;
                code.CategoryId = category.Id;
                codeScheme.Codes.Add(code);
            }
            variable.Response.CodeSchemeId = codeScheme.Id;
            return new CategorySchemeItem(categoryScheme, codeScheme);
        }

        private static VariableItem CreateVariableItem(XElement varElem, ReaderContext context)
        {
            string id = (string)varElem.Attribute(ATTR_ID);
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            string title = (string)varElem.Attribute(ATTR_NAME);
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }
            string representationType = (string)varElem.Attribute(ATTR_REPRESENTATION_TYPE);
            string responseTypeCode = GetTypeFromRepresentationType(representationType);
            if (responseTypeCode == null)
            {
                return null;
            }

            string files = (string)varElem.Attribute(ATTR_FILES);
            if (!string.IsNullOrEmpty(files))
            {
                //データセットのIDを記憶(データセット読み込み時に利用)
                context.DataSetIds[id] = SplitIds(files);
            }
                
            Variable variable = new Variable();
            variable.Id = id;
            variable.Title = title;
            variable.Label = (string)varElem.Element(cb + TAG_LABL);
            variable.Response.TypeCode = responseTypeCode;


            variable.ConceptId = context.FindConceptIdByVarId(variable.Id);
            Question question =  null;
            if (variable.ConceptId != null)            
            {
                //対応するConceptがないとViewModel生成時に例外で落ちる(落ちなくても画面に表示できない)
                question = CreateQuestion(varElem);
                if (question != null)
                {
                    question.ConceptId = variable.ConceptId;
                    variable.QuestionId = question.Id;
                }
            }

            CategorySchemeItem categorySchemeItem = null;
            if (variable.Response.IsTypeChoices)
            {
                categorySchemeItem = CreateCategorySchemeItem(varElem, variable);
            }
            else if (variable.Response.IsTypeNumber)
            {
                SetupRange(varElem, variable.Response);
            }
            else if (variable.Response.IsTypeFree)
            {
                SetupRange(varElem, variable.Response);
            }
            else if (variable.Response.IsTypeDateTime)
            {

            }
            return new VariableItem(variable, question, categorySchemeItem);
        }

        private static void CreateVariables(XElement codebookElem, StudyUnit studyUnit, ReaderContext context)
        {
            XElement dataDscrElem = codebookElem.Element(cb + TAG_DATA_DSCR);
            if (dataDscrElem == null)
            {
                return;
            }
            List<Variable> variables = new List<Variable>();
            List<Question> questions = new List<Question>();
            List<CategoryScheme> categorySchemes = new List<CategoryScheme>();
            List<CodeScheme> codeSchemes = new List<CodeScheme>();
            IEnumerable<XElement> varElems = dataDscrElem.Elements(cb + TAG_VAR);
            foreach (XElement varElem in varElems)
            {
                VariableItem variableItem = CreateVariableItem(varElem, context);
                if (variableItem != null)
                {
                    Variable variable = variableItem.Variable;
                    variables.Add(variable);

                    Question question = variableItem.Question;
                    if (question != null)
                    {
                        questions.Add(question);
                    }
                    if (question != null && question.ConceptId == null)
                    {
                        Debug.WriteLine("Question's conceptId is null");
                    }

                    CategorySchemeItem categorySchemeItem = variableItem.CategorySchemeItem;
                    if (categorySchemeItem != null)
                    {
                        categorySchemes.Add(categorySchemeItem.CategoryScheme);
                        codeSchemes.Add(categorySchemeItem.CodeScheme);
                    }
                }
            }
            if (variables.Count > 0)
            {
                studyUnit.Variables = variables;
            }
            if (questions.Count > 0)
            {
                studyUnit.Questions = questions;
            }
            if (categorySchemes.Count > 0)
            {
                studyUnit.CategorySchemes = categorySchemes;
            }
            if (codeSchemes.Count > 0)
            {
                studyUnit.CodeSchemes = codeSchemes;
            }

        }

        private static DataSetItem CreateDataSetItem(XElement fileDscrElem)
        {
            string id = (string)fileDscrElem.Attribute(ATTR_ID);
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            string uri = (string)fileDscrElem.Attribute(ATTR_URI);


            XElement fileTxtElem = fileDscrElem.Element(cb + TAG_FILE_TXT);
            if (fileTxtElem == null)
            {
                return null;
            }

            string title = (string)fileTxtElem.Element(cb + TAG_FILE_NAME);
            string memo = (string)fileTxtElem.Element(cb + TAG_FILE_CONT);
            string format = (string)fileTxtElem.Element(cb + TAG_FORMAT);
            DataSet dataSet = new DataSet()
            {
                Id = id,
                Title = title,
                Memo = memo
            };
            DataFile dataFile = new DataFile()
            {
                Uri = uri,
                Format = format
            };
            dataFile.DataSetId = dataSet.Id;

            return new DataSetItem(dataSet, dataFile);
        }

        private static void CreateDataSets(XElement codebookElem, StudyUnit studyUnit, ReaderContext context)
        {
            IEnumerable<XElement> fileDscrElems = codebookElem.Elements(cb + TAG_FILE_DSCR);
            List<DataSet> dataSets = new List<DataSet>();
            List<DataFile> dataFiles = new List<DataFile>();
            foreach (XElement fileDscrElem in fileDscrElems)
            {
                DataSetItem dataSetItem = CreateDataSetItem(fileDscrElem);
                if (dataSetItem != null)
                {
                    DataSet dataSet = dataSetItem.DataSet;

                    List<string> variableIds = new List<string>();
                    foreach (KeyValuePair<string, List<string>> pair in context.DataSetIds)
                    {
                        string variableId = pair.Key;
                        List<string> dataSetIds = pair.Value;
                        if (dataSetIds.Contains(dataSet.Id))
                        {
                            variableIds.Add(variableId);
                        }
                    }

                    dataSet.VariableGuids = variableIds;
                    dataSets.Add(dataSet);
                    dataFiles.Add(dataSetItem.DataFile);
                }
            }

            if (dataSets.Count > 0)
            {
                studyUnit.DataSets = dataSets;
                studyUnit.DataFiles = dataFiles;
            }
        }


        private static Book CreateBook(XElement citationElem)
        {
            XElement titlStmtElem = citationElem.Element(cb + TAG_TITL_STMT);
            if (titlStmtElem == null)
            {
                return null;
            }
            string title = (string)titlStmtElem.Element(cb + TAG_TITL);
            if (string.IsNullOrEmpty(title))
            {
                return null;
            }
            Book book = new Book();
            book.BookTypeCode = Options.BOOK_TYPE_TREATISE_WITH_PEER_REVIEW;
            book.Title = title;


            XElement rspStmtElem = citationElem.Element(cb + TAG_RSP_STMT);
            if (rspStmtElem != null)
            {
                //著者
                book.Author = (string)rspStmtElem.Element(cb + TAG_AUTH_ENTY);
                //編者
                book.Editor = (string)rspStmtElem.Element(cb + TAG_OTH_ID);
            }

            //XElement prodStmtElem = citationElem.Element(cb + TAG_PROD_STMT);
            //if (prodStmtElem != null)
            //{
            //    //出版社
            //    book.Publisher = (string)prodStmtElem.Element(cb + TAG_PRODUCER);
            //}

            XElement distStmtElem = citationElem.Element(cb + TAG_DIST_STMT);
            if (distStmtElem != null)
            {
                //発表日
                book.AnnouncementDate = (string)distStmtElem.Element(cb + TAG_DIST_DATE);
            }

            ParseIdentifier((string)citationElem.Element(cb + TAG_BIBL_CIT), book);

            XElement holdingsElem = citationElem.Element(cb + TAG_HOLDINGS);
            if (holdingsElem != null)
            {
                book.Url = (string)holdingsElem.Attribute(ATTR_URI);
            }

            book.Summary = (string)citationElem.Element(terms + TAG_ABSTRACT);
            book.Language = (string)citationElem.Element(dc + TAG_LANGUAGE);
            ParsePublisher((string)citationElem.Element(dc + TAG_PUBLISHER), book);

            return book;
        }

        private static void CreateBooks(XElement codebookElem, StudyUnit studyUnit)
        {
            XElement stdyDscrElem = codebookElem.Element(cb + TAG_STDY_DSCR);
            if (stdyDscrElem == null)
            {
                return;
            }
            XElement othrStdyMatElem = stdyDscrElem.Element(cb + TAG_OTHR_STDY_MAT);
            if (othrStdyMatElem == null)
            {
                return;
            }
            XElement relMatElem = othrStdyMatElem.Element(cb + TAG_REL_MAT);
            if (relMatElem == null)
            {
                return;
            }

            List<Book> books = new List<Book>();
            IEnumerable<XElement> citationElems = relMatElem.Elements(cb + TAG_CITATION);
            foreach (XElement citationElem in citationElems)
            {
                Book book = CreateBook(citationElem);
                if (book != null)
                {
                    books.Add(book);   
                }                
            }
            if (books.Count > 0)
            {
                studyUnit.Books = books;
            }
        }

        private static StudyUnit CreateStudyUnit(XElement codebookElem)
        {
            //StudyUnitの読み込み
            StudyUnit studyUnit = StudyUnit.CreateDefault();

            //イベントは該当なし

            //調査メンバー
            CreateMembers(codebookElem, studyUnit);

            //調査の概要
            CreateAbstract(codebookElem, studyUnit);

            //調査の範囲
            CreateCoverage(codebookElem, studyUnit);

            //研究資金
            CreateFundingInfos(codebookElem, studyUnit);

            //サンプリング(母集団の前に読み込む必要がある)
            ReaderContext context = new ReaderContext();
            CreateSamplings(codebookElem, studyUnit, context);

            //母集団
            CreateUniverses(codebookElem, studyUnit, context);

            //コンセプト
            CreateConceptSchemes(codebookElem, studyUnit, context);

//            //質問
//            CreateQuestions(codebookElem, studyUnit);

            //カテゴリ(varタグ以下に存在するので独立して読み込めないかも)
//            CreateCategorySchemes(codebookElem, studyUnit);

            //変数
            CreateVariables(codebookElem, studyUnit, context);

            //データセット
            CreateDataSets(codebookElem, studyUnit, context);

            //関連文献
            CreateBooks(codebookElem, studyUnit);

            return studyUnit;
        }

        private static EDOModel CreateSingleModel(XElement codebookElem)
        {
            EDOModel model = new EDOModel();
            StudyUnit studyUnit = CreateStudyUnit(codebookElem);
            if (studyUnit != null)
            {
                model.StudyUnits.Add(studyUnit);
            }
            return model;
        }

        private void MergeMember(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            //メンバーを追加
            if (curStudyUnit.Members.Count > 0)
            {
                //既にメンバーが存在する場合インポートするメンバーの中の調査代表者をその他に変更する。
                //(調査代表者はメンバーの中に一人に限定されているため)
                newStudyUnit.DisableDaihyosha();
            }
            curStudyUnit.Members.AddRange(newStudyUnit.Members);
            //組織を追加(メンバーフォームでは組織名をつかって検索するのでユニークにしないとまずい
            foreach (Organization organization in newStudyUnit.Organizations)
            {
                List<string> existNames = Organization.GetOrganizationNames(curStudyUnit.Organizations);
                organization.OrganizationName = EDOUtils.UniqueLabel(existNames, organization.OrganizationName);
                curStudyUnit.Organizations.Add(organization);
            }
        }

        private void MergeAbstract(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.Abstract = newStudyUnit.Abstract;
        }

        private void MergeCoverage(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            //調査範囲のマージは部分ごとに異なる
            Coverage newCoverage = newStudyUnit.Coverage;
            Coverage curCoverage = curStudyUnit.Coverage;

            //1.調査のトピック
            CheckOption.Merge(newCoverage.Topics, curCoverage.Topics);
            //2.キーワード
            curCoverage.Keywords.AddRange(newCoverage.Keywords);
            //3.日付
            curCoverage.DateRange = newCoverage.DateRange;
            //4.地域のレベルは該当なし
            //5.メモ
            curCoverage.Memo = newCoverage.Memo;
        }

        private void MergeFundingInfo(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.FundingInfos.AddRange(newStudyUnit.FundingInfos);
        }

        private void MergeSampling(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.Samplings.AddRange(newStudyUnit.Samplings);
        }

        private void MergeConcept(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.ConceptSchemes.AddRange(newStudyUnit.ConceptSchemes);
        }

        private void MergeQuestion(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.Questions.AddRange(newStudyUnit.Questions);
        }

        private void MergeVariable(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.Variables.AddRange(newStudyUnit.Variables);
            curStudyUnit.CategorySchemes.AddRange(newStudyUnit.CategorySchemes);
            curStudyUnit.CodeSchemes.AddRange(newStudyUnit.CodeSchemes);
        }

        private void MergeDataSet(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            if (newStudyUnit.DataSets.Count > 0 && newStudyUnit.DataSets[0].Title == EDOConstants.LABEL_ALL)
            {
                newStudyUnit.DataSets.RemoveAt(0);
            }
            curStudyUnit.DataSets.AddRange(newStudyUnit.DataSets);
            curStudyUnit.DataFiles.AddRange(newStudyUnit.DataFiles);
        }

        private void MergeBook(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.Books.AddRange(newStudyUnit.Books);

        }

        private void MergeStudyUnit(StudyUnit newStudyUnit, StudyUnit curStudyUnit, DDIImportOption importOption)
        {
            if (importOption.ImportMember)
            {
                MergeMember(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportAbstract)
            {
                MergeAbstract(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportCoverage)
            {
                MergeCoverage(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportFundingInfo)
            {
                MergeFundingInfo(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportSampling)
            {
                MergeSampling(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportConcept)
            {
                MergeConcept(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportQuestion)
            {
                MergeQuestion(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportVariable)
            {
                MergeVariable(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportDataSet)
            {
                MergeDataSet(newStudyUnit, curStudyUnit);
            }
            if (importOption.ImportBook)
            {
                MergeBook(newStudyUnit, curStudyUnit);
            }
        }

        public bool Read(StudyUnitVM curStudyUnit, EDOModel curEdoModel, string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));
            if (!Validate(path))
            {
                return false;
            }


            EDOModel newEdoModel = null;
            ClearError();
            XmlReaderSettings settings = new XmlReaderSettings();
            using (XmlReader reader = XmlReader.Create(path, settings))
            {
                XDocument doc = XDocument.Load(reader);
                XElement codebookElem = doc.Element(cb + TAG_CODEBOOK);
                if (codebookElem != null)
                {
                    newEdoModel = CreateSingleModel(codebookElem);
                }
            }
            if (newEdoModel == null)
            {
                return false;
            }

            DDI2ImportOption importOption = new DDI2ImportOption();
            SelectStudyUnitWindowVM vm = new SelectStudyUnitWindowVM(newEdoModel, curEdoModel, curStudyUnit.StudyUnitModel, importOption);
            SelectStudyUnitWindow window = new SelectStudyUnitWindow(vm);
            window.Owner = Application.Current.MainWindow;
            bool? result = window.ShowDialog();
            if (result != true)
            {
                return false;
            }
            StudyUnit fromStudyUnit = newEdoModel.StudyUnits[0];
            StudyUnit toStudyUnit = curEdoModel.StudyUnits[0];
            MergeStudyUnit(fromStudyUnit, toStudyUnit, vm.ImportOption);
            return true;
        }


    }
}
