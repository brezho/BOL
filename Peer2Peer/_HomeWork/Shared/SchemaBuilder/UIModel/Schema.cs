using SchemaBuilder;
using SchemaBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SchemaBuilder.UIModel
{
    public class XDataSet : XModelBase
    {
        public XList<XDataTable> Tables { get { return Get(() => Tables); } set { Set(() => Tables, value); } }
    }
    public class XDataTable : XObject
    {
        public XList<XDataRow> Rows { get { return Get(() => Rows); } set { Set(() => Rows, value); } }
    }
    public class XDataRow : XObject
    {

    }


    public class XSchema : XObject
    {
        public XSchema()
        {
            Name = "new";
        }
        [XmlAttribute]
        public string Name { get { return Get(() => Name); } set { Set(() => Name, value); } }

        [XmlAttribute]
        public string DefaultExtension { get { return Get(() => DefaultExtension); } set { Set(() => DefaultExtension, value); } }
        public XList<XDataType> Types { get { return Get(() => Types); } set { Set(() => Types, value); } }

        public override IEnumerable<XRule> GetRules()
        {
           yield return XRule.MustSet(() => Name);
        }
    }
    public class XDataType : XObject
    {
        [XmlAttribute]
        public string Name { get { return Get(() => Name); } set { Set(() => Name, value); } }
        public XList<XPropertySpec> Properties { get { return Get(() => Properties); } set { Set(() => Properties, value); } }
    }

    public class XPropertySpec : XObject
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute("Type")]
        public virtual string TypeName { get; set; }
    }
    public class List : XPropertySpec
    {
    }
    public class Calculated : XPropertySpec
    {
        [XmlText()]
        public string Formula { get; set; }
    }
}
