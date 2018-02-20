using Configuration.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConfigurationEditor
{
    public class ConfigurationObjectTreeNode : TreeNode
    {
        internal ConfigurationObjectTreeNode() { }
        public Configuration.Model.ConfigurationDataObject ConfigurationObject { get; set; }
    }

    public static class ConfigurationObjectExt
    {
        public static ConfigurationObjectTreeNode ToTreeNode(this ConfigurationDataObject configDataObj)
        {
            var res = new ConfigurationObjectTreeNode();
            res.ConfigurationObject = configDataObj;
            res.Text = configDataObj.ToString();

            return res;
        }

        public static ConfigurationEditor.CustomTypeDescriptor ToGridViewData(this ConfigurationDataObject configDataObj)
        {
            var res = new CustomTypeDescriptor();

            res.AddProperty("Id", typeof(string).FullName, () => configDataObj.Cid, null, "Object");

            foreach (var prop in configDataObj.TypeDefinition.Members)
            {
                var propName = prop.Name;
                var propertyType = prop.UnderlyingType;
                var propertyTypeName = propertyType.AssemblyQualifiedName;
                Func<object> getter = () => prop.Get(configDataObj);
                Action<object> setter = (x) => prop.Set(configDataObj, x);

                res.AddProperty
                    (
                        propName,
                        propertyTypeName,
                        getter,
                        setter,
                        prop.Category ?? "Main",
                        prop.Description ?? "",
                        prop.DefaultValue
                    );
            }

            res.AddMethod("uhuhuh", () => MessageBox.Show("ds"));

            //res.AddProperty(Name = "Hell   oB", Category = "B", TypeName = typeof(string).AssemblyQualifiedName, Setter = (c) => { toto["helloB"] = c; }, Getter = () => toto.ContainsKey("helloB") ? toto["helloB"] : string.Empty);
            //res.AddProperty(Name = "Bu   oB", Category = "B", TypeName = typeof(string).AssemblyQualifiedName, Setter = (c) => { toto["helloB"] = c; }, Getter = () => toto.ContainsKey("helloB") ? toto["helloB"] : string.Empty);
            //res.AddProperty(Name = "Hello", Category = "A", TypeName = typeof(string).AssemblyQualifiedName, Setter = (c) => { toto["helloa"] = c; }, Getter = () => toto.ContainsKey("helloa") ? toto["helloa"] : string.Empty);

            return res;
        }

    }


}