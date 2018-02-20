using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Registry
{
    class PropertiesList : Dictionary<string, Property>
    {
        KeyStore _registry;
        int _pointer;
        int _ownerPointer;
        public PropertiesList(KeyStore registry, int pointer, int ownerPointer)
        {
            _registry = registry;
            _pointer = pointer;
            _ownerPointer = ownerPointer;
        }

        public PropertiesList(KeyStore registry, BinaryReader reader)
        {
            _ownerPointer = reader.ReadInt32();
            var cnt = reader.ReadInt32();
            for (var i = 0; i < cnt; i++)
            {
                var propName = reader.ReadString();
                var it = new Property();
                it.Kind = (PropertyKind)reader.ReadUInt16();
                switch (it.Kind)
                {
                    case PropertyKind.String:
                        it.Value = reader.ReadString();
                        break;
                    case PropertyKind.Int:
                        it.Value = reader.ReadInt32();
                        break;
                    case PropertyKind.Binary:
                        var len = reader.ReadInt32();
                        it.Value = reader.ReadBytes(len);
                        break;
                    case PropertyKind.DateTime:
                        it.Value = FromUnixTime(reader.ReadInt64());
                        break;
                }
                this[propName] = it;
            }
        }

        internal void Pack(BinaryWriter writer)
        {
            writer.Write(_ownerPointer);
            writer.Write(Count);
            foreach (var prop in this)
            {
                writer.Write(prop.Key);
                writer.Write((ushort)prop.Value.Kind);
                switch (prop.Value.Kind)
                {
                    case PropertyKind.String:
                        writer.Write((string)prop.Value.Value);
                        break;
                    case PropertyKind.Int:
                        writer.Write((int)prop.Value.Value);
                        break;
                    case PropertyKind.Binary:
                        var val = (byte[])prop.Value.Value;
                        writer.Write(val.Length);
                        writer.Write(val);
                        break;
                    case PropertyKind.DateTime:
                        var dtVal = ToUnixTime((DateTime)prop.Value.Value);
                        writer.Write(dtVal);
                        break;
                }
            }
        }

        DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        public long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }
    }
}
