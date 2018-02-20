using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core.Documents.Internal;

namespace DocumentFormat.OpenXml.Packaging
{
     static class SpreadsheetDocumentExtensions
    {
        internal static IEnumerable<OfficeTable> Tables(this SpreadsheetDocument spreadsheet)
        {
            foreach (var worksheetPart in spreadsheet.WorkbookPart.WorksheetParts)
                foreach (var table in worksheetPart.TableDefinitionParts)
                {
                    XDocument tableDefDoc = table.GetXDocument();

                    OfficeTable t = new OfficeTable(worksheetPart)
                    {
                        Id = (int)tableDefDoc.Root.Attribute("id"),
                        TableName = (string)tableDefDoc.Root.Attribute("name"),
                        DisplayName = (string)tableDefDoc.Root.Attribute("displayName"),
                        Ref = (string)tableDefDoc.Root.Attribute("ref"),
                        TotalsRowCount = (int?)tableDefDoc.Root.Attribute("totalsRowCount"),
                        //HeaderRowCount = (int?)tableDefDoc.Root.Attribute("headerRowCount"),
                        HeaderRowCount = 1,  // currently there always is a header row
                        TableType = (string)tableDefDoc.Root.Attribute("tableType"),
                        TableDefinitionPart = table
                    };
                    yield return t;
                }
        }

        internal static OfficeTable Table(this SpreadsheetDocument spreadsheet,
            string tableName)
        {
            return spreadsheet.Tables().Where(t => t.TableName.ToLower() == tableName.ToLower()).FirstOrDefault();
        }
    }
}
