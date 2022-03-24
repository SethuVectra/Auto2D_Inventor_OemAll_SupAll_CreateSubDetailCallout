using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{

    [XmlRoot(ElementName = "PartListColumn")]
    public class PartListColumn
    {

        [XmlAttribute(AttributeName = "Number")]
        public string Number { get; set; }

        [XmlAttribute(AttributeName = "HeaderText")]
        public string HeaderText { get; set; }

        [XmlAttribute(AttributeName = "SourceType")]
        public string SourceType { get; set; }

        [XmlAttribute(AttributeName = "SourceValue")]
        public string SourceValue { get; set; }
    }

}
