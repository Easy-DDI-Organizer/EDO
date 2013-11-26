
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows;
using System.Diagnostics;
using EDO.Main;
using EDO.QuestionCategory.SequenceForm;
using EDO.QuestionCategory.QuestionForm;
using EDO.QuestionCategory.CodeForm;
using System.Collections.ObjectModel;


using Ovml = DocumentFormat.OpenXml.Vml.Office;
using V = DocumentFormat.OpenXml.Vml;
using EDO.QuestionCategory.QuestionGroupForm;
using EDO.Properties;

namespace EDO.Core.IO
{
    // 調査票出力
    public class QuestionnaireWriter :DocxWriter
    {
        #region 静的メソッド
        private const int BOX_CELL_WIDTH = 446;
        private const int HALF_BOX_CELL_WIDTH = 223;
        private const int TEXT_WIDTH = 210;
        private const int TEXT_MARGIN = 280;

        private static string CreateEmptyString(int spaceLength)
        {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < spaceLength; i++)
            {
                buf.Append(Resources.Space);
            }
            return buf.ToString();
        }

        private static int GetRealColumnCount(int? columnCount)
        {
            //それ以外はアンダーライン
            int minColumnCount = 1;
            //自由回答方式でマックスがデフォルト長を上回っている場合それを使う
            int realColumnCount = minColumnCount;
            if (columnCount.HasValue && columnCount > minColumnCount)
            {
                realColumnCount = (int)columnCount;
            }
            return realColumnCount;
        }

        private static int GetRealRowCount(int? rowCount)
        {
            //それ以外はアンダーライン
            int minRowCount = 1;
            //自由回答方式でマックスがデフォルト長を上回っている場合それを使う
            int realRowCount = minRowCount;
            if (rowCount.HasValue && rowCount > minRowCount)
            {
                realRowCount = (int)rowCount;
            }
            return realRowCount;
        }

        private static int CalcBoxCellWidth(int columnCount)
        {
            return BOX_CELL_WIDTH * columnCount;
        }

        private static int CalcHalfBoxCellWidth(int columnCount)
        {
            return HALF_BOX_CELL_WIDTH * columnCount;
        }

        private static string ToPx(double val)
        {
            return val + "pt";
        }

        #endregion

        #region コンストラクタとプライベートメソッド

        private EDOConfig config;
        private ControlConstructSchemeVM controlConstructScheme;
        public QuestionnaireWriter(EDOConfig config, ControlConstructSchemeVM controlConstructScheme)
        {
            this.config = config;
            this.controlConstructScheme = controlConstructScheme;
        }

        private int CalcTextWidth(int textLength)
        {
            double textWidth = TEXT_WIDTH;
            double textMargin = TEXT_MARGIN;
            if (config.IsLanguageEn)
            {
                textWidth /= 2.0;
                textMargin /= 2.0;
            }
            return (int)(textWidth * (double)textLength + textMargin);
        }

