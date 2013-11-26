using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.ObjectModel;
using System.Windows;
using EDO.StudyCategory.MemberForm;
using EDO.StudyCategory.FundingInfoForm;
using EDO.SamplingCategory.SamplingForm;
using EDO.Core.Model;
using EDO.VariableCategory.VariableForm;
using EDO.QuestionCategory.QuestionForm;
using EDO.Core.Util;
using EDO.QuestionCategory.CodeForm;
using EDO.Properties;

namespace EDO.Core.IO
{
    public class CodebookWriter :DocxWriter
    {
        private StudyUnitVM studyUnit;
        public CodebookWriter(StudyUnitVM studyUnit)
        {
            this.studyUnit = studyUnit;
        }

        private DocxParam CreateTitleParam()
        {
            DocxParam param = new DocxParam()
            {
                Justification = new Justification() { Val = JustificationValues.Center },
                FontSize = "32",
                RunFonts = new RunFonts() { Hint = FontTypeHintValues.EastAsia, AsciiTheme = ThemeFontValues.MajorEastAsia, HighAnsiTheme = ThemeFontValues.MajorEastAsia, EastAsiaTheme = ThemeFontValues.MajorEastAsia }
            };
            return param;
        }

        private Paragraph CreateEmptyTitleParagraph()
        {
            DocxParam param = CreateTitleParam();
            param.Justification = new Justification()
            {
                Val = JustificationValues.Left
            };
            return CreateParagraph("", param);
        }

        private Paragraph CreateTitleParagraph(string title)
        {            
            return CreateParagraph(title, CreateTitleParam());
        }

        private void WriteCover(Body body)
        {
            for (int i = 0; i < 6; i++)
            {
                Paragraph paragraph = CreateEmptyTitleParagraph();
                body.Append(paragraph);

            }
            DocxParam param = CreateTitleParam();
            Paragraph title1 = CreateParagraph(studyUnit.AbstractForm.Title, param);
            body.Append(title1);
            Paragraph title2 = CreateParagraph(Resources.Codebook, param);//コードブック
            body.Append(title2);
            Paragraph breakPage = CreateBreakPageParagraph();
            body.Append(breakPage);
        }

        private void WriteTableOfContents(Body body)
        {
            Paragraph p = CreateParagraph(Resources.Toc); //目次
            body.Append(p);

            p = CreateParagraph("1. " + Resources.StudyAbstract); //調査の概要
            body.Append(p);

            DocxParam param = new DocxParam()
            {
                Indentation = new Indentation() { Left = "420", LeftChars = 200 }
            };
            p = CreateParagraph("1.1. " + Resources.StudyMember, param); //調査メンバー
            body.Append(p);
            p = CreateParagraph("1.2. " + Resources.StudyPurpose, param); //調査目的
            body.Append(p);
            p = CreateParagraph("1.3. " + Resources.StudyAbstract, param); //調査の概要
            body.Append(p);
            p = CreateParagraph("1.4. " + Resources.StudyFund, param); //研究資金
            body.Append(p);
            p = CreateParagraph("");
            body.Append(p);
            p = CreateParagraph("2. " + Resources.DataCollectionMethod); //データの収集方法
            body.Append(p);
            p = CreateParagraph("2.1. " + Resources.Universe, param); //母集団
            body.Append(p);
            p = CreateParagraph("2.2. " + Resources.SamplingMethod, param); //サンプリング方法
            body.Append(p);
            p = CreateParagraph("2.3. " + Resources.CollectionPeriod, param); //データ収集期間
            body.Append(p);
            p = CreateParagraph("2.4. " + Resources.CollectionMethod, param); //データ収集方法
            body.Append(p);
            p = CreateParagraph("");
            body.Append(p);
            p = CreateParagraph("3. " + Resources.VariableSummary); //変数の概要
            body.Append(p);

            DocxParam param2 = new DocxParam()
            {
                Break = new Break() { Type = BreakValues.Page }
            };
            p = CreateBreakPageParagraph();
            body.Append(p);
        }

