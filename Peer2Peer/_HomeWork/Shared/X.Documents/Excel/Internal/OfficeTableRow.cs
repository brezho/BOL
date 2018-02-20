using System;
using System.Linq;

namespace Core.Documents.Internal
{
    class OfficeTableRow
    {
        public OfficeRow Row { get; set; }
        public OfficeTable Parent { get; set; }
        public OfficeTableRow(OfficeTable parent) { Parent = parent; }
        public OfficeTableCell this[string columnName]
        {
            get
            {
                OfficeTableColumn tc = Parent
                    .TableColumns()
                    .Where(x => x.Name.ToLower() == columnName.ToLower())
                    .FirstOrDefault();
                if (tc == null)
                    throw new Exception("Invalid column name: " + columnName);
                string[] refs = Parent.Ref.Split(':');
                string[] startRefs = LocalHelper.SplitAddress(refs[0]);
                string columnAddress = (startRefs[0].ColumnAddressToIndex() + tc.ColumnIndex).IndexToColumnAddress();
                OfficeCell cell = Row.Cells().Where(c => c.ColumnId == columnAddress).FirstOrDefault();
                if (cell != null)
                {
                    if (cell.Type == "s")
                        return new OfficeTableCell(cell.SharedString);
                    else
                        return new OfficeTableCell(cell.Value);
                }
                else
                    return new OfficeTableCell("");
            }
        }
    }
}
