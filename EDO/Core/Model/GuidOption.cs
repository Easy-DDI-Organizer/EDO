using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Model
{
    public class GuidOption :ICloneable
    {
        public GuidOption(string id, string label)
        {
            Id = id;
            Label = label;
        }

        public string Id { get; set; }
        public string Label { get; set; }


        #region ICloneable メンバー

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            GuidOption option = obj as GuidOption;
            if (option == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.Id == option.Id) && (this.Label == option.Label);
        }


        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Label.GetHashCode();
        }
    }
}
