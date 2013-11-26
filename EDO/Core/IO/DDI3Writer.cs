using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using EDO.Core.Model;
using System.Windows;
using EDO.Core.Util;
using EDO.Core.ViewModel;
using EDO.Properties;
using EDO.QuestionCategory.QuestionGroupForm;
using EDO.QuestionCategory.QuestionForm;

namespace EDO.Core.IO
{
    public class DDI3Writer :DDI3IO
    {
        private static XDeclaration DECLARATION = new XDeclaration("1.0", "UTF-8", "no");

        private static XElement CreateNullableDescription(string desc)
        {
            return CreateNullable(r + TAG_DESCRIPTION, desc);
        }

        private XElement CreateNullableLabel(object obj)
        {
            return CreateNullable(r + TAG_LABEL, obj);
        }

        private EDOConfig config;

        public DDI3Writer(EDOConfig config)
        {
            this.config = config;
        }

        private StudyUnitVM studyUnit;

        private void AddError(string message, FormVM form)
        {
            WriteError newErrorInfo = new WriteError(message, studyUnit, form);
            if (!ContainsError(newErrorInfo))
            {
                AddError(newErrorInfo);
            }
        }

        private int RemoveError(StudyUnitVM studyUnitVM)
        {
            return RemoveError(param =>
            {
                WriteError writeError = (WriteError)param;
                return writeError.EDOUnit == studyUnitVM;
            });
        }

        private XAttribute CreateIDAttribute(object id)
        {
            return new XAttribute(ATTR_ID, id);
        }

        private XAttribute CreateAgencyAttribute()
        {
            return new XAttribute(ATTR_AGENCY, AGENCY);
        }

        private XAttribute CreateVersionAttribute()
        {
            return new XAttribute(ATTR_VERSION, VERSION);
        }

        private XElement CreateUserID()
        {
            string userId = config.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                userId = IDUtils.NewGuid();
                config.UserId = userId;
            }
            return new XElement(r + TAG_USER_ID, new XAttribute(ATTR_TYPE, "EDO"), userId);
        }

        private XElement CreateID(string id)
        {
            return new XElement(r + TAG_ID, id);
        }

        private XElement CreateContent(object obj)
        {
            return new XElement(r + TAG_CONTENT, obj);
        }

