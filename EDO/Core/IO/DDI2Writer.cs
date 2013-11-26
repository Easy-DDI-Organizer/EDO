using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Main;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using EDO.StudyCategory.MemberForm;
using EDO.QuestionCategory.QuestionForm;
using EDO.VariableCategory.VariableForm;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.CodeForm;
using EDO.QuestionCategory.CategoryForm;
using EDO.DataCategory.DataSetForm;
using EDO.DataCategory.DataFileForm;
using EDO.DataCategory.BookForm;
using EDO.QuestionCategory.ConceptForm;

namespace EDO.Core.IO
{
    public class DDI2Writer :DDI2IO
    {
        private static XDeclaration DECLARATION = new XDeclaration("1.0", "UTF-8", "no");

        private EDOConfig config;

        private StudyUnitVM studyUnit;

        private Dictionary<string, string> convertIds;

        public DDI2Writer(EDOConfig config)
        {
            this.config = config;
            convertIds = new Dictionary<string, string>(); 
        }

        private XAttribute CreateVersionAttribute()
        {
            return new XAttribute(ATTR_VERSION, "2.5");
        }

        #region StdyDscr

        private XElement CreateRspStmt()
        {
            XElement authEnty = null;
            List<XElement> othIds = new List<XElement>();
            foreach (MemberVM member in studyUnit.Members)
            {
                if (member.IsLeader)
                {
                    authEnty = new XElement(cb + TAG_AUTH_ENTY, 
                        new XAttribute(ATTR_AFFILIATION, member.OrganizationName), 
                        ToString(member));
                }
                else
                {
                    XElement other = new XElement(cb + TAG_OTH_ID,
                        new XAttribute(ATTR_ROLE,  Options.RoleLabel(member.RoleCode)),
                        new XAttribute(ATTR_AFFILIATION, member.OrganizationName), 
                        ToString(member)
                        );
                    othIds.Add(other);
                }
            }
            if (authEnty == null && othIds.Count == 0)
            {
                return null;
            }

            XElement rspStmt = new XElement(cb + TAG_RSP_STMT);
            if (authEnty != null)
            {
                rspStmt.Add(authEnty);
            }
            rspStmt.Add(othIds);
            return rspStmt;
        }

        private XElement CreateProdStmt()
        {
            XElement prodStmt = new XElement(cb + TAG_PROD_STMT);
            List<XElement> fundAgs = new List<XElement>();
            List<XElement> grantNos = new List<XElement>();
            foreach (FundingInfo fundingInfo in studyUnit.FundingInfoModels)
            {
                fundAgs.Add(new XElement(cb + TAG_FUND_AG, fundingInfo.Organization.OrganizationName));
                grantNos.Add(new XElement(cb + TAG_GRANT_NO, fundingInfo.Number));
            }
            prodStmt.Add(fundAgs);
            prodStmt.Add(grantNos);
            return prodStmt;
        }

        private XElement CreateCitation()
        {
            //調査の全体的な内容
            XElement citation =
                new XElement(cb + TAG_CITATION,
                    new XElement(cb + TAG_TITL_STMT,
                        new XElement(cb + TAG_TITL, studyUnit.Title)
                        ));
            citation.Add(CreateRspStmt());
            citation.Add(CreateProdStmt());
            return citation;
        }

        private XElement CreateSubject()
        {
            XElement subject = new XElement(cb + TAG_SUBJECT);
            // キーワード
            foreach (Keyword keywordModel in studyUnit.CoverageModel.Keywords)
            {
                XElement keyword = new XElement(cb + TAG_KEYWORD, keywordModel.Content);
                subject.Add(keyword);
            }
            // 調査対象のチェックボックス
            foreach (CheckOption topicOption in studyUnit.CoverageModel.Topics)
            {
                if (!topicOption.IsChecked)
                {
                    continue;
                }
                string label = topicOption.HasMemo ? topicOption.Memo : Options.CoverageTopicLabel(topicOption.Code);
                XElement topic = new XElement(cb + TAG_TOPC_CLAS, label);
                subject.Add(topic);
            }
            return EmptyToNull(subject);
        }

