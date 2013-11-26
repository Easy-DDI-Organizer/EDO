using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.ViewModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using EDO.Core.View;
using System.Windows;
using EDO.Main;
using EDO.Core.Util;

namespace EDO.Core.IO
{
    public class DDI3Reader :DDI3IO
    {
        #region 読み込み用のユーティリティ

        public static string ReadReferenceID(XElement elem, XName name)
        {
            XElement refElem = elem.Descendants(name).FirstOrDefault();
            string refId = null;
            if (refElem != null)
            {
                refId = (string)refElem.Element(r + TAG_ID);
            }
            return refId;
        }

        public static DateRange ReadDateRange(XElement elem, XName name)
        {
            DateRange dateRange = null;
            XElement dateElem = elem.Descendants(name).FirstOrDefault();
            if (dateElem != null)
            {
                //日付
                DateTime? fromDate = (DateTime?)dateElem.Element(r + TAG_SIMPLE_DATE);
                DateTime? toDate = null;
                if (fromDate == null)
                {
                    fromDate = (DateTime?)dateElem.Element(r + TAG_START_DATE);
                    toDate = (DateTime?)dateElem.Element(r + TAG_END_DATE);
                }
                if (fromDate != null || toDate != null)
                {
                    dateRange = new DateRange(fromDate, toDate);
                }
            }
            return dateRange;
        }

        public static DateUnit ReadDateUnit(XElement rootElem, XName dateElemName)
        {
            DateUnit dateUnit = null;
            XElement dateElem = rootElem.Descendants(dateElemName).FirstOrDefault() ;
            if (dateElem != null)
            {
                string dateStr = (string)dateElem.Element(r + TAG_SIMPLE_DATE);
                dateUnit = ParseDateUnit(dateStr);
            }
            return dateUnit;
        }


        public static string ReadDateUnitAsString(XElement rootElem, XName dateElemName)
        {
            DateUnit dateUnit = ReadDateUnit(rootElem, dateElemName);
            if (dateUnit == null)
            {
                return null;
            }
            return dateUnit.ToString();
        }
        #endregion

        #region StudyUnitの読み込み

        public static Event CreateEvent(XElement lifeEventElem)
        {
            Event eventModel = new Event();
            string id = (string)lifeEventElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            eventModel.Id = id;
            //タイトル
            XElement descElem = lifeEventElem.Element(r + TAG_DESCRIPTION);
            if (descElem != null)
            {
                eventModel.Title = descElem.Value;
            }

            //メモ
            XElement labelElem = lifeEventElem.Element(r + TAG_LABEL);
            if (labelElem != null)
            {
                eventModel.Memo = labelElem.Value;
            }
            eventModel.DateRange = ReadDateRange(lifeEventElem, r + TAG_DATE);
            return eventModel;
        }

        public static void CreateEvents(XElement studyUnitElem, StudyUnit studyUnit)
        {
            //イベントの読み込み
            XElement lifeInfoElem = studyUnitElem.Descendants(r + TAG_LIFECYCLE_INFORMATION).FirstOrDefault();
            if (lifeInfoElem == null)
            {
                return;
            }
            List<Event> eventModels = new List<Event>();
            IEnumerable<XElement> elements = lifeInfoElem.Elements(r + TAG_LIFECYCLE_EVENT);
            foreach (XElement lifeEventElem in elements)
            {
                Event eventModel = CreateEvent(lifeEventElem);
                if (eventModel != null)
                {
                    ////タイトルで検索して同じものは削除しておく(DefaultStudyUnitで作られているものを想定)。
                    //Event.RemoveByTitle(studyUnit.Events, eventModel.Title);
                    eventModels.Add(eventModel);
                }
            }
            if (eventModels.Count > 0)
            {
                studyUnit.Events = eventModels;
            }
        }

        public static Organization CreateOrganization(XElement organizationElem)
        {
            string id = (string)organizationElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            Organization organizationModel = new Organization();
            organizationModel.Id = id;
            organizationModel.OrganizationName = (string)organizationElem.Element(a + TAG_ORGANIZATION_NAME);
            return organizationModel;
        }

        public static void CreateOrganizations(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement archiveElem = studyUnitElem.Element(a + TAG_ARCHIVE);
            if (archiveElem == null)
            {
                return;
            }
            string archiveId = (string)archiveElem.Attribute(ATTR_ID);
            if (archiveId == null)
            {
                return;
            }
            XElement organizationSchemeElem = archiveElem.Element(a + TAG_ORGANIZATION_SCHEME);
            if (organizationSchemeElem == null)
            {
                return;
            }
            string organizationSchemeId = (string)organizationSchemeElem.Attribute(ATTR_ID);
            if (organizationSchemeId == null)
            {
                return;
            }

            List<Organization> organizationModels = new List<Organization>();
            IEnumerable<XElement> elements = organizationSchemeElem.Elements(a + TAG_ORGANIZATION);
            foreach (XElement organizationElem in elements)
            {
                Organization organizationModel = CreateOrganization(organizationElem);
                if (organizationModel != null)
                {
                    organizationModels.Add(organizationModel);
                }
            }
            studyUnit.ArchiveId = archiveId;
            studyUnit.OrganizationSchemeId = organizationSchemeId;
            if (organizationModels.Count > 0)
            {
                studyUnit.Organizations = organizationModels;
            }
        }

        public static Member CreateMember(XElement individualElem)
        {
            Member memberModel = new Member();
            string id = (string)individualElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            memberModel.Id = id;
            XElement nameElem = individualElem.Element(a + TAG_INDIVIDUAL_NAME);
            if (nameElem != null)
            {
                memberModel.FirstName = (string)nameElem.Element(a + TAG_FIRST);
                memberModel.LastName = (string)nameElem.Element(a + TAG_LAST);
            }
            XElement posElem = individualElem.Element(a + TAG_POSITION);
            if (posElem != null)
            {
                memberModel.Position = (string)posElem.Element(a + TAG_TITLE);
            }
            string role = (string)individualElem.Element(r + TAG_DESCRIPTION);
            if (role != null)
            {
                memberModel.RoleCode = Options.RoleCodeByLabel(role);
            }

            XElement refElem = individualElem.Descendants(a + TAG_ORGANIZATION_REFERENCE).FirstOrDefault();
            if (refElem != null)
            {
                string refId = (string)refElem.Element(r + TAG_ID);
                memberModel.OrganizationId = refId;
            }
            return memberModel;
        }

        public static void CreateMembers(XElement studyUnitElem, StudyUnit studyUnit)
        {
            List<Member> members = new List<Member>();
            IEnumerable<XElement> elements = studyUnitElem.Descendants(a + TAG_INDIVIDUAL);
            foreach (XElement individualElem in elements)
            {
                Member memberModel = CreateMember(individualElem);
                if (memberModel != null && studyUnit.FindOrganization(memberModel.OrganizationId) != null)
                {
                    members.Add(memberModel);
                }
            }
            if (members.Count > 0)
            {
                studyUnit.Members = members;
            }
        }

        public static void CreateAbstract(XElement studyUnitElem, StudyUnit studyUnit)
        {
            ///// タイトル
            Abstract abstractModel = new Abstract();
            XElement citationElem = studyUnitElem.Element(r + TAG_CITATION);
            if (citationElem == null)
            {
                return;
            }
            XElement titleElement = citationElem.Element(r + TAG_TITLE);
            if (titleElement == null)
            {
                return;
            }
            abstractModel.Title = titleElement.Value;

            ///// 概要
            XElement abstractElem = studyUnitElem.Element(s + TAG_ABSTRACT);
            if (abstractElem == null)
            {
                return;
            }
            XAttribute summaryIdAttr = abstractElem.Attribute(ATTR_ID);
            if (summaryIdAttr == null)
            {
                return;
            }
            XElement contentElem = abstractElem.Element(r + TAG_CONTENT);
            if (contentElem == null)
            {
                return;
            }
            abstractModel.SummaryId = summaryIdAttr.Value;
            abstractModel.Summary = contentElem.Value;

            ///// 目的
            XElement purposeElem = studyUnitElem.Element(s + TAG_PURPOSE);
            if (purposeElem == null)
            {
                return;
            }
            XAttribute purposeIdAttr = purposeElem.Attribute(ATTR_ID);
            if (purposeIdAttr == null)
            {
                return;
            }
            contentElem = purposeElem.Element(r + TAG_CONTENT);
            if (contentElem == null)
            {
                return;
            }
            abstractModel.PurposeId = purposeIdAttr.Value;
            abstractModel.Purpose = contentElem.Value;
            studyUnit.Abstract = abstractModel;
        }

        private static Dictionary<string, string> ReadGeographicStructureElems(XElement studyUnitElem)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            XElement conceptualComponentElem = studyUnitElem.Element(c + TAG_CONCEPTUAL_COMPONENT);
            if (conceptualComponentElem == null)
            {
                return dict;
            }
            XElement geographicStructureSchemeElem = conceptualComponentElem.Element(c + TAG_GEOGRAPHIC_STRUCTURE_SCHEME);
            if (geographicStructureSchemeElem == null)
            {
                return dict;
            }
            IEnumerable<XElement> elems = geographicStructureSchemeElem.Elements(r + TAG_GEOGRAPHIC_STRUCTURE);
            foreach (XElement elem in elems)
            {
                string id = (string)elem.Attribute(ATTR_ID);
                if (id == null)
                {
                    continue;
                }
                XElement geographyElem = elem.Element(r + TAG_GEOGRAPHY);
                if (geographyElem == null)
                {
                    continue;
                }
                XElement levelElem = geographyElem.Element(r + TAG_LEVEL);
                if (levelElem == null)
                {
                    continue;
                }
                string name = (string)levelElem.Element(r + TAG_NAME);
                if (name == null)
                {
                    continue;
                }
                dict.Add(id, name);
            }
            return dict;
        }

