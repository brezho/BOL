using System;
using System.Collections.Generic;
using System.Serialization;
using System.Storage;
using System.Storage.Records;
using System.Text;

namespace Book
{
    class DataSource : Master
    {
        public ITable<Preference> Preferences;
        public DataSource(DataStore store) : base(store)
        {
            Preferences = this.GetTable<Preference>();
        }
    }

    [TypeId("adcd3eb6-f856-4859-a160-ed1298d62d29")]
    public class Preference : IIdentifiable
    {
        string _value;
        public string Key { get; set; }

        public string Value { get => _value; set => _value = value; }
        string IIdentifiable.Id { get => Key; set => Key = value; }

        void XSerializable.ReadWrite(XStream stream)
        {
            stream.ReadWrite(ref _value);
        }
    }
}
