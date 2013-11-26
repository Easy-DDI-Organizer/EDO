using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpssLib.FileParser;
using System.IO;

namespace SpssLib.SpssDataset
{
    public class SpssDataset
    {
        public VariablesCollection Variables { get; private set; }
        public RecordCollection Records { get; private set; }

        internal double SysmisValue { get; private set; }

        public SpssDataset()
        {
            this.Variables = new VariablesCollection();
            this.Records = new RecordCollection();
        }

        public SpssDataset(SavFileParser fileReader)
            : this()
        {
            foreach (var variable in fileReader.Variables)
            {
                this.Variables.Add(variable);
            }

            foreach (var dataRecord in fileReader.ParsedDataRecords)
            {
                this.Records.Add(new Record(dataRecord.ToArray(), this));
            }
        }

        public SpssDataset(Stream fileStream)
            : this(new SavFileParser(fileStream))
        {
        }
    }
}