        private List<XElement> CreateTimePrds(XName name, DateRange dateRange)
        {
            List<XElement> elements = new List<XElement>();
            if (!dateRange.IsEmpty)
            {
                if (dateRange.IsFromDateOnly)
                {
                    XElement timePrd1 = new XElement(name,
                        new XAttribute(ATTR_DATE, ToString(dateRange.FromDate)));
                    elements.Add(timePrd1);
                }
                else
                {
                    XElement timePrd1 = new XElement(name,
                        new XAttribute(ATTR_DATE, ToString(dateRange.FromDate)),
                        new XAttribute(ATTR_EVENT, "start"));
                    XElement timePrd2 = new XElement(name,
                        new XAttribute(ATTR_DATE, ToString(dateRange.ToDate)),
                        new XAttribute(ATTR_EVENT, "end"));

                    elements.Add(timePrd1);
                    elements.Add(timePrd2);
                }
            }
            return elements;
        }

        private XElement CreateSumDscr()
        {
            //カバーする時期
            XElement sumDscr = new XElement(cb + TAG_SUM_DSCR);
            DateRange dateRange = studyUnit.CoverageModel.DateRange;
            sumDscr.Add(CreateTimePrds(cb + TAG_TIME_PRD, dateRange));
            //データ収集年月
            foreach (Sampling sampling in studyUnit.SamplingModels)
            {
                sumDscr.Add(CreateTimePrds(cb + TAG_COLL_DATE, sampling.DateRange));
            }
            //メモ
            sumDscr.Add(CreateNullable(cb + TAG_GEOG_COVER, studyUnit.CoverageModel.Memo));

            //母集団
            foreach (Universe universeModel in studyUnit.AllUniverseModels)
            {
                XElement universe = new XElement(cb + TAG_UNIVERSE, ToString(universeModel));
                sumDscr.Add(universe);
            }

            return EmptyToNull(sumDscr);
        }

        private XElement CreateMethod()
        {
            //データ収集方法のタブごとに一つのdataCollになる
            XElement method = new XElement(cb + TAG_METHOD);
            foreach (Sampling sampling in studyUnit.SamplingModels)
            {
                XElement dataColl = new XElement(cb + TAG_DATA_COLL);
                method.Add(dataColl);
                //データ収集者
                MemberVM member = studyUnit.FindMember(sampling.MemberId);
                if (member != null)
                {
                    XElement dataCollector = new XElement(cb + TAG_DATA_COLLECTOR,
                        new XAttribute(ATTR_ROLE, Options.RoleLabel(member.RoleCode)),
                        new XAttribute(ATTR_AFFILIATION, member.OrganizationName),
                        ToString(member));
                    dataColl.Add(dataCollector);
                }

                //サンプリング方法
                foreach (Universe universe in sampling.Universes)
                {
                    //これを元に母集団を読み込むのでからでも書き出す
                    XElement sampProc = new XElement(cb + TAG_SAMP_PROC, universe.Method);
                    dataColl.Add(sampProc);
                }

                //データ収集方法選択欄
                dataColl.Add(CreateNullable(cb + TAG_COLL_MODE, Options.SamplingMethodLabel(sampling.MethodCode)));

                //データ収集時の状況
                dataColl.Add(CreateNullable(cb + TAG_COLL_SITU, sampling.Situation));
            }

            return method;
        }

        private XElement CreateStdyInfo()
        {
            XElement stdyInfo = new XElement(cb + TAG_STDY_INFO);


            stdyInfo.Add(CreateSubject());

            //調査概要
            stdyInfo.Add(new XElement(cb + TAG_ABSTRACT, studyUnit.AbstractModel.Summary));

            stdyInfo.Add(CreateSumDscr());

            return stdyInfo;
        }


        private XElement CreateRspStmt(Book book)
        {
            XElement rspStmt = new XElement(cb + TAG_RSP_STMT);

            //著者名
            rspStmt.Add(CreateNullable(cb + TAG_AUTH_ENTY, book.Author));

            //編者
            if (!string.IsNullOrEmpty(book.Editor))
            {
                XElement othId = new XElement(cb + TAG_OTH_ID,
                    new XAttribute(ATTR_ROLE, "editor"),
                    book.Editor);
                rspStmt.Add(othId);
            }

            return EmptyToNull(rspStmt);
        }

        private XElement CreateProdStmt(Book book)
        {
            //出版社
            XElement prodStmt = new XElement(cb + TAG_PROD_STMT);
            string publisher = BuildPublisher(book);
            prodStmt.Add(CreateNullable(cb + TAG_PRODUCER, publisher));
            return EmptyToNull(prodStmt);
        }

        private XElement CreateDistStmt(Book book)
        {
            //発表日
            XElement distStmt = new XElement(cb + TAG_DIST_STMT);
            if (!string.IsNullOrEmpty(book.AnnouncementDate))
            {
                XElement distDate = new XElement(cb + TAG_DIST_DATE, book.AnnouncementDate);
                distStmt.Add(distDate);
            }
            return  EmptyToNull(distStmt);
        }

