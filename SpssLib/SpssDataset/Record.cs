using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpssLib.SpssDataset
{
    public class Record
    {
        private object[] data;
        private SpssDataset dataset;

        internal Record(object[] data, SpssDataset dataset)
        {
            this.data = data;
            this.dataset = dataset;
        }

        public object this[int index]
        {
            get
            {
                return data[index];
            }
        }

        public object this[Variable variable]
        {
            get
            {
                return this[variable.Index];
            }
        }
    }
}
