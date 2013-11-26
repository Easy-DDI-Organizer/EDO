using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace EDO.Core.Util
{
    public static class CollectionExtender
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, ICollection<T> addCollection)
        {
            foreach (var addItem in addCollection)
            {
                collection.Add(addItem);
            }
        }
    }
}
