using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDO.Main;

namespace EDO.Core.ViewModel
{
    public class WriteError :IOError
    {
        public WriteError(string message, EDOUnitVM edoUnit, FormVM form)
        {
            Message = message;
            EDOUnit = edoUnit;
            Form = form;
        }
        public EDOUnitVM EDOUnit { get; set; }
        public FormVM Form { get; set; }

        public override bool Equals(object obj)
        {
            WriteError other = obj as WriteError;
            if (other == null)
            {
                return false;
            }
            return Message == other.Message && EDOUnit == other.EDOUnit && Form == other.Form;
        }

        public override int GetHashCode()
        {
            return Message.GetHashCode() ^ EDOUnit.GetHashCode() ^ Form.GetHashCode();
        }

        public override string ToString()
        {
            MenuItemVM menuItem = EDOUnit.FindMenuItem(Form);

            StringBuilder buf = new StringBuilder();
            buf.Append(EDOUnit.Title);
            buf.Append("-");
            buf.Append(menuItem.Title);
            buf.Append("-");
            buf.Append(Message);
            return buf.ToString();
        }
    }
}
