
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public class Option :ICloneable
    {
        public static Option Find(IEnumerable<Option> options, string code)
        {
            foreach (Option option in options)
            {
                if (option.Code == code)
                {
                    return option;
                }
            }
            return null;
        }

        public static T FindByLabel<T>(IEnumerable<T> options, string label) where T : Option
        {
            foreach (T option in options)
            {
                if (option.Label == label)
                {
                    return option;
                }
            }
            return null;
        }

        public static string FindLabel(IEnumerable<Option> options, string code)
        {
            Option option = Find(options, code);
            if (option == null)
            {
                return null;
            }
            return option.Label;
        }

        public static string FindCodeByLabel(IEnumerable<Option> options, string label)
        {
            Option option = FindByLabel(options, label);
            if (option == null)
            {
                return null;
            }
            return option.Code;
        }

        public Option() :this(null, null)
        {
        }
        public Option(string code, string label)
        {
            this.Code = code;
            this.Label = label;
        }

        public string Code { get; set;  }
        public string Label { get; set;  }

        public override string ToString()
        {
            return string.Format("#{0},{1}#", Code, Label);
        }

        public string Info
        {
            get
            {
                return string.Format("{0}: {1}", Code, Label);
            }
        }

        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }


        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Option option = obj as Option;
            if (option == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.Code == option.Code) && (this.Label == option.Label);
        }

 
        public override int GetHashCode()
        {
            return this.Code.GetHashCode() ^ this.Label.GetHashCode();
        }
        #endregion
    }
}
