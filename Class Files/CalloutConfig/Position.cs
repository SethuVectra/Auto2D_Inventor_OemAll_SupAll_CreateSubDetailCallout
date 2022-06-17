using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Auto2D_Inventor_OemAll_SupAll_CreateSubDetailCallout.Class_Files.CalloutConfig
{
    [XmlRoot(ElementName = "Position")]
    public class Position
    {
        [XmlElement(ElementName = "PositionItem")]
        public List<PositionItem> PositionItem { get; set; }


        [XmlIgnore] private Dictionary<string, PositionItem> _items;

        public double[] GetPosition(string sheetSize)
        {
            if (_items == null)
            {
                _items = new Dictionary<string, PositionItem>();
                foreach (var item in PositionItem)
                    _items.Add(item.Name, item);
            }
            return _items.ContainsKey(sheetSize) ? (new double[] { _items[sheetSize].X, _items[sheetSize].Y }) : (new double[] { 0, 0 });
        }
    }
}
