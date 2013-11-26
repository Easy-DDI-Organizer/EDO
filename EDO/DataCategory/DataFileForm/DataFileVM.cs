using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.ViewModel;
using EDO.Core.Model;
using EDO.DataCategory.DataSetForm;
using System.Collections.ObjectModel;

namespace EDO.DataCategory.DataFileForm
{
    public class DataFileVM :BaseVM, IStringIDProvider
    {
        public static DataFileVM FindByDataSetId(ICollection<DataFileVM> dataFiles, string dataSetId)
        {
            foreach (DataFileVM dataFile in dataFiles)
            {
                if (dataFile.DataSetId == dataSetId)
                {
                    return dataFile;
                }
            }
            return null;
        }

        private DataFile dataFile;

        private DataSetVM dataSet;

        public DataFileVM()
            : this(new DataFile(), new DataSetVM())
        {
        }

        public DataFileVM(DataFile dataFile, DataSetVM dataSet)
        {
            this.dataFile = dataFile;
            this.dataSet = dataSet;
        }

        public DataFile DataFile { get { return dataFile; } }

        public override object Model { get { return dataFile; } }

        public string Id { get { return dataFile.Id; } }

        public string DataSetId { get { return dataSet.Id; } }


        public ObservableCollection<DataSetVariableVM> Variables { get { return dataSet.Variables; } }

        private ObservableCollection<Option> delimiters;
        public ObservableCollection<Option> Delimiters {
            get
            {
                return delimiters;
            }
            set
            {
                this.delimiters = value;
            }
        }

        public string Title
        {
            get
            {
                return dataSet.Title;
            }
        }

        public string Uri
        {
            get
            {
                return dataFile.Uri;
            }
            set
            {
                if (dataFile.Uri != value)
                {
                    dataFile.Uri = value;
                    NotifyPropertyChanged("Uri");
                    Memorize(); // ファイルの場所は変更可能
                }
            }
        }

        public string Charset
        {
            get
            {
                return dataFile.Charset;
            }
            set
            {
                if (dataFile.Charset != value)
                {
                    dataFile.Charset = value;
                    NotifyPropertyChanged("Charset");
                }
            }
        }

        public string Format
        {
            get
            {
                return dataFile.Format;
            }
            set
            {
                if (dataFile.Format != value)
                {
                    dataFile.Format = value;
                    NotifyPropertyChanged("Format");
                }
            }
        }

        public string DelimiterCode
        {
            get
            {
                return dataFile.DelimiterCode;

            }
            set
            {
                if (dataFile.DelimiterCode != value)
                {
                    dataFile.DelimiterCode = value;
                    NotifyPropertyChanged("DelimiterCode");
                    Memorize();
                }
            }
        }


    }
}
