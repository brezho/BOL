using System.Xml.Linq;

namespace Core.Documents.Internal
{
    class OfficeCell
    {
        public XElement CellElement { get; set; }
        public string Row { get; set; }
        public string Column { get; set; }
        public string ColumnId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Formula { get; set; }
        public string SharedString { get; set; }
        public OfficeRow Parent { get; set; }
        public OfficeCell(OfficeRow parent) { Parent = parent; }
    }
}
