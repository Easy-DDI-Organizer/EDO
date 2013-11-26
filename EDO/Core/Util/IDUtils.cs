using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDO.Core.Util
{
    public static class IDUtils
    {
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static string ToId(string code, string suffix)
        {
            return code + "_" + suffix;
        }

        public static string ToCode(string id)
        {
            int index = id.IndexOf("_");
            if (index < 0)
            {
                return null;
            }
            return id.Substring(0, index);
        }
    }
}