        private void WriteAbstract(Body body)
        {
            Paragraph p = CreateMidashi1Paragraph("1. " + Resources.StudyAbstract); //調査の概要
            body.Append(p);

            p = CreateMidashi2Paragraph("1.1. " + Resources.StudyMember); //調査メンバー
            body.Append(p);
           
            foreach (MemberVM member in studyUnit.Members)
            {
                string text = member.LastName + member.FirstName + " " + member.OrganizationName + " " + member.Position;
                p = CreateParagraph(text);
                body.Append(p);
            }
            p = CreateEmptyParagraph();
            body.Append(p);

            p = CreateMidashi2Paragraph("1.2. " + Resources.StudyPurpose); //調査目的
            body.Append(p);
            p = CreateParagraph(studyUnit.AbstractForm.Purpose);
            body.Append(p);
            p = CreateEmptyParagraph();
            body.Append(p);


            p = CreateMidashi2Paragraph("1.3. " + Resources.StudyAbstract); //調査の概要
            body.Append(p);
            p = CreateParagraph(studyUnit.AbstractForm.Summary);
            body.Append(p);
            p = CreateEmptyParagraph();
            body.Append(p);

            p = CreateMidashi2Paragraph("1.4. " + Resources.StudyFund); //研究資金
            body.Append(p);
            foreach (FundingInfoVM fundingInfo in studyUnit.FundingInfos)
            {
                if (!string.IsNullOrEmpty(fundingInfo.Number))
                {
                    string text = fundingInfo.Title + " (" + fundingInfo.OrganizationName + ", " + Resources.SubjectNumber + ":" + fundingInfo.Number + ")"; //課題番号
                    p = CreateParagraph(text);
                    body.Append(p);
                }
            }
            p = CreateEmptyParagraph();
            body.Append(p);

            p = CreateBreakPageParagraph();
            body.Append(p);
        }

        private void WriteSamplingMethod(Body body)
        {
            Paragraph p = CreateMidashi1Paragraph("2. " + Resources.CollectionMethodOfData); //データの収集法法
            body.Append(p);

            int samplingIndex = 1;
            foreach (Sampling sampling in studyUnit.SamplingModels)
            {
                String prefix = "2." + samplingIndex + ".";
                p = CreateMidashi2Paragraph(prefix + " " + Resources.Universe + samplingIndex); //母集団
                samplingIndex++;
                body.Append(p);
                foreach (UniverseVM universe in studyUnit.Universes)
                {
                    p = CreateParagraph(universe.Memo);
                    body.Append(p);
                }
                p = CreateEmptyParagraph();
                body.Append(p);

                p = CreateMidashi2Paragraph(prefix + "1. " + Resources.SamplingMethod); //サンプリング方法
                body.Append(p);
                foreach (UniverseVM universe in studyUnit.Universes)
                {
                    p = CreateParagraph(universe.Method);
                    body.Append(p);
                }
                p = CreateEmptyParagraph();
                body.Append(p);

                p = CreateMidashi2Paragraph(prefix + "2. " + Resources.CollectionPeriod); //データ収集期間
                body.Append(p);
                DateRange range = sampling.DateRange;
                if (range != null)
                {
                    p = CreateParagraph(range.ToStringJa());
                    body.Append(p);
                }
                p = CreateEmptyParagraph();
                body.Append(p);

                p = CreateMidashi2Paragraph(prefix + "3. " + Resources.CollectionMethod); //データ収集方法
                body.Append(p);
                p = CreateParagraph(Option.FindLabel(Options.SamplingMethods, sampling.MethodCode));
                body.Append(p);
                p = CreateBreakPageParagraph();
                body.Append(p);
            }


            samplingIndex++;

        }

        private Paragraph CreateCodeParagraph(CodeVM code)
        {
            Paragraph paragraph1 = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            Indentation indentation1 = new Indentation(){ FirstLine = "210", FirstLineChars = 100 };

            paragraphProperties1.Append(indentation1);

            Run run1 = new Run();

            RunProperties runProperties1 = new RunProperties();
            RunFonts runFonts1 = new RunFonts(){ Hint = FontTypeHintValues.EastAsia };

            runProperties1.Append(runFonts1);
            Text text1 = new Text();
            text1.Text = code.Value;
            run1.Append(runProperties1);
            run1.Append(text1);

            Run run2 = new Run();

            RunProperties runProperties2 = new RunProperties();
            RunFonts runFonts2 = new RunFonts(){ Hint = FontTypeHintValues.EastAsia };

            runProperties2.Append(runFonts2);
            TabChar tabChar1 = new TabChar();

            run2.Append(runProperties2);
            run2.Append(tabChar1);

            Run run3 = new Run();

            RunProperties runProperties3 = new RunProperties();
            RunFonts runFonts3 = new RunFonts(){ Hint = FontTypeHintValues.EastAsia };

            runProperties3.Append(runFonts3);
            Text text2 = new Text();
            text2.Text = code.Label;

            run3.Append(runProperties3);
            run3.Append(text2);

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            paragraph1.Append(run2);
            paragraph1.Append(run3);
            return paragraph1;
        }

