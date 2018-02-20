using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace Core.Documents.Internal
{
    class OfficeRow
    {
        public XElement RowElement { get; set; }
        public string RowId { get; set; }
        public string Spans { get; set; }
        public IEnumerable<OfficeCell> Cells()
        {
            XNamespace s = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            SpreadsheetDocument doc = (SpreadsheetDocument)Parent.OpenXmlPackage;
            SharedStringTablePart sharedStringTable = doc.WorkbookPart.SharedStringTablePart;
            return
                from cell in this.RowElement.Elements(s + "c")
                let cellType = (string)cell.Attribute("t")
                let sharedString = cellType == "s" ?
                    sharedStringTable
                    .GetXDocument()
                    .Root
                    .Elements(s + "si")
                    .Skip((int)cell.Element(s + "v"))
                    .First()
                    .Descendants(s + "t")
                    .StringConcatenate(e => (string)e)
                    : null
                let column = (string)cell.Attribute("r")
                select new OfficeCell(this)
                {
                    CellElement = cell,
                    Row = (string)RowElement.Attribute("r"),
                    Column = column,
                    ColumnId = column.Split('0', '1', '2', '3', '4', '5', '6', '7', '8', '9').First(),
                    Type = (string)cell.Attribute("t"),
                    Formula = (string)cell.Element(s + "f"),
                    Value = (string)cell.Element(s + "v"),
                    SharedString = sharedString
                };
        }
        public WorksheetPart Parent { get; set; }
        public OfficeRow(WorksheetPart parent) { Parent = parent; }
    }

}
