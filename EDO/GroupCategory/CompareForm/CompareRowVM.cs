using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using EDO.Core.Model;
using System.ComponentModel;

namespace EDO.GroupCategory.CompareForm
{
    public class CompareRowVM :BaseVM, IEditableObject
    {
        public static CompareRowVM FindByStringId(ObservableCollection<CompareRowVM> rows, string rowId)
        {
            //文字列化されたIDから行を検索する
            foreach (CompareRowVM row in rows)
            {
                if (row.Id.ToString() == rowId)
                {
                    return row;
                }
            }
            return null;
        }

        private static void UpdateCell(CompareCell cell, DiffOption diffOption, ObservableCollection<CompareRowVM> rows)
        {
            if (diffOption == null)
            {
                return;
            }
            if (diffOption.IsPartialMatch)
            {
                //部分一致の場合CodeにはROWIDが入っているのでそこから行のGroupIdのリストをとりだして保存
                cell.CompareValue = Options.COMPARE_VALUE_PARTIALMATCH_CODE;
                CompareRowVM targetRow = FindByStringId(rows, diffOption.Code);
                cell.TargetTitle = targetRow.Title;
            }
            else
            {
                //○ OR ×の場合はそのまま保存
                cell.CompareValue = diffOption.Code;
                cell.TargetTitle = null;
            }
        }

        public CompareRowVM(List<string> studyUnitGuids)
        {
            rowModel = new CompareRow();
            this.studyUnitGuids = studyUnitGuids;
            diffOptions = new ObservableCollection<DiffOption>();
            selectedDiffOptions = new ObservableCollection<DiffOption>();
            foreach (string guid in studyUnitGuids)
            {
                selectedDiffOptions.Add(null);
            }
            backSelectedDiffOptions = new List<DiffOption>();
        }

        #region プロパティ

        private CompareRow rowModel;
        public CompareRow RowModel { get { return rowModel; } }
        public string Id { get { return rowModel.Id; } }
        public void AddGroupId(GroupId groupId)
        {
            rowModel.RowGroupIds.Add(groupId);
        }
        public List<GroupId> RowGroupIds { get { return rowModel.RowGroupIds; } }

        private List<string> studyUnitGuids;

        private ObservableCollection<DiffOption> diffOptions;
        public ObservableCollection<DiffOption> DiffOptions { get { return diffOptions; } }

        private ObservableCollection<DiffOption> selectedDiffOptions;
        public ObservableCollection<DiffOption> SelectedDiffOptions { get { return selectedDiffOptions; } }

        private string backTitle;
        private string backMemo;
        private List<DiffOption> backSelectedDiffOptions;

