using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EDO.Core.ViewModel;
using EDO.Main;
using EDO.Core.Model;
using System.Collections.Specialized;
using EDO.Core.Util;
using EDO.Properties;

namespace EDO.DataCategory.DataSetForm
{
    public class DataSetVM :BaseVM, IStringIDProvider
    {
        public static DataSetVM Find(ICollection<DataSetVM> dataSets, string dataSetId)
        {
            if (dataSetId == null)
            {
                return null;
            }
            foreach (DataSetVM dataSet in dataSets)
            {
                if (dataSet.Id == dataSetId)
                {
                    return dataSet;
                }
            }
            return null;
        }

        public static List<DataSetVM> FindByVariableId(ICollection<DataSetVM> dataSets, string variableId)
        {
            List<DataSetVM> results = new List<DataSetVM>();
            foreach (DataSetVM dataSet in dataSets)
            {
                if (dataSet.FindVariable(variableId) != null)
                {
                    results.Add(dataSet);
                }
            }
            return results;
        }


        public static IEnumerable<string> GetIds(List<DataSetVM> dataSets)
        {
            return dataSets.Select(p => p.Id);
        }

        public DataSetVM() :this(new DataSet(), new ObservableCollection<DataSetVariableVM>())
        {
        }

        public DataSetVM(DataSet dataSet, ObservableCollection<DataSetVariableVM> dataSetVariables)
        {
            this.dataSet = dataSet;
            variables = new ObservableCollection<DataSetVariableVM>();
            int i = 1;
            foreach (DataSetVariableVM variable in dataSetVariables)
            {
                variable.Parent = this;
                variable.Position = i++;
                variables.Add(variable);
            }
            modelSyncer = new ModelSyncher<DataSetVariableVM, string>(this, variables, dataSet.VariableGuids);
            modelSyncer.AddActionHandler = (param) => { param.Position = NextPosition; };
        }

        private int NextPosition
        {
            get
            {
                return DataSetVariableVM.GetMaxPosition(variables) + 1;
            }
        }

        private ModelSyncher<DataSetVariableVM, string> modelSyncer;

        private DataSet dataSet;

        public DataSet DataSet { get { return dataSet; } }

        public override object Model {get {return dataSet; }}

        public string Id { get { return dataSet.Id; } }

        public bool IsCreatedDataFile
        {
            get
            {
                return dataSet.IsCreatedDataFile;
            }
            set
            {
                dataSet.IsCreatedDataFile = value;
            }
        }

        private ObservableCollection<DataSetVariableVM> variables;
        public ObservableCollection<DataSetVariableVM> Variables { get { return variables; } }

        public string Title
        {
            get
            {
                return dataSet.Title;
            }
            set
            {
                if (dataSet.Title != value)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        value = Resources.UntitledDataSet; //無題のデータセット;
                    }
                    dataSet.Title = value;
                    NotifyPropertyChanged("Title");
                    Memorize();
                }
            }
        }

        public string Memo
        {
            get
            {
                return dataSet.Memo;
            }
            set
            {
                if (dataSet.Memo != value)
                {
                    dataSet.Memo = value;
                    NotifyPropertyChanged("Memo");
                    Memorize();
                }
            }
        }

        public bool IsExistVariable(string variableId)
        {
            return FindVariable(variableId) != null;
        }

        public DataSetVariableVM FindVariable(string variableId)
        {
            return DataSetVariableVM.Find(Variables, variableId);
        }

        public void RemoveVariable(string variableId)
        {
            DataSetVariableVM variable = FindVariable(variableId);
            if (variable != null)
            {

                Variables.Remove(variable);
            }
        }


    }
}
