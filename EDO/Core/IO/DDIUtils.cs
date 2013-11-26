using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Model;
using EDO.Core.Util;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace EDO.Core.IO
{
    public class RenameResult
    {
        public RenameResult(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; set; }
        public string OldId { get; set;  }
        public string NewId { get; set; }
        public bool IsRenamed
        {
            get
            {
                return OldId != null && NewId != null;
            }
        }
        public bool IsIdPropertyRenamed
        {
            get
            {
                return PropertyName == "Id" && IsRenamed;            
            }
        }
    }

    public static class DDIUtils
    {
        //List<string>ではなく、List<EDOID>のような形で保持してどの部分のIDかわかるようにしたほうがデバッグしやすいかも。
        private static void CollectIds(IIDPropertiesProvider obj, List<string> ids)
        {
            foreach (string propertyName in obj.IdProperties)
            {
                string id = (string)PropertyPathHelper.GetValue(obj, propertyName);
                ids.Add(id);
            }                
        }

        private static void CollectIds<T>(List<T> objects, List<string> ids) where T:IIDPropertiesProvider
        {
            foreach (T obj in objects)
            {
                CollectIds(obj, ids);
            }
        }

        public static List<string> CollectIds(StudyUnit studyUnit)
        {
            List<string> ids = new List<string>();
            //0.StudyUnit本体
            CollectIds(studyUnit, ids);
            //1.イベント
            CollectIds(studyUnit.Events, ids);
            //2.メンバー
            CollectIds(studyUnit.Members, ids);
            //3.組織
            CollectIds(studyUnit.Organizations, ids);
            //4.概要
            CollectIds(studyUnit.Abstract, ids);
            //5.調査の範囲
            CollectIds(studyUnit.Coverage, ids);
            //6.助成機関
            CollectIds(studyUnit.FundingInfos, ids);
            //6-1.助成機関に含まれる組織
            CollectIds(FundingInfo.GetOrganizations(studyUnit.FundingInfos), ids);
            //7.母集団
            CollectIds(Sampling.GetUniverses(studyUnit.Samplings), ids);
            //8.サンプリング
            CollectIds(studyUnit.Samplings, ids);
            //9.コンセプトスキーム
            CollectIds(studyUnit.ConceptSchemes, ids);
            //9-1.コンセプト
            CollectIds(ConceptScheme.GetConcepts(studyUnit.ConceptSchemes), ids);
            //10.質問
            CollectIds(studyUnit.Questions, ids);
            //10-1.回答
            CollectIds(Question.GetResponses(studyUnit.Questions), ids);
            //11.カテゴリースキーム
            CollectIds(studyUnit.CategorySchemes, ids);
            //11-1.カテゴリー
            CollectIds(CategoryScheme.GetCategories(studyUnit.CategorySchemes), ids);
            //12.コードスキーム
            CollectIds(studyUnit.CodeSchemes, ids);
            //12-1.コード
            CollectIds(CodeScheme.GetCodes(studyUnit.CodeSchemes), ids);
            //13.変数スキーム
            CollectIds(studyUnit.VariableScheme, ids);
            //14.変数
            CollectIds(studyUnit.Variables, ids);
            //14-1.回答
            CollectIds(Variable.GetResponses(studyUnit.Variables), ids);
            //15.データセット
            CollectIds(studyUnit.DataSets, ids);
            //16.データファイル
            CollectIds(studyUnit.DataFiles, ids);
            //17.質問の順序
            CollectIds(studyUnit.ControlConstructSchemes, ids);
            //17-1.Sequence
            CollectIds(ControlConstructScheme.GetSequences(studyUnit.ControlConstructSchemes), ids);
            //17-2.Constructs
            CollectIds(ControlConstructScheme.GetConstructs(studyUnit.ControlConstructSchemes), ids);

            return ids;
        }

        private static RenameResult RenameId(IIDPropertiesProvider obj, string propertyName, List<string> ids)
        {
            RenameResult result = new RenameResult(propertyName);
            string id = (string)PropertyPathHelper.GetValue(obj, propertyName);
            if (ids.Contains(id))
            {
                string newId =  IDUtils.NewGuid();
                PropertyPathHelper.SetValue(obj, propertyName, newId);
                result.OldId = id;
                result.NewId = newId;
                return result;
            }
            return result;
        }

        private static void RenameIds(IIDPropertiesProvider obj, List<string>ids)
        {
            RenameIds(obj, ids, null);
        }

        private static void RenameIds(IIDPropertiesProvider obj, List<string>ids, Action<RenameResult> action)
        {
            foreach (string propertyName in obj.IdProperties)
            {
                RenameResult result = RenameId(obj, propertyName, ids);
                if (result.IsIdPropertyRenamed && action != null)
                {
                    action(result);
                }
            }            
        }

        private static void RenameIds<T>(List<T> objects, List<string> ids) where T : IIDPropertiesProvider
        {
            RenameIds(objects, ids, null);
        }

        private static void RenameIds<T>(List<T> objects, List<string> ids, Action<RenameResult> action) where T: IIDPropertiesProvider
        {
            foreach (T obj in objects)
            {
                RenameIds(obj, ids, action);
            }
        }


        private static bool IsIdChanged(string propertyName, string old)
        {
            return (propertyName == "Id" && old != null);
        }

        public static void RenameIds(StudyUnit orgStudyUnit, StudyUnit newStudyUnit)
        {
            // orgStudyUnitのIDとnewStudyUnitのIDが重複しないように、newStudyUnitのIDを付け直す

            List<string> ids = CollectIds(orgStudyUnit);

            //0.StudyUnit自身
            RenameIds(newStudyUnit, ids);
            //1.イベント
            RenameIds(newStudyUnit.Events, ids);
            //2.メンバー
            RenameIds(newStudyUnit.Members, ids);
            //3.組織
            RenameIds(newStudyUnit.Organizations, ids, (result) =>
            {
                Member.ChangeOrganizationId(newStudyUnit.Members, result.OldId, result.NewId);
            });
            //4.概要
            RenameIds(newStudyUnit.Abstract, ids);
            //5.調査範囲
            RenameIds(newStudyUnit.Coverage, ids);
            //6.助成機関
            RenameIds(newStudyUnit.FundingInfos, ids);
            RenameIds(newStudyUnit.FundingInfoOrganizations, ids); //助成機関の組織
            ////7.母集団
            RenameIds(newStudyUnit.AllUniverses, ids, (result) =>
            {
                Variable.ChangeUniverseId(newStudyUnit.Variables, result.OldId, result.NewId);
            });
            //8.サンプリング
            RenameIds(newStudyUnit.Samplings, ids);
            //9.コンセプトスキーム
            RenameIds(newStudyUnit.ConceptSchemes, ids);
            //9-1.コンセプト
            RenameIds(ConceptScheme.GetConcepts(newStudyUnit.ConceptSchemes), ids, (result) =>
            {
                Question.ChangeConceptId(newStudyUnit.Questions, result.OldId, result.NewId);
                Variable.ChangeConceptId(newStudyUnit.Variables, result.OldId, result.NewId);
                Book.ChangeMetaDataId(newStudyUnit.Books, result.OldId, result.NewId);
            });
            //10.質問
            RenameIds(newStudyUnit.Questions, ids, (result) =>
            {
                ControlConstructScheme.ChangeQuestionId(newStudyUnit.ControlConstructSchemes, result.OldId, result.NewId);
                Variable.ChangeQuestionId(newStudyUnit.Variables, result.OldId, result.NewId);
                Book.ChangeMetaDataId(newStudyUnit.Books, result.OldId, result.NewId);
            });
            //11.カテゴリースキーム
            RenameIds(newStudyUnit.CategorySchemes, ids, (result) =>
            {
                CategoryScheme.ChangeCategorySchemeId(newStudyUnit.CategorySchemes, result.OldId, result.NewId);
            });
            //11-1.カテゴリー
            RenameIds(CategoryScheme.GetCategories(newStudyUnit.CategorySchemes), ids, (result) =>
            {
                CodeScheme.ChangeCategoryId(newStudyUnit.CodeSchemes, result.OldId, result.NewId);
            });
            //12.コードスキーム
            RenameIds(newStudyUnit.CodeSchemes, ids, (result) =>
            {
                CodeScheme.ChangeCodeSchemeId(newStudyUnit.CodeSchemes, result.OldId, result.NewId);                
            });
            //12-1.コード
            RenameIds(CodeScheme.GetCodes(newStudyUnit.CodeSchemes), ids);
            //13.変数スキーム
            RenameIds(newStudyUnit.VariableScheme, ids);
            //14.変数
            RenameIds(newStudyUnit.Variables, ids, (result) =>
            {
                DataSet.ChangeVariableId(newStudyUnit.DataSets, result.OldId, result.NewId);
                Book.ChangeMetaDataId(newStudyUnit.Books, result.OldId, result.NewId);
            });
            //15.データセット
            RenameIds(newStudyUnit.DataSets, ids, (result) =>
            {
                DataFile.ChangeDataSetId(newStudyUnit.DataFiles, result.OldId, result.NewId);
            });
            //16.データファイル
            RenameIds(newStudyUnit.DataFiles, ids);
            //17.質問の順序
            RenameIds(newStudyUnit.ControlConstructSchemes, ids);
            //17-1.Sequence
            RenameIds(ControlConstructScheme.GetSequences(newStudyUnit.ControlConstructSchemes), ids);
            //17-2.Constructs
            RenameIds(ControlConstructScheme.GetConstructs(newStudyUnit.ControlConstructSchemes), ids, (result) =>
            {
                ControlConstructScheme.ChangeControlConstructId(newStudyUnit.ControlConstructSchemes, result.OldId, result.NewId);
            });

            //関連文献
            RenameIds(newStudyUnit.Books, ids);
        }

        public static void CheckDuplicate(StudyUnit orgStudyUnit, StudyUnit newStudyUnit)
        {
            List<string> orgIds = CollectIds(orgStudyUnit);
            List<string> newIds = CollectIds(newStudyUnit);
            foreach (string newId in newIds)
            {
                if (orgIds.Contains(newId))
                {
                    Debug.WriteLine(string.Format("duplicate id {0}", newId));
                }
            }
        }

    }
}
