using System;

namespace Core.Documents.Internal
{
    class OfficeTableCell : IEquatable<OfficeTableCell>
    {
        public string Value { get; set; }
        public OfficeTableCell(string v)
        {
            Value = v;
        }
        public override string ToString()
        {
            return Value;
        }
        public override bool Equals(object obj)
        {
            return this.Value == ((OfficeTableCell)obj).Value;
        }
        bool IEquatable<OfficeTableCell>.Equals(OfficeTableCell other)
        {
            return this.Value == other.Value;
        }
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
        public static bool operator ==(OfficeTableCell left, OfficeTableCell right)
        {
            if ((object)left != (object)right) return false;
            return left.Value == right.Value;
        }
        public static bool operator !=(OfficeTableCell left, OfficeTableCell right)
        {
            if ((object)left != (object)right) return false;
            return left.Value != right.Value;
        }
        public static explicit operator string(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return cell.Value;
        }
        public static explicit operator bool(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return cell.Value == "1";
        }
        public static explicit operator bool?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return cell.Value == "1";
        }
        public static explicit operator int(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return Int32.Parse(cell.Value);
        }
        public static explicit operator int?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return Int32.Parse(cell.Value);
        }
        public static explicit operator uint(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return UInt32.Parse(cell.Value);
        }
        public static explicit operator uint?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return UInt32.Parse(cell.Value);
        }
        public static explicit operator long(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return Int64.Parse(cell.Value);
        }
        public static explicit operator long?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return Int64.Parse(cell.Value);
        }
        public static explicit operator ulong(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return UInt64.Parse(cell.Value);
        }
        public static explicit operator ulong?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return UInt64.Parse(cell.Value);
        }
        public static explicit operator float(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return Single.Parse(cell.Value);
        }
        public static explicit operator float?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return Single.Parse(cell.Value);
        }
        public static explicit operator double(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return Double.Parse(cell.Value);
        }
        public static explicit operator double?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return Double.Parse(cell.Value);
        }
        public static explicit operator decimal(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return Decimal.Parse(cell.Value);
        }
        public static explicit operator decimal?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return Decimal.Parse(cell.Value);
        }
        public static implicit operator DateTime(OfficeTableCell cell)
        {
            if (cell == null) throw new ArgumentNullException("OfficeTableCell");
            return new DateTime(1900, 1, 1).AddDays(Int32.Parse(cell.Value) - 2);
        }
        public static implicit operator DateTime?(OfficeTableCell cell)
        {
            if (cell == null) return null;
            return new DateTime(1900, 1, 1).AddDays(Int32.Parse(cell.Value) - 2);
        }
    }

}
