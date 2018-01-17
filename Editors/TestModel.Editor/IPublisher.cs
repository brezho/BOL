using System;
using System.Collections.Generic;
using System.Text;

namespace TestModel.Editor
{
    public interface IPublisher<T>
    {
        event EventHandler<T> OnNext;
    }
}
