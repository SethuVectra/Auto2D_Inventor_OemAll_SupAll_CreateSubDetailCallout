using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{

    [XmlRoot(ElementName = "TableData")]
    public class TableData
    {

        [XmlElement(ElementName = "PartListColumns")]
        public PartListColumns PartListColumns { get; set; }
    }
}
