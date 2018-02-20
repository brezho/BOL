using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core.Documents.Internal;

namespace DocumentFormat.OpenXml.Packaging
{
    public static class WorksheetPartExtensions
    {
        internal static IEnumerable<OfficeRow> Rows(this WorksheetPart worksheetPart)
        {
            XNamespace s = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            return
                from row in worksheetPart
                    .GetXDocument()
                    .Root
                    .Element(s + "sheetData")
                    .Elements(s + "row")
                select new OfficeRow(worksheetPart)
                {
                    RowElement = row,
                    RowId = (string)row.Attribute("r"),
                    Spans = (string)row.Attribute("spans")
                };
        }
    }
}
