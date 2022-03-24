using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{

	[XmlRoot(ElementName = "TableSettings")]
	public class TableSettings
	{

		[XmlAttribute(AttributeName = "BOMView")]
		public string BOMView { get; set; }

		[XmlAttribute(AttributeName = "Levels")]
		public string Levels { get; set; }

		[XmlAttribute(AttributeName = "Delimeter")]
		public string Delimeter { get; set; }

		[XmlAttribute(AttributeName = "Numbering")]
		public string Numbering { get; set; }

		[XmlAttribute(AttributeName = "MinDigits")]
		public int MinDigits { get; set; }

		[XmlAttribute(AttributeName = "TextWrapingDirection")]
		public string TextWrapingDirection { get; set; }

		[XmlAttribute(AttributeName = "AutomaticWrap")]
		public string AutomaticWrap { get; set; }

		[XmlAttribute(AttributeName = "MaximumRows")]
		public int MaximumRows { get; set; }

		[XmlAttribute(AttributeName = "NumberOfSections")]
		public int NumberOfSections { get; set; }
	}

}
