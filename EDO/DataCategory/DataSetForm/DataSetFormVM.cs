using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using EDO.Core.View;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using EDO.Core.Util;
using EDO.QuestionCategory.ConceptForm;
using EDO.VariableCategory.VariableForm;
using EDO.DataCategory.DataFileForm;
using EDO.Properties;

namespace EDO.DataCategory.DataSetForm
{
    public class DataSetFormVM :FormVM
    {

        public DataSetFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            dataSets = new ObservableCollection<DataSetVM>();
            foreach (DataSet dataSetModel in studyUnit.DataSetModels)
            {
                ObservableCollection<DataSetVariableVM> variables = new ObservableCollection<DataSetVariableVM>();
                foreach (string guid in dataSetModel.VariableGuids)
                {
                    DataSetVariableVM v = createDataSetVariable(guid);
                    if (v != null)
                    {
                        variables.Add(v);
                    }
                }
                DataSetVM dataSet = new DataSetVM(dataSetModel, variables)
                {
                    Parent = this
                };
                dataSets.Add(dataSet);
            }
            modelSyncher = new ModelSyncher<DataSetVM, DataSet>(this, dataSets, studyUnit.DataSetModels);
        }

        private ModelSyncher<DataSetVM, DataSet> modelSyncher;

        private DataSetVariableVM createDataSetVariable(string variableId)
        {
            VariableVM variable = StudyUnit.FindVariable(variableId);
            if (variable == null)
            {
                return null;
            }
            return createDataSetVariable(variable);
        }

        private DataSetVariableVM createDataSetVariable(VariableVM variable)
        {
            ConceptVM concept = StudyUnit.FindConcept(variable.ConceptId);
            //if (concept == null)
            //{
            //    return null;
            //}
            DataSetVariableVM v = new DataSetVariableVM();
            v.Id = variable.Variable.Id;
            v.Title = variable.Title;
            v.Label = variable.Label;
            v.ConceptTitle = concept == null ? null : concept.Title;
            return v;
        }