        private XElement CreateReferenceID(XName name, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }    
            return new XElement(name, CreateID(id),
                    new XElement(r + TAG_IDENTIFYING_AGENCY, AGENCY),
                    new XElement(r + TAG_VERSION, VERSION));
        }

        private XElement CreateReferenceID(XName name, string id, string schemeId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(schemeId))
            {
                return null;
            }
            XElement scheme = new XElement(r + TAG_SCHEME,
                CreateID(schemeId), 
                new XElement(r + TAG_IDENTIFYING_AGENCY, AGENCY),
                new XElement(r + TAG_VERSION, VERSION));

            return new XElement(name, 
                scheme,
                CreateID(id),
                new XElement(r + TAG_IDENTIFYING_AGENCY, AGENCY),
                new XElement(r + TAG_VERSION, VERSION));
        }

        private XElement CreateReferenceModuleID(XName name, string id, string moduleId)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(moduleId))
            {
                return null;
            }
            XElement module = new XElement(r + TAG_MODULE,
                CreateID(moduleId),
                new XElement(r + TAG_IDENTIFYING_AGENCY, AGENCY),
                new XElement(r + TAG_VERSION, VERSION));

            return new XElement(name,
                module,
                CreateID(id),
                new XElement(r + TAG_IDENTIFYING_AGENCY, AGENCY),
                new XElement(r + TAG_VERSION, VERSION));
        }

        private XElement CreateLabel(object obj)
        {
            return new XElement(r + TAG_LABEL, obj);
        }

        private XElement CreateDescription(object obj)
        {
            return new XElement(r + TAG_DESCRIPTION, obj);
        }

        private XElement CreateDate(XName name, DateRange dateRange)
        {
            if (dateRange.IsEmpty)
            {
                return null;
            }
            if (dateRange.IsFromDateOnly)
            {
                return new XElement(name,
                    new XElement(r + TAG_SIMPLE_DATE, ToString(dateRange.FromDateTime)));
            } 
            return new XElement(name,
                new XElement(r + TAG_START_DATE, ToString(dateRange.FromDateTime)),
                new XElement(r + TAG_END_DATE, ToString(dateRange.ToDateTime)));
        }

        private XElement CreateAgencyOrganizationReference(Organization organization)
        {
            if (organization == null)
            {
                AddError(Resources.InputOrganization, studyUnit.MemberForm); //組織を入力してください
                return null;
            }
            return CreateReferenceID(r + TAG_AGENCY_ORGANIZATION_REFERENCE, organization.Id, OrganizationSchemeId());
        }

        private XElement CreateLifecycleEvent(Event eventModel)
        {
            ///// LifeCycleEventの生成
            // Description以外必須。必須項目が未入力の場合場合nullを返す。
            XElement date = CreateDate(r + TAG_DATE, eventModel.DateRange);
            if (date == null)
            {
                return null;
            }
            XElement description = new XElement(r + TAG_DESCRIPTION, eventModel.Title);
            if (description == null)
            {
                return null;
            }
            Organization organizationModel = studyUnit.OrganizationModels.Count > 0 ? studyUnit.OrganizationModels[0] : null;
            XElement agencyOrganizationReference = CreateAgencyOrganizationReference(organizationModel);
            if (agencyOrganizationReference == null)
            {
                return null;
            }
            XElement memo = new XElement(r + TAG_LABEL, eventModel.Memo);
            XElement lifecycleEvent = new XElement(r + TAG_LIFECYCLE_EVENT, CreateIDAttribute(eventModel.Id));
            lifecycleEvent.Add(memo);
            lifecycleEvent.Add(date);
            lifecycleEvent.Add(agencyOrganizationReference);
            lifecycleEvent.Add(description);
            return lifecycleEvent;
        }

        private XElement CreateIndividual(Member member)
        {
            ///// a:Individual Memberの生成
            XElement individual = new XElement(a + TAG_INDIVIDUAL,
                CreateIDAttribute(member.Id));
            //IndividualName?
            XElement individualName = new XElement(a + TAG_INDIVIDUAL_NAME);
            //First?
            individualName.Add(CreateNullable(a + TAG_FIRST, member.FirstName));
            //Last?
            individualName.Add(CreateNullable(a + TAG_LAST, member.LastName));
            individual.Add(EmptyToNull(individualName));
            //Position*
            XElement position = new XElement(a + TAG_POSITION);
            //Title
            position.Add(CreateNullable(a + TAG_TITLE, member.Position));
            individual.Add(EmptyToNull(position));
            //r:Description*
            individual.Add(CreateNullable(r + TAG_DESCRIPTION, Options.RoleLabel(member.RoleCode)));
            //Relation*
            individual.Add(
                new XElement(a + TAG_RELATION,
                    CreateReferenceID(a + TAG_ORGANIZATION_REFERENCE, member.OrganizationId),
                    new XElement(r + TAG_DESCRIPTION) //r:Description+
                    ));
            return individual;
        }

        private XElement CreateOrganization(Organization organization)
        {
            ///// Organization
            return new XElement(a + TAG_ORGANIZATION,
                CreateIDAttribute(organization.Id),
                new XElement(a + TAG_ORGANIZATION_NAME, organization.OrganizationName));
        }

        private XElement CreateArchive()
        {
            ///// a:Archive
            XElement archive = new XElement(a + TAG_ARCHIVE,
                CreateIDAttribute(studyUnit.StudyUnitModel.ArchiveId),
                CreateVersionAttribute(),
                CreateAgencyAttribute()
                );
            if (studyUnit.OrganizationModels.Count == 0)
            {
                AddError(Resources.InputOrganization, studyUnit.MemberForm); //組織を入力してください
            } else {
                //a:ArchiveSpecific
                Organization organization = studyUnit.OrganizationModels[0];
                XElement organizationReference = new XElement(a + TAG_ARCHIVE_SPECIFIC,
                    CreateReferenceID(a + TAG_ARCHIVE_ORGANIZATION_REFERENCE, organization.Id, OrganizationSchemeId()));
                archive.Add(organizationReference);
            }
            //a:OrganizationScheme
            XElement organizationScheme = new XElement(a + TAG_ORGANIZATION_SCHEME, 
                CreateIDAttribute(studyUnit.StudyUnitModel.OrganizationSchemeId),
                CreateVersionAttribute(), 
                CreateAgencyAttribute()
                );
            archive.Add(organizationScheme);
            foreach (Organization organizationModel in studyUnit.OrganizationModels)
            {
                //Organization*
                XElement organization = CreateOrganization(organizationModel);
                organizationScheme.Add(organization);
            }
            foreach (FundingInfo fundingInfo in studyUnit.FundingInfoModels)
            {
                //助成機関の名前を組織として保存
                //Organization*
                XElement organization = CreateOrganization(fundingInfo.Organization);
                organizationScheme.Add(organization);
            }
            foreach (Member member in studyUnit.MemberModels)
            {
                //Individual*
                XElement individual = CreateIndividual(member);
                organizationScheme.Add(individual);
            }
            //r:LifecycleInformation? 調査イベント
            XElement lifecycleInformation = new XElement(r + TAG_LIFECYCLE_INFORMATION);
            foreach (Event eventModel in studyUnit.EventModels)
            {
                XElement lifecycleEvent = CreateLifecycleEvent(eventModel);
                lifecycleInformation.Add(lifecycleEvent);
            }
            archive.Add(EmptyToNull(lifecycleInformation));
            return archive;
        }

        private XElement CreateTopicalCoverage()
        {
            ///// r:TopicalCoverage
            XElement topicalCoverage = new XElement(r + TAG_TOPICAL_COVERAGE, CreateIDAttribute(studyUnit.CoverageModel.TopicalCoverageId));
            //r:Subject* 調査対象のチェックボックス
            foreach (CheckOption topicOption in studyUnit.CoverageModel.Topics)
            {
                if (!topicOption.IsChecked)
                {
                    continue;
                }
                string label = topicOption.HasMemo ? topicOption.Memo : Options.CoverageTopicLabel(topicOption.Code);
                XElement topic = new XElement(r + TAG_SUBJECT, label);
                topicalCoverage.Add(topic);
            }
            //r:Keyword* キーワード
            foreach (Keyword keywordModel in studyUnit.CoverageModel.Keywords)
            {
                XElement keyword = new XElement(r + TAG_KEYWORD, keywordModel.Content);
                topicalCoverage.Add(keyword);
            }
            //要素を含まない場合nullを返す
            return EmptyToNull(topicalCoverage);
        }

        private XElement CreateTemporalCoverage()
        {
            ///// r:TemporalCoverage
            //r: ReferenceDate* 調査日付
            XElement temporalCoverage = new XElement(r + TAG_TEMPORAL_COVERAGE,
                CreateIDAttribute(studyUnit.CoverageModel.TemporalCoverageId),
                CreateDate(r + TAG_REFERENCE_DATE, studyUnit.CoverageModel.DateRange));
            //要素を含まない場合nullを返す
            return EmptyToNull(temporalCoverage);
        }

        private XElement CreateSpatialCoverage()
        {
            ///// r:SpatialCoverage
            List<CheckOption> checkedAreas = studyUnit.CoverageModel.CheckedAreas;
            int areaCount = checkedAreas.Count;
            if (areaCount == 0)
            {
                return null;
            }
            //調査の地域
            XElement spatialCoverage = new XElement(r + TAG_SPATIAL_COVERAGE,
                CreateIDAttribute(studyUnit.CoverageModel.SpatialCoverageId));
            //Description* メモ
            spatialCoverage.Add(CreateNullableDescription(studyUnit.CoverageModel.Memo));
            //GeographicStructureReference*
            foreach (CheckOption option in checkedAreas)
            {
                XElement reference = CreateReferenceID(r + TAG_GEOGRAPHIC_STRUCTURE_REFERENCE, studyUnit.CoverageModel.GetGeographicStructureId(option));
                spatialCoverage.Add(reference);
            }
            //TopLevelReference
            CheckOption topArea = checkedAreas[0];
            XElement topLevelReference = new XElement(r + TAG_TOP_LEVEL_REFERENCE,
                CreateReferenceID(r + TAG_LEVEL_REFERENCE, studyUnit.CoverageModel.GetGeographicStructureId(topArea), GeographicStructureSchemeId()),
                new XElement(r + TAG_LEVEL_NAME, topArea.Label));
            spatialCoverage.Add(topLevelReference);
            //LowestLevelReference
            CheckOption lowestArea = checkedAreas[areaCount - 1];
            XElement lowestLevelReference = new XElement(r + TAG_LOWEST_LEVEL_REFERENCE,
                CreateReferenceID(r + TAG_LEVEL_REFERENCE, studyUnit.CoverageModel.GetGeographicStructureId(lowestArea), GeographicStructureSchemeId()),
                new XElement(r + TAG_LEVEL_NAME, lowestArea.Label));
            spatialCoverage.Add(lowestLevelReference);
            //必ず要素が含まれるのでNull変換は必要なし
            return spatialCoverage;
        }

        private XElement CreateCoverage()
        {
            ///// r:Coverage
            XElement coverage =  new XElement(r + TAG_COVERAGE);
            //r:TopicalCoverage?
            coverage.Add(CreateTopicalCoverage());
            //r:SpatialCoverage?
            coverage.Add(CreateSpatialCoverage());
            //r:TemporalCoverage?
            coverage.Add(CreateTemporalCoverage());
            //要素を含まない場合はnullを返す
            return EmptyToNull(coverage);
        }

        private XElement CreateFundingInformation(FundingInfo fundingInfo)
        {
            ///// r:FundingInformationの書き出し
            XElement fundingInfoElem =  new XElement(r + TAG_FUNDING_INFORMATION);
            //AgencyOrganizationReference+
            fundingInfoElem.Add(CreateAgencyOrganizationReference(fundingInfo.Organization));
            //GrantNumber*
            fundingInfoElem.Add(CreateNullable(r + TAG_GRANT_NUMBER, fundingInfo.Number));
            //Description*
            fundingInfoElem.Add(CreateNullable(r + TAG_DESCRIPTION, ToDDIFundingInfoMoney(fundingInfo.Money)));
            fundingInfoElem.Add(CreateNullable(r + TAG_DESCRIPTION, ToDDIFundingInfoTitle(fundingInfo.Title)));
            fundingInfoElem.Add(CreateNullable(r + TAG_DESCRIPTION, ToDDIFundingInfoDateRange(fundingInfo.DateRange)));
            return fundingInfoElem;
        }

        private string SamplingError(string message, Sampling samplingModel)
        {
            return message + "(" + samplingModel.Title + ")";
        }

        private XElement CreateConceptualComponent(bool isGroup)
        {
            //c:ConceptualComponent
            XElement conceptComponent = new XElement(c + TAG_CONCEPTUAL_COMPONENT, 
                CreateIDAttribute(studyUnit.StudyUnitModel.ConceptualComponentId),
                CreateVersionAttribute(), 
                CreateAgencyAttribute()
                );

            if (!isGroup)
            {
                //関連文献
                List<XElement> otherMaterials = CreateOtherMaterials(BookRelationType.Concept);
                conceptComponent.Add(otherMaterials);
            }

            //c:ConceptScheme*
            foreach (ConceptScheme conceptSchemeModel in studyUnit.ConceptSchemeModels)
            {
                XElement conceptScheme = new XElement(c + TAG_CONCEPT_SCHEME,
                    CreateIDAttribute(conceptSchemeModel.Id),
                    CreateVersionAttribute(), 
                    CreateAgencyAttribute(),
                    CreateNullable(r + TAG_LABEL, conceptSchemeModel.Title), // r:Label*
                    CreateNullable(r + TAG_DESCRIPTION, conceptSchemeModel.Memo)); // r:Description*

                //c:Concept*
                foreach (Concept conceptModel in conceptSchemeModel.Concepts)
                {
                    XElement concept = new XElement(c + TAG_CONCEPT,
                        CreateIDAttribute(conceptModel.Id),
                        CreateNullable(r + TAG_LABEL, conceptModel.Title),// r:Label*
                        CreateNullable(r + TAG_DESCRIPTION, conceptModel.Content));// r:Description*
                    conceptScheme.Add(EmptyToNull(concept));
                }
                conceptComponent.Add(EmptyToNull(conceptScheme));
            }

            // c:UniverseScheme* 母集団スキーム
            XElement universeScheme = new XElement(c + TAG_UNIVERSE_SCHEME,
                CreateIDAttribute(UniverseSchemeId()),
                CreateVersionAttribute(),
                CreateAgencyAttribute()
                );
            conceptComponent.Add(universeScheme);
            foreach (Sampling samplingModel in studyUnit.SamplingModels)
            {
                // メイン母集団
                Universe mainUniverseModel = samplingModel.MainUniverse;
                if (mainUniverseModel == null)
                {
                    if (isGroup)
                    {
                        continue;
                    }
                    AddError(SamplingError(Resources.InputUniverse, samplingModel), studyUnit.SamplingForm); //母集団を入力してください
                }
                else
                {

                    // c:Universe*(タイトルが<未入力>に変換されるので空要素になることはない)
                    XElement universe = new XElement(c + TAG_UNIVERSE,
                        CreateIDAttribute(mainUniverseModel.Id),
                        CreateNullable(r + TAG_LABEL, mainUniverseModel.Title), //r:Label*
                        CreateNullable(c + TAG_HUMAN_READABLE, mainUniverseModel.Memo)); //c:HumanReadable
                    universeScheme.Add(universe);
                    foreach (Universe subUniverseModel in samplingModel.Universes)
                    {
                        if (subUniverseModel == mainUniverseModel)
                        {
                            //メインは一つしか存在しないと仮定
                            continue;
                        }
                        //メイン以外はサブ母集団として保存する
                        //c:Subuniverse*
                        XElement subUniverse = new XElement(c + TAG_SUB_UNIVERSE,
                            CreateIDAttribute(subUniverseModel.Id),
                            CreateNullable(r + TAG_LABEL, subUniverseModel.Title),//r:Label*
                            CreateNullable(c + TAG_HUMAN_READABLE, subUniverseModel.Memo));//c:HumanReadable
                        universe.Add(subUniverse);
                    }
                }
            }

            if (!isGroup)
            {
                // 国・都道府県などのレベル
                //GeographicStructureScheme*
                XElement geographicsStructureScheme = new XElement(c + TAG_GEOGRAPHIC_STRUCTURE_SCHEME, 
                    CreateIDAttribute(GeographicStructureSchemeId()),
                    CreateVersionAttribute(),
                    CreateAgencyAttribute()
                    );
                conceptComponent.Add(geographicsStructureScheme);
                //r:GeographicStructure(ラベルが必ず保存されるのでNullチェック/変換は必要ない)
                foreach (CheckOption option in studyUnit.CoverageModel.CheckedAreas)
                {
                    XElement geographicStructure = new XElement(r + TAG_GEOGRAPHIC_STRUCTURE,
                        CreateIDAttribute(studyUnit.CoverageModel.GetGeographicStructureId(option)),
                        new XElement(r + TAG_GEOGRAPHY,
                            CreateIDAttribute(studyUnit.CoverageModel.GetGeographicId(option)),
                            new XElement(r + TAG_LEVEL,
                                new XElement(r + TAG_NAME, option.Label))));
                    geographicsStructureScheme.Add(geographicStructure);
                }

            }
            return conceptComponent;
        }

        private XElement CreateCodeResponse(XName name, Response response)
        {
            XElement codeResponse = new XElement(name);
            codeResponse.Add(CreateReferenceID(r + TAG_CODE_SCHEME_REFERENCE, response.CodeSchemeId, response.CodeSchemeId));
            return codeResponse;
        }

        private XAttribute CreateMissingValueAttribute(Response response)
        {
            if (response.MissingValues.Count == 0)
            {
                return null;
            }
            return new XAttribute(ATTR_MISSING_VALUE, response.JoinMissingValuesContent());
        }

        private XElement CreateNumberResponse(XName name, Response response)
        {
            return new XElement(name,
                CreateMissingValueAttribute(response),
                new XAttribute(ATTR_TYPE, Options.NumberTypeDDILabel(response.DetailTypeCode)),
                new XElement(r + TAG_NUMBER_RANGE,
                    new XElement(r + TAG_LOW, response.Min),
                    new XElement(r + TAG_HIGH, response.Max)
                    ));
        }

        private XElement CreateTextResponse(XName name, Response response)
        {
            XElement textDomain = new XElement(name,
                CreateMissingValueAttribute(response),
                new XAttribute(ATTR_MAX_LENGTH, response.Max),
                new XAttribute(ATTR_MIN_LENGTH, response.Min));
            return textDomain;
        }

        private XElement CreateDateTimeResponse(XName name, Response response)
        {
            XElement dateTimeDomain = new XElement(name,
                CreateMissingValueAttribute(response),
                new XAttribute(ATTR_TYPE, Options.DateTimeTypeDDILabel(response.DetailTypeCode))
                );
            return dateTimeDomain;
        }

        private string UniverseSchemeId()
        {
            return studyUnit.StudyUnitModel.UniverseSchemeId;
        }

        private string GeographicStructureSchemeId()
        {
            return studyUnit.StudyUnitModel.GeographicStructureSchemeId;        
        }

        private string OrganizationSchemeId()
        {
            return studyUnit.StudyUnitModel.OrganizationSchemeId;
        }

        private string QuestionSchemeId()
        {
            return studyUnit.StudyUnitModel.QuestionSchemeId;
        }

        private string VariableSchemeId() 
        {
            return studyUnit.VariableSchemeModel.Id;
        }
        
        private string LogicalProductId()
        {
            return studyUnit.StudyUnitModel.LogicalProductId;
        }

        private string PhysicalStructureSchemeId()
        {
            return studyUnit.StudyUnitModel.PhysicalStructureSchemeId;
        }

        private string RecordLayoutSchemeId()
        {
            return studyUnit.StudyUnitModel.RecordLayoutSchemeId;
        }

        private string ConceptSchemeId(string conceptId)
        {
            ConceptScheme conceptScheme = studyUnit.StudyUnitModel.FindConceptSchemeByConceptId(conceptId);
            return conceptScheme != null ? conceptScheme.Id : null;
        }

        private string CategorySchemeId(string categoryId)
        {
            CategoryScheme categoryScheme = studyUnit.StudyUnitModel.FindCategorySchemeByCategoryId(categoryId);
            return categoryScheme != null ? categoryScheme.Id : null;
        }


        private XElement CreateQuestionItem(Question questionModel)
        {
            //d:QuestionItem+(表示名、質問文は自動設定されるのでnullチェック不要)
            XElement questionItem = new XElement(d + TAG_QUESTION_ITEM,
                CreateIDAttribute(questionModel.Id),
                new XElement(d + TAG_QUESTION_ITEM_NAME, questionModel.Title), //QuestionItemName*
                new XElement(d + TAG_QUESTION_TEXT, //QuestionText*
                    new XElement(d + TAG_LITERAL_TEXT, //LiteralTextText+
                        new XElement(d + TAG_TEXT, questionModel.Text))));//Text

            //d:ResponseDomain
            XElement response = null;
            Response responseModel = questionModel.Response;
            if (responseModel.IsTypeChoices)
            {
                response = CreateCodeResponse(d + TAG_CODE_DOMAIN, responseModel);
                response.Add(new XElement(d + TAG_LABEL, responseModel.Title));
            }
            else if (responseModel.IsTypeNumber)
            {
                if (responseModel.DetailTypeCode == null)
                {
                    MenuItemVM menuItem = studyUnit.FindMenuItem(studyUnit.QuestionForm);
                    AddError(Resources.SelectNumberType, studyUnit.QuestionForm); //数値の型を選択してください
                }
                else if (responseModel.Min == null || responseModel.Max == null)
                {
                    AddError(Resources.InputMinAndMax, studyUnit.QuestionForm); //最大値・最小値を入力してください
                }
                else
                {
                    response = CreateNumberResponse(d + TAG_NUMERIC_DOMAIN, responseModel);
                    response.Add(new XElement(d + TAG_LABEL, responseModel.Title));
                }
            }
            else if (responseModel.IsTypeFree)
            {
                if (responseModel.Min == null || responseModel.Max == null)
                {
                    AddError(Resources.InputMinAndMax, studyUnit.QuestionForm); //最大値・最小値を入力してください
                }
                else
                {
                    response = CreateTextResponse(d + TAG_TEXT_DOMAIN, responseModel);
                    response.Add(new XElement(r + TAG_LABEL, responseModel.Title));
                }
            }
            else if (responseModel.IsTypeDateTime)
            {
                if (responseModel.DetailTypeCode == null)
                {
                    AddError(Resources.SelectDateType, studyUnit.QuestionForm); //日付の型を選択してください
                }
                else
                {
                    response = CreateDateTimeResponse(d + TAG_DATE_TIME_DOMAIN, responseModel);
                    response.Add(new XElement(d + TAG_LABEL, responseModel.Title));
                }
            }
            questionItem.Add(response);
            //ConceptReference*
            XElement conceptReference = CreateReferenceID(d + TAG_CONCEPT_REFERENCE, questionModel.ConceptId, ConceptSchemeId(questionModel.ConceptId));
            questionItem.Add(conceptReference);

            return questionItem;
        }

        private XElement CreateQuestionScheme()
        {
            if (studyUnit.QuestionModels.Count == 0)
            {
                return null;
            }
            //d:QuestionScheme
            XElement questionScheme = new XElement(d + TAG_QUESTION_SCHEME, 
                CreateIDAttribute(studyUnit.StudyUnitModel.QuestionSchemeId),
                CreateVersionAttribute(), 
                CreateAgencyAttribute()
                );
            //先にQuestionGroupに含まれない設問を書き出す
            foreach (Question questionModel in studyUnit.QuestionModels)
            {
                if (!studyUnit.StudyUnitModel.ContainsInQuestionGroup(questionModel))
                {
                    XElement questionItem = CreateQuestionItem(questionModel);
                    questionScheme.Add(questionItem);
                }
            }
            foreach (QuestionGroupVM questionGroup in studyUnit.QuestionGroups)
            {
                if (questionGroup.Questions.Count == 0)
                {
                    AddError(Resources.PleaseSelectQuestion, studyUnit.QuestionGroupForm); //質問を選択してください
                }
                else
                {
                    XElement multipleQuestionItem = new XElement(d + TAG_MULTIPLE_QUESTION_ITEM,
                        CreateIDAttribute(questionGroup.Id),
                        new XElement(d + TAG_MULTIPLE_QUESTION_ITEM_NAME, questionGroup.Title),
                        new XElement(d + TAG_QUESTION_TEXT,
                            new XElement(d + TAG_LITERAL_TEXT,
                                new XElement(d + TAG_TEXT, questionGroup.Memo))));
                    questionScheme.Add(multipleQuestionItem);

                    XElement subQuestions = new XElement(d + TAG_SUB_QUESTIONS);
                    multipleQuestionItem.Add(subQuestions);
                    foreach (QuestionVM question in questionGroup.Questions)
                    {
                        XElement questionItem = CreateQuestionItem((Question)question.Model);
                        subQuestions.Add(questionItem);
                    }
                }
            }
            return questionScheme;
        }

        private XElement CreateControlConstructSchemes(ControlConstructScheme schemeModel)
        {
            //d:ControlConstructScheme
            if (!schemeModel.HasConstruct)
            {
                return null;
            }
            XElement scheme = new XElement(d + TAG_CONTROL_CONSTRUCT_SCHEME, 
                CreateIDAttribute(schemeModel.Id),
                CreateVersionAttribute(), 
                CreateAgencyAttribute(),
                CreateNullable(d + TAG_CONTROL_CONSTRUCT_SCHEME_NAME, schemeModel.Title) //ControlConstructSchemeName*
                );

            //d:ControlConstruct+
            //d:QuestionConstruct 質問
            foreach (QuestionConstruct constructModel in schemeModel.QuestionConstructs)
            {
                XElement questionConstruct = new XElement(d + TAG_QUESTION_CONSTRUCT, CreateIDAttribute(constructModel.Id),
                    CreateNullable(r + TAG_LABEL, constructModel.No), //r:Label*
                    CreateReferenceID(d + TAG_QUESTION_REFERENCE, constructModel.QuestionId, QuestionSchemeId()) //QuestionReference
                    );
                scheme.Add(questionConstruct);
            }
            //質問グループ
            foreach (QuestionGroupConstruct constructModel in schemeModel.QuestionGroupConstructs)
            {
                XElement questionConstruct = new XElement(d + TAG_QUESTION_CONSTRUCT, CreateIDAttribute(constructModel.Id),
                    CreateNullable(r + TAG_LABEL, constructModel.No), //r:Label*
                    CreateReferenceID(d + TAG_QUESTION_REFERENCE, constructModel.QuestionGroupId, QuestionSchemeId()) //QuestionReference
                    );
                scheme.Add(questionConstruct);
            }


            //d:StatementItem 説明文
            foreach (Statement statementModel in schemeModel.Statements)
            {
                XElement statement = new XElement(d + TAG_STATEMENT_ITEM, CreateIDAttribute(statementModel.Id),
                    new XElement(r + TAG_LABEL, statementModel.No),//r:Label*
                    new XElement(d + TAG_DISPLAY_TEXT,// DisplayText+
                        new XElement(d + TAG_LITERAL_TEXT, //LiteralText+
                            new XElement(d + TAG_TEXT, statementModel.Text))) //Text+
                    );
                scheme.Add(statement);
            }
            //d:IfThenElse 分岐
            foreach (IfThenElse ifThenElseModel in schemeModel.IfThenElses)
            {
                XElement ifThenElse = new XElement(d + TAG_IF_THEN_ELSE, CreateIDAttribute(ifThenElseModel.Id));
                scheme.Add(ifThenElse);

                //IfCondition
                XElement ifCondition = new XElement(d + TAG_IF_CONDITION,
                    new XElement(r + TAG_CODE, ifThenElseModel.IfCondition.Code), //Code+
                    CreateReferenceID(r + TAG_SOURCE_QUESTION_REFERENCE, ifThenElseModel.IfCondition.QuestionId, QuestionSchemeId()) //SourceQuestionReference*
                    );
                ifThenElse.Add(ifCondition);

                //ThenConstructReference
                ifThenElse.Add(CreateReferenceID(d + TAG_THEN_CONSTRUCT_REFERENCE, ifThenElseModel.ThenConstructId, schemeModel.Id));

                //ElseIf*
                foreach (ElseIf elseIfModel in ifThenElseModel.ElseIfs)
                {
                    XElement elseIf = new XElement(d + TAG_ELSE_IF);
                    ifThenElse.Add(elseIf);

                    //IfCondition
                    XElement elseIfCondition = new XElement(d + TAG_IF_CONDITION,
                        new XElement(r + TAG_CODE, elseIfModel.IfCondition.Code),
                        CreateReferenceID(r + TAG_SOURCE_QUESTION_REFERENCE, elseIfModel.IfCondition.QuestionId, QuestionSchemeId())
                        );
                    elseIf.Add(elseIfCondition);
                    //ThenConstructReference
                    elseIf.Add(CreateReferenceID(d + TAG_THEN_CONSTRUCT_REFERENCE, elseIfModel.ThenConstructId, schemeModel.Id));
                }
                //ElseConstructReference?
                ifThenElse.Add(CreateReferenceID(d + TAG_ELSE_CONSTRUCT_REFERENCE, ifThenElseModel.ElseConstructId, schemeModel.Id));
            }

            // Sequence 順序
            XElement sequence = new XElement(d + TAG_SEQUENCE, CreateIDAttribute(schemeModel.Sequence.Id));
            scheme.Add(sequence);
            foreach (string id in schemeModel.Sequence.ControlConstructIds)
            {
                //ControlConstructReference*
                XElement controlConstruct = CreateReferenceID(d + TAG_CONTROL_CONSTRUCT_REFERENCE, id, schemeModel.Id);
                sequence.Add(controlConstruct);
            }
            return scheme;
        }

        private List<XElement> CreateControlConstructSchemes()
        {
            //r:ControlConstructScheme
            List<XElement> controlConstructSchemes = new List<XElement>();
            foreach (ControlConstructScheme scheme in studyUnit.ControlConstructSchemeModels)
            {
                controlConstructSchemes.Add(CreateControlConstructSchemes(scheme));
            }
            return controlConstructSchemes;
        }

        private XElement CreateDataCollection()
        {
            ///// d:DataCollection
            XElement dataCollection = new XElement(d + TAG_DATA_COLLECTION, 
                CreateIDAttribute(studyUnit.StudyUnitModel.DataCollectionId),
                CreateVersionAttribute(), 
                CreateAgencyAttribute()
                );

            dataCollection.Add(CreateOtherMaterials(BookRelationType.Question));

            //d:Methodology?(実際は母集団は入力必須なのでこのタグが空になることはない)
            XElement methodology = new XElement(d + TAG_METHODOLOGY, CreateIDAttribute(studyUnit.StudyUnitModel.MethodologyId));
            dataCollection.Add(methodology);
            foreach (Universe universeModel in studyUnit.AllUniverseModels)
            {
                //母集団との関連づけのためにContentは空でも保存する必要がある。
                XElement samplingProcesure =
                    new XElement(d + TAG_SAMPLING_PROCEDURE, CreateIDAttribute(universeModel.SamplingProcedureId),
                        new XElement(r + TAG_CONTENT, universeModel.Method));
                methodology.Add(samplingProcesure);
            }


            foreach (Sampling samplingModel in studyUnit.SamplingModels)
            {
                //CollectionEvent*
                XElement collectionEvent = new XElement(d + TAG_COLLECTION_EVENT, CreateIDAttribute(samplingModel.CollectionEventId));
                dataCollection.Add(collectionEvent);
                if (samplingModel.MemberId == null)
                {
                    AddError(SamplingError(Resources.InputCollectionLeader, samplingModel), studyUnit.SamplingForm); //データ収集の責任者を入力してください
                }
                else
                {
                    //DataCollectorOrganizationReference*
                    XElement organizationReference = CreateReferenceID(d + TAG_DATA_COLLECTOR_ORGANIZATION_REFERENCE, samplingModel.MemberId, OrganizationSchemeId());
                    collectionEvent.Add(organizationReference);
                }
                if (samplingModel.DateRange.IsEmpty)
                {
                    AddError(SamplingError(Resources.InputCollectionDate, samplingModel), studyUnit.SamplingForm); //データの収集日・期間を入力してください
                }
                else
                {
                    //DataCollectionDate
                    XElement date = CreateDate(d + TAG_DATA_COLLECTION_DATE, samplingModel.DateRange);
                    collectionEvent.Add(date);
                }
                //ModeOfCollection*(nullになるかも)
                XElement method = new XElement(d + TAG_MODE_OF_COLLECTION,
                    CreateIDAttribute(samplingModel.ModeOfCollectionId),
                    CreateNullable(r + TAG_CONTENT, Options.SamplingMethodLabel(samplingModel.MethodCode)));
                collectionEvent.Add(EmptyToNull(method));
                //CollectionSituation*(nullになるかも)
                XElement situation = new XElement(d + TAG_COLLECTION_SITUATION,
                    CreateIDAttribute(samplingModel.ModeOfCollectionId),
                    CreateNullable(r + TAG_CONTENT, samplingModel.Situation));
                collectionEvent.Add(EmptyToNull(situation));
            }

            // QuestionScheme*
            dataCollection.Add(CreateQuestionScheme());



            //ControlConstructScheme*
            List<XElement> controlConstructSchemes = CreateControlConstructSchemes();
            foreach (XElement controlConstructScheme in controlConstructSchemes)
            {
                dataCollection.Add(controlConstructScheme);
            }
            return dataCollection;
        }

        private XElement CreateCategoryScheme(CategoryScheme categorySchemeModel)
        {
            //l:CategoryScheme
            XElement categoryScheme = new XElement(l + TAG_CATEGORY_SCHEME,
                CreateIDAttribute(categorySchemeModel.Id),
                CreateVersionAttribute(), 
                CreateAgencyAttribute(),
                CreateLabel(categorySchemeModel.Title), //r:Label*
                CreateNullableDescription(categorySchemeModel.Memo));// r:Description*
            //l:Category*
            foreach (Category categoryModel in categorySchemeModel.Categories)
            {
                XAttribute missing = null;
                if (categoryModel.IsMissingValue)
                {
                    missing = new XAttribute(ATTR_MISSING, "1");
                }
                XElement category = new XElement(l + TAG_CATEGORY,
                    CreateIDAttribute(categoryModel.Id),
                    missing,
                    CreateLabel(categoryModel.Title),//r:Label*
                    CreateNullableDescription(categoryModel.Memo));// r:Description*
                categoryScheme.Add(category);
            }
            return categoryScheme;
        }

        private XElement CreateCodeScheme(CodeScheme codeSchemeModel)
        {
            //l:CodeScheme
            XElement codeScheme = new XElement(l + TAG_CODE_SCHEME,
                CreateIDAttribute(codeSchemeModel.Id),
                CreateVersionAttribute(), 
                CreateAgencyAttribute(),
                CreateLabel(codeSchemeModel.Title),//r:Label*
                CreateNullableDescription(codeSchemeModel.Memo)); //r:Description*,
            //Code*
            foreach (Code codeModel in codeSchemeModel.Codes)
            {
                XElement code = new XElement(l + TAG_CODE,
                    CreateReferenceID(l + TAG_CATEGORY_REFERENCE, codeModel.CategoryId, CategorySchemeId(codeModel.CategoryId)), //CategoryReference
                    new XElement(l + TAG_VALUE, codeModel.Value)); //Value
                codeScheme.Add(code);
            }
            return codeScheme;
        }

        private XElement CreateVariableScheme()
        {
            //l:VariableScheme
            if (studyUnit.VariableModels.Count == 0)
            {
                return null;
            }
            XElement variableScheme = new XElement(l + TAG_VARIABLE_SCHEME, 
                CreateIDAttribute(VariableSchemeId()),
                CreateVersionAttribute(), 
                CreateAgencyAttribute()
                );
            foreach (Variable variableModel in studyUnit.VariableModels)
            {
                //Variable*
                XElement variable = new XElement(l + TAG_VARIABLE,
                    CreateIDAttribute(variableModel.Id),
                    new XElement(l + TAG_VARIABLE_NAME, variableModel.Title), // VariableName*
                    CreateNullableLabel(variableModel.Label), //r:Label*
                    CreateReferenceID(r + TAG_UNIVERSE_REFERENCE, variableModel.UniverseId, UniverseSchemeId()), //r:UniverseReference*
                    CreateReferenceID(l + TAG_CONCEPT_REFERENCE, variableModel.ConceptId, ConceptSchemeId(variableModel.ConceptId)), //r:ConceptReference
                    CreateReferenceID(l + TAG_QUESTION_REFERENCE, variableModel.QuestionId, QuestionSchemeId())); //r:QuestionReference
                variableScheme.Add(variable);


                Response responseModel = variableModel.Response;
                XElement response = null;
                if (responseModel.IsTypeChoices)
                {
                    response = CreateCodeResponse(l + TAG_CODE_REPRESENTATION, responseModel);
                }
                else if (responseModel.IsTypeNumber)
                {
                    if (responseModel.DetailTypeCode == null)
                    {
                        AddError(Resources.SelectNumberType, studyUnit.VariableForm); //数値の型を選択してください
                    }
                    else if (responseModel.Min == null || responseModel.Max == null)
                    {
                        AddError(Resources.InputMinAndMax, studyUnit.VariableForm); //最大値・最小値を入力してください
                    }
                    else
                    {
                        response = CreateNumberResponse(l + TAG_NUMERIC_REPRESENTATION, responseModel);
                    }
                } else if (responseModel.IsTypeFree)
                {
                    if (responseModel.Min == null || responseModel.Max == null)
                    {
                        AddError(Resources.InputMinAndMax, studyUnit.VariableForm); //最大値・最小値を入力してください
                    }
                    else
                    {
                        response = CreateTextResponse(l + TAG_TEXT_REPRESENTATION, responseModel);
                    }
                } else if (responseModel.IsTypeDateTime)
                {
                    if (responseModel.DetailTypeCode == null)
                    {
                        AddError(Resources.SelectDateType, studyUnit.VariableForm); //日付の型を選択してください
                    }
                    else
                    {
                        response = CreateDateTimeResponse(l + TAG_DATE_TIME_REPRESENTATION, responseModel);
                    }
                }
                if (response != null)
                {
                    //Representation?
                    XElement representation = new XElement(l + TAG_REPRESENTATION);
                    representation.Add(response);
                    variable.Add(representation);
                }
            }
            return variableScheme;
        }

        private XElement CreateDataRelationship()
        {
            ///// l:DataRelationship
            if (studyUnit.DataSetModels.Count == 0)
            {
                return null;
            }
            XElement dataRelationship = new XElement(l + TAG_DATA_RELATIONSHIP, CreateIDAttribute(studyUnit.StudyUnitModel.DataCollectionId));
            //LogicalRecord+
            //LogicalRecord=DataSetが繰り替えし
            foreach (DataSet dataSetModel in studyUnit.DataSetModels)
            {
                XElement logicalRecord = new XElement(l + TAG_LOGICAL_RECORD,
                    CreateIDAttribute(dataSetModel.Id),
                    new XAttribute(ATTR_HAS_LOCATOR, "false"),
                    CreateNullable(l + TAG_LOGICAL_RECORD_NAME, dataSetModel.Title),//LogicalRecordName*
                    CreateNullableDescription(dataSetModel.Memo));// r:Description*
                dataRelationship.Add(logicalRecord);
                //l:VariablesInRecord 
                XElement variablesInRecord = new XElement(l + TAG_VARIABLES_IN_RECORD,
                    new XAttribute(ATTR_ALL_VARIABLES_IN_LOGICAL_PRODUCT, "false"));
                logicalRecord.Add(variablesInRecord);
                //VariableSchemeReference*
                variablesInRecord.Add(CreateReferenceID(l + TAG_VARIABLE_SCHEME_REFERENCE, VariableSchemeId(), VariableSchemeId()));
                foreach (string variableId in dataSetModel.VariableGuids)
                {
                    //VariableUsedReference*
                    variablesInRecord.Add(CreateReferenceID(l + TAG_VARIABLE_USED_REFERENCE, variableId, VariableSchemeId()));
                }
            }
            return dataRelationship;
        }

        private XElement CreateLogicalProduct()
        {
            //l:LogicalProduct
            XElement logicalProduct = new XElement(l + TAG_LOGICAL_PRODUCT,
                CreateIDAttribute(LogicalProductId()),
                CreateVersionAttribute(),
                CreateAgencyAttribute()
                );
            //l:DataRelationship*
            logicalProduct.Add(CreateDataRelationship());
            logicalProduct.Add(CreateOtherMaterials(BookRelationType.Variable));
            //l:CategoryScheme*
            foreach (CategoryScheme categorySchemeModel in studyUnit.CategorySchemeModels)
            {
                logicalProduct.Add(CreateCategoryScheme(categorySchemeModel));
            }
            //l:CodeScheme*
            foreach (CodeScheme codeSchemeModel in studyUnit.CodeSchemeModels)
            {
                logicalProduct.Add(CreateCodeScheme(codeSchemeModel));
            }
            //l:VariableScheme*
            logicalProduct.Add(CreateVariableScheme());
            return logicalProduct;
        }

        private XElement CreatePhysicalDataProduct()
        {
            //p:PhysicalDataProduct*
            XElement physicalDataProduct = new XElement(p + TAG_PHYSICAL_DATA_PRODUCT,
                CreateIDAttribute(studyUnit.StudyUnitModel.PhysicalDataProductId),
                CreateVersionAttribute(),
                CreateAgencyAttribute()
                );            
            //データファイルのインスタンスは必ず1つ以上あるはずだが一応チェック
            if (studyUnit.DataFileModels.Count > 0)
            {
                //PhysicalStructureScheme*
                XElement physicalStructureScheme = new XElement(p + TAG_PHYSICAL_STRUCTURE_SCHEME,
                    CreateIDAttribute(PhysicalStructureSchemeId()),
                    CreateVersionAttribute(),
                    CreateAgencyAttribute()
                    );
                physicalDataProduct.Add(physicalStructureScheme);
                
                //PhysicalStructure+
                //DataFileごとに作られる(フォーマットなど)。
                foreach (DataFile dataFileModel in studyUnit.DataFileModels)
                {
                    DataSet dataSetModel = studyUnit.FindDataSetModel(dataFileModel.DataSetId);
                    XElement physicalStructure = new XElement(p + TAG_PHYSICAL_STRUCTURE,
                        CreateIDAttribute(dataFileModel.Id),
                        CreateReferenceID(p + TAG_LOGICAL_PRODUCT_REFERENCE, LogicalProductId(), LogicalProductId()), //LogicalProductReference+
                        CreateNullable(p + TAG_FORMAT, dataFileModel.Format), //Format?
                        CreateNullable(p + TAG_DEFAULT_DELIMITER, Options.FindLabel(Options.Delimiters, dataFileModel.DelimiterCode)), //DefaultDelimiter?
                        new XElement(p + TAG_GROSS_RECORD_STRUCTURE,//GrossRecordStructure+
                            CreateIDAttribute(dataFileModel.GrossRecordStructureId),
                            CreateReferenceID(p + TAG_LOGICAL_RECORD_REFERENCE, dataSetModel.Id, LogicalProductId()), //LogicalRecordReference データセットのID
                            new XElement(p + TAG_PHYSICAL_RECORD_SEGMENT, CreateIDAttribute(dataFileModel.PhysicalRecordSegment))));  //PhysicalRecordSegment+
                    physicalStructureScheme.Add(physicalStructure);
                }

                //RecordLayoutScheme*
                //同じくDataFileごと。
                XElement recordLayoutScheme = new XElement(p + TAG_RECORD_LAYOUT_SCHEME, 
                    CreateIDAttribute(RecordLayoutSchemeId()),
                    CreateVersionAttribute(), 
                    CreateAgencyAttribute()
                    );
                foreach (DataFile dataFileModel in studyUnit.DataFileModels)
                {
                    DataSet dataSetModel = studyUnit.FindDataSetModel(dataFileModel.DataSetId);
                    if (dataSetModel.VariableGuids.Count > 0) {

                        XElement physicalStructureReference = CreateReferenceID(p + TAG_PHYSICAL_STRUCTURE_REFERENCE, dataFileModel.Id, PhysicalStructureSchemeId());
                        physicalStructureReference.Add(new XElement(p + TAG_PHYSICAL_RECORD_SEGMENT_USED, dataFileModel.PhysicalRecordSegment));

                        //RecordLayout+
                        XElement baseRecordLayout = new XElement(p + TAG_RECORD_LAYOUT,
                            CreateIDAttribute(dataFileModel.RecordLayoutId),
                            CreateUserID(),
                            physicalStructureReference, //PhysicalStructureReference
                            new XElement(p + TAG_CHARACTER_SET, "UTF-8"),
                            new XElement(p + TAG_ARRAY_BASE, "0"));
                        recordLayoutScheme.Add(baseRecordLayout);

                        //DataItem+
                        int i = 0;
                        foreach (string id in dataSetModel.VariableGuids)
                        {
                            XElement dataItem = new XElement(p + TAG_DATA_ITEM,
                                CreateReferenceID(p + TAG_VARIABLE_REFERENCE, id, VariableSchemeId()), //VariableReference
                                new XElement(p + TAG_PHYSICAL_LOCATION, //PhysicalLocation
                                    new XElement(p + TAG_ARRAY_POSITION, i++))); //ArrayPosition?
                            baseRecordLayout.Add(dataItem);
                        }
                    }
                }
                physicalDataProduct.Add(EmptyToNull(recordLayoutScheme));
            }
            return physicalDataProduct;
        }

        private XElement CreatePhysicalInstance(DataFile dataFileModel)
        {
            //d:PhysicalInstance
            DataSet dataSetModel = studyUnit.FindDataSetModel(dataFileModel.DataSetId);
            XElement physicalInstance = new XElement(pi + TAG_PHYSICAL_INSTANCE,
                CreateIDAttribute(dataFileModel.PhysicalInstanceId),
                CreateVersionAttribute(), 
                CreateAgencyAttribute(),
                CreateReferenceID(pi + TAG_RECORD_LAYOUT_REFERENCE, dataFileModel.RecordLayoutId, RecordLayoutSchemeId()), //RecordLayoutReference+
                new XElement(pi + TAG_DATA_FILE_IDENTIFICATION,  //DataFileIdentification+
                    CreateIDAttribute(dataFileModel.DataFileIdentificationId),
                    new XElement(pi + TAG_URI, dataFileModel.Uri)),//URI+
                new XElement(pi + TAG_GROSS_FILE_STRUCTURE, //GrossFileStructure?
                    CreateIDAttribute(dataFileModel.GrossFileStructureId),
                    new XElement(pi + TAG_CASE_QUANTITY, "0"), //CaseQuantity?
                    new XElement(pi + TAG_OVERALL_RECORD_COUNT, dataSetModel.VariableGuids.Count))); //OverallRecordCount?
            return physicalInstance;
        }


        private bool IsMatchBookType(Book book, BookRelationType type)
        {
            if (book.BookRelations.Count == 0)
            {
                return BookRelationType.Abstract == type;
            }
            foreach (BookRelation relation in book.BookRelations)
            {
                if (relation.BookRelationType == type)
                {
                    return true;
                }
            }
            return false;
        }

        private List<Book> GetBooks(BookRelationType type)
        {
            List<Book> books = new List<Book>();
            foreach (Book bookModel in studyUnit.BookModels)
            {
                if (IsMatchBookType(bookModel, type))
                {
                    //含まれる関連に従いすべての箇所で書き出す。
                    books.Add(bookModel);
                }
            }
            return books;
        }

        private XElement CreateOtherMaterial(Book book, BookRelationType type)
        {
            XElement otherMaterial = new XElement(r + TAG_OTHER_MATERIAL,
                CreateIDAttribute(book.GetBookId(type)),
                new XAttribute(ATTR_TYPE,GetDDIBookType(book.BookTypeCode)));

            XElement citation = new XElement(r + TAG_CITATION,
                new XElement(r + TAG_TITLE, book.Title)
                );
            otherMaterial.Add(citation);
            if (!string.IsNullOrEmpty(book.Author))
            {
                citation.Add(new XElement(r + TAG_CREATOR, book.Author));                    
            }
            string publisher = BuildPublisher(book);
            if (!string.IsNullOrEmpty(publisher))
            {
                citation.Add(new XElement(r + TAG_PUBLISHER, publisher));
            }
            if (!string.IsNullOrEmpty(book.Editor))
            {
                citation.Add(new XElement(r + TAG_CONTRIBUTOR, book.Editor));                   
            }
            string simpleDate =  ToSimpleDate(book.AnnouncementDate);
            if (!string.IsNullOrEmpty(simpleDate))
            {
                citation.Add(
                    new XElement(r + TAG_PUBLICATION_DATE,
                        new XElement(r + TAG_SIMPLE_DATE, simpleDate)));
            }
            if (!string.IsNullOrEmpty(book.Language))
            {
                citation.Add(new XElement(r + TAG_LANGUAGE, book.Language));
            }
            string identifier = BuildIdentifier(book);
            XElement identifierElement = null;
            if (!string.IsNullOrEmpty(identifier)) {
                identifierElement = new XElement(dc + TAG_DC_IDENTIFIER, identifier);
            }
            XElement descriptionElement = null;
            if (!string.IsNullOrEmpty(book.Summary))
            {
                descriptionElement = new XElement(dc + TAG_DC_DESCRIPTION, book.Summary);
            }
            if (identifierElement != null || descriptionElement != null)
            {
                XElement dcelements = new XElement(dce + TAG_DCELEMENTS);
                citation.Add(dcelements);
                dcelements.Add(identifierElement);
                dcelements.Add(descriptionElement);
            }

            if (!string.IsNullOrEmpty(book.Url))
            {
                otherMaterial.Add(new XElement(r + TAG_EXTERNAL_URL_REFERENCE, book.Url));
            }

            foreach (BookRelation relation in book.BookRelations)
            {
                string schemeId = null;
                string metaDataId = relation.MetadataId;
                if (relation.IsBookRelationTypeAbstract)
                {
                    schemeId = studyUnit.Id;
                    metaDataId = studyUnit.Id;
                    otherMaterial.Add(new XElement(r + TAG_RELATIONSHIP,
                        CreateReferenceModuleID(r + TAG_RELATED_TO_REFERENCE, metaDataId, schemeId)));
                }
                else
                {
                    if (relation.IsBookRelationTypeConcept)
                    {
                        ConceptScheme conceptScheme = studyUnit.StudyUnitModel.FindConceptSchemeByConceptId(metaDataId);
                        schemeId = conceptScheme.Id;
                    }
                    else if (relation.IsBookRelationTypeQuestion)
                    {
                        schemeId = studyUnit.StudyUnitModel.QuestionSchemeId;
                    }
                    else if (relation.IsBookRelationTypeVariable)
                    {
                        schemeId = studyUnit.StudyUnitModel.VariableScheme.Id;
                    }
                    otherMaterial.Add(new XElement(r + TAG_RELATIONSHIP,
                        CreateReferenceID(r + TAG_RELATED_TO_REFERENCE, relation.MetadataId, schemeId)));
                }
            }

            return otherMaterial;
        }

        private List<XElement> CreateOtherMaterials(BookRelationType type)
        {
            List<XElement> otherMaterials = new List<XElement>();
            List<Book> books = GetBooks(type);
            foreach (Book book in books)
            {
                otherMaterials.Add(CreateOtherMaterial(book, type));
            }
            return otherMaterials;
        }

        private XElement CreateStudyUnit()
        {
            ///// StudyUnitタグの書き出し
            XElement su = new XElement(s + TAG_STUDY_UNIT, 
                CreateIDAttribute(studyUnit.Id),
                CreateVersionAttribute(),
                CreateAgencyAttribute());
            //r:Citation
            su.Add(new XElement(r + TAG_CITATION, new XElement(r + TAG_TITLE, studyUnit.AbstractModel.Title)));
            //Abstract+
            su.Add(new XElement(s + TAG_ABSTRACT, CreateIDAttribute(studyUnit.AbstractModel.SummaryId), CreateContent(studyUnit.AbstractModel.Summary)));
            //r:UniverseReference+
            Universe universe = studyUnit.FindMainUniverseModel();
            if (universe == null)
            {
                AddError(Resources.InputUniverse, studyUnit.SamplingForm); //母集団を入力してください
            }
            else
            {
                XElement universeReference = CreateReferenceID(r + TAG_UNIVERSE_REFERENCE, universe.Id, UniverseSchemeId());
                su.Add(universeReference);
            }
            //r:FundingInformation*
            foreach (FundingInfo fundingInfo in studyUnit.FundingInfoModels)
            {
                su.Add(CreateFundingInformation(fundingInfo));
            }
            //r:Purpose
            su.Add(new XElement(s + TAG_PURPOSE, CreateIDAttribute(studyUnit.AbstractModel.PurposeId), CreateContent(studyUnit.AbstractModel.Purpose))); 
            //r:Coverage?
            su.Add(CreateCoverage());
            //r:OtherMaterial
            su.Add(CreateOtherMaterials(BookRelationType.Abstract));
            //c:ComceptualComponent*
            su.Add(CreateConceptualComponent(false));
            //d:DataCollection*
            su.Add(CreateDataCollection());
            //l:BaseLogicalProduct*
            su.Add(CreateLogicalProduct());
            //p:PhysicalDataProduct*
            su.Add(CreatePhysicalDataProduct());
            //pi:PhysicalInstance*
            foreach (DataFile dataFileModel in studyUnit.DataFileModels)
            {
                su.Add(CreatePhysicalInstance(dataFileModel));
            }
            //a:Archive?
            su.Add(CreateArchive());
            return su;
        }

        public void WriteStudyUnit(string path, StudyUnitVM studyUnit)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));
            this.studyUnit = studyUnit;

            ClearError();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.Encoding = Encoding.UTF8;
            using (XmlWriter xw = XmlWriter.Create(path, xws))
            {
                XDocument doc = new XDocument(
                    DECLARATION,
                    new XElement(ddi + TAG_DDI_INSTANCE,
                        CreateIDAttribute(studyUnit.StudyUnitModel.InstanceId),
                        CreateVersionAttribute(),
                        CreateAgencyAttribute(),
                        new XAttribute(XNamespace.Xmlns + "ddi", ddi),
                        new XAttribute(XNamespace.Xmlns + "s", s),
                        new XAttribute(XNamespace.Xmlns + "r", r),
                        new XAttribute(XNamespace.Xmlns + "a", a),
                        new XAttribute(XNamespace.Xmlns + "c", c),
                        new XAttribute(XNamespace.Xmlns + "d", d),
                        new XAttribute(XNamespace.Xmlns + "l", l),
                        new XAttribute(XNamespace.Xmlns + "p", p),
                        new XAttribute(XNamespace.Xmlns + "pi", pi),
                        new XAttribute(XNamespace.Xmlns + "dce", dce),
                        new XAttribute(XNamespace.Xmlns + "dc", dc),
                        CreateStudyUnit())
                );
                CheckError();
                doc.WriteTo(xw);
            }
        }

        private List<CompareItem> DoCreateCompareItem(GroupVM group, List<StudyUnitVM> studyUnits, CompareTable compareTable)
        {
            //■グループ比較テーブルを見て、CompareLitemの配列を返す。
            //画面で使用するのはCompareRow。この内部はCompareCellの配列を持っている。
            //これをDDI形式で使用するペア形式(CompareItem)の配列に変換する昨日を持つ。
            //
            //■CompareItemに保存されるIDは、
            //CompareTableがConceptSchemeのID、ConceptのID、VariableのIDのどれかになる。
            List<CompareItem> items = new List<CompareItem>();
            foreach (CompareRow row in compareTable.Rows)
            {
                //有効な○のセルをすべて取得して保存する。」
                //有効な○のセルとは「○がセットされてる、かつその列のStudyUnitに行の変数がふくまれているもの」
                //
                // S1にV1が、S2にV2が含まれている場合※の位置に入力された値は無視しないといけない。
                //
                //    S1     S2
                // V1        ※
                // V2 ※

                //○の保存
                List<CompareCell> matchCells = row.MatchValidCells();
                if (matchCells.Count > 1)
                {
                    //比較対象の○が存在しないと保存しない。○の場合次のように同じ行にペアで存在しないとだめ。
                    //     S1    S2
                    // V1  ○    ○

                    //セルに該当する変数を取得する。同じStudyUnitに同じ変数名の変数が存在する可能性があるため
                    //Listで処理する必要がある。
                    List<GroupId> sourceIds = row.RelatedGroupIds(matchCells[0]);
                    for (int i = 1; i < matchCells.Count; i++)
                    {
                        List<GroupId> targetIds = row.RelatedGroupIds(matchCells[i]);
                        foreach (GroupId sourceId in sourceIds)
                        {
                            foreach (GroupId targetId in targetIds)
                            {
                                CompareItem item = CompareItem.CreateMatch(sourceId, targetId, row.Memo);
                                items.Add(item);
                            }
                        }
                    }
                }
                //△の保存
                List<CompareCell> partialMatchCells = row.PartialMatchValidCells();
                foreach (CompareCell targetCell in partialMatchCells)                {
                    List<GroupId> targetIds = row.RelatedGroupIds(targetCell);

                    //△の場合他の行と比較しているので対象の行をみつけないといけない。
                    //それには△(V1)のように選択されている対象の変数をタイトルでみつける
                    //(行はタイトルでユニーク化されているのでひとつだけ見つかるはず)。
                    CompareRow sourceRow = compareTable.FindRowByTitle(targetCell.TargetTitle);
                    //対象行の全ての有効なセルた処理対象。
                    //    S1      S2      S3
                    //V1  △(V2)
                    //V2        △(V1) △(V1)
                    List<CompareCell> validCells = sourceRow.ValidCells();
                    foreach (CompareCell sourceCell in validCells)
                    {
                        List<GroupId> sourceIds = sourceRow.RelatedGroupIds(sourceCell);
                        foreach (GroupId sourceId in sourceIds)
                        {
                            foreach (GroupId targetId in targetIds)
                            {
                                CompareItem item = CompareItem.CreatePartialMatch(sourceId, targetId, row.Memo);
                                items.Add(item);
                            }
                        }
                    }
                }
                //※いまのところ資料にある暗黙の関係性までは保存していない
            }
            return items;
        }

        private GroupId FindParentConceptSchemeIdByConceptId(List<StudyUnitVM> studyUnits, GroupId conceptId)
        {
            StudyUnitVM studyUnit = StudyUnitVM.Find(studyUnits, conceptId.StudyUnitId);
            ConceptScheme conceptScheme = studyUnit.StudyUnitModel.FindConceptSchemeByConceptId(conceptId.Id);
            return new GroupId(conceptId.StudyUnitId, conceptScheme.Id);
        }

        private GroupId FindParentConceptSchemeIdByVariableId(List<StudyUnitVM> studyUnits, GroupId variableId)
        {
            StudyUnitVM studyUnit = StudyUnitVM.Find(studyUnits, variableId.StudyUnitId);
            ConceptScheme conceptScheme = studyUnit.StudyUnitModel.FindConceptSchemeByVariableId(variableId.Id);
            return new GroupId(variableId.StudyUnitId, conceptScheme.Id);
        }

        private XElement CreateConceptSchemeElement(CompareItem item, string prefix)
        {
            //cm:ConceptMap
            XElement element = new XElement(cm + TAG_CONCEPT_MAP,
                CreateIDAttribute(item.IdWithPrefix(prefix)),
                CreateReferenceID(cm + TAG_SOURCE_SCHEME_REFERENCE, item.SourceId.Id),
                CreateReferenceID(cm + TAG_TARGET_SCHEME_REFERENCE, item.TargetId.Id),
                new XElement(cm + TAG_CORRESPONDENCE, //Correspondence
                    new XElement(cm + TAG_COMMONALITY, item.Memo), //Commonality+
                    new XElement(cm + TAG_DIFFERENCE, item.Memo), //Difference+
                    new XElement(cm + TAG_COMMONALITY_WEIGHT, item.Weight) //CommonalityWeight?
                    ));
            return element;
        }

        private XElement CreateItemElement(CompareItem item)
        {
            XElement childElement = new XElement(cm + TAG_ITEM_MAP,
                new XElement(cm + TAG_SOURCE_ITEM, item.SourceItemId),
                new XElement(cm + TAG_TARGET_ITEM, item.TargetItemId),
                new XElement(cm + TAG_CORRESPONDENCE,
                    new XElement(cm + TAG_COMMONALITY, item.Memo),
                    new XElement(cm + TAG_DIFFERENCE, item.Memo),
                    new XElement(cm + TAG_COMMONALITY_WEIGHT, item.Weight)
                    ));
            return childElement;
        }

        private XElement CreateVariableElement(CompareItem item)
        {
            XElement element = new XElement(cm + TAG_VARIABLE_MAP,
                CreateIDAttribute(item.Id),
                CreateReferenceID(cm + TAG_SOURCE_SCHEME_REFERENCE, item.SourceId.Id),
                CreateReferenceID(cm + TAG_TARGET_SCHEME_REFERENCE, item.TargetId.Id),
                new XElement(cm + TAG_CORRESPONDENCE,
                    new XElement(cm + TAG_COMMONALITY, item.Memo),
                    new XElement(cm + TAG_DIFFERENCE, item.Memo)
                    ));
            return element;
        }

        private List<XElement> CreateConceptSchemeMap(GroupVM group, List<StudyUnitVM> studyUnits)
        {
            //ConceptとConceptSchemeの比較関係をタグに保存するだけなのだが親子関係を考慮しないといけないので複雑な処理となる。
            //処理の流れは
            //・Conceptの比較結果を取得。
            //・Conceptの比較結果を、同じ対応を持つConceptSchemeの比較結果ごとにまとめる
            //・ConceptSchemeの比較結果を取得。
            //・ConceptSchemeの比較結果を書き出す。対応したConceptの比較結果のリストが存在する場合これを書き出す。
            //・残りのConceptの比較結果を書き出す(ConceptSchemeレベルでは比較結果が存在しないがConceptでは違う場合がありうる)。

            //Conceptの対応関係を取得する
            List<CompareItem> conceptItems = DoCreateCompareItem(group, studyUnits, group.GroupModel.ConceptCompareTable);
            Dictionary<string, List<CompareItem>> conceptMap = new Dictionary<string,List<CompareItem>>();
            //同じ比較元、比較先のConceptScheme(親)をもつものは同じタグにぶら下げる必要があるためconceptMapを使って振り分けていく。
            foreach (CompareItem item in conceptItems)
            {
                //親をユニークに識別するためのPaentIdはConcept_XXXXX_YYYYYという形式。
                item.ParentIdPrefix = "Concept";
                item.ParentSourceId = FindParentConceptSchemeIdByConceptId(studyUnits, item.SourceId);
                item.ParentTargetId = FindParentConceptSchemeIdByConceptId(studyUnits, item.TargetId);
                List<CompareItem> list = null;
                if (conceptMap.ContainsKey(item.ParentId))
                {
                    list = conceptMap[item.ParentId];
                } else {
                    list = new List<CompareItem>();
                    conceptMap[item.ParentId] = list;
                }
                list.Add(item);
            }
            //ConceptSchemeの対応を取得する
            List<CompareItem> conceptSchemeItems = DoCreateCompareItem(group, studyUnits, group.GroupModel.ConceptSchemeCompareTable);
            List<XElement> elements = new List<XElement>();
            foreach (CompareItem item in conceptSchemeItems)
            {
                XElement element = CreateConceptSchemeElement(item, "Concept");
                elements.Add(element);
                if (conceptMap.ContainsKey(item.Id))
                {
                    List<CompareItem> concepts = conceptMap[item.Id];
                    foreach (CompareItem childItem in concepts)
                    {
                        XElement childElement = CreateItemElement(childItem);
                        element.Add(childElement);
                    }
                    conceptMap.Remove(item.Id);
                }
            }
            //残りのConceptMapを保存する。
            foreach (KeyValuePair<string, List<CompareItem>> pair in conceptMap)
            {
                List<CompareItem> concepts = pair.Value;
                CompareItem first = concepts.First();
                CompareItem parentItem = new CompareItem(
                    first.ParentSourceId,
                    first.ParentTargetId,
                    Resources.GenerateForCmpareConcept, "0"); //Concept比較のために生成
                XElement element = CreateConceptSchemeElement(parentItem, "Variable");
                elements.Add(element);
                foreach (CompareItem childItem in concepts)
                {
                    XElement childElement = CreateItemElement(childItem);
                    element.Add(childElement);
                }
            }

            return elements;
        }

        private List<XElement> CreateVariableMap(GroupVM group, List<StudyUnitVM> studyUnits)
        {
            List<CompareItem> variableItems = DoCreateCompareItem(group, studyUnits, group.GroupModel.VariableCompareTable);
            Dictionary<string, List<CompareItem>> variableMap = new Dictionary<string, List<CompareItem>>();
            foreach (CompareItem item in variableItems)
            {
                item.ParentSourceId = FindParentConceptSchemeIdByVariableId(studyUnits, item.SourceId);
                item.ParentTargetId = FindParentConceptSchemeIdByVariableId(studyUnits, item.TargetId);
                List<CompareItem> list = null;
                if (variableMap.ContainsKey(item.ParentId))
                {
                    list = variableMap[item.ParentId];
                }
                if (list == null)
                {
                    list = new List<CompareItem>();
                    variableMap[item.ParentId] = list;
                }
                list.Add(item);
            }

            List<XElement> elements = new List<XElement>();

            foreach (KeyValuePair<string, List<CompareItem>> pair in variableMap)
            {
                List<CompareItem> variables = pair.Value;
                CompareItem first = variables.First();
                CompareItem parentItem = new CompareItem(
                    first.ParentSourceId,
                    first.ParentTargetId,
                    Resources.GenerateForCmpareVariable, "0"); //Variable比較のために生成
                XElement element = CreateVariableElement(parentItem);
                elements.Add(element);
                foreach (CompareItem childItem in variables)
                {
                    XElement childElement = CreateItemElement(childItem);
                    element.Add(childElement);
                }
            }
            return elements;
        }

        private XElement CreateGroup(GroupVM group, List<StudyUnitVM> studyUnits)
        {
            Group groupModel = group.GroupModel;
            XElement gr = new XElement(g + TAG_GROUP,
                CreateIDAttribute(groupModel.Id),
                CreateVersionAttribute(),
                CreateAgencyAttribute(),
                new XAttribute(ATTR_DATA_SET, groupModel.DataSetCode),
                new XAttribute(ATTR_GEOGRAPHY, groupModel.GeographyCode),
                new XAttribute(ATTR_INSTRUMENT,groupModel.InstrumentCode),
                new XAttribute(ATTR_LANGUAGE, groupModel.LanguageCode),
                new XAttribute(ATTR_PANEL, groupModel.PanelCode),
                new XAttribute(ATTR_TIME, groupModel.TimeCode),
                new XElement(r + TAG_CITATION, new XElement(r + TAG_TITLE, group.Title)),
                new XElement(g + TAG_PURPOSE,
                    CreateIDAttribute(groupModel.PurposeId),
                    CreateContent(groupModel.Purpose))
                );

            //共通の母集団、変数の概念を書き出す
            //StudyUnit sharedStudyUnitModel = group.GroupModel.SharedStudyUnit;
            //XElement concepts = new XElement(g + TAG_CONCEPT);
            //XElement conceptualComponent = new XElement(c + TAG_CONCEPTUAL_COMPONENT);

            StudyUnitVM sharedStudyUnit = new StudyUnitVM(group.Main, group.GroupModel.SharedStudyUnit);
            this.studyUnit = sharedStudyUnit;

            XElement conceptualComponent = CreateConceptualComponent(true);
            if (conceptualComponent != null) 
            {            
                XElement concepts = new XElement(g + TAG_CONCEPTS, conceptualComponent);
                gr.Add(concepts);
            }
            XElement logicalProduct = CreateLogicalProduct();
            if (logicalProduct != null)
            {
                XElement logical = new XElement(g + TAG_LOGICAL_PRODUCT, logicalProduct);
                gr.Add(logical);
            }
            return gr;
        }

        private XElement CreateComparison(GroupVM group, List<StudyUnitVM> studyUnits)
        {
            //グループの比較結果を保存する
            Group groupModel = group.GroupModel;
            //cm::Comparison
            XElement comparison = new XElement(cm + TAG_COMPARISON, 
                CreateIDAttribute(groupModel.ComparisonId),
                CreateVersionAttribute(),
                CreateAgencyAttribute()
                );
            //ConceptSchemeとConceptの比較結果のリスト
            List<XElement> conceptSchemeMaps = CreateConceptSchemeMap(group, studyUnits);
            foreach (XElement conceptSchemeMap in conceptSchemeMaps)
            {
                comparison.Add(conceptSchemeMap);
            }
            //Variableの比較結果のリスト
            List<XElement> variableMaps = CreateVariableMap(group, studyUnits);
            foreach (XElement variableMap in variableMaps)
            {
                comparison.Add(variableMap);
            }
            return comparison;
        }

        private XElement CreateSharedStudyUnit(GroupVM group)
        {
            XElement su = new XElement(g + TAG_STUDY_UNIT);
            StudyUnitVM sharedStudyUnit = new StudyUnitVM(group.Main, group.GroupModel.SharedStudyUnit);
            this.studyUnit = sharedStudyUnit;
            su.Add(CreateStudyUnit());
            if (HasError)
            {
                DumpError();
            }
            int errorCount = RemoveError(sharedStudyUnit);
            if (errorCount > 0)
            {
                //エラーが発生した場合は出力しない(他のStudyUnitと区別できない?)
                return null;
            }
            return su;
        }

        public void WriteGroup(string path, GroupVM group, List<StudyUnitVM> studyUnits)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));

            ClearError();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.Encoding = Encoding.UTF8;
            using (XmlWriter xw = XmlWriter.Create(path, xws))
            {
                XElement gr = CreateGroup(group, studyUnits);
                foreach (StudyUnitVM studyUnit in studyUnits)
                {
                    XElement su = new XElement(g + TAG_STUDY_UNIT);
                    gr.Add(su);
                    this.studyUnit = studyUnit;
                    su.Add(CreateStudyUnit());
                }
                gr.Add(CreateComparison(group, studyUnits));

                XDocument doc = new XDocument(
                    DECLARATION,
                    new XElement(ddi + TAG_DDI_INSTANCE,
                        CreateIDAttribute(group.GroupModel.InstanceId),
                        CreateVersionAttribute(),
                        CreateAgencyAttribute(),
                        new XAttribute(XNamespace.Xmlns + "ddi", ddi),
                        new XAttribute(XNamespace.Xmlns + "s", s),
                        new XAttribute(XNamespace.Xmlns + "r", r),
                        new XAttribute(XNamespace.Xmlns + "a", a),
                        new XAttribute(XNamespace.Xmlns + "c", c),
                        new XAttribute(XNamespace.Xmlns + "d", d),
                        new XAttribute(XNamespace.Xmlns + "l", l),
                        new XAttribute(XNamespace.Xmlns + "p", p),
                        new XAttribute(XNamespace.Xmlns + "pi", pi),
                        new XAttribute(XNamespace.Xmlns + "g", g),
                        new XAttribute(XNamespace.Xmlns + "cm", cm),
                        new XAttribute(XNamespace.Xmlns + "dce", dce),
                        new XAttribute(XNamespace.Xmlns + "dc", dc),
                        gr
                        ));
                CheckError();
                doc.WriteTo(xw);
            }
        }
    }
}
