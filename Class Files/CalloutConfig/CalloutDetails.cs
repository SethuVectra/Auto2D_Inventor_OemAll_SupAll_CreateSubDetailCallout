using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{

    [XmlRoot(ElementName = "CalloutDetails")]
    public class CalloutDetails
    {

        [XmlElement(ElementName = "Oem")]
        public List<Oem> Oem { get; set; }

        [XmlIgnore]
        public Dictionary<string, Oem> Data { get; set; }

        public void Update()
        {
            Data = new Dictionary<string, Oem>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in Oem)
            {
                item.Update();
                Data.Add(item.Name, item);
            }
        }
    }
}
