using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Packing.Internals
{
    class DictionaryOfStuff<TKey, TValue> : IXSerializable
    {
        Dictionary<TKey, TValue> _data = null;
        int _size = -1;
        public DictionaryOfStuff()
        {
        }
        public DictionaryOfStuff(Dictionary<TKey, TValue> data)
        {
            if (data != null)
            {
                _data = data;
                _size = data.Count;
            }
        }
        public Dictionary<TKey, TValue> GetStuff()
        {
            return _data;
        }
        public void ReadWrite(IXStream stream)
        {
            var methods = stream
                .GetType()
                .Hype()
                .AllMethods
                .Where(m => m.Name == "ReadWrite" && m.GetParameters().Length == 1)
                .ToArray();

            var _ReadWriteKeyTyped = methods
                .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.GetElementType() == typeof(TKey)))
                .FirstOrDefault();

            if (_ReadWriteKeyTyped == null && typeof(TKey).Match(typeof(IXSerializable)))
            {
                _ReadWriteKeyTyped = methods
                    .Where(m => m.IsGenericMethodDefinition)
                    .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.HasElementType && !p.ParameterType.GetElementType().IsArray))
                    .FirstOrDefault();

                _ReadWriteKeyTyped = _ReadWriteKeyTyped.MakeGenericMethod(typeof(TKey));

            }

            var _ReadWriteValueTyped = methods
                 .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.GetElementType() == typeof(TValue)))
                 .FirstOrDefault();

            if (_ReadWriteValueTyped == null && typeof(TValue).Match(typeof(IXSerializable)))
            {
                _ReadWriteValueTyped = methods
                    .Where(m => m.IsGenericMethodDefinition)
                    .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.HasElementType && !p.ParameterType.GetElementType().IsArray))
                    .First();

                _ReadWriteValueTyped = _ReadWriteValueTyped.MakeGenericMethod(typeof(TValue));
            }


            stream.ReadWrite(ref _size);
            if (_size != -1)
            {
                if (stream.IsWriting)
                {
                    foreach (var item in _data)
                    {
                        var key = item.Key;
                        var val = item.Value;
                        _ReadWriteKeyTyped.Invoke(stream, new object[] { key });
                        _ReadWriteValueTyped.Invoke(stream, new object[] { val });
                    }
                }
                else
                {
                    _data = new Dictionary<TKey, TValue>(_size);
                    for (int i = 0; i < _size; i++)
                    {
                        var parameters = new object[] { null };
                        _ReadWriteKeyTyped.Invoke(stream, parameters);
                        var key = (TKey)parameters[0];
                        parameters = new object[] { null };
                        _ReadWriteValueTyped.Invoke(stream, parameters);
                        var val = (TValue)parameters[0];
                        _data.Add(key, val);
                    }
                }
            }
        }

    }
}
