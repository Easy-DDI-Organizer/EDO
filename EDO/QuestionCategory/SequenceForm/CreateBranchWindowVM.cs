using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Util;
using System.Collections.ObjectModel;
using EDO.Core.Model;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows;

namespace EDO.QuestionCategory.SequenceForm
{
    public class CreateBranchWindowVM :BaseVM
    {
        public CreateBranchWindowVM(ControlConstructSchemeVM controlConstructScheme) :this(controlConstructScheme, null)
        {
        }


        public CreateBranchWindowVM(ControlConstructSchemeVM controlConstructScheme, IfThenElse ifThenElse)
        {
            this.controlConstructScheme = controlConstructScheme;

            questionConstructs = new ObservableCollection<QuestionConstructVM>();
            questionConstructs.AddRange(controlConstructScheme.QuestionConstructs);

            thenConstructs = new ObservableCollection<ConstructVM>();
            thenConstructs.AddRange(controlConstructScheme.ThenConstructs);

            if (ifThenElse == null)
            {
                //新規作成の場合

                TargetQuestionConstruct = controlConstructScheme.SelectedConstruct as QuestionConstructVM;
                branches = new ObservableCollection<BranchVM>();
                //IF～THENを追加
                BranchVM ifBranch = new BranchVM(BranchVM.TYPE_IF_CODE)
                {
                    Parent = this,
                    No = 1,
                };
                ifBranch.Init();
                branches.Add(ifBranch);
            }
            else
            {
                // 編集の場合

                TargetQuestionConstruct = EDOUtils.Find(controlConstructScheme.QuestionConstructs, ifThenElse.IfCondition.QuestionId);
                branches = SequenceUtils.CreateBranches(ifThenElse, this);
            }
        }

        public IBranchEditor Editor {get; set; }
        private ControlConstructSchemeVM controlConstructScheme;
        private ObservableCollection<BranchVM> branches;
        public ObservableCollection<BranchVM> Branches { get { return branches; } }
        private ObservableCollection<QuestionConstructVM> questionConstructs;
        public ObservableCollection<QuestionConstructVM> QuestionConstructs { get { return questionConstructs; } }
        private ObservableCollection<ConstructVM> thenConstructs;
        public ObservableCollection<ConstructVM> ThenConstructs { get { return thenConstructs; } }
        public QuestionConstructVM TargetQuestionConstruct {get; set; }
        public ConstructVM NextConstruct 
        { 
            get 
            {
                QuestionConstructVM target = TargetQuestionConstruct;
                if (target == null)
                {
                    return null;
                }

                List<ConstructVM> constructs = controlConstructScheme.ThenConstructs;
                ConstructVM next = null;

                int index = constructs.IndexOf(target);
                if (index + 1 < constructs.Count)
                {
                    next = constructs[index + 1];
                }
                if (next == null)
                {
                    next = target;
                }
                return next;
            } 
        }

        private BranchVM selectedBranch;
        private BranchVM editingBranch;
        public BranchVM EditingBranch
        {
            get
            {
                return editingBranch;
            }
            set
            {
                if (editingBranch != value)
                {
                    editingBranch = value;
                    NotifyPropertyChanged("EditingBranch");
                    NotifyPropertyChanged("IsEditingBranch");
                }
            }
        }

        public bool IsEditingBranch
        {
            get
            {
                return editingBranch != null;
            }
        }

        public bool CanEditBranch(BranchVM branch)
        {
            return EditingBranch == null;
        }

        public void EditBranch(BranchVM branch)
        {
            selectedBranch = branch;
            selectedBranch.OnEditStarted();
            EditingBranch = selectedBranch.DeepCopy();
        }

        public bool CanAddBranch(BranchVM branch)
        {
            if (EditingBranch != null)
            {
                return false;
            }
            if (branch.IsTypeElse)
            {
                //ELSE節には追加できない
                return false;
            }
            if (branches.Count == 0)
            {
                return false;
            }
            // IF or ELSE IFの場合、最後のブランチか、ELSEの1つ前ならば良い
            if (branches.Last() == branch)
            {
                return true;
            }
            if (branches.Count < 2)
            {
                return false;
            }
            int size = branches.Count;
            if (branches.Last().IsTypeElse && branches[size - 2] == branch)
            {
                return true;
            }
            return false;
        }

        private void OnBranchOrderChanged()
        {
            foreach (BranchVM b in branches)
            {
                b.OnOrderChanged();
            }
        }

        public void AddBranch(BranchVM branch)
        {
            int index = Branches.IndexOf(branch);

            //ELSE IF～THEN～を追加
            BranchVM elseIfBranch = new BranchVM(BranchVM.TYPE_ELSE_IF_CODE)
            {
                Parent = this,
                IsNew = true
            };
            elseIfBranch.Init();
            branches.Insert(index + 1, elseIfBranch);
            OnBranchOrderChanged();

            Editor.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => { elseIfBranch.EditBranch(); }));
        }

        public bool CanRemoveBranch(BranchVM branch)
        {
            if (EditingBranch != null)
            {
                return false;
            }
            return !branch.IsTypeIf;
        }

        public void RemoveBranch(BranchVM branch)
        {
            Branches.Remove(branch);
            OnBranchOrderChanged();
            if (editingBranch != null)
            {
                editingBranch.OnOrderChanged();
            }
        }


        public IfThenElse IfThenElse { get; set; }

        public void Save()
        {
            IfThenElse = SequenceUtils.CreateIfThenElse(ValidBranches);
        }


        private List<BranchVM> ValidBranches
        {
            get
            {
                List<BranchVM> validBranches = new List<BranchVM>();
                foreach (BranchVM branch in branches)
                {
                    if (branch.IsValid)
                    {
                        validBranches.Add(branch);
                    }
                }
                return validBranches;
            }
        }

        public void SubmitEditingBranch()
        {
            if (selectedBranch == null || editingBranch == null)
            {
                return;
            }

            int index = branches.IndexOf(selectedBranch);
            branches.RemoveAt(index);
            branches.Insert(index, editingBranch);
            editingBranch.IsNew = false;

            selectedBranch = null;
            EditingBranch = null;

            OnBranchOrderChanged(); // 分岐追加ボタンを表示するために必要(ELSE節の追加)
        }

        public void CancelEditingBranch()
        {
            selectedBranch = null;
            EditingBranch = null;
        }

        public bool CanChangeType(BranchVM branch)
        {
            if (!branch.IsNew)
            {
                return false;
            }
            if (branch.IsTypeIf)
            {
                return false;
            }
            foreach (BranchVM b in branches)
            {
                if (b.IsTypeElse)
                {
                    return false;
                }
            }
            return true;
        }

    }
}
