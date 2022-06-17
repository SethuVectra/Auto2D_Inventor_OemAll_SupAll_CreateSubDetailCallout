using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{
    [XmlRoot(ElementName = "PositionItem")]
    public class PositionItem
    {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "X")]
        public int X { get; set; }

        [XmlAttribute(AttributeName = "Y")]
        public int Y { get; set; }
    }
}
