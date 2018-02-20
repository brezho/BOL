using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Registry
{
    public class Key
    {
        internal event EventHandler NamedChanged = (x, a) => { };
        KeyStore _registry;
        int _pointer;
        int _childrenPointer;
        int _propertiesPointer;

        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _registry.Set(_pointer, this);
                NamedChanged(this, EventArgs.Empty);
            }
        }

        internal Key(KeyStore registry, string name, int pointer, int childrenPointer, int propertiesPointer)
        {
            this._registry = registry;
            _pointer = pointer;
            _name = name;
            _childrenPointer = childrenPointer;
            _propertiesPointer = propertiesPointer;
        }

        internal Key(KeyStore registry, BinaryReader reader)
        {
            this._registry = registry;
            _pointer = reader.ReadInt32();
            _name = reader.ReadString();
            _childrenPointer = reader.ReadInt32();
            _propertiesPointer = reader.ReadInt32();
        }

        internal void Pack(BinaryWriter writer)
        {
            writer.Write(_pointer);
            writer.Write(_name);
            writer.Write(_childrenPointer);
            writer.Write(_propertiesPointer);
        }

        public Key CreateSubKey(string p)
        {
            var thisChildren = _registry.GetSubKeysList(_childrenPointer);

            if (thisChildren.ContainsKey(p)) throw new ApplicationException("Key already exists");

            var subKey = _registry.CreateKey(p);
            thisChildren.Add(p, subKey._pointer);
            _registry.Set(_childrenPointer, thisChildren);
            Monitor(subKey);
            return subKey;
        }

        public string[] GetSubKeyNames()
        {
            var thisChildren = _registry.GetSubKeysList(_childrenPointer);
            return thisChildren.Keys.ToArray();
        }

        public string[] GetPropertiesNames()
        {
            var props = _registry.GetPropertiesList(_propertiesPointer);
            return props.Select(x => x.Key).ToArray();
        }

        public Key OpenSubKey(string p)
        {
            var thisChildren = _registry.GetSubKeysList(_childrenPointer);

            int pointer = -1;
            if (!thisChildren.TryGetValue(p, out pointer)) return null;

            var subKey = _registry.GetKey(pointer);
            Monitor(subKey);
            return subKey;
        }

        void Monitor(Key key)
        {
            key.NamedChanged += (s, a) =>
            {
                var child = (Key)s;
                var thisChildren = _registry.GetSubKeysList(_childrenPointer);

                if (thisChildren.ContainsKey(child.Name)) throw new ApplicationException("Key already exists");

                var res = thisChildren.First(x => x.Value == child._pointer);
                thisChildren.Remove(res.Key);
                thisChildren.Add(child.Name, child._pointer);
                _registry.Set(_childrenPointer, thisChildren);
            };
        }

        internal void Save()
        {
            _registry.Set(this._pointer, this);
        }

        public object GetProperty(string propertyName)
        {
            var properties = this._registry.GetPropertiesList(_propertiesPointer);

            Property res;
            if (!properties.TryGetValue(propertyName, out res)) return null;
            return res.Value;
        }
        public PropertyKind GetPropertyKind(string propertyName)
        {
            var properties = this._registry.GetPropertiesList(_propertiesPointer);

            Property res;
            if (!properties.TryGetValue(propertyName, out res)) throw new ApplicationException("Property does not exist");
            return res.Kind;
        }

        public void SetProperty(string propertyName, int value)
        {
            var property = new Property() { Value = value, Kind = PropertyKind.Int };
            SetProperty(propertyName, property);
        }
        public void SetProperty(string propertyName, string value)
        {
            var property = new Property() { Value = value, Kind = PropertyKind.String };
            SetProperty(propertyName, property);
        }
        public void SetProperty(string propertyName, byte[] value)
        {
            var property = new Property() { Value = value, Kind = PropertyKind.Binary };
            SetProperty(propertyName, property);
        }
        public void SetProperty(string propertyName, DateTime value)
        {
            var property = new Property() { Value = value, Kind = PropertyKind.DateTime };
            SetProperty(propertyName, property);
        }
        void SetProperty(string propertyName, Property prop)
        {
            var properties = this._registry.GetPropertiesList(_propertiesPointer);
            properties[propertyName] = prop;
            _registry.Set(_propertiesPointer, properties);
        }
    }
}
