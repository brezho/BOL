using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace Core.Documents.Internal
{
    class OfficeTable
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string DisplayName { get; set; }
        public string Ref { get; set; }
        public int? HeaderRowCount { get; set; }
        public int? TotalsRowCount { get; set; }
        public string TableType { get; set; }  // external frameworkDatabase query, frameworkDatabase in worksheet, or XML frameworkDatabase
        public TableDefinitionPart TableDefinitionPart { get; set; }
        public WorksheetPart Parent { get; set; }
        public OfficeTable(WorksheetPart parent) { Parent = parent; }
        public IEnumerable<OfficeTableColumn> TableColumns()
        {
            XNamespace x = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            return TableDefinitionPart
                .GetXDocument()
                .Root
                .Element(x + "tableColumns")
                .Elements(x + "tableColumn")
                .Select((c, i) =>
                    new OfficeTableColumn(this)
                    {
                        Id = (int)c.Attribute("id"),
                        Name = (string)c.Attribute("name"),
                        FormatId = (int?)c.Attribute("dataDxfId"),
                        QueryTableFieldId = (int?)c.Attribute("queryTableFieldId"),
                        UniqueName = (string)c.Attribute("uniqueName"),
                        ColumnIndex = i,
                    }
                );
        }

        public IEnumerable<OfficeTableRow> TableRows()
        {
            string refStart = Ref.Split(':').First();
            int rowStart = Int32.Parse(LocalHelper.SplitAddress(refStart)[1]);
            string refEnd = Ref.Split(':').ElementAt(1);
            int rowEnd = Int32.Parse(LocalHelper.SplitAddress(refEnd)[1]);
            int headerRowsCount = HeaderRowCount == null ? 0 : (int)HeaderRowCount;
            int totalRowsCount = TotalsRowCount == null ? 0 : (int)TotalsRowCount;
            return Parent
                .Rows()
               // .Skip(headerRowsCount)
                .SkipLast(totalRowsCount)
                .Where(r =>
                {
                    int rowId = Int32.Parse(r.RowId);
                    return rowId >= rowStart && rowId <= rowEnd;
                }
                )
                .Select(r => new OfficeTableRow(this) { Row = r });
        }
    }
}
