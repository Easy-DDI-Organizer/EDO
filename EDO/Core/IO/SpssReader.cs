using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using System.Diagnostics;
using System.IO;
using SpssLib.FileParser;
using EDO.Main;
using EDO.Core.Util;
using EDO.Properties;

namespace EDO.Core.IO
{
    public class SpssReader
    {
        static bool ExistValue(CodeScheme codeScheme, string missingValue)
        {
            //とりあえずdoubleの文字列表現で比較(εで比較する必要があるかも)。
            string missingValueStr = missingValue.ToString();
            foreach (Code existCode in codeScheme.Codes)
            {
                if (existCode.Value == missingValueStr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Read(string path, StudyUnitVM studyUnit)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));
            StudyUnit studyUnitModel = studyUnit.StudyUnitModel;
            FileStream stream = File.OpenRead(path);
            SavFileParser parser = new SavFileParser(stream);
            List<Variable> variables = new List<Variable>();
            foreach (SpssLib.SpssDataset.Variable v in parser.Variables)
            {
                Variable variable = new Variable();
                variable.Title = v.Name;
                variable.Label = v.Label;
                variables.Add(variable);
                if (v.Type == SpssLib.SpssDataset.DataType.Text)
                {
                    variable.Response.TypeCode = Options.RESPONSE_TYPE_FREE_CODE;
                }
                else
                {
                    if (v.ValueLabels.Count > 0)
                    {
                        CategoryScheme categoryScheme = new CategoryScheme();
                        categoryScheme.Title = v.Label;
                        studyUnitModel.CategorySchemes.Add(categoryScheme);
                        CodeScheme codeScheme = new CodeScheme();
                        codeScheme.Title = v.Label; ;
                        studyUnitModel.CodeSchemes.Add(codeScheme);

                        variable.Response.TypeCode = Options.RESPONSE_TYPE_CHOICES_CODE;
                        variable.Response.CodeSchemeId = codeScheme.Id;
                        // 選択肢の作成
                        foreach (KeyValuePair<double, string> pair in v.ValueLabels)
                        {
                            Category category = new Category();
                            categoryScheme.Categories.Add(category);
                            category.Title = pair.Value;
                            category.CategorySchemeId = categoryScheme.Id;

                            Code code = new Code();
                            codeScheme.Codes.Add(code);
                            code.CategoryId = category.Id;
                            code.CodeSchemeId = codeScheme.Id;
                            code.Value = pair.Key.ToString();
                        }
                        // 欠損値の追加
                        if (v.MissingValues.Count > 0)
                        {
                            foreach (double missingValue in v.MissingValues)
                            {
                                string missingValueStr = missingValue.ToString();
                                if (ExistValue(codeScheme, missingValueStr))
                                {
                                    continue;
                                }
                                Category category = new Category();
                                categoryScheme.Categories.Add(category);
                                category.Title = Resources.MissingValue; //欠損値
                                category.IsMissingValue = true;
                                category.CategorySchemeId = categoryScheme.Id;

                                Code code = new Code();
                                codeScheme.Codes.Add(code);
                                code.CategoryId = category.Id;
                                code.CodeSchemeId = codeScheme.Id;
                                code.Value = missingValueStr;
                            }
                        }
                    }
                    else
                    {
                        variable.Response.TypeCode = Options.RESPONSE_TYPE_NUMBER_CODE;
                    }
                }
            }

            if (variables.Count > 0)
            {
                ConceptScheme conceptScheme = new ConceptScheme();
                conceptScheme.Title = EDOUtils.UniqueLabel(ConceptScheme.GetTitles(studyUnitModel.ConceptSchemes), Resources.ImportVariable); //インポートした変数
                string name = Path.GetFileName(path);
                conceptScheme.Memo = EDOUtils.UniqueLabel(ConceptScheme.GetMemos(studyUnitModel.ConceptSchemes), string.Format(Resources.ImportVariableFrom, name)); //{0}からインポートした変数

                Concept concept = new Concept();
                concept.Title = EDOUtils.UniqueLabel(ConceptScheme.GetConceptTitles(studyUnitModel.ConceptSchemes), Resources.ImportVariable);//インポートした変数
                concept.Content = EDOUtils.UniqueLabel(ConceptScheme.GetConceptContents(studyUnitModel.ConceptSchemes), string.Format(Resources.ImportVariableFrom, name));//{0}からインポートした変数
                conceptScheme.Concepts.Add(concept);
                studyUnitModel.ConceptSchemes.Add(conceptScheme);

                foreach (Variable variable in variables)
                {
                    Question question = new Question();
                    question.Title = variable.Label;
                    question.ConceptId = concept.Id;
                    question.Response.TypeCode = variable.Response.TypeCode;
                    question.Response.CodeSchemeId = variable.Response.CodeSchemeId;
                    studyUnitModel.Questions.Add(question);

                    variable.ConceptId = concept.Id;
                    variable.QuestionId = question.Id;
                    studyUnitModel.Variables.Add(variable);

                }
            }
            return true;
        }
    }
}
