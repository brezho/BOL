using System;
namespace DataTypes
{
    public class Archive : IEquatable<Archive>
    {
        public Archive(string titi, int tata, decimal tutu)
        {
            _titi = titi;
            _tata = tata;
            _tutu = tutu;
        }
        string _titi;
        int _tata;
        decimal _tutu;
        public bool Equals(Archive other)
        {
            if (other == null) return false;
            if (other._titi != _titi) return false;
            if (other._tata != _tata) return false;
            if (other._tutu != _tutu) return false;
            return true;
        }
        public static bool operator ==(Archive x, Archive y)
        {
            if ((object)x == null) return ((object)y == null);
            return x.Equals(y);
        }
        public static bool operator !=(Archive x, Archive y)
        {
            return !(x == y);
        }
        public override int GetHashCode()
        {
            int hashCode = 17;
            if (_titi != default(string)) hashCode = hashCode * 59 + _titi.GetHashCode();
            if (_tata != default(int)) hashCode = hashCode * 59 + _tata.GetHashCode();
            if (_tutu != default(decimal)) hashCode = hashCode * 59 + _tutu.GetHashCode();
            return hashCode;
        }
        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other.GetType() != this.GetType()) return false;
            return this.Equals((Archive)other);
        }
    }
}

