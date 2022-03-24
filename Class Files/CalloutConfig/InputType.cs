using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{
    
    [XmlRoot(ElementName = "InputType")]
    public class InputType
    {

        [XmlElement(ElementName = "TableSettings")]
        public TableSettings TableSettings { get; set; }

        [XmlElement(ElementName = "TableData")]
        public TableData TableData { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }
}
