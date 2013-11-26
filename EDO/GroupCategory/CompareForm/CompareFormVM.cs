using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using System.Collections.ObjectModel;
using EDO.Core.View;

namespace EDO.GroupCategory.CompareForm
{
    public class CompareFormVM :FormVM
    {
        public CompareFormVM(GroupVM group, CompareTable compareTable) :base(group)
        {
            this.groupModel = group.GroupModel;
            this.compareTable = compareTable;
            rows = new ObservableCollection<CompareRowVM>();
        }

        private Group groupModel;
        private CompareTable compareTable;

        public List<StudyUnitVM> StudyUnits { get { return Main.StudyUnits; } }
        private ObservableCollection<CompareRowVM> rows;
        public ObservableCollection<CompareRowVM> Rows { get { return rows; } }

        private CompareRowVM CreateRow(Dictionary<string, CompareRowVM> rowMap, GroupId groupId, string title)
        {
            //タイトルに該当する行を作成したかどうかを覚えておくためのマップ
            CompareRowVM row = null;
            if (rowMap.ContainsKey(title))
            {
                row = rowMap[title];
                row.AddGroupId(groupId);
                return null;
            }
            row = new CompareRowVM(StudyUnitVM.GetStudyUnitGuids(StudyUnits))
            {
                Title = title,
                Parent = this
            };
            row.AddGroupId(groupId);
            rowMap[title] = row;
            return row;
        }

        private void InitRows()
        {
            foreach (CompareRowVM row in rows)
            {
                CompareRow rowModel = compareTable.FindRowByTitle(row.Title);
                row.Init(rows, rowModel);
            }
        }

        private void ReloadForConceptScheme()
        {
            //行を生成する
            Dictionary<string, CompareRowVM> rowMap = new Dictionary<string, CompareRowVM>();
            foreach (StudyUnitVM studyUnit in StudyUnits)
            {
                foreach (ConceptScheme conceptScheme in studyUnit.ConceptSchemeModels)
                {
                    CompareRowVM row = CreateRow(rowMap, new GroupId(studyUnit.Id, conceptScheme.Id), conceptScheme.Title);
                    if (row != null)
                    {
                        rows.Add(row);
                    }
                }
            }
            //行を初期化する
            InitRows();
        }

        private void ReloadForConcept()
        {
            //行を生成する
            Dictionary<string, CompareRowVM> rowMap = new Dictionary<string, CompareRowVM>();
            foreach (StudyUnitVM studyUnit in StudyUnits)
            {
                foreach (Concept concept in studyUnit.AllConceptModels)
                {
                    CompareRowVM row = CreateRow(rowMap, new GroupId(studyUnit.Id, concept.Id), concept.Title);
                    if (row != null)
                    {
                        rows.Add(row);
                    }
                }
            }
            //行を初期化する
            InitRows();
        }

        private void ReloadForVariable()
        {
            //行を生成する
            Dictionary<string, CompareRowVM> rowMap = new Dictionary<string, CompareRowVM>();
            foreach (StudyUnitVM studyUnit in StudyUnits)
            {
                foreach (Variable variable in studyUnit.VariableModels)
                {
                    CompareRowVM row = CreateRow(rowMap, new GroupId(studyUnit.Id, variable.Id), variable.Title);
                    if (row != null)
                    {
                        rows.Add(row);
                    }
                }
            }
            //行を初期化する
            InitRows();
        }

        protected override void Reload(VMState state)
        {
            rows.Clear();
            if (compareTable.IsCompareTypeConceptScheme)
            {
                ReloadForConceptScheme();
            } else if (compareTable.IsCompareTypeConcept)
            {
                ReloadForConcept();
            } else if (compareTable.IsCompareTypeVariable)
            {
                ReloadForVariable();
            }
        }

        public void UpdateModel()
        {
            compareTable.Rows.Clear();
            foreach (CompareRowVM row in rows)
            {
                row.UpdateModel(rows);
                compareTable.Rows.Add(row.RowModel);
            }
        }

        public void SyncModel()
        {
            Reload();
            UpdateModel();
        }

        protected override Action GetCompleteAction(VMState state)
        {
            return () => { UpdateModel(); };
        }

    }
}
