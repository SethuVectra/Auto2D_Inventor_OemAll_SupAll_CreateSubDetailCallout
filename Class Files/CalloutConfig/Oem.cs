using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{

    [XmlRoot(ElementName = "Oem")]
    public class Oem
    {

        [XmlElement(ElementName = "Supplier")]
        public List<Supplier> Supplier { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        //public Dictionary<string, Supplier> Data { get; set; }

        //public void Update()
        //{
        //    foreach (var item in Supplier)
        //    {
        //        item.Update();
        //        Data.Add(item.Name, item);
        //    }
        //}
    }
}
