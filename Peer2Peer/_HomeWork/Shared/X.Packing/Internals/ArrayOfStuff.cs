using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X.Packing.Internals
{
    class ArrayOfStuff<T> : IXSerializable
    {
        T[] _data = null;
        int _size = -1;
        public ArrayOfStuff()
        {
        }
        public ArrayOfStuff(T[] data)
        {
            if (data != null)
            {
                _data = data;
                _size = data.Length;
            }
        }
        public T[] GetStuff()
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
            
            var _ReadWriteTyped = methods
                .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.GetElementType() == typeof(T)))
                .FirstOrDefault();
            if (_ReadWriteTyped == null && typeof(T).Match(typeof(IXSerializable)))
            {
                _ReadWriteTyped = methods
                    .Where(m => m.IsGenericMethodDefinition)
                    .Where(m => m.GetParameters().Any(p => p.ParameterType.IsByRef && p.ParameterType.HasElementType && !p.ParameterType.GetElementType().IsArray))
                    .FirstOrDefault();

                _ReadWriteTyped = _ReadWriteTyped.MakeGenericMethod(typeof(T));

            }

            stream.ReadWrite(ref _size);

            if (_size != -1)
            {
                if (!stream.IsWriting) _data = new T[_size];
                for (int i = 0; i < _size; i++)
                {
                    var parameters = new object[] { _data[i] };
                    _ReadWriteTyped.Invoke(stream, parameters);
                    _data[i] = (T)parameters[0];
                }
            }
        }

    }
}
