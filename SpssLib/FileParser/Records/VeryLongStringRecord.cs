﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpssLib.FileParser.Records
{
    public class VeryLongStringRecord
    {
        private InfoRecord record;

        internal VeryLongStringRecord(InfoRecord record)
        {
            if (record.SubType != 14 || record.ItemSize != 1)
                throw new UnexpectedFileFormatException();
            this.record = record;

            this.LongStringDictionary = new Dictionary<string, string>();

            // Not very efficient, but for now the best way I can come up with
            //   without sacrificing the Inforecords-design.
            var originalBytes = (from item in this.record.Items where item[0] != 0 select item[0]).ToArray();
            var dictionaryString = Encoding.ASCII.GetString(originalBytes);

            // split on tabs:
            var entries = dictionaryString.Split('\t');

            foreach (var entry in entries)
            {
                var values = entry.Split('=');
                this.LongStringDictionary.Add(values[0], values[1]);
            }

        }
        public Dictionary<string, string> LongStringDictionary
        {
            get;
            private set;
        }
    }
}