        private XElement CreateOthrStdyMat()
        {
            //関連文献
            XElement othrStdyMat = new XElement(cb + TAG_OTHR_STDY_MAT);
            XElement relMat = new XElement(cb + TAG_REL_MAT);
            othrStdyMat.Add(relMat);
            foreach (BookVM book in studyUnit.Books)
            {
                Book bookModel = book.Book;

                XElement citation = new XElement(cb + TAG_CITATION);
                relMat.Add(citation);

                //タイトル
                XElement titl = new XElement(cb + TAG_TITL_STMT,
                           new XElement(cb + TAG_TITL, book.Title));
                citation.Add(titl);
                citation.Add(CreateRspStmt(bookModel));
//                citation.Add(CreateProdStmt(bookModel)); 
                citation.Add(CreateDistStmt(bookModel));
                citation.Add(CreateNullable(cb + TAG_BIBL_CIT, BuildIdentifier(bookModel)));

                string url = bookModel.Url;
                if (!string.IsNullOrEmpty(url))
                {
                    citation.Add(new XElement(cb + TAG_HOLDINGS,
                        new XAttribute(ATTR_URI, url)));
                }

                citation.Add(CreateNullable(terms + TAG_ABSTRACT, bookModel.Summary));
                citation.Add(CreateNullable(dc + TAG_LANGUAGE, bookModel.Language));
                citation.Add(CreateNullable(dc + TAG_PUBLISHER, BuildPublisher(bookModel)));
            }
            return othrStdyMat;
        }

        private XElement CreateStdyDscr()
        {
            XElement stdyDscr = new XElement(cb + TAG_STDY_DSCR);
            stdyDscr.Add(CreateCitation());
            stdyDscr.Add(CreateStdyInfo());
            stdyDscr.Add(CreateMethod());
            stdyDscr.Add(CreateOthrStdyMat());
            return stdyDscr;
        }

        #endregion


        #region for DataDscr

        public static string ToSafeId(object id)
        {
            return "ID_" + id;
        }

        private string JoinConceptIds(ConceptScheme conceptScheme)
        {
            List<string> ids = Concept.GetIds(conceptScheme.Concepts);
            List<string> safeIds = new List<string>();
            foreach (string id in ids)
            {
                safeIds.Add(ConvertId(id));
            }
            return string.Join(" ", safeIds);
        }

        private string JoinVariableIds(Concept concept)
        {
            List<Variable> variables = studyUnit.StudyUnitModel.FindVariablesByConceptId(concept.Id);
            IEnumerable<string> ids = Variable.GetIds(variables);
            IEnumerable<string> safeIds = ids.Select(p => ConvertId(p));
            return string.Join(" ", safeIds);        
        }

        private string JoinDataSetIds(Variable variable)
        {
            List<DataSetVM> dataSets = studyUnit.FindDataSetsByVariableId(variable.Id);
            IEnumerable<string> ids = DataSetVM.GetIds(dataSets);
            IEnumerable<string> safeIds = ids.Select(p => ConvertId(p));
            return string.Join(" ", safeIds);
        }

        private XAttribute CreateIDAttribute(object id)
        {
            return new XAttribute(ATTR_ID, id);
        }

        private XAttribute CreateSafeIDAttribute(object id)
        {
            return new XAttribute(ATTR_ID, ToSafeId(id));
        }

        private string JoinCodeIds(IEnumerable<CodeVM> codes, string convertParentId)
        {
            IEnumerable<string> ids = codes.Select(p => ConvertChildId(p.CodeId, convertParentId));
            return string.Join(" ", ids);
        }

        private XElement CreateQstn(QuestionVM question)
        {
            string type = GetResponseDomainType(question.Response.TypeCode);
            XElement qstn = new XElement(cb + TAG_QSTN,
                CreateNullableAttr(ATTR_RESPONSE_DOMAIN_TYPE, type),
                CreateNullable(cb + TAG_QSTN_LIT, question.Text));
            return qstn;
        }

        private XElement CreateVar(QuestionVM question)
        {
            string parentConvertId = ConvertId(question.Id); 
            XElement varElem = new XElement(cb + TAG_VAR,
                CreateIDAttribute(parentConvertId),
                CreateNullableAttr(ATTR_NAME, question.Title)
                );
            varElem.Add(CreateQstn(question));
            AppendResponseElelems(varElem, parentConvertId, question.Response);
            return varElem;
        }

