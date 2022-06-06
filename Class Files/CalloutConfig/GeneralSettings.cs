using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{
    [XmlRoot(ElementName = "GeneralSettings")]
    public class GeneralSettings
    {

        [XmlAttribute(AttributeName = "BalloonStyle")]
        public string BalloonStyle { get; set; }

        [XmlAttribute(AttributeName = "PartListType")]
        public string PartListType { get; set; }

        [XmlAttribute(AttributeName = "BorderOffset")]
        public int BorderOffset { get; set; }
    }

}