        public static void CreateCoverage(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement coverageElem = studyUnitElem.Element(r + TAG_COVERAGE);
            if (coverageElem == null)
            {
                return;
            }
            Coverage coverageModel = Coverage.CreateDefault();
            XElement topicElem = coverageElem.Element(r + TAG_TOPICAL_COVERAGE);
            if (topicElem != null)
            {
                //調査のトピック
                IEnumerable<XElement> elements = topicElem.Elements(r + TAG_SUBJECT);
                List<string> labels = new List<string>();
                foreach (XElement subjectElem in elements)
                {
                    labels.Add(subjectElem.Value);
                }
                coverageModel.CheckTopics(labels);

                //キーワード
                elements = topicElem.Elements(r + TAG_KEYWORD);
                foreach (XElement keywordElem in elements)
                {
                    Keyword keyword = new Keyword()
                    {
                        Content = keywordElem.Value
                    };
                    coverageModel.Keywords.Add(keyword);
                }
            }

            //調査の日付
            XElement temporalElem = coverageElem.Element(r + TAG_TEMPORAL_COVERAGE);
            if (temporalElem != null)
            {
                coverageModel.DateRange = ReadDateRange(temporalElem, r + TAG_REFERENCE_DATE);
            }

            //メモ
            XElement spatialElem = coverageElem.Element(r + TAG_SPATIAL_COVERAGE);
            if (spatialElem != null)
            {
                //カバーする地域のレベル
                Dictionary<string, string> labelDict = ReadGeographicStructureElems(studyUnitElem);

                List<string> checkLabels = new List<string>();
                IEnumerable<XElement> elements = spatialElem.Elements(r + TAG_GEOGRAPHIC_STRUCTURE_REFERENCE);
                foreach (XElement refElem in elements)
                {
                    string refId = (string)refElem.Element(r + TAG_ID);
                    if (refId == null) 
                    {
                        continue;
                    }
                    string code = IDUtils.ToCode(refId);
                    if (code == null)
                    {
                        continue;
                    }
                    if (!labelDict.ContainsKey(refId))
                    {
                        continue;
                    }
                    string label = labelDict[refId];
                    checkLabels.Add(label);
                }
                coverageModel.CheckAreas(checkLabels);

                //メモ
                string memo = (string)spatialElem.Element(r + TAG_DESCRIPTION);
                coverageModel.Memo = memo;
            }



            studyUnit.Coverage = coverageModel;
        }

        public static FundingInfo CreateFundingInfo(XElement fundingInfoElem, List<Organization> organizations)
        {
            FundingInfo fundingInfo = new FundingInfo();
            string id = ReadReferenceID(fundingInfoElem, r + TAG_AGENCY_ORGANIZATION_REFERENCE);
            if (id == null)
            {
                return null;
            }
            Organization organizationModel = Organization.GetOrganization(organizations, id);
            if (organizationModel == null)
            {
                return null;
            }
            organizations.Remove(organizationModel);
            fundingInfo.Organization = organizationModel;
            fundingInfo.Number = (string)fundingInfoElem.Element(r + TAG_GRANT_NUMBER);
            IEnumerable<XElement> elements = fundingInfoElem.Elements(r + TAG_DESCRIPTION);
            foreach (XElement elem in elements)
            {
                string value = null;
                if (IsDDIFundingInfoTitle(elem.Value, ref value))
                {
                    fundingInfo.Title = value;
                }
                else if (IsDDIFundingInfoMoney(elem.Value, ref value))
                {
                    decimal money;
                    if (Decimal.TryParse(value, out money))
                    {
                        fundingInfo.Money = money;
                    }
                }
                else if (IsDDIFundingInfoDateRange(elem.Value, ref value))
                {
                    fundingInfo.DateRange = ParseDateRange(value);
                }
            }
            return fundingInfo;
        }

        public static void CreateFundingInfos(XElement studyUnitElem, StudyUnit studyUnit)
        {
            List<FundingInfo> fundingInfoModels = new List<FundingInfo>();
            IEnumerable<XElement> elements = studyUnitElem.Elements(r + TAG_FUNDING_INFORMATION);
            foreach (XElement fundingInfoElem in elements)
            {
                FundingInfo fundingInfoModel = CreateFundingInfo(fundingInfoElem, studyUnit.Organizations);
                if (fundingInfoModel != null)
                {
                    fundingInfoModels.Add(fundingInfoModel);
                }
            }
            if (fundingInfoModels.Count > 0)
            {
                studyUnit.FundingInfos = fundingInfoModels;
            }
        }

        public static Universe CreateUniverse(XElement universeElem)
        {
            string id = (string)universeElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }

            Universe universe = new Universe();
            universe.Id = id;

            universe.Title = (string)universeElem.Element(r + TAG_LABEL);
            universe.Memo = (string)universeElem.Element(c + TAG_HUMAN_READABLE);

