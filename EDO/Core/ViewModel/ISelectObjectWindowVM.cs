using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace EDO.Core.ViewModel
{
    public interface ISelectObjectWindowVM
    {
        object SelectedObject { get; set; }
        string DisplayMemberPath { get; set; }
        IEnumerable Objects { get; }
        void Filter(string text);
    }
}
