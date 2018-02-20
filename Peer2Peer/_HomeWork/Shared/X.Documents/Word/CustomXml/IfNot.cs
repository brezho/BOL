using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;

namespace X.Documents.Word
{
    /// <summary>
    /// Custum Xml IfNot Node Class
    /// </summary>
    public class IfNot : CustomXmlBase
    {
        /// <summary>
        /// Gets the condition field.
        /// </summary>
        /// <value>The condition field.</value>
        public string ConditionField { get; private set; }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="IfNot"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public IfNot(XElement element)
            : base(element)
        {
            if (element.LastAttribute.Value != "IfNot")
                throw new ArgumentException("Value of attribute 'element' must be 'IfNot'.", "element");

            var propertiesNode = element.XPathSelectElement("./w:customXmlPr", Namespace);
            if (propertiesNode == null)
                return;

            ConditionField =
                propertiesNode.XPathSelectElement("./w:attr[@w:name='Condition']", Namespace).LastAttribute.Value;
            if (string.IsNullOrWhiteSpace(ConditionField))
                throw new ArgumentException("Property Condition of an IfNot tag must be specified.", "ConditionField");

            propertiesNode.Remove();

            Content = element.Elements();
        }
    }
}