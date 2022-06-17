using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{

    [XmlRoot(ElementName = "Supplier")]
    public class Supplier
    {

        [XmlElement(ElementName = "Division")]
        public List<Division> Division { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlIgnore]
        public Dictionary<string, Division> Data { get; set; }

        public void Update()
        {
            Data = new Dictionary<string, Division>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in Division)
            {
                item.Update();
                Data.Add(item.Name, item);
            }
        }
    }
}
