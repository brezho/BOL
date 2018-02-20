using SchemaBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SchemaBuilder.UIModel
{
    public abstract class XModelBase : XObject
    {
        
        [XmlAttribute]
        public string Name { get { return Get(() => Name); } set { Set(() => Name, value); } }
    }
}
