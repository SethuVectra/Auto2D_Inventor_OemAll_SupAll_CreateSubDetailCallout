using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{

    [XmlRoot(ElementName = "Division")]
    public class Division
    {

        [XmlElement(ElementName = "InputType")]
        public List<InputType> InputType { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlIgnore]
        public Dictionary<string, InputType> Data { get; set; }

        public void Update()
        {
            Data = new Dictionary<string, InputType>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in InputType)
            {
                Data.Add(item.Name, item);
            }
        }
    }
}
