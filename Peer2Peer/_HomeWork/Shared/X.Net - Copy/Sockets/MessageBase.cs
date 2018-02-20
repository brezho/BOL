using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Net.Sockets
{
    public abstract class MessageBase
    {

        static List<Type> allMessagesTypes;
        static object locker = new object();

        static MessageBase()
        {
            lock (locker)
            {
                allMessagesTypes = typeof(MessageBase).Hype().GetMatchingTypes(x => x.IsConcrete()).OrderBy(x => x.Name).ToList();
            }
        }
        public abstract void Write(MessageWriter writer);
        public abstract void Read(MessageReader reader);
        internal int ID
        {
            get
            {
                int id = -1;
                lock (locker)
                {
                    id = allMessagesTypes.IndexOf(this.GetType());
                }
                return id;

            }
        }
        internal byte[] GetBytes()
        {
            byte[] body = null;

            using (var ms = new MemoryStream())
            {
                var writer = new MessageWriter(ms);
                Write(writer);
                body = ms.ToArray();
            }

            return body;
        }
        internal void SetBytes(byte[] bytes)
        {
            using (var reader = new MessageReader(new MemoryStream(bytes)))
            {
                Read(reader);
            }
        }

        internal static MessageBase Create(int ID)
        {
            var t = allMessagesTypes[ID];
            return (MessageBase)Activator.CreateInstance(t);
        }
    }
}
