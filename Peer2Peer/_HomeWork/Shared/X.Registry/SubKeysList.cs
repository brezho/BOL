using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Registry
{
    class SubKeysList : Dictionary<string, int>
    {
        KeyStore _registry;
        int _pointer;
        int _ownerPointer;

        public SubKeysList(KeyStore registry, int pointer, int ownerPointer)
        {
            _registry = registry;
            _pointer = pointer;
            _ownerPointer = ownerPointer;
        }

        public SubKeysList(KeyStore registry, BinaryReader reader)
        {
            _ownerPointer = reader.ReadInt32();
            var cnt = reader.ReadInt32();
            for (var i = 0; i < cnt; i++)
            {
                this.Add(reader.ReadString(), reader.ReadInt32());
            }
        }

        internal void Pack(BinaryWriter writer)
        {
            writer.Write(_ownerPointer);
            writer.Write(Count);
            foreach (var it in this)
            {
                writer.Write(it.Key);
                writer.Write(it.Value);
            };
        }
    }
}
