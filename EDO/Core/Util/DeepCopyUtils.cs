using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;

namespace EDO.Core.Util
{
    public static class DeepCopyUtils
    {
        public static T DeepCopy<T>(T target)
        {

            T result;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
 

            MemoryStream mem = new MemoryStream();
            try
            {
                serializer.Serialize(mem, target);
                mem.Position = 0;
                result = (T)serializer.Deserialize(mem);
            }
            finally
            {
                mem.Close();
            }
            return result;
        }
    }
}
