using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using EDO.Core.ViewModel;
using EDO.Core.View;
using System.Windows;
using EDO.Main;
using EDO.Core.Model;
using EDO.DataCategory.DataSetForm;
using System.Collections.Specialized;
using EDO.Core.Util;

namespace EDO.DataCategory.DataFileForm
{
    public class DataFileFormVM :FormVM
    {
        public DataFileFormVM(StudyUnitVM studyUnit) :base(studyUnit)
        {
            dataFiles = new ObservableCollection<DataFileVM>();
            foreach (DataFile dataFileModel in studyUnit.DataFileModels)
            {
                DataSetVM dataSet = studyUnit.FindDataSet(dataFileModel.DataSetId);
                if (dataSet != null)
                {
                    DataFileVM dataFile = createDataFile(dataFileModel, dataSet);
                    dataFiles.Add(dataFile);
                }
            }
            modelSyncher = new ModelSyncher<DataFileVM, DataFile>(this, dataFiles, studyUnit.DataFileModels);
        }

        private ModelSyncher<DataFileVM, DataFile> modelSyncher;

        private ObservableCollection<DataFileVM> dataFiles;
        public ObservableCollection<DataFileVM> DataFiles { get { return dataFiles; } }

        private DataFileVM createDataFile(DataFile dataFileModel, DataSetVM dataSet)
        {
            DataFileVM dataFile = new DataFileVM(dataFileModel, dataSet);
            dataFile.Parent = this;
            dataFile.Delimiters = Options.Delimiters;
            return dataFile;
        }

        protected override void Reload(VMState state)
        {
            if (state != null)
            {
                SelectedDataFile = EDOUtils.Find(dataFiles, state.State1);
            }
            if (SelectedDataFile == null)
            {
                SelectedDataFile = EDOUtils.GetFirst(dataFiles);
            }
        }

        public override VMState SaveState()
        {
            if (SelectedDataFile == null)
            {
                return null;
            }
            return new VMState(SelectedDataFile.Id);
        }

        private DataFileVM selectedDataFile;

        public DataFileVM SelectedDataFile
        {
            get
            {
                return selectedDataFile;
            }
            set
            {
                if (selectedDataFile != value)
                {
                    selectedDataFile = value;
                    NotifyPropertyChanged("SelectedDataFile");
                }
            }
        }

        private DataFileVM FindByDataSetId(string dataSetId)
        {
            return DataFileVM.FindByDataSetId(this.DataFiles, dataSetId);
        }

        public bool IsExistByDataSetId(string dataSetId)
        {
            return  FindByDataSetId(dataSetId) != null;
        }

        public void RemoveByDataSetId(string dataSetId)
        {
            DataFileVM dataFile = FindByDataSetId(dataSetId);
            if (dataFile == null)
            {
                return;
            }
            dataFiles.Remove(dataFile);
        }

        public void CreateDetaFiles(ObservableCollection<DataSetVM> dataSets)
        {
            foreach (DataSetVM dataSet in dataSets)
            {
                if (dataSet.IsCreatedDataFile)
                {
                    continue;
                }
                if (IsExistByDataSetId(dataSet.Id))
                {
                    continue;
                }
                dataSet.IsCreatedDataFile = true;
                DataFile dataFileModel = DataFile.createDataFile();
                dataFileModel.DataSetId = dataSet.Id;
                DataFileVM dataFile = createDataFile(dataFileModel, dataSet);
                dataFiles.Add(dataFile);
            }
        }
    }
}