        private void AppendResponseElelems(XElement varElem, string parentConvertId, ResponseVM response)
        {

            if (response.IsTypeChoices)
            {
                List<XElement> catgryGrps = new List<XElement>();
                List<XElement> catgries = new List<XElement>();

                CodeSchemeVM codeScheme = response.CodeScheme;
                ObservableCollection<CodeVM> codes = codeScheme.Codes;

                if (codes.Count > 0)
                {
                    XElement catgryGrp = new XElement(cb + TAG_CATGRY_GRP,
                        CreateIDAttribute(ConvertChildId(codeScheme.Id, parentConvertId)),
                        new XAttribute(ATTR_CATGRY, JoinCodeIds(codes, parentConvertId)),
                        new XElement(cb + TAG_LABL, codeScheme.Title),
                        new XElement(cb + TAG_TXT, codeScheme.Memo));
                    varElem.Add(catgryGrp);

                    foreach (CodeVM code in codes)
                    {
                        XElement catgry = new XElement(cb + TAG_CATGRY,
                            CreateIDAttribute(ConvertChildId(code.CodeId, parentConvertId)),
                            new XElement(cb + TAG_CAT_VALU, code.Value),
                            new XElement(cb + TAG_LABL, code.Label),
                            new XElement(cb + TAG_TXT, code.Category.Memo)
                            );
                        varElem.Add(catgry);
                    }
                }

                XElement varFormat = new XElement(cb + TAG_VAR_FORMAT,
                    new XAttribute(ATTR_TYPE, "numeric"),
                    new XAttribute(ATTR_SCHEMA, "other"));
                varElem.Add(varFormat);
            }
            else if (response.IsTypeNumber)
            {
                XElement valrng = new XElement(cb + TAG_VALRNG,
                    new XElement(cb + TAG_RANGE,
                        CreateNullableAttr(ATTR_MIN, response.Min),
                        CreateNullableAttr(ATTR_MAX, response.Max)));
                varElem.Add(valrng);

                XElement varFormat = new XElement(cb + TAG_VAR_FORMAT);
                varElem.Add(varFormat);
            }
            else if (response.IsTypeFree)
            {
                XElement varlng = new XElement(cb + TAG_VALRNG,
                    new XElement(cb + TAG_RANGE,
                        CreateNullableAttr(ATTR_MIN, response.Min),
                        CreateNullableAttr(ATTR_MAX, response.Max)));
                varElem.Add(varlng);

                XElement varFormat = new XElement(cb + TAG_VAR_FORMAT,
                    new XAttribute(ATTR_TYPE, "character"),
                    new XAttribute(ATTR_SCHEMA, "other"));
                varElem.Add(varFormat);
            }
            else if (response.IsTypeDateTime)
            {
                XElement varFormat = new XElement(cb + TAG_VAR_FORMAT,
                    new XAttribute(ATTR_TYPE, "numeric"),
                    new XAttribute(ATTR_SCHEMA, "other"),
                    new XAttribute(ATTR_CATEGORY, "date"));
                varElem.Add(varFormat);
            }
        }

        private XElement CreateVar(VariableVM variable)
        {
            string representationType = GetRepresentationType(variable.Response.TypeCode);
            string parentConvertId = ConvertId(variable.Id);
            XElement varElem = new XElement(cb + TAG_VAR,
                CreateIDAttribute(parentConvertId),        
                CreateNullableAttr(ATTR_FILES, JoinDataSetIds(variable.Variable)),
                new XAttribute(ATTR_NAME, variable.Title),
                CreateNullableAttr(ATTR_REPRESENTATION_TYPE, representationType),
                new XAttribute(ATTR_INTRVL, "discrete"),
                CreateNullable(cb + TAG_LABL, variable.Label));
            QuestionVM question = studyUnit.FindQuestion(variable.QuestionId);
            if (question != null)
            {
                XElement qstn = CreateQstn(question);
                varElem.Add(qstn);
            }
            AppendResponseElelems(varElem, parentConvertId, variable.Response);
            return varElem;
        }