        private ObservableCollection<DataSetVM> dataSets;
        public ObservableCollection<DataSetVM> DataSets { get { return dataSets; } }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedDataSet = EDOUtils.Find(dataSets, state.State1);
            }
            if (SelectedDataSet == null)
            {
                SelectedDataSet = EDOUtils.GetFirst(dataSets);
            }
        }

        public override VMState SaveState()
        {
            if (SelectedDataSet == null)
            {
                return null;
            }
            return new VMState(SelectedDataSet.Id);
        }

        private DataSetVM selectedDataSet;
        public DataSetVM SelectedDataSet
        {
            get
            {
                return selectedDataSet;
            }
            set
            {
                if (selectedDataSet != value)
                {
                    selectedDataSet = value;
                    NotifyPropertyChanged("SelectedDataSet");
                }
            }
        }

        public void AddDataSet()
        {
            InputDialog dlg = new InputDialog();
            dlg.Title = Resources.InputDataSetName; //データセットの名前を入力してください
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                DataSetVM dataSet = new DataSetVM() { Title = dlg.textBox.Text };
                dataSets.Add(dataSet);
                if (SelectedDataSet == null)
                {
                    SelectedDataSet = dataSet;
                }
                Memorize();
            }
        }

        private ICommand addVariableCommand;
        public ICommand AddVariableCommand
        {
            get
            {
                if (addVariableCommand == null)
                {
                    addVariableCommand = new RelayCommand(param => this.AddVariable(), param => this.CanAddVariable);
                }
                return addVariableCommand;
            }
        }

        public bool CanAddVariable
        {
            get
            {
                if (SelectedDataSet == null)
                {
                    return false;
                }
                if (StudyUnit.IsDefaultDataSet(SelectedDataSet))
                {
                    return false;
                }
                return true;
            }
        }

        public void AddVariable()
        {
            if (this.SelectedDataSet == null)
            {
                return;
            }
            SelectVariableWindow dlg = new SelectVariableWindow(this.StudyUnit);
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
            if (dlg.DialogResult == true)
            {
                ObservableCollection<VariableVM> variables = dlg.SelectedVariables;
                foreach (VariableVM variable in variables)
                {
                    DataSetVariableVM v = createDataSetVariable(variable);
                    if (v != null)
                    {
                        this.SelectedDataSet.Variables.Add(v);
                    }
                }
                Memorize();
            }
        }

        private ICommand removeDataSetCommand;
        public ICommand RemoveDataSetCommand
        {
            get
            {
                if (removeDataSetCommand == null)
                {
                    removeDataSetCommand = new RelayCommand(param => RemoveDataSet(), param => CanRemoveDataSet);
                }
                return removeDataSetCommand;
            }
        }

        public bool CanRemoveDataSet
        {
            get
            {
                if (SelectedDataSet == null)
                {
                    return false;
                }
                if (StudyUnit.IsDefaultDataSet(SelectedDataSet))
                {
                    return false;
                }
                return true;
            }
        }

        public void RemoveDataSet()
        {
            StudyUnit.RemoveDataSet(SelectedDataSet);
        }

        private ICommand removeVariableCommand;
        public ICommand RemoveVariableCommand
        {
            get
            {
                if (removeVariableCommand == null)
                {
                    removeVariableCommand = new RelayCommand(param => RemoveVariable(), param => CanRemoveVariable);
                }
                return removeVariableCommand;
            }
        }

        private bool CanOperateVariable
        {
            get
            {
                if (SelectedDataSet == null || SelectedVariable == null)
                {
                    return false;
                }
                if (StudyUnit.IsDefaultDataSet(SelectedDataSet))
                {
                    return false;
                }
                return true;
            }
        }

        public bool CanRemoveVariable
        {
            get
            {
                return CanOperateVariable;
            }
        }

        public void RemoveVariable()
        {
            SelectedDataSet.Variables.Remove(SelectedVariable);
            SelectedItem = null;
        }

        private int SelectedVariableIndex
        {
            get
            {
                if (SelectedDataSet == null || SelectedVariable == null)
                {
                    return -1;
                }
                return SelectedDataSet.Variables.IndexOf(SelectedVariable);
            }
        }

        private bool CanReorderVariable()
        {
            //ビューに問い合わせて、ソートされていたら順序変更不可能と判断する。
            DataSetFormView window = (DataSetFormView)View;
            if (window != null && window.IsDataGridSorting)
            {
                return false;
            }           
            return true;
        }

        private void RenumberPosition()
        {
            int i = 1;
            foreach (DataSetVariableVM variable in SelectedDataSet.Variables)
            {
                variable.Position = i++;
            }
        }

        private ICommand upVariableCommand;
        public ICommand UpVariableCommand
        {
            get
            {
                if (upVariableCommand == null)
                {
                    upVariableCommand = new RelayCommand(param => UpVariable(), param => CanUpVariable);
                }
                return upVariableCommand;
            }
        }

        public bool CanUpVariable
        {
            get
            {
                if (!CanOperateVariable)
                {
                    return false;
                }
                if (!CanReorderVariable())
                {
                    return false;
                }
                if (SelectedVariableIndex == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public void UpVariable()
        {
            DataSetVariableVM variable = SelectedVariable;
            int index = SelectedVariableIndex;
            SelectedDataSet.Variables.Move(index, index - 1);
            RenumberPosition();
            ((DataSetFormView)View).FocusCell();
        }

        private ICommand downVariableCommand;
        public ICommand DownVariableCommand
        {
            get
            {
                if (downVariableCommand == null)
                {
                    downVariableCommand = new RelayCommand(param => DownVariable(), param => CanDownVariable);
                }
                return downVariableCommand;
            }
        }

        public bool CanDownVariable
        {
            get
            {
                if (!CanOperateVariable)
                {
                    return false;
                }
                if (!CanReorderVariable())
                {
                    return false;
                }
                if (SelectedVariableIndex == SelectedDataSet.Variables.Count - 1)
                {
                    return false;
                }
                return true;
            }
        }

        public void DownVariable()
        {
            DataSetVariableVM variable = SelectedVariable;
            int index = SelectedVariableIndex;
            SelectedDataSet.Variables.Move(index, index + 1);
            RenumberPosition();
            ((DataSetFormView)View).FocusCell();
        }


        private object selectedItem;
        public object SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    NotifyPropertyChanged("SelectedItem");
                }
            }
        }

        public DataSetVariableVM SelectedVariable
        {
            get
            {
                return (DataSetVariableVM)SelectedItem;
            }
        }

        private DataSetVM FindDefaultDataSet()
        {
            string id = this.StudyUnit.DefaultDataSetId;
            return FindDataSet(id);
        }

        public DataSetVM FindDataSet(string dataSetId)
        {
            return DataSetVM.Find(this.DataSets, dataSetId);
        }

        public List<DataSetVM> FindDataSetsByVariableId(string variableId)
        {
            return DataSetVM.FindByVariableId(DataSets, variableId);
        }

        public void CreateDataSets(ObservableCollection<VariableVM> variables)
        {
            foreach (DataSetVM dataSet in dataSets)
            {
                foreach (VariableVM variable in variables)
                {
                    DataSetVariableVM v = null;
                    bool shouldCreateVariable = (dataSet.Id == StudyUnit.DefaultDataSetId) &&
                        !variable.IsCreatedDataSet && !dataSet.IsExistVariable(variable.Id);
                    if (shouldCreateVariable)
                    {
                        v = new DataSetVariableVM();
                        v.Id = variable.Id;
                        variable.IsCreatedDataSet = true;
                        dataSet.Variables.Add(v);
                    }
                    else
                    {
                        v = dataSet.FindVariable(variable.Id);
                    }
                    if (v != null)
                    {
                        ConceptVM concept = StudyUnit.FindConcept(variable.ConceptId);
                        v.Title = variable.Title;
                        v.Label = variable.Label;
                        v.ConceptTitle = concept == null ? null : concept.Title;
                    }
                }
            }
        }

        public void RemoveVariable(VariableVM variable)
        {
            foreach (DataSetVM dataSet in dataSets)
            {
                dataSet.RemoveVariable(variable.Id);
            }
        }


        protected override Action GetCompleteAction(VMState state)
        {
            return () => { StudyUnit.CompleteDataSets(); };
        }

    }

}
