using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using X.Registry.Storage;

namespace X.Registry
{
    public class KeyStore : BinaryStore<int>
    {
        internal event EventHandler<Key> KeySet = (s, a) => { };
        const ulong counterPointer = 0;
        const ulong rootPointer = 0;
        public KeyStore(string location)
            : base(location)
        {
            CheckSetup();
        }

        bool IsSetup()
        {
            return ContainsKey(RootPointer);
        }
        void CheckSetup()
        {
            if (!IsSetup())
            {
                using (var tr = new TransactionScope())
                {
                    CreateKey("/", true);
                    tr.Complete();
                }
            }
        }

        internal Key CreateKey(string name, bool isRoot = false)
        {
            var pointer = Reserve(3);

            var newKey = new Key(this, name, pointer, pointer + 1, pointer + 2);
            var children = new SubKeysList(this, pointer + 1, pointer);
            var properties = new PropertiesList(this, pointer + 2, pointer);

            Set(pointer, newKey);
            Set(pointer + 1, children);
            Set(pointer + 2, properties);

            if (isRoot) Set(RootPointer, pointer);

            return newKey;
        }

        public Key GetRoot()
        {
            var rootPointer = GetInt32(RootPointer).Value;
            return GetKey(rootPointer);
        }


        internal void Set(int ptr, Key key)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                key.Pack(writer);
                this[ptr] = ms.ToArray();
                KeySet(this, key);
            }
        }
        internal Key GetKey(int ptr)
        {
            var b = this[ptr];
            if (b != null)
            {
                using (var ms = new MemoryStream(b))
                using (var reader = new BinaryReader(ms))
                {
                    return new Key(this, reader);
                }
            }
            return null;
        }

        internal void Set(int ptr, SubKeysList key)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                key.Pack(writer);
                this[ptr] = ms.ToArray();
            }
        }

        internal SubKeysList GetSubKeysList(int ptr)
        {
            var b = this[ptr];
            if (b != null)
            {
                using (var ms = new MemoryStream(b))
                using (var reader = new BinaryReader(ms))
                {
                    return new SubKeysList(this, reader);
                }
            }
            return null;
        }

        internal void Set(int ptr, PropertiesList key)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                key.Pack(writer);
                this[ptr] = ms.ToArray();
            }
        }

        internal PropertiesList GetPropertiesList(int ptr)
        {
            var b = this[ptr];
            if (b != null)
            {
                using (var ms = new MemoryStream(b))
                using (var reader = new BinaryReader(ms))
                {
                    return new PropertiesList(this, reader);
                }
            }
            return null;
        }

        internal int NextPointer()
        {
            return Reserve(1);
        }

        const int SequencePointer = 0;
        const int RootPointer = 1;
        const int MinSequence = 1048576;
        int Reserve(int number)
        {
            var start = GetInt32(SequencePointer) ?? MinSequence;
            Set(SequencePointer, start + number);
            return start;
        }

        public void Set(int ptr, int i)
        {
            this[ptr] = BitConverter.GetBytes(i);
        }
        public void Set(int ptr, string s)
        {
            this[ptr] = Encoding.UTF8.GetBytes(s);
        }
        public void Set(int ptr, long l)
        {
            this[ptr] = BitConverter.GetBytes(l);
        }

        static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public void Set(int ptr, DateTime dt)
        {
            DateTime dateTimeUtc = dt;
            long dtValue = 0;

            if (dt.Kind != DateTimeKind.Utc) dateTimeUtc = dt.ToUniversalTime();

            if (dateTimeUtc.ToUniversalTime() > UnixEpoch)
                dtValue = (long)(dateTimeUtc - UnixEpoch).TotalSeconds;

            Set(ptr, dtValue);
        }

        public int? GetInt32(int ptr)
        {
            var b = this[ptr];
            return (b != null) ? BitConverter.ToInt32(b, 0) : (int?)null;
        }

        public string GetString(int ptr)
        {
            var b = this[ptr];
            return (b != null) ? Encoding.UTF8.GetString(b) : (string)null;
        }

        public long? GetInt64(int ptr)
        {
            var b = this[ptr];
            return (b != null) ? BitConverter.ToInt64(b, 0) : (long?)null;
        }
        public DateTime? GetDateTime(int ptr, DateTimeKind kind = DateTimeKind.Utc)
        {
            var dtValue = GetInt64(ptr);
            if (!dtValue.HasValue) return (DateTime?)null;

            var res = UnixEpoch.AddSeconds(dtValue.Value);
            if (kind == DateTimeKind.Local) res = res.ToLocalTime();
            return res;
        }
    }
}
