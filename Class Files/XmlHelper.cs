using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files
{
    public class XmlHelper
    {
        public static T DeserializeXmlFileToObject<T>(string xmlFilename)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(xmlFilename)) return default(T);

            try
            {
                StreamReader xmlStream = new StreamReader(xmlFilename);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                returnObject = (T)serializer.Deserialize(xmlStream);
            }
            catch (Exception ex)
            {
                
            }
            return returnObject;
        }
    }
}
