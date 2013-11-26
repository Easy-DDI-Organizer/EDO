using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Diagnostics;
using EDO.Main;
using EDO.DataCategory.DataFileForm;
using EDO.VariableCategory.VariableForm;
using EDO.DataCategory.DataSetForm;
using EDO.QuestionCategory.QuestionForm;
using System.Collections.ObjectModel;
using EDO.QuestionCategory.CodeForm;
using EDO.Core.Model;
using EDO.Properties;

namespace EDO.Core.IO
{
    public class SetupSyntaxWriter
    {
        private StudyUnitVM studyUnit;

        private DataFileVM dataFile;

        public SetupSyntaxWriter(StudyUnitVM studyUnit, DataFileVM dataFile)
        {
            this.studyUnit = studyUnit;
            this.dataFile = dataFile;
        }

        protected void PreviewText(string path)
        {
            Process process = Process.Start("notepad", path);
            process.WaitForExit();
        }


        private string VariableType(VariableVM variable)
        {
            ResponseVM response = variable.Response;
            string type = "";
            if (response.IsTypeChoices)
            {
                if (response.ValidCodes.Count > 0)
                {
                    type = "F";
                }
                else
                {
                    type = "A";
                }
            } else if (response.IsTypeNumber)
            {
                type = "F";
            }
            else if (response.IsTypeFree)
            {
                type = "A";
            }
            else if (response.IsTypeDateTime)
            {
                type = "F";
            }
            return type;
        }

        private List<VariableVM> Variables
        {
            get
            {
                List<VariableVM> variables = new List<VariableVM>();
                foreach (DataSetVariableVM dataSetVariable in dataFile.Variables)
                {

                    //DataSetVariableとVariableのIDは同じ
                    VariableVM variable = studyUnit.FindVariable(dataSetVariable.Id);
                    if (variable == null)
                    {
                        throw new ApplicationException(Resources.VariableIsNotFound + dataSetVariable.Id); //変数が見つかりません。
                    }
                    variables.Add(variable);
                }
                return variables;
            }
        }

        private List<VariableVM> GetChoicesVariables(List<VariableVM> variables)
        {
            List<VariableVM> choicesVariables = new List<VariableVM>();
            foreach (VariableVM variable in variables)
            {
                if (variable.Response.IsTypeChoices)
                {
                    choicesVariables.Add(variable);
                }
            }
            return choicesVariables;
        }

        private string Delimiter()
        {
            string delimiter = "";
            if (dataFile.DelimiterCode == Options.DELIMITER_COMMA)
            {
                delimiter = ",";
            }
            else if (dataFile.DelimiterCode == Options.DELIMITER_TAB)
            {
                delimiter = @"\t";
            }
            else if (dataFile.DelimiterCode == Options.DELIMITER_SPACE)
            {
                delimiter = " ";
            }
            return delimiter;
        }

        public void Write(string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.WriteLine("get data");
                    writer.WriteLine("/type=txt");
                    writer.WriteLine("/file=\"" + dataFile.Uri + "\"");
                    writer.WriteLine("/arrangement=Delimited");
                    writer.WriteLine("/delimiters=\"" + Delimiter() + "\"");

                    //変数
                    StringBuilder buf = new StringBuilder();
                    buf.Append("/variables=");

                    List<VariableVM> variables = Variables;
                    foreach (VariableVM variable in variables)
                    {

                        //DataSetVariableとVariableのIDは同じ
                        if (variable != variables.First())
                        {
                            buf.Append(" ");
                        }
                        buf.Append(variable.Title);
                        string type = VariableType(variable);
                        if (!string.IsNullOrEmpty(type))
                        {
                            buf.Append(" ");
                            buf.Append(type);
                        }
                    }
                    writer.WriteLine(buf.ToString());
                    writer.WriteLine(".");
                    
                    //変数ラベル
                    writer.WriteLine("variable labels");
                    foreach (VariableVM variable in variables)
                    {
                        buf.Clear();
                        buf.Append(variable.Title);
                        buf.Append(" \"");
                        buf.Append(variable.Label);
                        buf.Append("\"");
                        if (variable != variables.Last())
                        {
                            buf.Append("/");
                        }
                        writer.WriteLine(buf.ToString());
                    }
                    writer.WriteLine(".");

                    //値ラベル
                    writer.WriteLine("value labels");
                    List<VariableVM> choicesVariables = GetChoicesVariables(variables);
                    foreach (VariableVM variable in choicesVariables)
                    {
                        List<CodeVM> codes = variable.Response.ValidCodes;
                        if (codes.Count == 0)
                        {
                            continue;
                        }

                        buf.Clear();
                        buf.Append(variable.Title);

                        foreach (CodeVM code in codes)
                        {
                            buf.Append(" ");
                            buf.Append(code.Value);
                            buf.Append(" \"");
                            buf.Append(code.Label);
                            buf.Append("\"");
                        }
                        if (variable != choicesVariables.Last())
                        {
                            buf.Append("/");
                        }
                        writer.WriteLine(buf.ToString());
                    }
                    writer.WriteLine(".");

                }
                // 書き出しに成功しました。表示しますか?  書き出し完了
                if (MessageBox.Show(Resources.WriteSuccess, Resources.WriteFinish, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    PreviewText(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.WriteError + ex.Message);
            }
        }
    }
}
