using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Registry.Storage;

namespace X.Registry
{
    public class Actual
    {
        Store store;
        SchemaData schema;
        public Actual(string dir)
        {
            store = new X.Registry.Storage.Store(dir);
            schema = GetSchema();
        }

        SchemaData GetSchema()
        {
            var schemaData = store.GetBytes(1);
            if (schemaData == null)
            {
                schema = new SchemaData() { Store = this.store, KeyId = 1 };
                store.Set(1, schema.ToBytes());
            }
            else schema = PersistableBase.From<SchemaData>(this.store, schemaData);
            return schema;
        }
    }

    public class SchemaData : PersistableBase
    {
        Dictionary<string, int> AllTypes = new Dictionary<string, int>();

        protected override void WriteThis(BinaryWriter writer)
        {
            writer.Write(AllTypes.Count);
            foreach (var it in AllTypes)
            {
                writer.Write(it.Key);
                writer.Write(it.Value);
            }
        }

        protected override void ReadThis(BinaryReader reader)
        {
            switch (this.Version)
            {
                case 0:
                    int cnt = reader.ReadInt32();
                    for (int i = 0; i < cnt; i++)
                    {
                        AllTypes.Add(reader.ReadString(), reader.ReadInt32());
                    }
                    break;
            }
        }

        internal int GetTypeId(Type type)
        {
            int res;
            if (AllTypes.TryGetValue(type.FullName, out res)) return res;
            else
            {
                res = AllTypes.Count;
                AllTypes[type.FullName] = res;
                Save();
            }
            return res;
        }
    }
    public abstract class PersistableBase
    {
        public Store Store;
        public int KeyId = int.MinValue;
        public int Version;
        public int ClassId;
        public byte[] ToBytes()
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(KeyId);
                writer.Write(Version);
                WriteThis(writer);
                return ms.ToArray();
            }
        }

        internal static T From<T>(Store store, byte[] data) where T : PersistableBase, new()
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new BinaryReader(ms))
            {
                var res = new T();
                res.KeyId = reader.ReadInt32();
                res.Version = reader.ReadInt32();
                res.Store = store;
                res.ReadThis(reader);
                return res;
            }
        }

        public struct PersistableReference
        {
            public static PersistableReference Empty = new PersistableReference();
            public int TargetKeyId;
            public string Data;
        }

        protected abstract void WriteThis(BinaryWriter writer);
        protected abstract void ReadThis(BinaryReader reader);

        public int Save()
        {
            if (this.KeyId == int.MinValue) this.KeyId = this.Store.Add(this.ToBytes());
            else this.Store.Set(this.KeyId, this.ToBytes());
            return this.KeyId;
        }
    }
    public class KeyDTO : PersistableBase
    {
        public string Name;
        public HashSet<PersistableReference> ChildrenRefs;
        public HashSet<PersistableReference> Properties;
        protected override void WriteThis(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(ChildrenRefs.Count);
            foreach (var it in ChildrenRefs)
            {
                writer.Write(it.TargetKeyId);
                writer.Write(it.Data);
            }
            writer.Write(Properties.Count);
            foreach (var it in Properties)
            {
                writer.Write(it.TargetKeyId);
                writer.Write(it.Data);
            }
        }

        protected override void ReadThis(BinaryReader reader)
        {
            Name = reader.ReadString();
            int cnt = reader.ReadInt32();
            for (int i = 0; i < cnt; i++)
            {
                ChildrenRefs.Add(new PersistableReference { TargetKeyId = reader.ReadInt32(), Data = reader.ReadString() });
            }
            cnt = reader.ReadInt32();
            for (int i = 0; i < cnt; i++)
            {
                Properties.Add(new PersistableReference { TargetKeyId = reader.ReadInt32(), Data = reader.ReadString() });
            }
        }
    }

}
