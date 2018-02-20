using CodeGen.M2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PieceOfWork
{
    class Program
    {
        public static void Main(string[] args)
        {
            var name = new M2Attribute() { Name = "Name", DataTypeName = typeof(string).FullName, IsMultipleValue = false };
            var datatypename = new M2Attribute() { Name = "DataTypeName", DataTypeName = typeof(string).FullName, IsMultipleValue = false };
            var attributes = new M2Attribute() { Name = "Attributes", DataTypeName = typeof(M2Attribute).FullName, IsMultipleValue = true };
            var classes = new M2Attribute() { Name = "Classes", DataTypeName = typeof(M2Attribute).FullName, IsMultipleValue = true };
            var isMultiple = new M2Attribute() { Name = "IsMultipleValue", DataTypeName = typeof(bool).FullName, IsMultipleValue = false };

            var cls = new M2Class() { Name = "M2Class" };
            var att = new M2Class() { Name = "M2Attribute" };
            var model = new M2Schema() { Name = "M2" };

            cls.Attributes = new[] { name, attributes };
            att.Attributes = new[] { name, datatypename };

            model.Classes = new[] { cls, att };
            var xml = model.ToXML();
        }
    }
}