        private XElement CreateDataDscr()
        {
            XElement dataDscr = new XElement(cb + TAG_DATA_DSCR);
            foreach (ConceptScheme conceptScheme in studyUnit.ConceptSchemeModels)
            {
                XElement parentVarGrp = new XElement(cb + TAG_VAR_GRP,
                    CreateNullableAttr(ATTR_VAR_GRP, JoinConceptIds(conceptScheme)));
                parentVarGrp.Add(CreateNullable(cb + TAG_CONCEPT, conceptScheme.Title));
                parentVarGrp.Add(CreateNullable(cb + TAG_DEFNTH, conceptScheme.Memo));
                dataDscr.Add(parentVarGrp);

                foreach (Concept concept in conceptScheme.Concepts)
                {
                    XElement varGrp = new XElement(cb + TAG_VAR_GRP,
                        CreateIDAttribute(ConvertId(concept.Id)),
                        CreateNullableAttr(ATTR_VAR, JoinVariableIds(concept)));
                    varGrp.Add(CreateNullable(cb + TAG_CONCEPT, concept.Title)); 
                    varGrp.Add(CreateNullable(cb + TAG_DEFNTH, concept.Content));
                    dataDscr.Add(varGrp);
                }
            }
            //foreach (QuestionVM question in studyUnit.AllQuestions)
            //{
            //    dataDscr.Add(CreateVar(question));
            //}

            foreach (VariableVM variable in studyUnit.Variables)
            {
                dataDscr.Add(CreateVar(variable));
            }

            return dataDscr;
        }

        #endregion

        #region for FileDscr

        private List<XElement> CreateFileDscrs()
        {
            List<XElement> fileDscrs = new List<XElement>();
            foreach (DataFileVM dataFile in studyUnit.DataFiles)
            {
                DataSetVM dataSet = studyUnit.FindDataSet(dataFile.DataSetId);

                XElement fileDscr = new XElement(cb + TAG_FILE_DSCR,
                    CreateIDAttribute(ConvertId(dataSet.Id)),
                    CreateNullableAttr(ATTR_URI, dataFile.Uri),
                    new XElement(cb + TAG_FILE_TXT,
                        new XElement(cb + TAG_FILE_NAME, dataSet.Title),
                        new XElement(cb + TAG_FILE_CONT, dataSet.Memo),
                        new XElement(cb + TAG_FORMAT, dataFile.Format)
                        ));
                    
                fileDscrs.Add(fileDscr);               
            }
            return fileDscrs;
        }

        #endregion


        private void CreateConvertIds()
        {
            convertIds.Clear();
            int seqNo = 1;
            foreach (ConceptVM concept in studyUnit.AllConcepts)
            {
                convertIds[concept.Id] = "VG" + seqNo++;
            }

            seqNo = 1;
            foreach (QuestionVM question in studyUnit.AllQuestions)
            {
                convertIds[question.Id] = "Q" + seqNo++;
            }

            seqNo = 1;
            foreach (VariableVM variable in studyUnit.Variables)
            {
                convertIds[variable.Id] = "V" + seqNo++;
            }

            int categoryGroupSeqNo = 1;
            int categorySeqNo = 1;
            foreach (CodeSchemeVM codeScheme in studyUnit.CodeSchemes)
            {
                convertIds[codeScheme.Id] = "CG" + categoryGroupSeqNo++; 
                foreach (CodeVM code in codeScheme.Codes)
                {
                    convertIds[code.CodeId] = "C" + categorySeqNo++;
                }
            }

            seqNo = 1;
            foreach (DataSetVM dataSet in studyUnit.DataSets)
            {
                convertIds[dataSet.Id] = "F" + seqNo++;
            }
        }

        private string ConvertId(string orgId)
        {
            return convertIds[orgId];
        }

        private string ConvertChildId(string orgId, string parentConvertId)
        {
            return ConvertId(orgId) + "_" + parentConvertId;
        }

        public void WriteCodebook(string path, StudyUnitVM studyUnit)
        {

            Debug.Assert(!string.IsNullOrEmpty(path));
            this.studyUnit = studyUnit;
            ClearError();
            CreateConvertIds();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.Encoding = Encoding.UTF8;
            using (XmlWriter xw = XmlWriter.Create(path, xws))
            {
                XDocument doc = new XDocument(
                    DECLARATION,
                    new XElement(cb + TAG_CODEBOOK,
                        CreateSafeIDAttribute(studyUnit.Id),
                        CreateVersionAttribute(),
                        new XAttribute("xmlns", cb),
                        new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                        new XAttribute(XNamespace.Xmlns + "dc", dc),
                        new XAttribute(XNamespace.Xmlns + "terms", terms),
                        new XAttribute(XNamespace.Xmlns + "schemaLocation", schemaLocation),
                        CreateStdyDscr(),
                        CreateFileDscrs(),
                        CreateDataDscr()
                        )
                );
                CheckError();
                doc.WriteTo(xw);
            }

        }
    }
}