        private void WriteVariables(Body body)
        {
            Paragraph p = CreateMidashi1Paragraph("3. " + Resources.VariableSummary); //変数の概要
            body.Append(p);

            ObservableCollection<QuestionVM> questions = studyUnit.AllQuestions;
            foreach (VariableVM variable in studyUnit.Variables)
            {
                StringBuilder buf = new StringBuilder();
                buf.Append(variable.Title);
                buf.Append(" ");
                buf.Append(variable.Label);
                buf.Append(" (");
                buf.Append(variable.Response.TypeName);
                buf.Append(" )");
                p = CreateParagraph(buf.ToString());
                body.Append(p);

                QuestionVM question = EDOUtils.Find<QuestionVM>(questions, variable.QuestionId);
                buf = new StringBuilder();
                buf.Append(Resources.CorrespondingQuestionSentence); //対応する質問文
                if (question != null)
                {
                    buf.Append(" " + question.Text);
                }

                //質問文の段落
                p = CreateParagraph(buf.ToString());
                body.Append(p);

                if (variable.Response.IsTypeChoices)
                {
                    p = CreateEmptyParagraph();
                    body.Append(p);

                    Table table = new Table();
                    body.Append(table);

                    TableProperties tableProperties = new TableProperties();
                    DocumentFormat.OpenXml.Wordprocessing.TableStyle tableStyle = new DocumentFormat.OpenXml.Wordprocessing.TableStyle() { Val = "TableGrid" };
                    TableWidth tableWidth = new TableWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
                    TableLook tableLook = new TableLook()
                    {
                        Val = "04A0",
                        FirstRow = true,
                        LastRow = false,
                        FirstColumn = true,
                        LastColumn = false,
                        NoHorizontalBand = false,
                        NoVerticalBand = true
                    };
                    tableProperties.Append(tableStyle);
                    tableProperties.Append(tableWidth);
                    tableProperties.Append(tableLook);
                    table.Append(tableProperties);

                    TableRow tableRow = new TableRow();
                    table.Append(tableRow);

                    TableCell tableCell1 = new TableCell();
                    tableCell1.Append(CreateParagraph(Resources.Code));
                    tableRow.Append(tableCell1);

                    TableCell tableCell2 = new TableCell();
                    tableCell2.Append(CreateParagraph(Resources.Category));
                    tableRow.Append(tableCell2);

                    TableCell tableCell3 = new TableCell();
                    tableCell3.Append(CreateParagraph(Resources.Total));
                    tableRow.Append(tableCell3);

                    ObservableCollection<CodeVM> codes = variable.Response.Codes;
                    foreach (CodeVM code in codes)
                    {
                        tableRow = new TableRow();
                        table.Append(tableRow);

                        tableCell1 = new TableCell();
                        tableCell1.Append(CreateParagraph(code.Value));
                        tableRow.Append(tableCell1);

                        tableCell2 = new TableCell();
                        tableCell2.Append(CreateParagraph(code.Label));
                        tableRow.Append(tableCell2);

                        tableCell3 = new TableCell();
                        tableCell3.Append(CreateEmptyParagraph());
                        tableRow.Append(tableCell3);
                    }

                }

                //空の段落
                p = CreateEmptyParagraph();
                body.Append(p);

            }
            p = CreateBreakPageParagraph();
            body.Append(p);


        }

        protected override void WriteBody(Body body)
        {
            //表紙の書き出し
            WriteCover(body);

            //目次の書き出し
            WriteTableOfContents(body);

            //1.調査の概要
            WriteAbstract(body);

            //2.データの収集方法
            WriteSamplingMethod(body);

            //3.変数
            WriteVariables(body);
        }

    }
}