        private Table CreateDefaultTable()
        {
            //枠線有りテーブルを生成する(幅は自動)
            Table table = new Table();
            TableProperties tableProperties = new TableProperties();
            TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };
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
            return table;
        }

        private Table CreateFilledTable()
        {
            //枠線有りテーブルを生成する(幅はページの幅)
            Table table = new Table();
            TableProperties tableProperties = new TableProperties();
            TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };
            TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };
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
            return table;
        }

        private Table CreateFilledTable(bool hasBorder, bool hasInsideBorder)
        {
            //枠線有りテーブルを生成する(幅はページの幅)
            Table table = new Table();
            TableProperties tableProperties = new TableProperties();
            TableStyle tableStyle = new TableStyle() { Val = "TableGrid" };
            TableWidth tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };
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

            TableBorders borders = new TableBorders();
            tableProperties.Append(borders);

            EnumValue<BorderValues> val = hasBorder ? BorderValues.Single : BorderValues.Nil;
            TopBorder topBorder = new TopBorder() { Val = val };
            BottomBorder bottomBorder = new BottomBorder() { Val = val };
            LeftBorder leftBorder = new LeftBorder() { Val = val };
            RightBorder rightBorder = new RightBorder() { Val = val };
            borders.Append(topBorder);
            borders.Append(bottomBorder);
            borders.Append(leftBorder);
            borders.Append(rightBorder);

            val = hasInsideBorder ? BorderValues.Single : BorderValues.Nil;
            InsideHorizontalBorder horizontalBorder = new InsideHorizontalBorder() { Val = val };
            InsideVerticalBorder verticalBorder = new InsideVerticalBorder() { Val = val };
            borders.Append(horizontalBorder);
            borders.Append(verticalBorder);


            table.Append(tableProperties);
            return table;
        }

        private Table CreateTable(int? rowCount, int? columnCount)
        {
            //枠線、罫線ありの表を指定の行列分生成する
            int realRowCount = GetRealRowCount(rowCount);
            int realColumnCount = GetRealColumnCount(columnCount);
            Table table = CreateDefaultTable();
            for (int r = 0; r < realRowCount; r++)
            {
                TableRow tableRow = new TableRow();
                table.Append(tableRow);

                for (int c = 0; c < realColumnCount; c++)
                {
                    TableCellProperties tableCellProperties = new TableCellProperties();
                    TableCellWidth tableCellWidth = new TableCellWidth() { Width = BOX_CELL_WIDTH.ToString(), Type = TableWidthUnitValues.Dxa };
                    tableCellProperties.Append(tableCellWidth);

                    TableCell tableCell = new TableCell();
                    tableCell.Append(tableCellProperties);
                    tableCell.Append(new Paragraph());
                    tableRow.Append(tableCell);
                }
            }
            return table;
        }

        private TableCell CreateUnderlineCell(int cellWidth)
        {
            TableCellProperties tableCellProperties8 = new TableCellProperties();
            TableCellWidth tableCellWidth8 = new TableCellWidth() { Width = cellWidth.ToString(), Type = TableWidthUnitValues.Dxa };

            TableCellBorders tableCellBorders1 = new TableCellBorders();
            TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Nil };
            LeftBorder leftBorder1 = new LeftBorder() { Val = BorderValues.Nil };
            BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)2U, Space = (UInt32Value)0U };
            RightBorder rightBorder1 = new RightBorder() { Val = BorderValues.Nil };

            tableCellBorders1.Append(topBorder1);
            tableCellBorders1.Append(leftBorder1);
            tableCellBorders1.Append(bottomBorder1);
            tableCellBorders1.Append(rightBorder1);

            tableCellProperties8.Append(tableCellWidth8);
            tableCellProperties8.Append(tableCellBorders1);

            TableCell tableCell = new TableCell();
            tableCell.Append(tableCellProperties8);
            tableCell.Append(CreateEmptyParagraph());
            return tableCell;
        }

        private TableCell CreateBorderCell(int cellWidth)
        {
            //ボーダーつきのセルを生成する(幅指定あり)
            TableCellProperties tableCellProperties = new TableCellProperties();
            EnumValue<TableWidthUnitValues> type = cellWidth == 0 ? TableWidthUnitValues.Auto : TableWidthUnitValues.Dxa;
            TableCellWidth tableCellWidth = new TableCellWidth() { Width = cellWidth.ToString(), Type = type };
            tableCellProperties.Append(tableCellWidth);
            TableCell tableCell = new TableCell();
            tableCell.Append(tableCellProperties);
            tableCell.Append(CreateEmptyParagraph());
            return tableCell;
        }

        private TableCell CreateBorderCell(string text)
        {
            int cellWidth = CalcTextWidth(text.Length);
            TableCellProperties tableCellProperties = new TableCellProperties();
            EnumValue<TableWidthUnitValues> type = cellWidth == 0 ? TableWidthUnitValues.Auto : TableWidthUnitValues.Dxa;
            TableCellWidth tableCellWidth = new TableCellWidth() { Width = cellWidth.ToString(), Type = type };
            tableCellProperties.Append(tableCellWidth);
            TableCell tableCell = new TableCell();
            tableCell.Append(tableCellProperties);
            tableCell.Append(CreateTextParagraph(text));
            return tableCell;
        }

        private List<TableCell> CreateBorderCells(int count, int cellWidth)
        {
            List<TableCell> tableCells = new List<TableCell>();
            for (int i = 0; i < count; i++)
            {
                TableCell cell = CreateBorderCell(cellWidth);
                tableCells.Add(cell);
            }
            return tableCells;
        }

        private TableCell CreateNoBorderCell(string text)
        {
            TableCellProperties tableCellProperties = new TableCellProperties();
            int cellWidth = CalcTextWidth(text.Length);

            TableCellWidth tableCellWidth = new TableCellWidth() { Width = cellWidth.ToString(), Type = TableWidthUnitValues.Dxa };
            TableCellBorders tableCellBorders1 = new TableCellBorders();
            TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Nil };
            LeftBorder leftBorder1 = new LeftBorder() { Val = BorderValues.Nil };
            BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Nil };
            RightBorder rightBorder1 = new RightBorder() { Val = BorderValues.Nil };

            tableCellBorders1.Append(topBorder1);
            tableCellBorders1.Append(leftBorder1);
            tableCellBorders1.Append(bottomBorder1);
            tableCellBorders1.Append(rightBorder1);

            tableCellProperties.Append(tableCellWidth);
            tableCellProperties.Append(tableCellBorders1);

            TableCell tableCell = new TableCell();
            tableCell.Append(tableCellProperties);
            tableCell.Append(CreateTextParagraph(text));
            return tableCell;
        }

        private TableCell CreateLastNoBorderCell()
        {
            TableCellProperties tableCellProperties = new TableCellProperties();

            TableCellWidth tableCellWidth = new TableCellWidth() { Type = TableWidthUnitValues.Auto };
            TableCellBorders tableCellBorders1 = new TableCellBorders();
            TopBorder topBorder1 = new TopBorder() { Val = BorderValues.Nil };
            LeftBorder leftBorder1 = new LeftBorder() { Val = BorderValues.Nil };
            BottomBorder bottomBorder1 = new BottomBorder() { Val = BorderValues.Nil };
            RightBorder rightBorder1 = new RightBorder() { Val = BorderValues.Nil };

            tableCellBorders1.Append(topBorder1);
            tableCellBorders1.Append(leftBorder1);
            tableCellBorders1.Append(bottomBorder1);
            tableCellBorders1.Append(rightBorder1);

            tableCellProperties.Append(tableCellWidth);
            tableCellProperties.Append(tableCellBorders1);

            TableCell tableCell = new TableCell();
            tableCell.Append(tableCellProperties);
            tableCell.Append(CreateEmptyParagraph());
            return tableCell;
        }

        private Paragraph CreateCenterParagraph(string str)
        {
            Paragraph paragraph = new Paragraph();
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            paragraph.Append(paragraphProperties);
            paragraphProperties.Append(new Justification() { Val = JustificationValues.Center });
            Run run = new Run();
            RunProperties runProperties = new RunProperties();
            RunFonts runFonts = new RunFonts() { Hint = FontTypeHintValues.EastAsia };
            runProperties.Append(runFonts);
            Text text = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text.Text = str;
            run.Append(runProperties);
            run.Append(text);
            paragraph.Append(run);
            return paragraph;
        }

        private TableCell CreateCenterCell(string content)
        {
            List<string> contents = new List<string>();
            contents.Add(content);
            return CreateCenterCell(contents);
        }

        private TableCell CreateCenterCell(List<string> contents)
        {
            TableCellProperties tableCellProperties = new TableCellProperties();
            //セルの幅の均等割はTableWidthUnitValues.Autoじゃなく、TableWidthUnitValues.Nilを使うのが正解?
            TableCellWidth tableCellWidth = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Nil };
            tableCellProperties.Append(tableCellWidth);

            TableCell tableCell = new TableCell();
            tableCell.Append(tableCellProperties);

            foreach (string content in contents)
            {
                tableCell.Append(CreateCenterParagraph(content));
            }

            return tableCell;
        }

        private Paragraph CreateTextParagraph(string str)
        {
            Paragraph paragraph = new Paragraph() {};           

            Run run = new Run();
            RunProperties runProperties = new RunProperties();
            RunFonts runFonts = new RunFonts() { Hint = FontTypeHintValues.EastAsia };

            runProperties.Append(runFonts);
            Text text = new Text() { Space = SpaceProcessingModeValues.Preserve };
            text.Text = str;

            run.Append(runProperties);
            run.Append(text);

            paragraph.Append(run);
            return paragraph;
        }


        #endregion

        #region 自由回答

        private Table CreateFreeResponseOfUnderline(QuestionVM question)
        {
            ResponseVM response = question.Response;
            FreeLayoutVM freeLayout = (FreeLayoutVM)response.Layout;

            //枠線なし、横罫線だけの表を作る。
            int realRowCount = GetRealRowCount(freeLayout.RowCount);
            int realColumnCount = GetRealColumnCount(freeLayout.ColumnCount);

            Table table = CreateDefaultTable();
            int cellWidth = CalcBoxCellWidth(realColumnCount);
            for (int r = 0; r < realRowCount; r++)
            {
                //各行にセルをつくりそのボーダーの上左右を線なしに設定する。
                TableRow tableRow = new TableRow();
                table.Append(tableRow);
                TableCell tableCell = CreateUnderlineCell(cellWidth);
                tableRow.Append(tableCell);

            }
            return table;
        }

        private Table CreateFreeResponseOfBox(QuestionVM question)
        {
            ResponseVM response = question.Response;
            FreeLayoutVM freeLayout = (FreeLayoutVM)response.Layout;

            int realRowCount = GetRealRowCount(freeLayout.RowCount);
            int realColumnCount = GetRealColumnCount(freeLayout.ColumnCount);

            return CreateTable(realRowCount, realColumnCount);
        }

        private Table CreateFreeResponseOfTextbox(QuestionVM question)
        {
            ResponseVM response = question.Response;
            FreeLayoutVM freeLayout = (FreeLayoutVM)response.Layout;

            //枠線、罫線ありの表を生成する。
            int realRowCount = GetRealRowCount(freeLayout.RowCount);
            int realColumnCount = GetRealColumnCount(freeLayout.ColumnCount);

            Table table = CreateDefaultTable();

            TableRow tableRow = new TableRow();
            table.Append(tableRow);

            //1つのセルを作成
            int cellWidth = CalcBoxCellWidth(realColumnCount);
            TableCell tableCell = CreateBorderCell(cellWidth);
            tableRow.Append(tableCell);
            for (int r = 0; r < realRowCount - 1; r++) //一行追加済みなのでその分へらす
            {
                //その中に行数分のパラグラフ
                tableCell.Append(CreateEmptyParagraph());

            }
            return table;
        }

        private void WriteFreeResponse(Body body, QuestionVM question)
        {
            ResponseVM response = question.Response;
            FreeLayoutVM freeLayout = (FreeLayoutVM)response.Layout;

            if (freeLayout.Style == LayoutStyle.Underline)
            {
                //アンダーライン(各セルにアンダーラインのある表)
                Table table = CreateFreeResponseOfUnderline(question);
                body.Append(table);
            }
            else if (freeLayout.Style == LayoutStyle.Box)
            {
                //マス(普通の表)
                Table table = CreateFreeResponseOfBox(question);
                body.Append(table);
            }
            else if (freeLayout.Style == LayoutStyle.Textbox)
            {
                //テキストボックス(罫線なしの表)
                Table table = CreateFreeResponseOfTextbox(question);
                body.Append(table);
            }
        }

        #endregion

        #region 日付と時間

        private bool HasYearPrefix(DateTimeLayoutVM dateTimeLayout)
        {
            if (dateTimeLayout.CalendarEra == DateTimeLayoutCalendarEra.Japanese ||
                dateTimeLayout.CalendarEra == DateTimeLayoutCalendarEra.Western)
            {
                return true;
            }
            return false;
        }

        private bool HasMonth(ResponseVM response)
        {
            string code = response.DetailTypeCode;
            if (Options.IsDateTimeTypeDateTime(code) ||
                Options.IsDateTimeTypeDate(code) ||
                Options.IsDateTimeTypeYearMonth(code) ||
                Options.IsDateTimeTypeDuration(code)
                )
            {
                return true;
            }
            return false;
        }

        private bool HasDay(ResponseVM response)
        {
            string code = response.DetailTypeCode;
            if (Options.IsDateTimeTypeDateTime(code) ||
                Options.IsDateTimeTypeDate(code) ||
                Options.IsDateTimeTypeDuration(code)
                )
            {
                return true;
            }
            return false;
        }

        private bool HasTime(ResponseVM response)
        {
            string code = response.DetailTypeCode;
            if (Options.IsDateTimeTypeDateTime(code) 
                )
            {
                return true;
            }
            return false;
        }

        private bool HasRange(ResponseVM response)
        {
            string code = response.DetailTypeCode;
            if (Options.IsDateTimeTypeDuration(code)
                )
            {
                return true;
            }
            return false;
        }

        private List<TableCell> CreateNumberInputCells(LayoutStyle style, int count)
        {
            List<TableCell> cells = new List<TableCell>();
            if (style == LayoutStyle.Underline)
            {
                TableCell cell = CreateUnderlineCell(CalcHalfBoxCellWidth(count));
                cells.Add(cell);
            }
            else if (style == LayoutStyle.Textbox)
            {
                TableCell cell = CreateBorderCell(CalcHalfBoxCellWidth(count));
                cells.Add(cell);
            }
            else if (style == LayoutStyle.Box)
            {
                cells = CreateBorderCells(count, CalcHalfBoxCellWidth(1));
            }
            return cells;
        }

        private TableRow CreateDateTimeResponseRowJa(QuestionVM question, string prefix)
        {
            ResponseVM response = question.Response;
            DateTimeLayoutVM dateTimeLayout = (DateTimeLayoutVM)response.Layout; 
            
            TableRow tableRow = new TableRow();

            //年の入力欄の前の文字
            if (HasYearPrefix(dateTimeLayout))
            {
                string text = dateTimeLayout.CalendarEra == DateTimeLayoutCalendarEra.Japanese ? Resources.JapaneseEraTSH : Resources.WesternEra;

                //各行にセルをつくりそのボーダーの上左右を線なしに設定する。
                TableCell calendarEraCell = CreateNoBorderCell(text);
                tableRow.Append(calendarEraCell);
            }

            //年の入力欄
            List<TableCell> cells = CreateNumberInputCells(dateTimeLayout.Style, 4);
            tableRow.Append(cells);

            //年の入力欄の後の文字
            TableCell yearLabelCell = CreateNoBorderCell(Resources.WordYear);
            tableRow.Append(yearLabelCell);

            //月の入力欄
            if (HasMonth(response))
            {
                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);
                TableCell monthLabelCell = CreateNoBorderCell(Resources.WordMonth);
                tableRow.Append(monthLabelCell);
            }

            //日の入力欄
            if (HasDay(response))
            {
                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);
                TableCell dayLabelCell = CreateNoBorderCell(Resources.WordDay);
                tableRow.Append(dayLabelCell);
            }

            //時間の入力欄
            if (HasTime(response))
            {
                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);
                TableCell hourLabelCell = CreateNoBorderCell(Resources.WordHour);
                tableRow.Append(hourLabelCell);

                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);
                TableCell minLabelCell = CreateNoBorderCell(Resources.WordMinute);
                tableRow.Append(minLabelCell);
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                tableRow.Append(CreateNoBorderCell(prefix));
            }

            TableCell lastCell = CreateLastNoBorderCell();
            tableRow.Append(lastCell);

            return tableRow;
        }

        private TableRow CreateDateTimeResponseRowEn(QuestionVM question, string prefix)
        {
            ResponseVM response = question.Response;
            DateTimeLayoutVM dateTimeLayout = (DateTimeLayoutVM)response.Layout;

            TableRow tableRow = new TableRow();

            TableCell cell = null;
            List<TableCell> cells = null;
            //月の入力欄
            if (HasMonth(response))
            {
                cell = CreateNoBorderCell(Resources.WordMonth);
                tableRow.Append(cell);
                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);
            }

            //日の入力欄
            if (HasDay(response))
            {
                cell = CreateNoBorderCell(Resources.WordDay);
                tableRow.Append(cell);
                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);
            }

            //年の入力欄
            string yearLabel = Resources.WordYear;
            if (dateTimeLayout.CalendarEra == DateTimeLayoutCalendarEra.Japanese)
            {
                yearLabel += "[" + Resources.JapaneseEraTSH + "]";
            }
            cell = CreateNoBorderCell(yearLabel);
            tableRow.Append(cell);
            cells = CreateNumberInputCells(dateTimeLayout.Style, 4);
            tableRow.Append(cells);


            //時間の入力欄
            if (HasTime(response))
            {
                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);

                CreateNoBorderCell(":");

                cells = CreateNumberInputCells(dateTimeLayout.Style, 2);
                tableRow.Append(cells);
            }

            if (!string.IsNullOrEmpty(prefix))
            {
                tableRow.Append(CreateNoBorderCell(prefix));
            }

            TableCell lastCell = CreateLastNoBorderCell();
            tableRow.Append(lastCell);

            return tableRow;
        }

        private void WriteDateTimeResponse(Body body, QuestionVM question)
        {
            ResponseVM response = question.Response;
            DateTimeLayoutVM dateTimeLayout = (DateTimeLayoutVM)response.Layout;

            Table table = CreateFilledTable();

            string prefix = null;
            if (HasRange(response))
            {
                prefix = Resources.FromTo;
            }

            if (config.IsLanguageEn)
            {
                TableRow row1 = CreateDateTimeResponseRowEn(question, prefix);
                table.Append(row1);
                if (HasRange(response))
                {
                    TableRow row2 = CreateDateTimeResponseRowEn(question, " ");
                    table.Append(row2);
                }
            }
            else
            {
                TableRow row1 = CreateDateTimeResponseRowJa(question, prefix);
                table.Append(row1);
                if (HasRange(response))
                {
                    TableRow row2 = CreateDateTimeResponseRowJa(question, " ");
                    table.Append(row2);
                }
            }

            body.Append(table);
        }


        #endregion

        #region 数値

        private int CalcNumericColumnCount(ResponseVM response)
        {
            decimal? max = response.Max;
            if (max == null || !max.HasValue)
            {
                return 3;
            }
            return max.ToString().Length;
        }

        private string ToSideStatement(string str)
        {
            string result = str;
            if (string.IsNullOrEmpty(result))
            {
                result = " ";
            }
            return result;
        }

        private string BuildNumericLine(NumericLayoutVM numericLayout)
        {
            double sidePartLength = 2;
            double midPartLength = 4;


            int? length = numericLayout.Length;
            double totalLength = 30.0; //デフォルト3cm
            if (length != null && length.HasValue)
            {
                totalLength = length.Value * 10; //cmで入力なのでmmに変換
            }

            double midLength = totalLength - sidePartLength * 2;

            string mid = "";
            double curLength  = 0;
            while (curLength < midLength)
            {
                curLength += midPartLength;
                mid += Resources.WordMidMark;
            }

            return Resources.WordStartMark + mid + Resources.WordEndMark;
        }

        private void WriteNumerResponse(Body body, QuestionVM question)
        {
            ResponseVM response = question.Response;
            NumericLayoutVM numericLayout = (NumericLayoutVM)response.Layout;


            Table table = CreateFilledTable();
            TableRow tableRow = new TableRow();
            table.Append(tableRow);
            if (numericLayout.MeasurementMethod == NumericLayoutMeasurementMethod.Number)
            {
                //数値を記入する場合
                int count = CalcNumericColumnCount(response);
                tableRow.Append(CreateNumberInputCells(numericLayout.Style, count));

                if (!string.IsNullOrEmpty(numericLayout.Unit))
                {
                    tableRow.Append(CreateNoBorderCell(numericLayout.Unit));
                }
                tableRow.Append(CreateLastNoBorderCell());
            }
            else
            {
                string leftStatement = ToSideStatement(numericLayout.LeftStatement);
                tableRow.Append(CreateNoBorderCell(leftStatement));

                tableRow.Append(CreateNoBorderCell(BuildNumericLine(numericLayout)));

                string rightStatement = ToSideStatement(numericLayout.RightStatement);
                tableRow.Append(CreateNoBorderCell(rightStatement));

                tableRow.Append(CreateLastNoBorderCell());
            }

            body.Append(table);
        }
        #endregion

        #region 選択肢

        private string ToChoiceString(CodeVM code)
        {
            string text = code.Value + ". " + code.Label;
            return text;
        }

        private TableCell CreateChoicesCell(List<CodeVM> codes, int firstCodesCount)
        {
            TableCellProperties tableCellProperties = new TableCellProperties();

            TableCellWidth tableCellWidth = new TableCellWidth() { Width = "0", Type = TableWidthUnitValues.Auto };
            tableCellProperties.Append(tableCellWidth);

            TableCell tableCell = new TableCell();
            tableCell.Append(tableCellProperties);

            foreach (CodeVM code in codes)
            {
                string text = ToChoiceString(code);
                Paragraph paragraph = CreateTextParagraph(text);
                tableCell.Append(paragraph);
            }

            int diff = firstCodesCount - codes.Count;
            if (diff != 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    tableCell.Append(CreateEmptyParagraph());
                }
            }

            return tableCell;
        }

        private int GetSplitCount(ChoicesLayoutVM choicesLayout)
        {
            int count = 1;
            if (choicesLayout.ColumnCount != null && choicesLayout.ColumnCount.HasValue)
            {
                count = choicesLayout.ColumnCount.Value;
            }
            return count;
        }

        private int DivideCeil(int v1, int v2)
        {
            double v = (double)v1 / (double)v2;
            return (int)Math.Ceiling(v);
        }

        private List<CodeVM> GetValidCodes(ICollection<CodeVM> codes)
        {
            List<CodeVM> validCodes = new List<CodeVM>();
            foreach (CodeVM code in codes)
            {
                if (!code.IsExcludeValue)
                {
                    validCodes.Add(code);
                }
            }
            return validCodes;
        }

        private List<List<CodeVM>> SplitCodes(List<CodeVM> validCodes, ChoicesLayoutVM choicesLayout)
        {
            List<List<CodeVM>> codesList = new List<List<CodeVM>>();

            int count = GetSplitCount(choicesLayout);
            int countPerArea = DivideCeil(validCodes.Count, count);

            if (choicesLayout.Direction == ChoicesLayoutDirection.Vertical)
            {
                List<CodeVM> codesPerArea = null;
                int i = 0;
                foreach (CodeVM code in validCodes)
                {
                    if (i % countPerArea == 0)
                    {
                        codesPerArea = new List<CodeVM>();
                        codesList.Add(codesPerArea);
                    }
                    codesPerArea.Add(code);
                    i++;
                }
            }
            else
            {
                int areaCount = DivideCeil(validCodes.Count, countPerArea); //これがセルの数になる
                for (int j = 0; j < areaCount; j++)
                {
                    codesList.Add(new List<CodeVM>());
                }
                int i = 0; //列を順番に入れ替えながらうめていく。
                foreach (CodeVM code in validCodes)
                {
                    int codesIndex = i % areaCount;
                    codesList[codesIndex].Add(code);
                    i++;
                }
            }
            return codesList;
        }

        private void WriteChoicesResponse(Body body, QuestionVM question)
        {
            ResponseVM response = question.Response;
            ChoicesLayoutVM choicesLayout = (ChoicesLayoutVM)response.Layout;
            List<CodeVM> validCodes = GetValidCodes(response.Codes);

            if (choicesLayout.MeasurementMethod == ChoicesLayoutMesurementMethod.Single
                || choicesLayout.MeasurementMethod == ChoicesLayoutMesurementMethod.Multiple)
            {
                //測定法が単一回答か多重回答の場合
                Table table = CreateFilledTable(choicesLayout.Surround, false);
                List<List<CodeVM>> codesList = SplitCodes(validCodes, choicesLayout);

                //縦向きの場合(splitCount=列数となる)
                TableRow tableRow = new TableRow();
                table.Append(tableRow);
                int firstCodesCount = codesList[0].Count; //セル中のパラグラフ数をあわせるため(レイアウト枠を表示するため)に最後の列では空パラグラフを追加する。
                foreach (List<CodeVM> codes in codesList)
                {
                    TableCell cell = CreateChoicesCell(codes, firstCodesCount);
                    tableRow.Append(cell);
                }
                body.Append(table);
            }
            else
            {
                //意味微分法(SD法の場合)
                Table table = CreateFilledTable(true, true);

                TableRow tableRow1 = new TableRow();
                TableRow tableRow2 = new TableRow();
                table.Append(tableRow1);
                table.Append(tableRow2);
                foreach (CodeVM code in validCodes)
                {
                    tableRow1.Append(CreateCenterCell(code.Value));
                    tableRow2.Append(CreateCenterCell(code.Label));
                }

                body.Append(table);

            }
        }

        #endregion

        #region 質問グループ

        private void WriteQuestionGroupResponse(Body body, QuestionGroupVM questionGroup)
        {
            ICollection<QuestionVM> questions = questionGroup.Questions;
            if (questions.Count == 0)
            {
                return;
            }

            Table table = CreateFilledTable(true, true);

            if (questionGroup.Orientation == QuestionGroupOrientation.Row)
            {
                //行方向の場合(選択肢が横に並んでいる)
                QuestionVM firstQuestion = questions.First();
                List<CodeVM> codes = GetValidCodes(firstQuestion.Response.Codes);
                int firstQuestionCount = codes.Count;

                TableRow tableRow = null;
                if (questionGroup.Sentence == QuestionGroupSentence.Top)
                {
                    //ヘッダー行
                    tableRow = new TableRow();
                    table.Append(tableRow);
                    tableRow.Append(CreateBorderCell(0));
                    foreach (CodeVM code in codes)
                    {
                        tableRow.Append(CreateBorderCell(code.Label));
                    }
                }

                foreach (QuestionVM question in questions)
                {
                    tableRow = new TableRow();
                    table.Append(tableRow);
                    //最初に質問文
                    tableRow.Append(CreateBorderCell(question.Title));
                    //その後にセル
                    codes = GetValidCodes(question.Response.Codes);
                    foreach (CodeVM code in codes)
                    {
                        List<string> contents = new List<string>();
                        contents.Add(code.Value);
                        if (questionGroup.Sentence == QuestionGroupSentence.EachCell)
                        {
                            contents.Add(code.Label);
                        }
                        tableRow.Append(CreateCenterCell(contents));
                    }
                    int diff = firstQuestionCount - codes.Count;
                    if (diff > 0)
                    {
                        for (int i = 0; i < diff; i++)
                        {
                            tableRow.Append(CreateBorderCell(0));
                        }
                    }
                }
            }
            else
            {
                //列方向の場合

                //ヘッダー行
                TableRow tableRow = new TableRow();
                table.Append(tableRow);
                if (questionGroup.Sentence == QuestionGroupSentence.Top)
                {
                    tableRow.Append(CreateBorderCell(0));
                }

                //質問文
                foreach (QuestionVM question in questions)
                {
                    tableRow.Append(CreateBorderCell(question.Title));
                }

                QuestionVM firstQuestion = questions.First();
                List<CodeVM> codes = GetValidCodes(firstQuestion.Response.Codes);
                int firstQuestionCount = codes.Count;

                int codeIndex = 0;
                foreach (CodeVM code in codes)
                {
                    tableRow = new TableRow();
                    table.Append(tableRow);

                    //選択肢
                    if (questionGroup.Sentence == QuestionGroupSentence.Top)
                    {
                        tableRow.Append(CreateBorderCell(code.Label));
                    }

                    foreach (QuestionVM question in questions)
                    {
                        List<CodeVM> questionCodes = GetValidCodes(question.Response.Codes);
                        if (codeIndex < questionCodes.Count)
                        {
                            List<string> contents = new List<string>();
                            contents.Add(questionCodes[codeIndex].Value);
                            if (questionGroup.Sentence == QuestionGroupSentence.EachCell)
                            {
                                contents.Add(questionCodes[codeIndex].Label);
                            }
                            tableRow.Append(CreateCenterCell(contents));
                        }
                        else
                        {
                            tableRow.Append(CreateBorderCell(0));
                        }
                    }
                    codeIndex++;
                }
            }

            body.Append(table);
        }

        #endregion

        protected override void WriteBody(Body body)
        {
            ObservableCollection<ConstructVM> constructs = controlConstructScheme.Constructs;
            foreach (ConstructVM construct in constructs)
            {
                if (construct is QuestionConstructVM)
                {
                    //質問の書き出し
                    QuestionConstructVM questionConstruct = (QuestionConstructVM)construct;

                    //タイトル
                    string text = questionConstruct.No + ". " + questionConstruct.Text;
                    Paragraph paragraph = CreateMidashi1Paragraph(text);
                    body.Append(paragraph);
                    paragraph = CreateEmptyParagraph();
                    body.Append(paragraph);


                    //回答欄
                    QuestionVM question = questionConstruct.Question;
                    if (question.IsResponseTypeChoices)
                    {
                        WriteChoicesResponse(body, question);
                    } else if (question.IsResponseTypeFree)
                    {
                        WriteFreeResponse(body, question);
                    } else if (question.IsResponseTypeDateTime)
                    {
                        WriteDateTimeResponse(body, question);
                    }
                    else if (question.IsResponseTypeNumber)
                    {
                        WriteNumerResponse(body, question);
                    }
                    paragraph = CreateEmptyParagraph();
                    body.Append(paragraph);
                }
                else if (construct is StatementVM)
                {
                    //説明文の書き出し
                    StatementVM statement = (StatementVM)construct;
                    Paragraph paragraph = CreateParagraph(statement.Text);
                    body.Append(paragraph);
                    paragraph = CreateEmptyParagraph();
                    body.Append(paragraph);

                }
                else if (construct is QuestionGroupConstructVM)
                {
                    //質問グループの書き出し


                    QuestionGroupConstructVM questionGroupConstruct = (QuestionGroupConstructVM)construct;
                    QuestionGroupVM questionGroup = questionGroupConstruct.QuestionGroup;

                    string text = questionGroupConstruct.No + ". " + questionGroupConstruct.Title;
                    Paragraph paragraph = CreateParagraph(text);
                    body.Append(paragraph);
                    paragraph = CreateEmptyParagraph();
                    body.Append(paragraph);

                    WriteQuestionGroupResponse(body, questionGroup);
                    paragraph = CreateEmptyParagraph();
                    body.Append(paragraph);
                }
            }
        }
    }
}