        public string Title
        {
            get
            {
                return rowModel.Title;
            }
            set
            {
                if (rowModel.Title != value)
                {
                    rowModel.Title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Memo
        {
            get
            {
                return rowModel.Memo;
            }
            set
            {
                if (rowModel.Memo != value)
                {
                    rowModel.Memo = value;
                    NotifyPropertyChanged("Memo");
                }
            }
        }

        public bool ContainsAll(List<GroupId> rowGroupIds)
        {
            return rowModel.ContainsAll(rowGroupIds);
        }

        #endregion

        #region モデルから初期化・モデルに反映

        private DiffOption FindDiffOptionByCode(string code)
        {
            foreach (DiffOption diffOption in diffOptions)
            {
                if (diffOption.Code == code)
                {
                    return diffOption;
                }
            }
            return null;
        }

        private DiffOption FindDiffOptionByTitle(ObservableCollection<CompareRowVM> rows, string title)
        {
            //全DiffOptionをループ
            foreach (DiffOption diffOption in diffOptions)
            {
                if (diffOption.IsMatchOrNotMatch)
                {
                    continue;
                }
                //DiffOptionに対応した行を取得(△の場合のみ可能)
                CompareRowVM row = FindByStringId(rows, diffOption.Code);
                if (row.Title == title)
                {
                    //現在の行に以前の内容が完全に含まれていたらそれを選択(あるいは逆かも?)
                    return diffOption;
                }
            }
            return null;
        }

        private DiffOption FindDiffOption(ObservableCollection<CompareRowVM> rows, CompareCell cell)
        {
            //既存のセルの内容からDiffOptionを検索
            if (cell == null)
            {
                return null;
            }
            DiffOption diffOption = null;
            if (Options.IsPartialMatch(cell.CompareValue))
            {
                diffOption = FindDiffOptionByTitle(rows, cell.TargetTitle);
            }
            else
            {
                diffOption = FindDiffOptionByCode(cell.CompareValue);
            }
            return diffOption;
        }

        private void CreateDiffOptions(ObservableCollection<CompareRowVM> rows)
        {
            diffOptions.Clear();
            diffOptions.Add(new DiffOption(Options.CompareValueMatch));
            diffOptions.Add(new DiffOption(Options.CompareValueNotMatch));
            foreach (CompareRowVM row in rows)
            {
                if (row != this)
                {
                    diffOptions.Add(new DiffOption(row.Id.ToString(), Options.CompareValuePartialMatch.Label, row.Title));
                }
            }
        }

        private void CreateCells()
        {
            rowModel.Cells.Clear();
            foreach (string studyUnitId in studyUnitGuids)
            {
                CompareCell cell = new CompareCell();
                rowModel.Cells.Add(cell);
                cell.ColumnStudyUnitId = studyUnitId;
            }
        }

        private void UpdateViewModel(ObservableCollection<CompareRowVM> rows, CompareRow existRowModel)
        {
            if (existRowModel == null)
            {
                return;
            }
            //タイトルは常に最新のものを使用する
            //メモは既存のものを反映する
            Memo = existRowModel.Memo;
            //セルの値を更新
            selectedDiffOptions.Clear();
            foreach (CompareCell cell in rowModel.Cells)
            {
                //セルに対応した既存のセルを取得
                CompareCell existCell = existRowModel.FindCell(cell.ColumnStudyUnitId);
                //既存のセルが存在するならばそれに対応したDiffOptionに変換
                DiffOption diffOption = FindDiffOption(rows, existCell);
                //DiffOptionに追加(画面上ではselectedDiffOptionsが更新される)。
                selectedDiffOptions.Add(diffOption);
            }
        }

        public void Init(ObservableCollection<CompareRowVM> rows, CompareRow existRowModel)
        {
            //選択肢の生成
            CreateDiffOptions(rows);
            //セルの生成
            CreateCells();
            //既存のモデルの値を使ってViewModelを更新する
            UpdateViewModel(rows, existRowModel);
        }

        public void UpdateModel(ObservableCollection<CompareRowVM> rows)
        {
            //モデルの情報を更新する
            int i = 0;
            foreach (DiffOption diffOption in selectedDiffOptions)
            {
                CompareCell cell = rowModel.Cells[i++];
                UpdateCell(cell, diffOption, rows);
            }
        }
        #endregion

        #region IEditableObject

        public bool InEdit { get { return inEdit; } }

        private bool inEdit;

        public void BeginEdit()
        {
            if (inEdit)
            {
                return;
            }
            inEdit = true;
            backTitle = Title;
            backMemo = Memo;
            backSelectedDiffOptions.Clear();
            foreach (DiffOption diffOption in selectedDiffOptions)
            {
                backSelectedDiffOptions.Add(diffOption);
            }
        }

        public void CancelEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            Title = backTitle;
            Memo = backMemo;
            selectedDiffOptions.Clear();
            foreach (DiffOption diffOption in backSelectedDiffOptions)
            {
                selectedDiffOptions.Add(diffOption);
            }
        }

        public Action<IEditableObject> ItemEndEditAction { get; set; }

        public void EndEdit()
        {
            if (!inEdit)
            {
                return;
            }
            inEdit = false;
            backTitle = null;
            backMemo = null;
            backSelectedDiffOptions.Clear();

            CompareFormVM parent = (CompareFormVM)Parent;
            parent.UpdateModel();
            Memorize();
        }

        #endregion
    }
}
