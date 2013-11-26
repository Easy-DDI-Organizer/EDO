using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.IO
{
    public class FileDialogResult
    {
        public FileDialogResult(string fileName, int filterIndex)
        {
            FileName = fileName;
            FilterIndex = filterIndex;
        }

        public string FileName { get; set; }
        public int FilterIndex { get; set; }
    }

}