            return universe;
        }

        public static void CreateUniverses(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement universeSchemeElem = studyUnitElem.Descendants(c + TAG_UNIVERSE_SCHEME).FirstOrDefault();
            if (universeSchemeElem == null)
            {
                return;
            }
            string universeSchemeId = (string)universeSchemeElem.Attribute(ATTR_ID);
            if (universeSchemeId == null)
            {
                return;
            }

//            List<Universe> universeModels = new List<Universe>();

            ///// メインの母集団の読み込み
            IEnumerable<XElement> elements = studyUnitElem.Descendants(c + TAG_UNIVERSE);
            int index = 0;
            foreach (XElement universeElem in elements) //このループはサンプリング方法のタブに相当
            {
                Sampling sampling = studyUnit.GetSamplingAt(index++);
                if (sampling == null)
                {
                    break;
                }
                Universe universeModel = CreateUniverse(universeElem);
                if (universeModel == null)
                {
                    continue;
                }
                universeModel.IsMain = true;
                sampling.Universes.Add(universeModel);
                ///// サブ母集団の読み込み
                IEnumerable<XElement> subElements = universeElem.Elements(c + TAG_SUB_UNIVERSE);
                foreach (XElement subUniverseElem in subElements)
                {
                    Universe subUniverseModel = CreateUniverse(subUniverseElem);
                    if (subUniverseModel != null)
                    {
                        sampling.Universes.Add(subUniverseModel);
                    }
                }
            }

            List<Universe> universeModels = studyUnit.AllUniverses;
            ///// サンプリング方法は順番に読み込んで関連づける
            XElement dataCollectionElem = studyUnitElem.Element(d + TAG_DATA_COLLECTION);
            if (dataCollectionElem != null)
            {
                XElement methodologyElem = dataCollectionElem.Element(d + TAG_METHODOLOGY);
                if (methodologyElem != null)
                {
                    index = 0;
                    IEnumerable<XElement> samplingProcedureElems = methodologyElem.Elements(d + TAG_SAMPLING_PROCEDURE);
                    foreach (XElement samplingProcedureElem in samplingProcedureElems)
                    {
                        string id = (string)samplingProcedureElem.Attribute(ATTR_ID);
                        string content = (string)samplingProcedureElem.Element(r + TAG_CONTENT);
                        if (index < universeModels.Count && id != null && content != null)
                        {
                            Universe universeModel = universeModels[index++];
                            universeModel.SamplingProcedureId = id;
                            universeModel.Method = content;
                        }
                    }
                }
            }

            studyUnit.UniverseSchemeId = universeSchemeId;
        }

        public static void CreateSampling(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement dataCollectionElem = studyUnitElem.Element(d + TAG_DATA_COLLECTION);
            if (dataCollectionElem == null)
            {
                return;
            }
            string dataCollectionId = (string)dataCollectionElem.Attribute(ATTR_ID);
            if (dataCollectionId == null)
            {
                return;
            }
            XElement methodologyElem = dataCollectionElem.Element(d + TAG_METHODOLOGY);
            if (methodologyElem == null)
            {
                return;
            }
            string methodologyId = (string)methodologyElem.Attribute(ATTR_ID);
            if (methodologyElem == null)
            {
                return;
            }

            List<Sampling> samplingModesl = new List<Sampling>();

            IEnumerable<XElement> collectionEventElems = dataCollectionElem.Elements(d + TAG_COLLECTION_EVENT);
            foreach (XElement collectionEventElem in collectionEventElems)
            {
                Sampling samplingModel = new Sampling();
                samplingModel.DateRange = ReadDateRange(collectionEventElem, d + TAG_DATA_COLLECTION_DATE);
                samplingModel.MemberId = ReadReferenceID(collectionEventElem, d + TAG_DATA_COLLECTOR_ORGANIZATION_REFERENCE);

                XElement modeOfCollection = collectionEventElem.Element(d + TAG_MODE_OF_COLLECTION);
                if (modeOfCollection != null)
                {
                    string content = (string)modeOfCollection.Element(r + TAG_CONTENT);
                    samplingModel.MethodCode = Option.FindCodeByLabel(Options.SamplingMethods, content);
                }

                XElement situationElem = collectionEventElem.Element(d + TAG_COLLECTION_SITUATION);
                if (situationElem != null)
                {
                    samplingModel.Situation = (string)situationElem.Element(r + TAG_CONTENT);
                }
                samplingModesl.Add(samplingModel);
            }
            studyUnit.MethodologyId = methodologyId;
            studyUnit.DataCollectionId = dataCollectionId;


            if (samplingModesl.Count > 0)
            {
                studyUnit.Samplings = samplingModesl;
            }
        }

        public static ConceptScheme CreateConceptScheme(XElement conceptSchemeElem)
        {
            string id = (string)conceptSchemeElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            ConceptScheme conceptSchemeModel = new ConceptScheme();
            conceptSchemeModel.Id = id;
            conceptSchemeModel.Title = (string)conceptSchemeElem.Element(r + TAG_LABEL);
            conceptSchemeModel.Memo = (string)conceptSchemeElem.Element(r + TAG_DESCRIPTION);

            IEnumerable<XElement> elements = conceptSchemeElem.Descendants(c + TAG_CONCEPT);
            foreach (XElement conceptElem in elements)
            {
                string conceptId = (string)conceptElem.Attribute(ATTR_ID);
                if (conceptId == null)
                {
                    continue;
                }
                Concept conceptModel = new Concept();
                conceptModel.Id = conceptId;
                conceptModel.Title = (string)conceptElem.Element(r + TAG_LABEL);
                conceptModel.Content = (string)conceptElem.Element(r + TAG_DESCRIPTION);
                conceptSchemeModel.Concepts.Add(conceptModel);
            }
            return conceptSchemeModel;
        }

        public static void CreateConceptSchemes(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement conceptualComponentElem = studyUnitElem.Element(c + TAG_CONCEPTUAL_COMPONENT);
            if (conceptualComponentElem == null)
            {
                return;
            }
            string conceptualComponentId = (string)conceptualComponentElem.Attribute(ATTR_ID);
            if (conceptualComponentId == null)
            {
                return;
            }
            List<ConceptScheme> conceptSchemeModels = new List<ConceptScheme>();
            IEnumerable<XElement> elements = conceptualComponentElem.Elements(c + TAG_CONCEPT_SCHEME);
            foreach (XElement conceptSchemeElem in elements)
            {
                ConceptScheme conceptSchemeModel = CreateConceptScheme(conceptSchemeElem);
                if (conceptSchemeModel != null)
                {
                    conceptSchemeModels.Add(conceptSchemeModel);
                }
            }
            studyUnit.ConceptualComponentId = conceptualComponentId;
            if (conceptSchemeModels.Count > 0)
            {
                studyUnit.ConceptSchemes = conceptSchemeModels;
            }

            XElement geographicStructureSchemeElem = conceptualComponentElem.Element(c + TAG_GEOGRAPHIC_STRUCTURE_SCHEME);
            if (geographicStructureSchemeElem != null)
            {
                studyUnit.GeographicStructureSchemeId = (string)geographicStructureSchemeElem.Attribute(ATTR_ID);
            }
        }

        private static List<MissingValue> CreateMissingValues(XElement responseElem)
        {
            List<MissingValue> missingValues = new List<MissingValue>();
            string missingValueStr = (string)responseElem.Attribute(ATTR_MISSING_VALUE);
            if (string.IsNullOrEmpty(missingValueStr))
            {
                return missingValues;
            }
            string[] missingValueStrs = missingValueStr.Split(' ');
            foreach (string str in missingValueStrs)
            {
                MissingValue missingValue = new MissingValue() { Content = str };
                missingValues.Add(missingValue);
            }
            return missingValues;
        }

        public static Question CreateQuestion(XElement questionElem)
        {
            string id = (string)questionElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            Question question = new Question();
            question.Id = id;

            //タイトル
            question.Title = (string)questionElem.Element(d + TAG_QUESTION_ITEM_NAME);
            //テキスト
            XElement questionTextElem = questionElem.Element(d + TAG_QUESTION_TEXT);
            if (questionTextElem != null)
            {
                XElement literalTextElem = questionTextElem.Descendants(d + TAG_TEXT).FirstOrDefault();
                if (literalTextElem != null)
                {
                    question.Text = literalTextElem.Value;
                }
            }
            //コンセプト
            string conceptId = ReadReferenceID(questionElem, d + TAG_CONCEPT_REFERENCE);
            question.ConceptId = conceptId;

            //種類
            XElement codeDomainElem = questionElem.Element(d + TAG_CODE_DOMAIN);
            XElement numericDomainElem = questionElem.Element(d + TAG_NUMERIC_DOMAIN);
            XElement textDomainElem = questionElem.Element(d + TAG_TEXT_DOMAIN);
            XElement dateTimeDomain = questionElem.Element(d + TAG_DATE_TIME_DOMAIN);
            if (codeDomainElem != null)
            {
                question.Response.TypeCode = Options.RESPONSE_TYPE_CHOICES_CODE;
                question.Response.Title = (string)codeDomainElem.Element(d + TAG_LABEL);
                question.Response.CodeSchemeId = ReadReferenceID(codeDomainElem, r + TAG_CODE_SCHEME_REFERENCE);

            } else if (numericDomainElem != null)
            {
                //数値の場合の回答
                question.Response.TypeCode = Options.RESPONSE_TYPE_NUMBER_CODE;
                question.Response.Title = (string)numericDomainElem.Element(d + TAG_LABEL);
                string numericTypeLabel = (string)numericDomainElem.Attribute(ATTR_TYPE);
                question.Response.DetailTypeCode = Options.NumberTypeCode(numericTypeLabel);
                XElement numerRangeElem = numericDomainElem.Element(r + TAG_NUMBER_RANGE);
                if (numerRangeElem != null)
                {
                    question.Response.Min = (decimal?)numerRangeElem.Element(r + TAG_LOW);
                    question.Response.Max = (decimal?)numerRangeElem.Element(r + TAG_HIGH);
                }
                question.Response.MissingValues = CreateMissingValues(numericDomainElem);
            } else if (textDomainElem != null)
            {
                //自由テキストの回答
                question.Response.TypeCode = Options.RESPONSE_TYPE_FREE_CODE;
                question.Response.Title = (string)textDomainElem.Element(r + TAG_LABEL);
                question.Response.Min = (decimal?)textDomainElem.Attribute(ATTR_MIN_LENGTH);
                question.Response.Max = (decimal?)textDomainElem.Attribute(ATTR_MAX_LENGTH);
                question.Response.MissingValues = CreateMissingValues(textDomainElem);
            }
            else if (dateTimeDomain != null)
            {
                //日付の回答
                question.Response.TypeCode = Options.RESPONSE_TYPE_DATETIME_CODE;
                question.Response.Title = (string)dateTimeDomain.Element(d + TAG_LABEL);
                string typeLabel = (string)dateTimeDomain.Attribute(ATTR_TYPE);
                question.Response.DetailTypeCode = Options.DateTimeTypeCode(typeLabel);
                question.Response.MissingValues = CreateMissingValues(dateTimeDomain);
            }
            else
            {
                return null;
            }
            return question;
        }

        public static QuestionGroup CreateQuestionGroup(XElement questionGroupElem, List<Question> questionModels) 
        {
            string id = (string)questionGroupElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            QuestionGroup questionGroup = new QuestionGroup();
            questionGroup.Id = id;

            //タイトル
            questionGroup.Title = (string)questionGroupElem.Element(d + TAG_MULTIPLE_QUESTION_ITEM_NAME);
            //メモ
            XElement questionTextElem = questionGroupElem.Element(d + TAG_QUESTION_TEXT);
            if (questionTextElem != null)
            {
                XElement literalTextElem = questionTextElem.Descendants(d + TAG_TEXT).FirstOrDefault();
                if (literalTextElem != null)
                {
                    questionGroup.Memo = literalTextElem.Value;
                }
            }

            IEnumerable<XElement> elements = questionGroupElem.Descendants(d + TAG_QUESTION_ITEM);
            foreach (XElement questionElem in elements)
            {
                Question questionModel = CreateQuestion(questionElem);
                if (questionModel != null)
                {
                    questionModels.Add(questionModel);
                }
            }

            foreach (Question questionModel in questionModels)
            {
                questionGroup.QuestionIds.Add(questionModel.Id);
            }
            return questionGroup;
        }

        public static void CreateQuestions(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement questionSchemeElem = studyUnitElem.Descendants(d + TAG_QUESTION_SCHEME).FirstOrDefault();
            if (questionSchemeElem == null)
            {
                return;
            }
            string questionSchemeId = (string)questionSchemeElem.Attribute(ATTR_ID);
            List<Question> questionModels = new List<Question>();
            IEnumerable<XElement> elements = questionSchemeElem.Elements(d + TAG_QUESTION_ITEM);
            foreach (XElement questionItem in elements)
            {
                Question questionModel = CreateQuestion(questionItem);
                if (questionModel != null)
                {
                    questionModels.Add(questionModel);
                }
            }
            List<QuestionGroup> questionGroupModels = new List<QuestionGroup>();
            elements = questionSchemeElem.Elements(d + TAG_MULTIPLE_QUESTION_ITEM);
            foreach (XElement questionGroupElem in elements)
            {
                List<Question> questionModelsInGroup = new List<Question>();
                QuestionGroup questionGroupModel = CreateQuestionGroup(questionGroupElem, questionModelsInGroup);
                if (questionModelsInGroup != null)
                {
                    questionGroupModels.Add(questionGroupModel);
                    questionModels.AddRange(questionModelsInGroup);
                }
            }
            studyUnit.QuestionSchemeId = questionSchemeId;
            if (questionModels.Count > 0)
            {
                studyUnit.Questions = questionModels;
            }
            if (questionGroupModels.Count > 0)
            {
                studyUnit.QuestionGroups = questionGroupModels;
            }
        }

        private static bool IsAttrMissingValid(string missing)
        {
            if (string.IsNullOrEmpty(missing))
            {
                return false;
            }
            return missing == "1" || missing.ToLower() == "true";
        }

        public static CategoryScheme CreateCategoryScheme(XElement categorySchemeElem)
        {
            string id = (string)categorySchemeElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            CategoryScheme categorySchemeModel = new CategoryScheme();
            categorySchemeModel.Id = id;
            categorySchemeModel.Title = (string)categorySchemeElem.Element(r + TAG_LABEL);
            categorySchemeModel.Memo = (string)categorySchemeElem.Element(r + TAG_DESCRIPTION);

            IEnumerable<XElement> elements = categorySchemeElem.Elements(l + TAG_CATEGORY);
            foreach (XElement categoryElem in elements)
            {
                string categoryId = (string)categoryElem.Attribute(ATTR_ID);
                if (categoryId == null)
                {
                    continue;
                }
                Category categoryModel = new Category();
                if (IsAttrMissingValid((string)categoryElem.Attribute(ATTR_MISSING)))
                {
                    categoryModel.IsMissingValue = true;
                }
                categoryModel.Id = categoryId;
                categoryModel.Title = (string)categoryElem.Element(r + TAG_LABEL);
                categoryModel.Memo = (string)categoryElem.Element(r + TAG_DESCRIPTION);
                categorySchemeModel.Categories.Add(categoryModel);
            }
            return categorySchemeModel;
        }

        public static void CreateCategorySchemes(XElement studyUnitElem, StudyUnit studyUnit)
        {
            List<CategoryScheme> categorySchemeModels = new List<CategoryScheme>();
            IEnumerable<XElement> elements = studyUnitElem.Descendants(l + TAG_CATEGORY_SCHEME);
            foreach (XElement categorySchemeElem in elements)
            {
                CategoryScheme categorySchemeModel = CreateCategoryScheme(categorySchemeElem);
                if (categorySchemeModel != null)
                {
                    categorySchemeModels.Add(categorySchemeModel);
                }
            }
            if (categorySchemeModels.Count > 0)
            {
                studyUnit.CategorySchemes = categorySchemeModels;
            }
        }

        public static CodeScheme CreateCodeScheme(XElement codeSchemeElem)
        {
            string id = (string)codeSchemeElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            CodeScheme codeSchemeModel = new CodeScheme();
            codeSchemeModel.Id = id;
            codeSchemeModel.Title = (string)codeSchemeElem.Element(r + TAG_LABEL);
            codeSchemeModel.Memo = (string)codeSchemeElem.Element(r + TAG_DESCRIPTION);

            IEnumerable<XElement> elements = codeSchemeElem.Elements(l + TAG_CODE);
            foreach (XElement codeElem in elements)
            {
                //string codeId = (string)codeElem.Attribute(ATTR_ID);
                //if (codeId == null)
                //{
                //    continue;
                //}
                Code codeModel = new Code();
                //codeModel.Id = codeId;
                codeModel.CategoryId = ReadReferenceID(codeElem, l + TAG_CATEGORY_REFERENCE);
                codeModel.Value = (string)codeElem.Element(l + TAG_VALUE);
                codeSchemeModel.Codes.Add(codeModel);
            }

            return codeSchemeModel;
        }

        public static void CreateCodeSchemes(XElement studyUnitElem, StudyUnit studyUnit)
        {
            List<CodeScheme> codeSchemeModels = new List<CodeScheme>();
            IEnumerable<XElement> elements = studyUnitElem.Descendants(l + TAG_CODE_SCHEME);
            foreach (XElement codeSchemeElem in elements)
            {
                CodeScheme codeSchemeModel = CreateCodeScheme(codeSchemeElem);
                if (codeSchemeModel != null)
                {
                    codeSchemeModels.Add(codeSchemeModel);
                }
            }
            if (codeSchemeModels.Count > 0)
            {
                studyUnit.CodeSchemes = codeSchemeModels;
            }
        }

        public static Variable CreateVariable(XElement variableElem)
        {
            string id = (string)variableElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            Variable variable = new Variable();
            variable.Id = id;
            variable.Title = (string)variableElem.Element(l + TAG_VARIABLE_NAME);
            variable.Label = (string)variableElem.Element(r + TAG_LABEL);
            variable.ConceptId = ReadReferenceID(variableElem, l + TAG_CONCEPT_REFERENCE);
            variable.QuestionId = ReadReferenceID(variableElem, l + TAG_QUESTION_REFERENCE);
            variable.UniverseId = ReadReferenceID(variableElem, r + TAG_UNIVERSE_REFERENCE);
            XElement representationElem = variableElem.Element(l + TAG_REPRESENTATION);
            if (representationElem != null)
            {
                //種類
                XElement codeRepresentationElem = representationElem.Element(l + TAG_CODE_REPRESENTATION);
                XElement numericRepresentationElem = representationElem.Element(l + TAG_NUMERIC_REPRESENTATION);
                XElement textRepresentationElem = representationElem.Element(l + TAG_TEXT_REPRESENTATION);
                XElement dateTimeRepresentationElem = representationElem.Element(l + TAG_DATE_TIME_REPRESENTATION);
                if (codeRepresentationElem != null)
                {
                    variable.Response.TypeCode = Options.RESPONSE_TYPE_CHOICES_CODE;
                    variable.Response.CodeSchemeId = ReadReferenceID(codeRepresentationElem, r + TAG_CODE_SCHEME_REFERENCE);

                }
                else if (numericRepresentationElem != null)
                {
                    //数値の場合の回答
                    variable.Response.TypeCode = Options.RESPONSE_TYPE_NUMBER_CODE;
                    variable.Response.Title = (string)numericRepresentationElem.Element(d + TAG_LABEL);
                    string numericTypeLabel = (string)numericRepresentationElem.Attribute(ATTR_TYPE);
                    variable.Response.DetailTypeCode = Options.NumberTypeCode(numericTypeLabel);
                    XElement numerRangeElem = numericRepresentationElem.Element(r + TAG_NUMBER_RANGE);
                    if (numerRangeElem != null)
                    {
                        variable.Response.Min = (decimal?)numerRangeElem.Element(r + TAG_LOW);
                        variable.Response.Max = (decimal?)numerRangeElem.Element(r + TAG_HIGH);
                    }
                    variable.Response.MissingValues = CreateMissingValues(numericRepresentationElem);
                }
                else if (textRepresentationElem != null)
                {
                    //自由テキストの回答
                    variable.Response.TypeCode = Options.RESPONSE_TYPE_FREE_CODE;
                    variable.Response.Title = (string)textRepresentationElem.Element(r + TAG_LABEL);
                    variable.Response.Min = (decimal?)textRepresentationElem.Attribute(ATTR_MIN_LENGTH);
                    variable.Response.Max = (decimal?)textRepresentationElem.Attribute(ATTR_MAX_LENGTH);
                    variable.Response.MissingValues = CreateMissingValues(textRepresentationElem);
                }
                else if (dateTimeRepresentationElem != null)
                {
                    //日付の回答
                    variable.Response.TypeCode = Options.RESPONSE_TYPE_DATETIME_CODE;
                    variable.Response.Title = (string)dateTimeRepresentationElem.Element(d + TAG_LABEL);
                    string typeLabel = (string)dateTimeRepresentationElem.Attribute(ATTR_TYPE);
                    variable.Response.DetailTypeCode = Options.DateTimeTypeCode(typeLabel);
                    variable.Response.MissingValues = CreateMissingValues(dateTimeRepresentationElem);
                }
                else
                {
                    return null;
                }
            }
            return variable;
        }

        public static void CreateVariables(XElement studyUnitElem, StudyUnit studyUnit)
        {
            List<Variable> variableModels = new List<Variable>();
            IEnumerable<XElement> elements = studyUnitElem.Descendants(l + TAG_VARIABLE);
            foreach (XElement variableElem in elements)
            {
                Variable variableModel = CreateVariable(variableElem);
                if (variableModel != null)
                {
                    variableModels.Add(variableModel);
                }
            }
            if (variableModels.Count > 0)
            {
                studyUnit.Variables = variableModels;
            }
        }


        public static DataSet CreateDataSet(XElement logicalRecordElem)
        {
            string id = (string)logicalRecordElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            DataSet dataSetModel = new DataSet();
            dataSetModel.Id = id;
            dataSetModel.Title = (string)logicalRecordElem.Element(l + TAG_LOGICAL_RECORD_NAME);
            dataSetModel.Memo = (string)logicalRecordElem.Element(r + TAG_DESCRIPTION);

            IEnumerable<XElement> elements = logicalRecordElem.Descendants(l + TAG_VARIABLE_USED_REFERENCE);
            foreach (XElement variableInRecordElem in elements)
            {
                string variableId = (string)variableInRecordElem.Element(r + TAG_ID);
                if (variableId != null)
                {
                    dataSetModel.VariableGuids.Add(variableId);
                }
            }
            return dataSetModel;
        }

        public static void CreateDataSets(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement logicalProductElem = studyUnitElem.Element(l + TAG_LOGICAL_PRODUCT);
            if (logicalProductElem == null)
            {
                return;
            }
            string logicalProductId = (string)logicalProductElem.Attribute(ATTR_ID);
            if (logicalProductId == null)
            {
                return;
            }
            XElement dataRelationshipElem = logicalProductElem.Element(l + TAG_DATA_RELATIONSHIP);
            if (dataRelationshipElem == null)
            {
                return;
            }
            string dataRelationShipId = (string)dataRelationshipElem.Attribute(ATTR_ID);
            if (dataRelationShipId == null)
            {
                return;
            }
            List<DataSet> dataSetModels = new List<DataSet>();
            IEnumerable<XElement> elements = dataRelationshipElem.Elements(l + TAG_LOGICAL_RECORD);
            foreach (XElement logicalRecordElem in elements)
            {
                DataSet dataSetModel = CreateDataSet(logicalRecordElem);
                if (dataSetModel != null)
                {
                    dataSetModels.Add(dataSetModel);
                }
            }
            studyUnit.LogicalProductId = logicalProductId;
            studyUnit.DataCollectionId = dataRelationShipId;
            if (dataSetModels.Count > 0)
            {
                studyUnit.DataSets = dataSetModels;
            }
        }

        public static XElement FindRecordLayoutElem(IEnumerable<XElement> recordLayoutElems, string id)
        {
            foreach (XElement recordLayoutElem in recordLayoutElems)
            {
                string physicalStructureReferenceId = ReadReferenceID(recordLayoutElem, p + TAG_PHYSICAL_STRUCTURE_REFERENCE);
                if (physicalStructureReferenceId == id)
                {
                    return recordLayoutElem;
                }
            }
            return null;
        }

        public static XElement FindPhysicalInstanceElem(IEnumerable<XElement> physicalInstanceElems, string id)
        {
            foreach (XElement physicalInstanceElem in physicalInstanceElems)
            {
                string recordLayoutId = ReadReferenceID(physicalInstanceElem, pi + TAG_RECORD_LAYOUT_REFERENCE);
                if (recordLayoutId == id)
                {
                    return physicalInstanceElem;
                }
            }
            return null;
        }

        public static DataFile CreateDataFile(XElement physicalStructureElem, IEnumerable<XElement> recordLayoutElems, IEnumerable<XElement> physicalInstanceElems)
        {
            string id = (string)physicalStructureElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            DataFile dataFileModel = new DataFile();
            dataFileModel.Id = id;
            dataFileModel.Format = (string)physicalStructureElem.Element(p + TAG_FORMAT);
            string delimiterLabel = (string)physicalStructureElem.Element(p + TAG_DEFAULT_DELIMITER);
            dataFileModel.DelimiterCode = Options.FindCodeByLabel(Options.Delimiters, delimiterLabel);
            dataFileModel.DataSetId = ReadReferenceID(physicalStructureElem, p + TAG_LOGICAL_RECORD_REFERENCE);

            XElement recordLayoutElem = FindRecordLayoutElem(recordLayoutElems, dataFileModel.Id);
            if (recordLayoutElem != null)
            {
                string recordLayoutId = (string)recordLayoutElem.Attribute(ATTR_ID);
                XElement physicalInstanceElem = FindPhysicalInstanceElem(physicalInstanceElems, recordLayoutId);
                if (physicalInstanceElem != null)
                {
                    XElement dataFileIdentificationElem = physicalInstanceElem.Element(pi + TAG_DATA_FILE_IDENTIFICATION);
                    if (dataFileIdentificationElem != null)
                    {
                        dataFileModel.Uri = (string)dataFileIdentificationElem.Element(pi + TAG_URI);
                    }
                }
            }
            return dataFileModel;
        }

        public static void CreateDataFiles(XElement studyUnitElem, StudyUnit studyUnit)
        {
            XElement physicalDataProductElem = studyUnitElem.Element(p + TAG_PHYSICAL_DATA_PRODUCT);
            if (physicalDataProductElem == null)
            {
                return;
            }
            string physicalDataProductId = (string)physicalDataProductElem.Attribute(ATTR_ID);
            if (physicalDataProductId == null)
            {
                return;
            }
            XElement physicalStructureSchemeElem = physicalDataProductElem.Element(p + TAG_PHYSICAL_STRUCTURE_SCHEME);
            if (physicalStructureSchemeElem == null)
            {
                return;
            }
            string physicalStructureSchemeId = (string)physicalStructureSchemeElem.Attribute(ATTR_ID);
            if (physicalStructureSchemeId == null)
            {
                return;
            }
            XElement recordLayoutSchemeElem = physicalDataProductElem.Element(p + TAG_RECORD_LAYOUT_SCHEME);
            if (recordLayoutSchemeElem == null)
            {
                return;
            }
            string recordLayoutSchemeId = (string)recordLayoutSchemeElem.Attribute(ATTR_ID);
            if (recordLayoutSchemeId == null)
            {
                return;
            }

            List<DataFile> dataFileModels = new List<DataFile>();
            IEnumerable<XElement> physicalStructureElems = physicalStructureSchemeElem.Elements(p + TAG_PHYSICAL_STRUCTURE);
            IEnumerable<XElement> recordLayoutElems = recordLayoutSchemeElem.Descendants(p + TAG_RECORD_LAYOUT);
            IEnumerable<XElement> physicalInstanceElems = studyUnitElem.Descendants(pi + TAG_PHYSICAL_INSTANCE);
            foreach (XElement physicalStructureElem in physicalStructureElems)
            {
                DataFile dataFileModel = CreateDataFile(physicalStructureElem, recordLayoutElems, physicalInstanceElems);
                if (dataFileModel != null)
                {
                    dataFileModels.Add(dataFileModel);
                }
            }

            studyUnit.PhysicalDataProductId = physicalDataProductId;
            studyUnit.PhysicalStructureSchemeId = physicalStructureSchemeId;
            studyUnit.RecordLayoutSchemeId = recordLayoutSchemeId;
            if (dataFileModels.Count > 0)
            {
                studyUnit.DataFiles = dataFileModels;
            }
        }

        private static BookRelationType GetBookRelationType(StudyUnit studyUnit, string id)
        {
            Concept concept = studyUnit.FindConcept(id);
            if (concept != null)
            {
                return BookRelationType.Concept;
            }
            Question question = studyUnit.FindQuestion(id);
            if (question != null)
            {
                return BookRelationType.Question;
            }
            Variable variable = studyUnit.FindVariable(id);
            if (variable != null)
            {
                return BookRelationType.Variable;
            }
            return BookRelationType.Abstract;
        }

        private static Book CreateBook(XElement otherMaterialElem, StudyUnit studyUnit)
        {
            string bookId = (string)otherMaterialElem.Attribute(ATTR_ID);
            if (bookId == null)
            {
                return null;
            }
            string type = (string)otherMaterialElem.Attribute(ATTR_TYPE);
            if (type == null)
            {
                return null;
            }

            XElement citationElem = otherMaterialElem.Element(r + TAG_CITATION);
            if (citationElem == null)
            {
                return null;
            }

            string title = (string)citationElem.Element(r + TAG_TITLE);
            if (title == null)
            {
                return null;
            }
            Book bookModel = new Book();
            bookModel.BookTypeCode = GetBookTypeCode(type);
            bookModel.Id = bookId;
            bookModel.Title = title;

            bookModel.Author = (string)citationElem.Element(r + TAG_CREATOR);
            // PublisherとCityをセット
            ParsePublisher((string)citationElem.Element(r + TAG_PUBLISHER), bookModel);
            bookModel.Editor = (string)citationElem.Element(r + TAG_CONTRIBUTOR);
            bookModel.AnnouncementDate = ReadDateUnitAsString(citationElem, r + TAG_PUBLICATION_DATE);
            bookModel.Language = (string)citationElem.Element(r + TAG_LANGUAGE);


            XElement dcelements = citationElem.Element(dce + TAG_DCELEMENTS);
            if (dcelements != null)
            {
                bookModel.Summary = (string)dcelements.Element(dc + TAG_DC_DESCRIPTION);
                ParseIdentifier((string)dcelements.Element(dc + TAG_DC_IDENTIFIER), bookModel);
            }

            bookModel.Url = (string)otherMaterialElem.Element(r + TAG_EXTERNAL_URL_REFERENCE);
            IEnumerable<XElement> relationshipElems = otherMaterialElem.Elements(r + TAG_RELATIONSHIP);
            foreach (XElement relationshipElem in relationshipElems)
            {
                string id = ReadReferenceID(relationshipElem, r + TAG_RELATED_TO_REFERENCE);
                BookRelation relation = new BookRelation();
                relation.BookRelationType = GetBookRelationType(studyUnit, id);
                if (relation.IsBookRelationTypeAbstract)
                {
                    id = null;
                }
                relation.MetadataId = id;
                bookModel.BookRelations.Add(relation);
            }
            return bookModel;
        }

        private static List<Book> CreateBooksFrom(XElement parentElement, StudyUnit studyUnit)
        {
            List<Book> bookModels = new List<Book>();
            if (parentElement == null)
            {
                return bookModels;  
            }
            IEnumerable<XElement> otherMaterialElems = parentElement.Elements(r + TAG_OTHER_MATERIAL);
            foreach (XElement otherMaterialElem in otherMaterialElems)
            {
                Book bookModel = CreateBook(otherMaterialElem, studyUnit);
                if (bookModel != null)
                {
                    bookModels.Add(bookModel);
                }
            }
            return bookModels;
        }

        public static void CreateBooks(XElement studyUnitElement, StudyUnit studyUnit)
        {
            List<Book> allBooks = new List<Book>();

            //概要と関連づいているもの(StudyUnit直下)
            List<Book> abstractBooks = CreateBooksFrom(studyUnitElement, studyUnit);
            allBooks.AddRange(abstractBooks);

            //変数のイメージと関連づいているもの(ConceptualComponentの下)
            XElement conceptualComponentElem = studyUnitElement.Element(c + TAG_CONCEPTUAL_COMPONENT);
            List<Book> conceptBooks = CreateBooksFrom(conceptualComponentElem, studyUnit);
            allBooks.AddRange(conceptBooks);

            //質問と関連づいているもの
            XElement dataCollectionElem = studyUnitElement.Element(d + TAG_DATA_COLLECTION);
            List<Book> questionBooks = CreateBooksFrom(dataCollectionElem, studyUnit);
            allBooks.AddRange(questionBooks);

            //変数と関連づいているもの
            XElement logicalProductElem = studyUnitElement.Element(l + TAG_LOGICAL_PRODUCT);
            List<Book> variableBooks = CreateBooksFrom(logicalProductElem, studyUnit);
            allBooks.AddRange(variableBooks);


            List<Book> uniqBooks = new List<Book>();
            foreach (Book book in allBooks)
            {
                Book existBook = Book.FindByTitle(uniqBooks, book.Title);
                if (existBook == null)
                {
                    uniqBooks.Add(book);
                }
            }

            if (uniqBooks.Count > 0)
            {
                studyUnit.Books = uniqBooks;
            }
        }

        public static ControlConstructScheme CreateControlConstructScheme(XElement controlConstructSchemeElem, StudyUnit studyUnit)
        {
            string id = (string)controlConstructSchemeElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return null;
            }
            ControlConstructScheme controlConstructSchemeModel = new ControlConstructScheme();
            controlConstructSchemeModel.Title = (string)controlConstructSchemeElem.Element(d + TAG_CONTROL_CONSTRUCT_SCHEME_NAME);
            IEnumerable<XElement> questionConstructElems = controlConstructSchemeElem.Elements(d + TAG_QUESTION_CONSTRUCT);
            foreach (XElement questionConstructElem in questionConstructElems)
            {
                string questionConstructId = (string)questionConstructElem.Attribute(ATTR_ID);
                if (questionConstructId == null)
                {
                    continue;
                }
                string questionId = ReadReferenceID(questionConstructElem, d + TAG_QUESTION_REFERENCE);
                if (questionId == null)
                {
                    continue;
                }
                string no = (string)questionConstructElem.Element(r + TAG_LABEL);
                if (no == null)
                {
                    continue;
                }
                if (studyUnit.FindQuestion(questionId) != null)
                {
                    QuestionConstruct questionConstruct = new QuestionConstruct();
                    questionConstruct.Id = questionConstructId;
                    questionConstruct.No = no;
                    questionConstruct.QuestionId = questionId;
                    controlConstructSchemeModel.QuestionConstructs.Add(questionConstruct);

                }
                else if (studyUnit.FindQuestionGroup(questionId) != null)
                {
                    QuestionGroupConstruct questionGroupConstruct = new QuestionGroupConstruct();
                    questionGroupConstruct.Id = questionConstructId;
                    questionGroupConstruct.No = no;
                    questionGroupConstruct.QuestionGroupId = questionId;
                    controlConstructSchemeModel.QuestionGroupConstructs.Add(questionGroupConstruct);
                }

            }

            IEnumerable<XElement> statementItemElems = controlConstructSchemeElem.Elements(d + TAG_STATEMENT_ITEM);
            foreach (XElement statementItemElem in statementItemElems)
            {
                string statementId = (string)statementItemElem.Attribute(ATTR_ID);
                if (statementId == null)
                {
                    continue;
                }
                string no = (string)statementItemElem.Attribute(r + TAG_LABEL);
                if (no == null)
                {
                    continue;
                }
                Statement statement = new Statement();
                statement.Id = statementId;
                statement.No = no;
                XElement textElem = statementItemElem.Descendants(d + TAG_TEXT).FirstOrDefault();
                if (textElem != null)
                {
                    statement.Text = textElem.Value;
                }
                controlConstructSchemeModel.Statements.Add(statement);
            }

            IEnumerable<XElement> ifThenElseElems = controlConstructSchemeElem.Elements(d + TAG_IF_THEN_ELSE);
            foreach (XElement ifThenElseElem in ifThenElseElems)
            {
                string ifThenElseId = (string)ifThenElseElem.Attribute(ATTR_ID);
                if (ifThenElseId == null)
                {
                    continue;
                }
                XElement ifConditionElem = ifThenElseElem.Element(d + TAG_IF_CONDITION);
                if (ifConditionElem == null)
                {
                    continue;
                }
                string thenConstructId = ReadReferenceID(ifThenElseElem, d + TAG_THEN_CONSTRUCT_REFERENCE);
                if (thenConstructId == null)
                {
                    continue;
                }

                IfThenElse ifThenElse = new IfThenElse();
                ifThenElse.Id = ifThenElseId;
                ifThenElse.No = ControlConstructScheme.IFTHENELSE_NO;
                ifThenElse.IfCondition.Code = (string)ifConditionElem.Element(r + TAG_CODE);
                ifThenElse.IfCondition.QuestionId = ReadReferenceID(ifConditionElem, r + TAG_SOURCE_QUESTION_REFERENCE);
                ifThenElse.ThenConstructId = thenConstructId;
                controlConstructSchemeModel.IfThenElses.Add(ifThenElse);

                IEnumerable<XElement> elseIfElems = ifThenElseElem.Elements(d + TAG_ELSE_IF);
                foreach (XElement elseIfElem in elseIfElems)
                {
                    XElement ifConditionElem2 = elseIfElem.Element(d + TAG_IF_CONDITION);
                    if (ifConditionElem2 == null)
                    {
                        continue;
                    }
                    string thenConstructId2 = ReadReferenceID(elseIfElem, d + TAG_THEN_CONSTRUCT_REFERENCE);
                    if (thenConstructId2 == null)
                    {
                        continue;
                    }
                    ElseIf elseIf = new ElseIf();
                    elseIf.IfCondition.Code = (string)ifConditionElem2.Element(r + TAG_CODE);
                    elseIf.IfCondition.QuestionId = ReadReferenceID(ifConditionElem2, r + TAG_SOURCE_QUESTION_REFERENCE);
                    elseIf.ThenConstructId = thenConstructId2;
                    ifThenElse.ElseIfs.Add(elseIf);
                }
            }

            XElement sequenceElem = controlConstructSchemeElem.Element(d + TAG_SEQUENCE);
            if (sequenceElem != null)
            {
                controlConstructSchemeModel.Sequence.Id = (string)sequenceElem.Attribute(ATTR_ID);
                IEnumerable<XElement> controlConstructReferenceElems = sequenceElem.Elements(d + TAG_CONTROL_CONSTRUCT_REFERENCE);
                foreach (XElement controlConstructReferenceElem in controlConstructReferenceElems)
                {
                    string controlConstructId = (string)controlConstructReferenceElem.Element(r + TAG_ID);
                    if (controlConstructId != null)
                    {
                        controlConstructSchemeModel.Sequence.ControlConstructIds.Add(controlConstructId);
                    }
                }
            }
            return controlConstructSchemeModel;
        }

        public static void CreateControlConstructSchemes(XElement studyUnitElem, StudyUnit studyUnit)
        {
            List<ControlConstructScheme> controlConstructSchemeModels = new List<ControlConstructScheme>();
            IEnumerable<XElement> elements = studyUnitElem.Descendants(d + TAG_CONTROL_CONSTRUCT_SCHEME);
            foreach (XElement controlConstructSchemeElem in elements)
            {
                ControlConstructScheme controlConstructSchemeModel = CreateControlConstructScheme(controlConstructSchemeElem, studyUnit);
                if (controlConstructSchemeModel != null)
                {
                    controlConstructSchemeModels.Add(controlConstructSchemeModel);
                }
            }
            if (controlConstructSchemeModels.Count > 0)
            {
                studyUnit.ControlConstructSchemes = controlConstructSchemeModels;
            }
        }

        private static StudyUnit CreateStudyUnit(XElement studyUnitElement)
        {
            //StudyUnitの読み込み
            StudyUnit studyUnit = StudyUnit.CreateDefault();

            //イベント
            DDI3Reader.CreateEvents(studyUnitElement, studyUnit);

            //組織
            DDI3Reader.CreateOrganizations(studyUnitElement, studyUnit);

            //調査メンバー
            DDI3Reader.CreateMembers(studyUnitElement, studyUnit);

            //調査の概要
            DDI3Reader.CreateAbstract(studyUnitElement, studyUnit);
 
            //調査の範囲
            DDI3Reader.CreateCoverage(studyUnitElement, studyUnit);

            //助成機関
            DDI3Reader.CreateFundingInfos(studyUnitElement, studyUnit);

            //サンプリング(母集団の前に読み込む必要がある)
            DDI3Reader.CreateSampling(studyUnitElement, studyUnit);

            //母集団
            DDI3Reader.CreateUniverses(studyUnitElement, studyUnit);

            //コンセプト
            DDI3Reader.CreateConceptSchemes(studyUnitElement, studyUnit);

            //質問
            DDI3Reader.CreateQuestions(studyUnitElement, studyUnit);

            //カテゴリ
            DDI3Reader.CreateCategorySchemes(studyUnitElement, studyUnit);

            //コード
            DDI3Reader.CreateCodeSchemes(studyUnitElement, studyUnit);

            //順序
            DDI3Reader.CreateControlConstructSchemes(studyUnitElement, studyUnit);

            //変数
            DDI3Reader.CreateVariables(studyUnitElement, studyUnit);

            //データセット
            DDI3Reader.CreateDataSets(studyUnitElement, studyUnit);

            //データファイル
            DDI3Reader.CreateDataFiles(studyUnitElement, studyUnit);

            //関連文献
            DDI3Reader.CreateBooks(studyUnitElement, studyUnit);

            return studyUnit;
        }

        #endregion

        #region Groupの読み込み

        private static EDOModel CreateSingleModel(XElement studyUnitElem)
        {
            EDOModel model = new EDOModel();
            StudyUnit studyUnit = CreateStudyUnit(studyUnitElem);
            if (studyUnit != null)
            {
                model.StudyUnits.Add(studyUnit);
            }
            return model;
        }

        private static CompareItem CreateConceptSchemeCompareItem(XElement conceptMapElem, List<StudyUnit> studyUnits)
        {
            string sourceSchemeId = ReadReferenceID(conceptMapElem, cm + TAG_SOURCE_SCHEME_REFERENCE);
            if (sourceSchemeId == null) {
                return null;
            }
            StudyUnit sourceStudyUnit = StudyUnit.FindByConceptSchemeId(studyUnits, sourceSchemeId);
            if (sourceStudyUnit == null)
            {
                return null;
            }
            ConceptScheme sourceConceptScheme = sourceStudyUnit.FindConceptScheme(sourceSchemeId);

            string targetSchemeId = ReadReferenceID(conceptMapElem, cm + TAG_TARGET_SCHEME_REFERENCE);
            if (targetSchemeId == null) 
            {
                return null;
            }
            StudyUnit targetStudyUnit = StudyUnit.FindByConceptSchemeId(studyUnits, targetSchemeId);
            if (targetStudyUnit == null)
            {
                return null;
            }
            ConceptScheme targetConceptScheme = targetStudyUnit.FindConceptScheme(targetSchemeId);
            if (targetConceptScheme == null)
            {
                return null;
            }

            XElement correspondenceElem = conceptMapElem.Element(cm + TAG_CORRESPONDENCE);
            if (correspondenceElem == null) 
            {
                return null;
            }

            string weightStr = (string)correspondenceElem.Element(cm + TAG_COMMONALITY_WEIGHT);
            if (string.IsNullOrEmpty(weightStr))
            {
                return null;
            }
            double weight = 0;
            if (!double.TryParse(weightStr, out weight))
            {
                return null;
            }
            bool validWeight = weight > 0;
            if (!validWeight) {
                return null;
            }                        
            string memo = (string)correspondenceElem.Element(cm + TAG_COMMONALITY);
            GroupId sourceId = new GroupId(sourceStudyUnit.Id, sourceSchemeId);
            GroupId targetId = new GroupId(targetStudyUnit.Id, targetSchemeId);
            CompareItem compareItem = new CompareItem(sourceId, targetId, memo, weightStr);
            compareItem.SourceTitle = sourceConceptScheme.Title;
            compareItem.TargetTitle = targetConceptScheme.Title;
            return compareItem;
        }

        private static CompareItem CreateConceptCompareItem(XElement conceptMapElem, List<StudyUnit> studyUnits)
        {
            string rawSourceConceptId = (string)conceptMapElem.Element(cm + TAG_SOURCE_ITEM);
            if (rawSourceConceptId == null)
            {
                return null;
            }
            string sourceConceptId = CompareItem.ToOrigId(rawSourceConceptId);
            StudyUnit sourceStudyUnit = StudyUnit.FindByConceptId(studyUnits, sourceConceptId);
            if (sourceStudyUnit == null)
            {
                return null;
            }
            Concept sourceConcept = sourceStudyUnit.FindConcept(sourceConceptId);

            string rawTargetConceptId = (string)conceptMapElem.Element(cm + TAG_TARGET_ITEM);
            if (rawTargetConceptId == null)
            {
                return null;
            }
            string targetConceptId = CompareItem.ToOrigId(rawTargetConceptId);
            StudyUnit targetStudyUnit = StudyUnit.FindByConceptId(studyUnits, targetConceptId);
            if (targetStudyUnit == null)
            {
                return null;
            }
            Concept targetConcept = targetStudyUnit.FindConcept(targetConceptId);
            if (targetConcept == null)
            {
                return null;
            }

            XElement correspondenceElem = conceptMapElem.Element(cm + TAG_CORRESPONDENCE);
            if (correspondenceElem == null)
            {
                return null;
            }

            string weightStr = (string)correspondenceElem.Element(cm + TAG_COMMONALITY_WEIGHT);
            if (string.IsNullOrEmpty(weightStr))
            {
                return null;
            }
            double weight = 0;
            if (!double.TryParse(weightStr, out weight))
            {
                return null;
            }
            bool validWeight = weight > 0;
            if (!validWeight)
            {
                return null;
            }
            string memo = (string)correspondenceElem.Element(cm + TAG_COMMONALITY);
            GroupId sourceId = new GroupId(sourceStudyUnit.Id, sourceConceptId);
            GroupId targetId = new GroupId(targetStudyUnit.Id, targetConceptId);
            CompareItem compareItem = new CompareItem(sourceId, targetId, memo, weightStr);
            compareItem.SourceTitle = sourceConcept.Title;
            compareItem.TargetTitle = targetConcept.Title;
            return compareItem;
        }

        private static CompareItem CreateVariableCompareItem(XElement variableMapElem, List<StudyUnit> studyUnits)
        {
            string rawVariableId = (string)variableMapElem.Element(cm + TAG_SOURCE_ITEM);
            if (rawVariableId == null)
            {
                return null;
            }
            string sourceVariableId = CompareItem.ToOrigId(rawVariableId);
            StudyUnit sourceStudyUnit = StudyUnit.FindByVariableId(studyUnits, sourceVariableId);
            if (sourceStudyUnit == null)
            {
                return null;
            }
            Variable sourceVariable = sourceStudyUnit.FindVariable(sourceVariableId);

            string rawTargetVariableId = (string)variableMapElem.Element(cm + TAG_TARGET_ITEM);
            if (rawTargetVariableId == null)
            {
                return null;
            }
            string targetVariableId = CompareItem.ToOrigId(rawTargetVariableId);
            StudyUnit targetStudyUnit = StudyUnit.FindByVariableId(studyUnits, targetVariableId);
            if (targetStudyUnit == null)
            {
                return null;
            }
            Variable targetVariable = targetStudyUnit.FindVariable(targetVariableId);
            if (targetVariable == null)
            {
                return null;
            }

            XElement correspondenceElem = variableMapElem.Element(cm + TAG_CORRESPONDENCE);
            if (correspondenceElem == null)
            {
                return null;
            }

            string weightStr = (string)correspondenceElem.Element(cm + TAG_COMMONALITY_WEIGHT);
            if (string.IsNullOrEmpty(weightStr))
            {
                return null;
            }
            double weight = 0;
            if (!double.TryParse(weightStr, out weight))
            {
                return null;
            }
            bool validWeight = weight > 0;
            if (!validWeight)
            {
                return null;
            }
            string memo = (string)correspondenceElem.Element(cm + TAG_COMMONALITY);
            GroupId sourceId = new GroupId(sourceStudyUnit.Id, sourceVariableId);
            GroupId targetId = new GroupId(targetStudyUnit.Id, targetVariableId);
            CompareItem compareItem = new CompareItem(sourceId, targetId, memo, weightStr);
            compareItem.SourceTitle = sourceVariable.Title;
            compareItem.TargetTitle = targetVariable.Title;
            return compareItem;
        }

        private static ICollection<CompareRow> CreateCompareRows(List<CompareItem> compareItems)
        {
            Dictionary<string, CompareRow> rowMap = new Dictionary<string, CompareRow>();
            foreach (CompareItem compareItem in compareItems)
            {
                CompareRow row = null;
                if (rowMap.ContainsKey(compareItem.TargetTitle))
                {
                    row = rowMap[compareItem.TargetTitle];
                }
                else
                {
                    row = new CompareRow();
                    row.Memo = compareItem.Memo;
                    row.Title = compareItem.TargetTitle;
                    rowMap[row.Title] = row;
                }
                row.RowGroupIds.Add(compareItem.TargetId); //あくまでもターゲットが基本

                if (compareItem.IsMatch)
                {
                    CompareCell sourceCell = row.FindCell(compareItem.SourceId.StudyUnitId);
                    if (sourceCell == null)
                    {
                        sourceCell = new CompareCell();
                        sourceCell.CompareValue = Options.COMPARE_VALUE_MATCH_CODE;
                        sourceCell.ColumnStudyUnitId = compareItem.SourceId.StudyUnitId;
                        row.Cells.Add(sourceCell);
                    }
                }
                CompareCell targetCell = row.FindCell(compareItem.TargetId.StudyUnitId);
                if (targetCell == null)
                {
                    targetCell = new CompareCell();
                    targetCell.CompareValue = compareItem.IsMatch ? Options.COMPARE_VALUE_MATCH_CODE : Options.COMPARE_VALUE_PARTIALMATCH_CODE; //一致してないのは書き出していないはず
                    if (compareItem.IsPartialPatch)
                    {
                        targetCell.TargetTitle = compareItem.SourceTitle;
                    }
                    targetCell.ColumnStudyUnitId = compareItem.TargetId.StudyUnitId;
                    row.Cells.Add(targetCell);
                }
            }
            return rowMap.Values;
        }

        private static void InitCompareTable(XElement comparisonElem, EDOModel model)
        {
            IEnumerable<XElement> conceptMapElems = comparisonElem.Elements(cm + TAG_CONCEPT_MAP);
            List<CompareItem> conceptSchemeItems = new List<CompareItem>();
            List<CompareItem> conceptItems = new List<CompareItem>();
            List<CompareItem> variableItems = new List<CompareItem>();
            foreach (XElement conceptMapElem in conceptMapElems)
            {
                CompareItem compareItem = CreateConceptSchemeCompareItem(conceptMapElem, model.StudyUnits);
                if (compareItem != null)
                {
                    conceptSchemeItems.Add(compareItem);
                }
                IEnumerable<XElement> itemMapElems = conceptMapElem.Elements(cm + TAG_ITEM_MAP);
                foreach (XElement itemMapElem in itemMapElems)
                {
                    CompareItem conceptItem = CreateConceptCompareItem(itemMapElem, model.StudyUnits);
                    if (conceptItem != null)
                    {
                        conceptItems.Add(conceptItem);
                    }
                }
            }
            IEnumerable<XElement> variableMapElems = comparisonElem.Elements(cm + TAG_VARIABLE_MAP);
            foreach (XElement variableMapElem in variableMapElems)
            {
                IEnumerable<XElement> itemMapElems = variableMapElem.Elements(cm + TAG_ITEM_MAP);
                foreach (XElement itemMapElem in itemMapElems)
                {
                    CompareItem variableItem = CreateVariableCompareItem(itemMapElem, model.StudyUnits);
                    if (variableItem != null)
                    {
                        variableItems.Add(variableItem);
                    }
                }
            }

            ICollection<CompareRow> conceptSchemeCompareRows = CreateCompareRows(conceptSchemeItems);
            model.Group.ConceptSchemeCompareTable.Rows.AddRange(conceptSchemeCompareRows);
            ICollection<CompareRow> conceptCompareRows = CreateCompareRows(conceptItems);
            model.Group.ConceptCompareTable.Rows.AddRange(conceptCompareRows);
            ICollection<CompareRow> variableCompareRows = CreateCompareRows(variableItems);
            model.Group.VariableCompareTable.Rows.AddRange(variableCompareRows);
        }

        private static void InitGroup(XElement groupElem, EDOModel model)
        {
            string id = (string)groupElem.Attribute(ATTR_ID);
            if (id == null)
            {
                return;
            }
            Group group = Group.CreateDefault();
            model.Group = group;
            group.Id = id;
            group.DataSetCode = (string)groupElem.Attribute(ATTR_DATA_SET);
            group.GeographyCode = (string)groupElem.Attribute(ATTR_GEOGRAPHY);
            group.InstrumentCode = (string)groupElem.Attribute(ATTR_INSTRUMENT);
            group.LanguageCode = (string)groupElem.Attribute(ATTR_LANGUAGE);
            group.PanelCode = (string)groupElem.Attribute(ATTR_PANEL);
            group.TimeCode = (string)groupElem.Attribute(ATTR_TIME);
            XElement purposeElem = groupElem.Element(g + TAG_PURPOSE);
            if (purposeElem != null)
            {
                group.PurposeId = (string)purposeElem.Attribute(ATTR_ID);
                group.Purpose = (string)purposeElem.Element(r + TAG_CONTENT);
            }
            XElement comparisonElem = groupElem.Element(cm + TAG_COMPARISON);
            if (comparisonElem != null)
            {
                InitCompareTable(comparisonElem, model);
            }
        }

        private static EDOModel CreateGroupModel(XElement groupElem)
        {
            EDOModel model = new EDOModel();
            IEnumerable<XElement> elements = groupElem.Descendants(s + TAG_STUDY_UNIT);
            foreach (XElement studyUnitElem in elements)
            {
                StudyUnit studyUnit = CreateStudyUnit(studyUnitElem);
                if (studyUnit != null)
                {
                    if (Group.IsSharedStudyUnit(studyUnit))
                    {
                        model.Group.SharedStudyUnit = studyUnit;
                    }
                    else
                    {
                        model.StudyUnits.Add(studyUnit);
                    }
                }
            }
            InitGroup(groupElem, model);
            return model;
        }

        #endregion
        private void MergeEvent(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            //イベントを単純に追加
            curStudyUnit.Events.AddRange(newStudyUnit.Events);
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
            //調査の概要は上書き
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

            //3.カバーする時期
            curCoverage.DateRange = newCoverage.DateRange;

            //4.カバーする地域のレベル
            CheckOption.Merge(newCoverage.Areas, curCoverage.Areas);

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

        private void MergeCategory(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.CategorySchemes.AddRange(newStudyUnit.CategorySchemes);
        }

        private void MergeCode(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.CodeSchemes.AddRange(newStudyUnit.CodeSchemes);
        }

        private void MergeSequence(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.ControlConstructSchemes.AddRange(newStudyUnit.ControlConstructSchemes);
        }

        private void MergeQuestionGroup(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.QuestionGroups.AddRange(newStudyUnit.QuestionGroups);
        }

        private void MergeVariable(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.Variables.AddRange(newStudyUnit.Variables);
        }

        private void MergeDataSet(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            if (newStudyUnit.DataSets.Count > 0 && newStudyUnit.DataSets[0].Title == EDOConstants.LABEL_ALL)
            {
                newStudyUnit.DataSets.RemoveAt(0);
            }
            curStudyUnit.DataSets.AddRange(newStudyUnit.DataSets);
        }


        private void MergeDataFile(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            int upper = newStudyUnit.DataFiles.Count - 1;
            for (int i = upper; i >= 0; i--)
            {
                DataFile dataFile = newStudyUnit.DataFiles[i];
                if (newStudyUnit.FindDataSet(dataFile.DataSetId) == null)
                {
                    newStudyUnit.DataFiles.RemoveAt(i);
                }
            }
            curStudyUnit.DataFiles.AddRange(newStudyUnit.DataFiles);
        }

        private void MergeBook(StudyUnit newStudyUnit, StudyUnit curStudyUnit)
        {
            curStudyUnit.Books.AddRange(newStudyUnit.Books);
        }

        private void MergeStudyUnit(StudyUnit newStudyUnit, StudyUnit curStudyUnit, DDIImportOption importOption)
        {
            DDIUtils.RenameIds(curStudyUnit, newStudyUnit);
            //イベント
            if (importOption.ImportEvent)
            {
                MergeEvent(newStudyUnit, curStudyUnit);
            }

            //調査メンバー
            if (importOption.ImportMember)
            {
                MergeMember(newStudyUnit, curStudyUnit);
            }

            //調査の概要
            if (importOption.ImportAbstract)
            {
                MergeAbstract(newStudyUnit, curStudyUnit);
            }

            //調査の範囲
            if (importOption.ImportCoverage)
            {
                MergeCoverage(newStudyUnit, curStudyUnit);
            }

            //研究資金
            if (importOption.ImportFundingInfo)
            {
                MergeFundingInfo(newStudyUnit, curStudyUnit);
            }

            //データの収集法法
            if (importOption.ImportSampling)
            {
                MergeSampling(newStudyUnit, curStudyUnit);
            }

            //変数のイメージ
            if (importOption.ImportConcept)
            {
                MergeConcept(newStudyUnit, curStudyUnit);
            }

            //質問
            if (importOption.ImportQuestion)
            {
                MergeQuestion(newStudyUnit, curStudyUnit);
            }

            //選択肢
            if (importOption.ImportCategory)
            {
                MergeCategory(newStudyUnit, curStudyUnit);
            }

            //コード
            if (importOption.ImportCode)
            {
                MergeCode(newStudyUnit, curStudyUnit);
            }

            //順序
            if (importOption.ImportQuestion)
            {
                MergeSequence(newStudyUnit, curStudyUnit);
            }

            if (importOption.ImportQuestionGroup)
            {
                MergeQuestionGroup(newStudyUnit, curStudyUnit);
            }

            //変数
            if (importOption.ImportVariable)
            {
                MergeVariable(newStudyUnit, curStudyUnit);
            }

            //データセット
            if (importOption.ImportDataSet)
            {
                MergeDataSet(newStudyUnit, curStudyUnit);
            }

            //データファイル
            if (importOption.ImportDataFile)
            {
                MergeDataFile(newStudyUnit, curStudyUnit);
            }

            //関連文献
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
                XElement groupElem = doc.Descendants(g + TAG_GROUP).FirstOrDefault();
                if (groupElem != null)
                {
                    newEdoModel = CreateGroupModel(groupElem);
                }
                else
                {
                    XElement studyUnitElem = doc.Descendants(s + TAG_STUDY_UNIT).FirstOrDefault();
                    if (studyUnitElem != null)
                    {
                        newEdoModel = CreateSingleModel(studyUnitElem);
                    }
                }
            }
            if (newEdoModel == null)
            {
                return false;
            }
            DDI3ImportOption importOption = new DDI3ImportOption();
            SelectStudyUnitWindowVM vm = new SelectStudyUnitWindowVM(newEdoModel, curEdoModel, curStudyUnit.StudyUnitModel, importOption);
            SelectStudyUnitWindow window = new SelectStudyUnitWindow(vm);
            window.Owner = Application.Current.MainWindow;
            bool? result = window.ShowDialog();
            if (result != true)
            {
                return false;   
            }
            MergeStudyUnit(vm.FromStudyUnit, vm.ToStudyUnit, vm.ImportOption);
            return true;
        }
    }
}
