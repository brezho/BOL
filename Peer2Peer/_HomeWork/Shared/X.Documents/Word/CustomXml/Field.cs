using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;

namespace X.Documents.Word
{
    /// <summary>
    /// Custom Xml Field Node Class
    /// </summary>
    public class Field : CustomXmlBase
    {
        /// <summary>
        /// Gets the value element.
        /// </summary>
        /// <value>The value element.</value>
        public XElement ValueElement { get; private set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; private set; }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public Field(XElement element)
            : base(element)
        {
            if (element.LastAttribute.Value != "Field")
                throw new ArgumentException("Value of attribute 'element' must be 'Field'.", "element");

            var elems = element.XPathSelectElements(".//w:t", Namespace);

            ValueElement = elems.FirstOrDefault();
            if (ValueElement == null) return;

            Key = string.Join("", elems.Select(x => x.Value).ToArray());
            

            if (string.IsNullOrWhiteSpace(Key))
                return;
            foreach (var elem in elems)
            {
                elem.Value = string.Empty;
            }
        }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="frameworkDatabase">The frameworkDatabase.</param>
        public Field(XElement element, object data)
            : base(element)
        {
            if (element.LastAttribute.Value != "Field")
                throw new ArgumentException("Value of attribute 'element' must be 'Field'.", "element");

            var isSimpleType = data.GetType().IsPrimitive || data.GetType().Equals(typeof (string));

            ValueElement = element.XPathSelectElements(".//w:t", Namespace).LastOrDefault();
            if (ValueElement == null)
                return;

            Key = ValueElement.Value;
            
            if (string.IsNullOrWhiteSpace(Key))
                return;

            Value = isSimpleType ? data : data.Hype().GetValue<string>(Key);

            ValueElement.Value = Value == null ? string.Empty : (string) Value;
        }
    }
}