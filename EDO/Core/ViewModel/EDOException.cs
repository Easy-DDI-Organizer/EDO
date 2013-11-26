using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.ViewModel
{
    public class EDOException :Exception
    {
        public EDOException(List<IOError> errors)
        {
            this.errors = new List<IOError>(errors);
        }
        private List<IOError> errors;
        public List<WriteError> WriteErrors { 
            get {
                List<WriteError> writeErrors = new List<WriteError>();
                foreach (IOError error in errors)
                {
                    WriteError writeError = error as WriteError;
                    if (writeError != null)
                    {
                        writeErrors.Add(writeError);
                    }
                }
                return writeErrors;
            } 
        }
    }
}
