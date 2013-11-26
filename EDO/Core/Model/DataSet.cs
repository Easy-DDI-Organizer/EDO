using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Core.Util;

namespace EDO.Core.Model
{
    public class DataSet :IIDPropertiesProvider
    {
        public static DataSet Find(List<DataSet> dataSets, string dataSetId)
        {
            foreach (DataSet dataSet in dataSets)
            {
                if (dataSet.Id == dataSetId)
                {
                    return dataSet;
                }
            }
            return null;
        }

        public static void ChangeVariableId(List<DataSet> dataSets, string oldId, string newId)
        {
            foreach (DataSet dataSet in dataSets)
            {
                for (int i = 0; i < dataSet.VariableGuids.Count; i++)
                {
                    if (dataSet.VariableGuids[i] == oldId)
                    {
                        dataSet.VariableGuids[i] = newId;
                    }
                }
            }
        }

        public string[] IdProperties
        {
            get
            {
                return new string[] { "Id" };
            }
        }

        public DataSet()
        {
            Id = IDUtils.NewGuid();
            VariableGuids = new List<string>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Memo { get; set; }
        public List<string> VariableGuids { get; set; }
        public bool IsCreatedDataFile { get; set; }
    }
}
