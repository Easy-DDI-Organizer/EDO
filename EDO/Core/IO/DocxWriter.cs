using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using EDO.Properties;

namespace EDO.Core.IO
{
    public abstract class DocxWriter
    {
        public static T CloneValue<T>(ICloneable obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }
            return (T)obj.Clone();
        }

        public class DocxParam
        {
            public string FontSize { get; set; }
            private Justification justification;
            public Justification Justification
            {
                get
                {
                    return CloneValue<Justification>(justification);
                }
                set
                {
                    justification = value;
                }
            }

            private RunFonts runFonts;
            public RunFonts RunFonts
            {
                get
                {
                    return CloneValue<RunFonts>(runFonts);
                }
                set
                {
                    runFonts = value;
                }
            }

            private Break breakValue;
            public Break Break
            {
                get
                {
                    return CloneValue<Break>(breakValue);
                }
                set
                {
                    breakValue = value;
                }
            }

            private Indentation indentation;
            public Indentation Indentation
            {
                get
                {
                    return CloneValue<Indentation>(indentation);
                }
                set
                {
                    indentation = value;
                }
            }

            public string StyleId
            {
                get;
                set;
            }

        }

        protected static void PreviewWord(string path)
        {
            string safePath = "\"" +  path + "\"";
            Process process = Process.Start("winword", safePath);
            process.WaitForExit();
        }

        protected Paragraph CreateUnderline(string content)
        {
            Paragraph paragraph = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            RunFonts runFonts1 = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            Underline underline1 = new Underline() { Val = UnderlineValues.Single };
            paragraphMarkRunProperties1.Append(runFonts1);
            paragraphMarkRunProperties1.Append(underline1);
            paragraphProperties1.Append(paragraphMarkRunProperties1);

            RunProperties runProperties = new RunProperties();
            RunFonts runFonts = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            Underline underline = new Underline()
            {
                Val = UnderlineValues.Single
            };
            runProperties.Append(runFonts);
            runProperties.Append(underline);

            Text text = new Text()
            {
                Space = SpaceProcessingModeValues.Preserve,
            };
            // UnderlineTrailingSpacesを設定しておかないと単独の空白にアンダーラインはひかれない
            text.Text = content;

            Run run = new Run();
            run.Append(runProperties);
            run.Append(text);
            paragraph.Append(paragraphProperties1);
            paragraph.Append(run);

            return paragraph;
        }

        protected Paragraph CreateBreakPageParagraph()
        {
            DocxParam param = new DocxParam()
            {
                Break = new Break() { Type = BreakValues.Page }
            };
            return CreateParagraph("", param);
        }

        protected Paragraph CreateMidashi1Paragraph(string text)
        {
            DocxParam param = new DocxParam()
            {
                StyleId = MIDASHI1
            };
            return CreateParagraph(text, param);
        }

        protected Paragraph CreateMidashi2Paragraph(string text)
        {
            DocxParam param = new DocxParam()
            {
                StyleId = MIDASHI2
            };
            return CreateParagraph(text, param);
        }

        protected Paragraph CreateEmptyParagraph()
        {
            return CreateParagraph("");
        }

        protected Paragraph CreateParagraph(string text)
        {
            return CreateParagraph(text, new DocxParam());
        }

        protected Paragraph CreateParagraph(string title, DocxParam param)
        {
            Paragraph paragraph1 = new Paragraph();

            ParagraphProperties paragraphProperties1 = new ParagraphProperties();
            ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();
            if (param.RunFonts != null)
            {
                paragraphMarkRunProperties1.Append(param.RunFonts);
            }
            if (param.FontSize != null)
            {
                FontSize fontSize1 = new FontSize() { Val = param.FontSize };
                FontSizeComplexScript fontSizeComplexScript1 = new FontSizeComplexScript() { Val = param.FontSize };
                paragraphMarkRunProperties1.Append(fontSize1);
                paragraphMarkRunProperties1.Append(fontSizeComplexScript1);
            }
            if (param.StyleId != null)
            {
                ParagraphStyleId paragraphStyleId = new ParagraphStyleId() { Val = param.StyleId };
                paragraphProperties1.Append(paragraphStyleId);
            }
            if (param.Justification != null)
            {
                paragraphProperties1.Append(param.Justification);
            }
            if (param.Indentation != null)
            {
                paragraphProperties1.Append(param.Indentation);
            }
            paragraphProperties1.Append(paragraphMarkRunProperties1);


            Run run1 = new Run();

            RunProperties runProperties1 = new RunProperties();
            if (param.RunFonts != null)
            {
                runProperties1.Append(param.RunFonts);
            }
            if (param.FontSize != null)
            {
                FontSize fontSize2 = new FontSize() { Val = param.FontSize };
                FontSizeComplexScript fontSizeComplexScript2 = new FontSizeComplexScript() { Val = param.FontSize };
                runProperties1.Append(fontSize2);
                runProperties1.Append(fontSizeComplexScript2);
            }
            Text text1 = new Text();
            text1.Text = title;

            run1.Append(runProperties1);
            run1.Append(text1);
            if (param.Break != null)
            {
                run1.Append(param.Break);
            }

            paragraph1.Append(paragraphProperties1);
            paragraph1.Append(run1);
            return paragraph1;
        }


        protected const string MIDASHI1 = "1";
        protected const string MIDASHI2 = "2";

        private Style GenerateGridTableStyle()
        {
            Style style1 = new Style() { Type = StyleValues.Table, StyleId = "TableGrid" };
            StyleName styleName1 = new StyleName() { Val = "Table Grid" };
            BasedOn basedOn1 = new BasedOn() { Val = "TableNormal" };
            UIPriority uIPriority1 = new UIPriority() { Val = 59 };
            Rsid rsid1 = new Rsid() { Val = "005F1CC5" };

            StyleParagraphProperties styleParagraphProperties1 = new StyleParagraphProperties();
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines()
            {
                After = "0",
                Line = "240",
                LineRule = LineSpacingRuleValues.Auto
            };

            styleParagraphProperties1.Append(spacingBetweenLines1);

            StyleTableProperties styleTableProperties1 = new StyleTableProperties();
            TableIndentation tableIndentation1 = new TableIndentation()
            {
                Width = 0,
                Type = TableWidthUnitValues.Dxa
            };

            TableBorders tableBorders1 = new TableBorders();
            TopBorder topBorder1 = new TopBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            LeftBorder leftBorder1 = new LeftBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            BottomBorder bottomBorder1 = new BottomBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            RightBorder rightBorder1 = new RightBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            InsideHorizontalBorder insideHorizontalBorder1 = new InsideHorizontalBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            InsideVerticalBorder insideVerticalBorder1 = new InsideVerticalBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };

            tableBorders1.Append(topBorder1);
            tableBorders1.Append(leftBorder1);
            tableBorders1.Append(bottomBorder1);
            tableBorders1.Append(rightBorder1);
            tableBorders1.Append(insideHorizontalBorder1);
            tableBorders1.Append(insideVerticalBorder1);

            TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();
            TopMargin topMargin1 = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            TableCellLeftMargin tableCellLeftMargin1 = new TableCellLeftMargin()
            {
                Width = 108,
                Type = TableWidthValues.Dxa
            };
            BottomMargin bottomMargin1 = new BottomMargin()
            {
                Width = "0",
                Type = TableWidthUnitValues.Dxa
            };
            TableCellRightMargin tableCellRightMargin1 = new TableCellRightMargin()
            {
                Width = 108,
                Type = TableWidthValues.Dxa
            };

            tableCellMarginDefault1.Append(topMargin1);
            tableCellMarginDefault1.Append(tableCellLeftMargin1);
            tableCellMarginDefault1.Append(bottomMargin1);
            tableCellMarginDefault1.Append(tableCellRightMargin1);

            styleTableProperties1.Append(tableIndentation1);
            styleTableProperties1.Append(tableBorders1);
            styleTableProperties1.Append(tableCellMarginDefault1);

            style1.Append(styleName1);
            style1.Append(basedOn1);
            style1.Append(uIPriority1);
            style1.Append(rsid1);
            style1.Append(styleParagraphProperties1);
            style1.Append(styleTableProperties1);
            return style1;
        }

        private Style GenerateUnderlineTableStyle()
        {
            Style style1 = new Style() { Type = StyleValues.Table, StyleId = "TableUnderline" };
            StyleName styleName1 = new StyleName() { Val = "Table Underline" };
            BasedOn basedOn1 = new BasedOn() { Val = "TableNormal" };
            UIPriority uIPriority1 = new UIPriority() { Val = 59 };
            Rsid rsid1 = new Rsid() { Val = "005F1CC5" };

            StyleParagraphProperties styleParagraphProperties1 = new StyleParagraphProperties();
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines()
            {
                After = "0",
                Line = "240",
                LineRule = LineSpacingRuleValues.Auto
            };

            styleParagraphProperties1.Append(spacingBetweenLines1);

            StyleTableProperties styleTableProperties1 = new StyleTableProperties();
            TableIndentation tableIndentation1 = new TableIndentation()
            {
                Width = 0,
                Type = TableWidthUnitValues.Dxa
            };

            TableBorders tableBorders1 = new TableBorders();
            TopBorder topBorder1 = new TopBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            LeftBorder leftBorder1 = new LeftBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            BottomBorder bottomBorder1 = new BottomBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            RightBorder rightBorder1 = new RightBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            InsideHorizontalBorder insideHorizontalBorder1 = new InsideHorizontalBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };
            InsideVerticalBorder insideVerticalBorder1 = new InsideVerticalBorder()
            {
                Val = BorderValues.Single,
                Color = "auto",
                Size = (UInt32Value)4U,
                Space = (UInt32Value)0U
            };

            tableBorders1.Append(topBorder1);
            tableBorders1.Append(leftBorder1);
            tableBorders1.Append(bottomBorder1);
            tableBorders1.Append(rightBorder1);
            tableBorders1.Append(insideHorizontalBorder1);
            tableBorders1.Append(insideVerticalBorder1);

            TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();
            TopMargin topMargin1 = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            TableCellLeftMargin tableCellLeftMargin1 = new TableCellLeftMargin()
            {
                Width = 108,
                Type = TableWidthValues.Dxa
            };
            BottomMargin bottomMargin1 = new BottomMargin()
            {
                Width = "0",
                Type = TableWidthUnitValues.Dxa
            };
            TableCellRightMargin tableCellRightMargin1 = new TableCellRightMargin()
            {
                Width = 108,
                Type = TableWidthValues.Dxa
            };

            tableCellMarginDefault1.Append(topMargin1);
            tableCellMarginDefault1.Append(tableCellLeftMargin1);
            tableCellMarginDefault1.Append(bottomMargin1);
            tableCellMarginDefault1.Append(tableCellRightMargin1);

            styleTableProperties1.Append(tableIndentation1);
            styleTableProperties1.Append(tableBorders1);
            styleTableProperties1.Append(tableCellMarginDefault1);

            style1.Append(styleName1);
            style1.Append(basedOn1);
            style1.Append(uIPriority1);
            style1.Append(rsid1);
            style1.Append(styleParagraphProperties1);
            style1.Append(styleTableProperties1);
            return style1;
        }

        // styleDefinitionsPart1 のコンテンツを生成します。
        //(実際にスタイルを適用したdocxファイルをProductivity Toolに読み込ませてこの部分をコピーする。そうしないとスタイル定義があっても使えない)。
        protected void GenerateStyleDefinitionsPart1Content3(StyleDefinitionsPart styleDefinitionsPart1)
        {
            Styles styles1 = new Styles();
            styles1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            styles1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");

            DocDefaults docDefaults1 = new DocDefaults();

            RunPropertiesDefault runPropertiesDefault1 = new RunPropertiesDefault();

            RunPropertiesBaseStyle runPropertiesBaseStyle1 = new RunPropertiesBaseStyle();
            RunFonts runFonts21 = new RunFonts() { AsciiTheme = ThemeFontValues.MinorHighAnsi, HighAnsiTheme = ThemeFontValues.MinorHighAnsi, EastAsiaTheme = ThemeFontValues.MinorEastAsia, ComplexScriptTheme = ThemeFontValues.MinorBidi };
            Kern kern1 = new Kern() { Val = (UInt32Value)2U };
            FontSize fontSize7 = new FontSize() { Val = "21" };
            FontSizeComplexScript fontSizeComplexScript7 = new FontSizeComplexScript() { Val = "22" };
            Languages languages1 = new Languages() { Val = "en-US", EastAsia = "ja-JP", Bidi = "ar-SA" };

            runPropertiesBaseStyle1.Append(runFonts21);
            runPropertiesBaseStyle1.Append(kern1);
            runPropertiesBaseStyle1.Append(fontSize7);
            runPropertiesBaseStyle1.Append(fontSizeComplexScript7);
            runPropertiesBaseStyle1.Append(languages1);

            runPropertiesDefault1.Append(runPropertiesBaseStyle1);
            ParagraphPropertiesDefault paragraphPropertiesDefault1 = new ParagraphPropertiesDefault();

            docDefaults1.Append(runPropertiesDefault1);
            docDefaults1.Append(paragraphPropertiesDefault1);

            LatentStyles latentStyles1 = new LatentStyles() { DefaultLockedState = false, DefaultUiPriority = 99, DefaultSemiHidden = true, DefaultUnhideWhenUsed = true, DefaultPrimaryStyle = false, Count = 267 };
            LatentStyleExceptionInfo latentStyleExceptionInfo1 = new LatentStyleExceptionInfo() { Name = "Normal", UiPriority = 0, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo2 = new LatentStyleExceptionInfo() { Name = "heading 1", UiPriority = 9, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo3 = new LatentStyleExceptionInfo() { Name = "heading 2", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo4 = new LatentStyleExceptionInfo() { Name = "heading 3", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo5 = new LatentStyleExceptionInfo() { Name = "heading 4", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo6 = new LatentStyleExceptionInfo() { Name = "heading 5", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo7 = new LatentStyleExceptionInfo() { Name = "heading 6", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo8 = new LatentStyleExceptionInfo() { Name = "heading 7", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo9 = new LatentStyleExceptionInfo() { Name = "heading 8", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo10 = new LatentStyleExceptionInfo() { Name = "heading 9", UiPriority = 9, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo11 = new LatentStyleExceptionInfo() { Name = "toc 1", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo12 = new LatentStyleExceptionInfo() { Name = "toc 2", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo13 = new LatentStyleExceptionInfo() { Name = "toc 3", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo14 = new LatentStyleExceptionInfo() { Name = "toc 4", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo15 = new LatentStyleExceptionInfo() { Name = "toc 5", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo16 = new LatentStyleExceptionInfo() { Name = "toc 6", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo17 = new LatentStyleExceptionInfo() { Name = "toc 7", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo18 = new LatentStyleExceptionInfo() { Name = "toc 8", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo19 = new LatentStyleExceptionInfo() { Name = "toc 9", UiPriority = 39 };
            LatentStyleExceptionInfo latentStyleExceptionInfo20 = new LatentStyleExceptionInfo() { Name = "caption", UiPriority = 35, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo21 = new LatentStyleExceptionInfo() { Name = "Title", UiPriority = 10, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo22 = new LatentStyleExceptionInfo() { Name = "Default Paragraph Font", UiPriority = 1 };
            LatentStyleExceptionInfo latentStyleExceptionInfo23 = new LatentStyleExceptionInfo() { Name = "Subtitle", UiPriority = 11, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo24 = new LatentStyleExceptionInfo() { Name = "Strong", UiPriority = 22, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo25 = new LatentStyleExceptionInfo() { Name = "Emphasis", UiPriority = 20, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo26 = new LatentStyleExceptionInfo() { Name = "Table Grid", UiPriority = 59, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo27 = new LatentStyleExceptionInfo() { Name = "Placeholder Text", UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo28 = new LatentStyleExceptionInfo() { Name = "No Spacing", UiPriority = 1, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo29 = new LatentStyleExceptionInfo() { Name = "Light Shading", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo30 = new LatentStyleExceptionInfo() { Name = "Light List", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo31 = new LatentStyleExceptionInfo() { Name = "Light Grid", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo32 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo33 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo34 = new LatentStyleExceptionInfo() { Name = "Medium List 1", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo35 = new LatentStyleExceptionInfo() { Name = "Medium List 2", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo36 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo37 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo38 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo39 = new LatentStyleExceptionInfo() { Name = "Dark List", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo40 = new LatentStyleExceptionInfo() { Name = "Colorful Shading", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo41 = new LatentStyleExceptionInfo() { Name = "Colorful List", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo42 = new LatentStyleExceptionInfo() { Name = "Colorful Grid", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo43 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 1", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo44 = new LatentStyleExceptionInfo() { Name = "Light List Accent 1", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo45 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 1", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo46 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 1", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo47 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 1", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo48 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 1", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo49 = new LatentStyleExceptionInfo() { Name = "Revision", UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo50 = new LatentStyleExceptionInfo() { Name = "List Paragraph", UiPriority = 34, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo51 = new LatentStyleExceptionInfo() { Name = "Quote", UiPriority = 29, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo52 = new LatentStyleExceptionInfo() { Name = "Intense Quote", UiPriority = 30, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo53 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 1", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo54 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 1", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo55 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 1", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo56 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 1", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo57 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 1", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo58 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 1", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo59 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 1", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo60 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 1", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo61 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 2", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo62 = new LatentStyleExceptionInfo() { Name = "Light List Accent 2", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo63 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 2", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo64 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 2", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo65 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 2", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo66 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 2", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo67 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 2", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo68 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 2", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo69 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 2", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo70 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 2", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo71 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 2", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo72 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 2", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo73 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 2", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo74 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 2", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo75 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 3", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo76 = new LatentStyleExceptionInfo() { Name = "Light List Accent 3", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo77 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 3", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo78 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 3", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo79 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 3", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo80 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 3", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo81 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 3", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo82 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 3", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo83 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 3", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo84 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 3", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo85 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 3", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo86 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 3", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo87 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 3", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo88 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 3", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo89 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 4", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo90 = new LatentStyleExceptionInfo() { Name = "Light List Accent 4", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo91 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 4", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo92 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 4", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo93 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 4", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo94 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 4", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo95 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 4", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo96 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 4", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo97 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 4", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo98 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 4", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo99 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 4", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo100 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 4", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo101 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 4", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo102 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 4", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo103 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 5", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo104 = new LatentStyleExceptionInfo() { Name = "Light List Accent 5", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo105 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 5", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo106 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 5", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo107 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 5", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo108 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 5", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo109 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 5", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo110 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 5", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo111 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 5", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo112 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 5", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo113 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 5", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo114 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 5", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo115 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 5", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo116 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 5", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo117 = new LatentStyleExceptionInfo() { Name = "Light Shading Accent 6", UiPriority = 60, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo118 = new LatentStyleExceptionInfo() { Name = "Light List Accent 6", UiPriority = 61, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo119 = new LatentStyleExceptionInfo() { Name = "Light Grid Accent 6", UiPriority = 62, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo120 = new LatentStyleExceptionInfo() { Name = "Medium Shading 1 Accent 6", UiPriority = 63, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo121 = new LatentStyleExceptionInfo() { Name = "Medium Shading 2 Accent 6", UiPriority = 64, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo122 = new LatentStyleExceptionInfo() { Name = "Medium List 1 Accent 6", UiPriority = 65, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo123 = new LatentStyleExceptionInfo() { Name = "Medium List 2 Accent 6", UiPriority = 66, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo124 = new LatentStyleExceptionInfo() { Name = "Medium Grid 1 Accent 6", UiPriority = 67, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo125 = new LatentStyleExceptionInfo() { Name = "Medium Grid 2 Accent 6", UiPriority = 68, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo126 = new LatentStyleExceptionInfo() { Name = "Medium Grid 3 Accent 6", UiPriority = 69, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo127 = new LatentStyleExceptionInfo() { Name = "Dark List Accent 6", UiPriority = 70, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo128 = new LatentStyleExceptionInfo() { Name = "Colorful Shading Accent 6", UiPriority = 71, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo129 = new LatentStyleExceptionInfo() { Name = "Colorful List Accent 6", UiPriority = 72, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo130 = new LatentStyleExceptionInfo() { Name = "Colorful Grid Accent 6", UiPriority = 73, SemiHidden = false, UnhideWhenUsed = false };
            LatentStyleExceptionInfo latentStyleExceptionInfo131 = new LatentStyleExceptionInfo() { Name = "Subtle Emphasis", UiPriority = 19, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo132 = new LatentStyleExceptionInfo() { Name = "Intense Emphasis", UiPriority = 21, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo133 = new LatentStyleExceptionInfo() { Name = "Subtle Reference", UiPriority = 31, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo134 = new LatentStyleExceptionInfo() { Name = "Intense Reference", UiPriority = 32, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo135 = new LatentStyleExceptionInfo() { Name = "Book Title", UiPriority = 33, SemiHidden = false, UnhideWhenUsed = false, PrimaryStyle = true };
            LatentStyleExceptionInfo latentStyleExceptionInfo136 = new LatentStyleExceptionInfo() { Name = "Bibliography", UiPriority = 37 };
            LatentStyleExceptionInfo latentStyleExceptionInfo137 = new LatentStyleExceptionInfo() { Name = "TOC Heading", UiPriority = 39, PrimaryStyle = true };

            latentStyles1.Append(latentStyleExceptionInfo1);
            latentStyles1.Append(latentStyleExceptionInfo2);
            latentStyles1.Append(latentStyleExceptionInfo3);
            latentStyles1.Append(latentStyleExceptionInfo4);
            latentStyles1.Append(latentStyleExceptionInfo5);
            latentStyles1.Append(latentStyleExceptionInfo6);
            latentStyles1.Append(latentStyleExceptionInfo7);
            latentStyles1.Append(latentStyleExceptionInfo8);
            latentStyles1.Append(latentStyleExceptionInfo9);
            latentStyles1.Append(latentStyleExceptionInfo10);
            latentStyles1.Append(latentStyleExceptionInfo11);
            latentStyles1.Append(latentStyleExceptionInfo12);
            latentStyles1.Append(latentStyleExceptionInfo13);
            latentStyles1.Append(latentStyleExceptionInfo14);
            latentStyles1.Append(latentStyleExceptionInfo15);
            latentStyles1.Append(latentStyleExceptionInfo16);
            latentStyles1.Append(latentStyleExceptionInfo17);
            latentStyles1.Append(latentStyleExceptionInfo18);
            latentStyles1.Append(latentStyleExceptionInfo19);
            latentStyles1.Append(latentStyleExceptionInfo20);
            latentStyles1.Append(latentStyleExceptionInfo21);
            latentStyles1.Append(latentStyleExceptionInfo22);
            latentStyles1.Append(latentStyleExceptionInfo23);
            latentStyles1.Append(latentStyleExceptionInfo24);
            latentStyles1.Append(latentStyleExceptionInfo25);
            latentStyles1.Append(latentStyleExceptionInfo26);
            latentStyles1.Append(latentStyleExceptionInfo27);
            latentStyles1.Append(latentStyleExceptionInfo28);
            latentStyles1.Append(latentStyleExceptionInfo29);
            latentStyles1.Append(latentStyleExceptionInfo30);
            latentStyles1.Append(latentStyleExceptionInfo31);
            latentStyles1.Append(latentStyleExceptionInfo32);
            latentStyles1.Append(latentStyleExceptionInfo33);
            latentStyles1.Append(latentStyleExceptionInfo34);
            latentStyles1.Append(latentStyleExceptionInfo35);
            latentStyles1.Append(latentStyleExceptionInfo36);
            latentStyles1.Append(latentStyleExceptionInfo37);
            latentStyles1.Append(latentStyleExceptionInfo38);
            latentStyles1.Append(latentStyleExceptionInfo39);
            latentStyles1.Append(latentStyleExceptionInfo40);
            latentStyles1.Append(latentStyleExceptionInfo41);
            latentStyles1.Append(latentStyleExceptionInfo42);
            latentStyles1.Append(latentStyleExceptionInfo43);
            latentStyles1.Append(latentStyleExceptionInfo44);
            latentStyles1.Append(latentStyleExceptionInfo45);
            latentStyles1.Append(latentStyleExceptionInfo46);
            latentStyles1.Append(latentStyleExceptionInfo47);
            latentStyles1.Append(latentStyleExceptionInfo48);
            latentStyles1.Append(latentStyleExceptionInfo49);
            latentStyles1.Append(latentStyleExceptionInfo50);
            latentStyles1.Append(latentStyleExceptionInfo51);
            latentStyles1.Append(latentStyleExceptionInfo52);
            latentStyles1.Append(latentStyleExceptionInfo53);
            latentStyles1.Append(latentStyleExceptionInfo54);
            latentStyles1.Append(latentStyleExceptionInfo55);
            latentStyles1.Append(latentStyleExceptionInfo56);
            latentStyles1.Append(latentStyleExceptionInfo57);
            latentStyles1.Append(latentStyleExceptionInfo58);
            latentStyles1.Append(latentStyleExceptionInfo59);
            latentStyles1.Append(latentStyleExceptionInfo60);
            latentStyles1.Append(latentStyleExceptionInfo61);
            latentStyles1.Append(latentStyleExceptionInfo62);
            latentStyles1.Append(latentStyleExceptionInfo63);
            latentStyles1.Append(latentStyleExceptionInfo64);
            latentStyles1.Append(latentStyleExceptionInfo65);
            latentStyles1.Append(latentStyleExceptionInfo66);
            latentStyles1.Append(latentStyleExceptionInfo67);
            latentStyles1.Append(latentStyleExceptionInfo68);
            latentStyles1.Append(latentStyleExceptionInfo69);
            latentStyles1.Append(latentStyleExceptionInfo70);
            latentStyles1.Append(latentStyleExceptionInfo71);
            latentStyles1.Append(latentStyleExceptionInfo72);
            latentStyles1.Append(latentStyleExceptionInfo73);
            latentStyles1.Append(latentStyleExceptionInfo74);
            latentStyles1.Append(latentStyleExceptionInfo75);
            latentStyles1.Append(latentStyleExceptionInfo76);
            latentStyles1.Append(latentStyleExceptionInfo77);
            latentStyles1.Append(latentStyleExceptionInfo78);
            latentStyles1.Append(latentStyleExceptionInfo79);
            latentStyles1.Append(latentStyleExceptionInfo80);
            latentStyles1.Append(latentStyleExceptionInfo81);
            latentStyles1.Append(latentStyleExceptionInfo82);
            latentStyles1.Append(latentStyleExceptionInfo83);
            latentStyles1.Append(latentStyleExceptionInfo84);
            latentStyles1.Append(latentStyleExceptionInfo85);
            latentStyles1.Append(latentStyleExceptionInfo86);
            latentStyles1.Append(latentStyleExceptionInfo87);
            latentStyles1.Append(latentStyleExceptionInfo88);
            latentStyles1.Append(latentStyleExceptionInfo89);
            latentStyles1.Append(latentStyleExceptionInfo90);
            latentStyles1.Append(latentStyleExceptionInfo91);
            latentStyles1.Append(latentStyleExceptionInfo92);
            latentStyles1.Append(latentStyleExceptionInfo93);
            latentStyles1.Append(latentStyleExceptionInfo94);
            latentStyles1.Append(latentStyleExceptionInfo95);
            latentStyles1.Append(latentStyleExceptionInfo96);
            latentStyles1.Append(latentStyleExceptionInfo97);
            latentStyles1.Append(latentStyleExceptionInfo98);
            latentStyles1.Append(latentStyleExceptionInfo99);
            latentStyles1.Append(latentStyleExceptionInfo100);
            latentStyles1.Append(latentStyleExceptionInfo101);
            latentStyles1.Append(latentStyleExceptionInfo102);
            latentStyles1.Append(latentStyleExceptionInfo103);
            latentStyles1.Append(latentStyleExceptionInfo104);
            latentStyles1.Append(latentStyleExceptionInfo105);
            latentStyles1.Append(latentStyleExceptionInfo106);
            latentStyles1.Append(latentStyleExceptionInfo107);
            latentStyles1.Append(latentStyleExceptionInfo108);
            latentStyles1.Append(latentStyleExceptionInfo109);
            latentStyles1.Append(latentStyleExceptionInfo110);
            latentStyles1.Append(latentStyleExceptionInfo111);
            latentStyles1.Append(latentStyleExceptionInfo112);
            latentStyles1.Append(latentStyleExceptionInfo113);
            latentStyles1.Append(latentStyleExceptionInfo114);
            latentStyles1.Append(latentStyleExceptionInfo115);
            latentStyles1.Append(latentStyleExceptionInfo116);
            latentStyles1.Append(latentStyleExceptionInfo117);
            latentStyles1.Append(latentStyleExceptionInfo118);
            latentStyles1.Append(latentStyleExceptionInfo119);
            latentStyles1.Append(latentStyleExceptionInfo120);
            latentStyles1.Append(latentStyleExceptionInfo121);
            latentStyles1.Append(latentStyleExceptionInfo122);
            latentStyles1.Append(latentStyleExceptionInfo123);
            latentStyles1.Append(latentStyleExceptionInfo124);
            latentStyles1.Append(latentStyleExceptionInfo125);
            latentStyles1.Append(latentStyleExceptionInfo126);
            latentStyles1.Append(latentStyleExceptionInfo127);
            latentStyles1.Append(latentStyleExceptionInfo128);
            latentStyles1.Append(latentStyleExceptionInfo129);
            latentStyles1.Append(latentStyleExceptionInfo130);
            latentStyles1.Append(latentStyleExceptionInfo131);
            latentStyles1.Append(latentStyleExceptionInfo132);
            latentStyles1.Append(latentStyleExceptionInfo133);
            latentStyles1.Append(latentStyleExceptionInfo134);
            latentStyles1.Append(latentStyleExceptionInfo135);
            latentStyles1.Append(latentStyleExceptionInfo136);
            latentStyles1.Append(latentStyleExceptionInfo137);

            Style style1 = new Style() { Type = StyleValues.Paragraph, StyleId = "a", Default = true };
            StyleName styleName1 = new StyleName() { Val = "Normal" };
            PrimaryStyle primaryStyle1 = new PrimaryStyle();
            Rsid rsid4 = new Rsid() { Val = "00BF3A2C" };

            StyleParagraphProperties styleParagraphProperties1 = new StyleParagraphProperties();
            WidowControl widowControl1 = new WidowControl() { Val = false };
            Justification justification7 = new Justification() { Val = JustificationValues.Both };

            styleParagraphProperties1.Append(widowControl1);
            styleParagraphProperties1.Append(justification7);

            style1.Append(styleName1);
            style1.Append(primaryStyle1);
            style1.Append(rsid4);
            style1.Append(styleParagraphProperties1);

            Style style2 = new Style() { Type = StyleValues.Paragraph, StyleId = "1" };
            StyleName styleName2 = new StyleName() { Val = "heading 1" };
            BasedOn basedOn1 = new BasedOn() { Val = "a" };
            NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "a" };
            LinkedStyle linkedStyle1 = new LinkedStyle() { Val = "10" };
            UIPriority uIPriority1 = new UIPriority() { Val = 9 };
            PrimaryStyle primaryStyle2 = new PrimaryStyle();
            Rsid rsid5 = new Rsid() { Val = "004F3C1B" };

            StyleParagraphProperties styleParagraphProperties2 = new StyleParagraphProperties();
            KeepNext keepNext1 = new KeepNext();
            OutlineLevel outlineLevel1 = new OutlineLevel() { Val = 0 };

            styleParagraphProperties2.Append(keepNext1);
            styleParagraphProperties2.Append(outlineLevel1);

            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
            RunFonts runFonts22 = new RunFonts() { AsciiTheme = ThemeFontValues.MajorHighAnsi, HighAnsiTheme = ThemeFontValues.MajorHighAnsi, EastAsiaTheme = ThemeFontValues.MajorEastAsia, ComplexScriptTheme = ThemeFontValues.MajorBidi };
            FontSize fontSize8 = new FontSize() { Val = "24" };
            FontSizeComplexScript fontSizeComplexScript8 = new FontSizeComplexScript() { Val = "24" };

            styleRunProperties1.Append(runFonts22);
            styleRunProperties1.Append(fontSize8);
            styleRunProperties1.Append(fontSizeComplexScript8);

            style2.Append(styleName2);
            style2.Append(basedOn1);
            style2.Append(nextParagraphStyle1);
            style2.Append(linkedStyle1);
            style2.Append(uIPriority1);
            style2.Append(primaryStyle2);
            style2.Append(rsid5);
            style2.Append(styleParagraphProperties2);
            style2.Append(styleRunProperties1);

            Style style3 = new Style() { Type = StyleValues.Paragraph, StyleId = "2" };
            StyleName styleName3 = new StyleName() { Val = "heading 2" };
            BasedOn basedOn2 = new BasedOn() { Val = "a" };
            NextParagraphStyle nextParagraphStyle2 = new NextParagraphStyle() { Val = "a" };
            LinkedStyle linkedStyle2 = new LinkedStyle() { Val = "20" };
            UIPriority uIPriority2 = new UIPriority() { Val = 9 };
            UnhideWhenUsed unhideWhenUsed1 = new UnhideWhenUsed();
            PrimaryStyle primaryStyle3 = new PrimaryStyle();
            Rsid rsid6 = new Rsid() { Val = "002A2462" };

            StyleParagraphProperties styleParagraphProperties3 = new StyleParagraphProperties();
            KeepNext keepNext2 = new KeepNext();
            OutlineLevel outlineLevel2 = new OutlineLevel() { Val = 1 };

            styleParagraphProperties3.Append(keepNext2);
            styleParagraphProperties3.Append(outlineLevel2);

            StyleRunProperties styleRunProperties2 = new StyleRunProperties();
            RunFonts runFonts23 = new RunFonts() { AsciiTheme = ThemeFontValues.MajorHighAnsi, HighAnsiTheme = ThemeFontValues.MajorHighAnsi, EastAsiaTheme = ThemeFontValues.MajorEastAsia, ComplexScriptTheme = ThemeFontValues.MajorBidi };

            styleRunProperties2.Append(runFonts23);

            style3.Append(styleName3);
            style3.Append(basedOn2);
            style3.Append(nextParagraphStyle2);
            style3.Append(linkedStyle2);
            style3.Append(uIPriority2);
            style3.Append(unhideWhenUsed1);
            style3.Append(primaryStyle3);
            style3.Append(rsid6);
            style3.Append(styleParagraphProperties3);
            style3.Append(styleRunProperties2);

            Style style4 = new Style() { Type = StyleValues.Character, StyleId = "a0", Default = true };
            StyleName styleName4 = new StyleName() { Val = "Default Paragraph Font" };
            UIPriority uIPriority3 = new UIPriority() { Val = 1 };
            SemiHidden semiHidden1 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed2 = new UnhideWhenUsed();

            style4.Append(styleName4);
            style4.Append(uIPriority3);
            style4.Append(semiHidden1);
            style4.Append(unhideWhenUsed2);

            Style style5 = new Style() { Type = StyleValues.Table, StyleId = "a1", Default = true };
            StyleName styleName5 = new StyleName() { Val = "Normal Table" };
            UIPriority uIPriority4 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden2 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed3 = new UnhideWhenUsed();
            PrimaryStyle primaryStyle4 = new PrimaryStyle();

            StyleTableProperties styleTableProperties1 = new StyleTableProperties();
            TableIndentation tableIndentation1 = new TableIndentation() { Width = 0, Type = TableWidthUnitValues.Dxa };

            TableCellMarginDefault tableCellMarginDefault1 = new TableCellMarginDefault();
            TopMargin topMargin1 = new TopMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            TableCellLeftMargin tableCellLeftMargin1 = new TableCellLeftMargin() { Width = 108, Type = TableWidthValues.Dxa };
            BottomMargin bottomMargin1 = new BottomMargin() { Width = "0", Type = TableWidthUnitValues.Dxa };
            TableCellRightMargin tableCellRightMargin1 = new TableCellRightMargin() { Width = 108, Type = TableWidthValues.Dxa };

            tableCellMarginDefault1.Append(topMargin1);
            tableCellMarginDefault1.Append(tableCellLeftMargin1);
            tableCellMarginDefault1.Append(bottomMargin1);
            tableCellMarginDefault1.Append(tableCellRightMargin1);

            styleTableProperties1.Append(tableIndentation1);
            styleTableProperties1.Append(tableCellMarginDefault1);

            style5.Append(styleName5);
            style5.Append(uIPriority4);
            style5.Append(semiHidden2);
            style5.Append(unhideWhenUsed3);
            style5.Append(primaryStyle4);
            style5.Append(styleTableProperties1);

            Style style6 = new Style() { Type = StyleValues.Numbering, StyleId = "a2", Default = true };
            StyleName styleName6 = new StyleName() { Val = "No List" };
            UIPriority uIPriority5 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden3 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed4 = new UnhideWhenUsed();

            style6.Append(styleName6);
            style6.Append(uIPriority5);
            style6.Append(semiHidden3);
            style6.Append(unhideWhenUsed4);

            Style style7 = new Style() { Type = StyleValues.Paragraph, StyleId = "a3" };
            StyleName styleName7 = new StyleName() { Val = "header" };
            BasedOn basedOn3 = new BasedOn() { Val = "a" };
            LinkedStyle linkedStyle3 = new LinkedStyle() { Val = "a4" };
            UIPriority uIPriority6 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden4 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed5 = new UnhideWhenUsed();
            Rsid rsid7 = new Rsid() { Val = "004F3C1B" };

            StyleParagraphProperties styleParagraphProperties4 = new StyleParagraphProperties();

            Tabs tabs1 = new Tabs();
            TabStop tabStop1 = new TabStop() { Val = TabStopValues.Center, Position = 4252 };
            TabStop tabStop2 = new TabStop() { Val = TabStopValues.Right, Position = 8504 };

            tabs1.Append(tabStop1);
            tabs1.Append(tabStop2);
            SnapToGrid snapToGrid1 = new SnapToGrid() { Val = false };

            styleParagraphProperties4.Append(tabs1);
            styleParagraphProperties4.Append(snapToGrid1);

            style7.Append(styleName7);
            style7.Append(basedOn3);
            style7.Append(linkedStyle3);
            style7.Append(uIPriority6);
            style7.Append(semiHidden4);
            style7.Append(unhideWhenUsed5);
            style7.Append(rsid7);
            style7.Append(styleParagraphProperties4);

            Style style8 = new Style() { Type = StyleValues.Character, StyleId = "a4", CustomStyle = true };
            StyleName styleName8 = new StyleName() { Val = Resources.HeaderStyle }; //ヘッダー (文字)
            BasedOn basedOn4 = new BasedOn() { Val = "a0" };
            LinkedStyle linkedStyle4 = new LinkedStyle() { Val = "a3" };
            UIPriority uIPriority7 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden5 = new SemiHidden();
            Rsid rsid8 = new Rsid() { Val = "004F3C1B" };

            style8.Append(styleName8);
            style8.Append(basedOn4);
            style8.Append(linkedStyle4);
            style8.Append(uIPriority7);
            style8.Append(semiHidden5);
            style8.Append(rsid8);

            Style style9 = new Style() { Type = StyleValues.Paragraph, StyleId = "a5" };
            StyleName styleName9 = new StyleName() { Val = "footer" };
            BasedOn basedOn5 = new BasedOn() { Val = "a" };
            LinkedStyle linkedStyle5 = new LinkedStyle() { Val = "a6" };
            UIPriority uIPriority8 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden6 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed6 = new UnhideWhenUsed();
            Rsid rsid9 = new Rsid() { Val = "004F3C1B" };

            StyleParagraphProperties styleParagraphProperties5 = new StyleParagraphProperties();

            Tabs tabs2 = new Tabs();
            TabStop tabStop3 = new TabStop() { Val = TabStopValues.Center, Position = 4252 };
            TabStop tabStop4 = new TabStop() { Val = TabStopValues.Right, Position = 8504 };

            tabs2.Append(tabStop3);
            tabs2.Append(tabStop4);
            SnapToGrid snapToGrid2 = new SnapToGrid() { Val = false };

            styleParagraphProperties5.Append(tabs2);
            styleParagraphProperties5.Append(snapToGrid2);

            style9.Append(styleName9);
            style9.Append(basedOn5);
            style9.Append(linkedStyle5);
            style9.Append(uIPriority8);
            style9.Append(semiHidden6);
            style9.Append(unhideWhenUsed6);
            style9.Append(rsid9);
            style9.Append(styleParagraphProperties5);

            Style style10 = new Style() { Type = StyleValues.Character, StyleId = "a6", CustomStyle = true };
            StyleName styleName10 = new StyleName() { Val = Resources.FooterStyle }; //フッター (文字)
            BasedOn basedOn6 = new BasedOn() { Val = "a0" };
            LinkedStyle linkedStyle6 = new LinkedStyle() { Val = "a5" };
            UIPriority uIPriority9 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden7 = new SemiHidden();
            Rsid rsid10 = new Rsid() { Val = "004F3C1B" };

            style10.Append(styleName10);
            style10.Append(basedOn6);
            style10.Append(linkedStyle6);
            style10.Append(uIPriority9);
            style10.Append(semiHidden7);
            style10.Append(rsid10);

            Style style11 = new Style() { Type = StyleValues.Character, StyleId = "10", CustomStyle = true };
            StyleName styleName11 = new StyleName() { Val = Resources.HeaderStyle }; //見出し 1 (文字)
            BasedOn basedOn7 = new BasedOn() { Val = "a0" };
            LinkedStyle linkedStyle7 = new LinkedStyle() { Val = "1" };
            UIPriority uIPriority10 = new UIPriority() { Val = 9 };
            Rsid rsid11 = new Rsid() { Val = "004F3C1B" };

            StyleRunProperties styleRunProperties3 = new StyleRunProperties();
            RunFonts runFonts24 = new RunFonts() { AsciiTheme = ThemeFontValues.MajorHighAnsi, HighAnsiTheme = ThemeFontValues.MajorHighAnsi, EastAsiaTheme = ThemeFontValues.MajorEastAsia, ComplexScriptTheme = ThemeFontValues.MajorBidi };
            FontSize fontSize9 = new FontSize() { Val = "24" };
            FontSizeComplexScript fontSizeComplexScript9 = new FontSizeComplexScript() { Val = "24" };

            styleRunProperties3.Append(runFonts24);
            styleRunProperties3.Append(fontSize9);
            styleRunProperties3.Append(fontSizeComplexScript9);

            style11.Append(styleName11);
            style11.Append(basedOn7);
            style11.Append(linkedStyle7);
            style11.Append(uIPriority10);
            style11.Append(rsid11);
            style11.Append(styleRunProperties3);

            Style style12 = new Style() { Type = StyleValues.Paragraph, StyleId = "a7" };
            StyleName styleName12 = new StyleName() { Val = "Document Map" };
            BasedOn basedOn8 = new BasedOn() { Val = "a" };
            LinkedStyle linkedStyle8 = new LinkedStyle() { Val = "a8" };
            UIPriority uIPriority11 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden8 = new SemiHidden();
            UnhideWhenUsed unhideWhenUsed7 = new UnhideWhenUsed();
            Rsid rsid12 = new Rsid() { Val = "002A2462" };

            StyleRunProperties styleRunProperties4 = new StyleRunProperties();
            RunFonts runFonts25 = new RunFonts() { Ascii = "MS UI Gothic", EastAsia = "MS UI Gothic" };
            FontSize fontSize10 = new FontSize() { Val = "18" };
            FontSizeComplexScript fontSizeComplexScript10 = new FontSizeComplexScript() { Val = "18" };

            styleRunProperties4.Append(runFonts25);
            styleRunProperties4.Append(fontSize10);
            styleRunProperties4.Append(fontSizeComplexScript10);

            style12.Append(styleName12);
            style12.Append(basedOn8);
            style12.Append(linkedStyle8);
            style12.Append(uIPriority11);
            style12.Append(semiHidden8);
            style12.Append(unhideWhenUsed7);
            style12.Append(rsid12);
            style12.Append(styleRunProperties4);

            Style style13 = new Style() { Type = StyleValues.Character, StyleId = "a8", CustomStyle = true };
            StyleName styleName13 = new StyleName() { Val = Resources.HeadingMap }; // 見出しマップ (文字)
            BasedOn basedOn9 = new BasedOn() { Val = "a0" };
            LinkedStyle linkedStyle9 = new LinkedStyle() { Val = "a7" };
            UIPriority uIPriority12 = new UIPriority() { Val = 99 };
            SemiHidden semiHidden9 = new SemiHidden();
            Rsid rsid13 = new Rsid() { Val = "002A2462" };

            StyleRunProperties styleRunProperties5 = new StyleRunProperties();
            RunFonts runFonts26 = new RunFonts() { Ascii = "MS UI Gothic", EastAsia = "MS UI Gothic" };
            FontSize fontSize11 = new FontSize() { Val = "18" };
            FontSizeComplexScript fontSizeComplexScript11 = new FontSizeComplexScript() { Val = "18" };

            styleRunProperties5.Append(runFonts26);
            styleRunProperties5.Append(fontSize11);
            styleRunProperties5.Append(fontSizeComplexScript11);

            style13.Append(styleName13);
            style13.Append(basedOn9);
            style13.Append(linkedStyle9);
            style13.Append(uIPriority12);
            style13.Append(semiHidden9);
            style13.Append(rsid13);
            style13.Append(styleRunProperties5);

            Style style14 = new Style() { Type = StyleValues.Character, StyleId = "20", CustomStyle = true };
            StyleName styleName14 = new StyleName() { Val = Resources.Heading2 }; //見出し 2 (文字)
            BasedOn basedOn10 = new BasedOn() { Val = "a0" };
            LinkedStyle linkedStyle10 = new LinkedStyle() { Val = "2" };
            UIPriority uIPriority13 = new UIPriority() { Val = 9 };
            Rsid rsid14 = new Rsid() { Val = "002A2462" };

            StyleRunProperties styleRunProperties6 = new StyleRunProperties();
            RunFonts runFonts27 = new RunFonts() { AsciiTheme = ThemeFontValues.MajorHighAnsi, HighAnsiTheme = ThemeFontValues.MajorHighAnsi, EastAsiaTheme = ThemeFontValues.MajorEastAsia, ComplexScriptTheme = ThemeFontValues.MajorBidi };

            styleRunProperties6.Append(runFonts27);

            style14.Append(styleName14);
            style14.Append(basedOn10);
            style14.Append(linkedStyle10);
            style14.Append(uIPriority13);
            style14.Append(rsid14);
            style14.Append(styleRunProperties6);

            Style style15 = new Style() { Type = StyleValues.Paragraph, StyleId = "a9" };
            StyleName styleName15 = new StyleName() { Val = "Title" };
            BasedOn basedOn11 = new BasedOn() { Val = "a" };
            NextParagraphStyle nextParagraphStyle3 = new NextParagraphStyle() { Val = "a" };
            LinkedStyle linkedStyle11 = new LinkedStyle() { Val = "aa" };
            UIPriority uIPriority14 = new UIPriority() { Val = 10 };
            PrimaryStyle primaryStyle5 = new PrimaryStyle();
            Rsid rsid15 = new Rsid() { Val = "00586494" };

            StyleParagraphProperties styleParagraphProperties6 = new StyleParagraphProperties();
            SpacingBetweenLines spacingBetweenLines1 = new SpacingBetweenLines() { Before = "240", After = "120" };
            Justification justification8 = new Justification() { Val = JustificationValues.Center };
            OutlineLevel outlineLevel3 = new OutlineLevel() { Val = 0 };

            styleParagraphProperties6.Append(spacingBetweenLines1);
            styleParagraphProperties6.Append(justification8);
            styleParagraphProperties6.Append(outlineLevel3);

            StyleRunProperties styleRunProperties7 = new StyleRunProperties();
            //ＭＳ ゴシック
            RunFonts runFonts28 = new RunFonts() { EastAsia = Resources.Font1, AsciiTheme = ThemeFontValues.MajorHighAnsi, HighAnsiTheme = ThemeFontValues.MajorHighAnsi, ComplexScriptTheme = ThemeFontValues.MajorBidi };
            FontSize fontSize12 = new FontSize() { Val = "32" };
            FontSizeComplexScript fontSizeComplexScript12 = new FontSizeComplexScript() { Val = "32" };

            styleRunProperties7.Append(runFonts28);
            styleRunProperties7.Append(fontSize12);
            styleRunProperties7.Append(fontSizeComplexScript12);

            style15.Append(styleName15);
            style15.Append(basedOn11);
            style15.Append(nextParagraphStyle3);
            style15.Append(linkedStyle11);
            style15.Append(uIPriority14);
            style15.Append(primaryStyle5);
            style15.Append(rsid15);
            style15.Append(styleParagraphProperties6);
            style15.Append(styleRunProperties7);

            Style style16 = new Style() { Type = StyleValues.Character, StyleId = "aa", CustomStyle = true };
            //表題 (文字)
            StyleName styleName16 = new StyleName() { Val = Resources.TitleStyle };
            BasedOn basedOn12 = new BasedOn() { Val = "a0" };
            LinkedStyle linkedStyle12 = new LinkedStyle() { Val = "a9" };
            UIPriority uIPriority15 = new UIPriority() { Val = 10 };
            Rsid rsid16 = new Rsid() { Val = "00586494" };

            StyleRunProperties styleRunProperties8 = new StyleRunProperties();
            //ＭＳ ゴシック
            RunFonts runFonts29 = new RunFonts() { EastAsia = Resources.Font1, AsciiTheme = ThemeFontValues.MajorHighAnsi, HighAnsiTheme = ThemeFontValues.MajorHighAnsi, ComplexScriptTheme = ThemeFontValues.MajorBidi };
            FontSize fontSize13 = new FontSize() { Val = "32" };
            FontSizeComplexScript fontSizeComplexScript13 = new FontSizeComplexScript() { Val = "32" };

            styleRunProperties8.Append(runFonts29);
            styleRunProperties8.Append(fontSize13);
            styleRunProperties8.Append(fontSizeComplexScript13);

            style16.Append(styleName16);
            style16.Append(basedOn12);
            style16.Append(linkedStyle12);
            style16.Append(uIPriority15);
            style16.Append(rsid16);
            style16.Append(styleRunProperties8);

            styles1.Append(docDefaults1);
            styles1.Append(latentStyles1);
            styles1.Append(style1);
            styles1.Append(style2);
            styles1.Append(style3);
            styles1.Append(style4);
            styles1.Append(style5);
            styles1.Append(style6);
            styles1.Append(style7);
            styles1.Append(style8);
            styles1.Append(style9);
            styles1.Append(style10);
            styles1.Append(style11);
            styles1.Append(style12);
            styles1.Append(style13);
            styles1.Append(style14);
            styles1.Append(style15);
            styles1.Append(style16);
            styles1.Append(GenerateGridTableStyle());
            styleDefinitionsPart1.Styles = styles1;
        }

        // documentSettingsPart1 のコンテンツを生成します。
        protected void GenerateDocumentSettingsPart1Content(DocumentSettingsPart documentSettingsPart1)
        {
            DocumentFormat.OpenXml.Wordprocessing.Settings settings1 = new DocumentFormat.OpenXml.Wordprocessing.Settings();

            BordersDoNotSurroundHeader bordersDoNotSurroundHeader1 = new BordersDoNotSurroundHeader();
            BordersDoNotSurroundFooter bordersDoNotSurroundFooter1 = new BordersDoNotSurroundFooter();
            ProofState proofState1 = new ProofState() { Spelling = ProofingStateValues.Clean, Grammar = ProofingStateValues.Dirty };
            DefaultTabStop defaultTabStop1 = new DefaultTabStop() { Val = 840 };
            DisplayHorizontalDrawingGrid displayHorizontalDrawingGrid1 = new DisplayHorizontalDrawingGrid() { Val = 0 };
            DisplayVerticalDrawingGrid displayVerticalDrawingGrid1 = new DisplayVerticalDrawingGrid() { Val = 2 };
            CharacterSpacingControl characterSpacingControl1 = new CharacterSpacingControl() { Val = CharacterSpacingValues.CompressPunctuation };

            Compatibility compatibility1 = new Compatibility();
            SpaceForUnderline spaceForUnderline1 = new SpaceForUnderline();
            BalanceSingleByteDoubleByteWidth balanceSingleByteDoubleByteWidth1 = new BalanceSingleByteDoubleByteWidth();
            DoNotLeaveBackslashAlone doNotLeaveBackslashAlone1 = new DoNotLeaveBackslashAlone();
            UnderlineTrailingSpaces underlineTrailingSpaces1 = new UnderlineTrailingSpaces();
            DoNotExpandShiftReturn doNotExpandShiftReturn1 = new DoNotExpandShiftReturn();
            AdjustLineHeightInTable adjustLineHeightInTable1 = new AdjustLineHeightInTable();
            UseFarEastLayout useFarEastLayout1 = new UseFarEastLayout();

            compatibility1.Append(spaceForUnderline1);
            compatibility1.Append(balanceSingleByteDoubleByteWidth1);
            compatibility1.Append(doNotLeaveBackslashAlone1);
            compatibility1.Append(underlineTrailingSpaces1);
            compatibility1.Append(doNotExpandShiftReturn1);
            compatibility1.Append(adjustLineHeightInTable1);
            compatibility1.Append(useFarEastLayout1);
            settings1.Append(compatibility1);
            documentSettingsPart1.Settings = settings1;
        }

        protected abstract void WriteBody(Body body);

        public void Write(string path)
        {
            ///// 調査票の書き出しメソッド
            try
            {
                using (var package = WordprocessingDocument.Create(
                    path, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainDocumentPart = package.AddMainDocumentPart();
                    DocumentSettingsPart documentSettingsPart = mainDocumentPart.AddNewPart<DocumentSettingsPart>("rId2");
                    GenerateDocumentSettingsPart1Content(documentSettingsPart);
                    StyleDefinitionsPart styleDefinitionsPart1 = mainDocumentPart.AddNewPart<StyleDefinitionsPart>("rId1");
                    GenerateStyleDefinitionsPart1Content3(styleDefinitionsPart1);

                    Document document = new Document();
                    Body body = new Body();
                    WriteBody(body);
                    document.Append(body);
                    mainDocumentPart.Document = document;
                }
                // 書き出しに成功しました。表示しますか?  書き出し完了
                if (System.Windows.MessageBox.Show(Resources.WriteSuccess, Resources.WriteFinish, System.Windows.MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
                {
                    PreviewWord(path);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(Resources.WriteError + ex.Message);
            }
        }


    }
}
