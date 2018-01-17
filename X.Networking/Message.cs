using System;
using System.Collections.Generic;
using System.Text;

namespace X.Networking
{
    public enum MessageType
    {
        Text,
        Binary
    }
    public struct Message
    {
        public MessageType Type { get; private set; }
        public byte[] Data { get; private set; }

        internal Message(MessageType type, byte[] data)
        {
            Type = type;
            Data = data;
        }
        public string Decode()
        {
            return UTF8Encoding.UTF8.GetString(Data);
        }
        public static Message New(string text)
        {
            return new Message(MessageType.Text, UTF8Encoding.UTF8.GetBytes(text));
        }
        public static Message New(byte[] data)
        {
            return new Message(MessageType.Binary, data);
        }
    }
}
