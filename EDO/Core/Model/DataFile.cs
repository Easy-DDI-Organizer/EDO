using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class DataFile :IIDPropertiesProvider
    {
        public static DataFile Find(List<DataFile> dataFiles, string id)
        {
            foreach (DataFile dataFile in dataFiles)
            {
                if (dataFile.Id == id)
                {
                    return dataFile;
                }
            }
            return null;
        }

        public static DataFile createDataFile()
        {
            DataFile dataFile = new DataFile();
            dataFile.Charset = "UTF-8";
            dataFile.Format = "Delimited";
            dataFile.DelimiterCode = "1";
            return dataFile;
        }

        public static void ChangeDataSetId(List<DataFile> dataFiles, string oldId, string newId)
        {
            foreach (DataFile dataFile in dataFiles)
            {
                if (dataFile.DataSetId == oldId)
                {
                    dataFile.DataSetId = newId;
                }
            }
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id", "GrossRecordStructureId", "RecordLayoutId", "PhysicalRecordSegment", "PhysicalInstanceId", "DataFileIdentificationId", "GrossFileStructureId" };
            }
        }

        public DataFile()
        {
            Id = IDUtils.NewGuid();
            GrossRecordStructureId = IDUtils.NewGuid();
            RecordLayoutId = IDUtils.NewGuid();
            PhysicalRecordSegment = "Segment_" + IDUtils.NewGuid();
            PhysicalInstanceId = IDUtils.NewGuid();
            DataFileIdentificationId = IDUtils.NewGuid();
            GrossFileStructureId = IDUtils.NewGuid();
        }
        public string Id { get; set; }
        public string DataSetId { get; set; }
        public string Uri { get; set; }
        public string Charset { get; set; }
        public string Format { get; set; }
        public string DelimiterCode { get; set; }

        public string GrossRecordStructureId { get; set; } // for DDI
        public string RecordLayoutId { get; set; } // for DDI
        public string PhysicalRecordSegment { get; set; } // for DDI
        public string PhysicalInstanceId { get; set; } // for DDI
        public string DataFileIdentificationId { get; set; } // for DDI
        public string GrossFileStructureId { get; set; } // for DDI
    }
}
