
namespace Core.Documents.Internal
{
    class OfficeTableColumn
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? FormatId { get; set; }  // dataDxfId
        public int? QueryTableFieldId { get; set; }
        public string UniqueName { get; set; }
        public int ColumnIndex { get; set; }
        public OfficeTable Parent { get; set; }
        public OfficeTableColumn(OfficeTable parent) { Parent = parent; }
    }
}
