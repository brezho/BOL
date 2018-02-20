using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;

namespace X.Documents.Word
{
    /// <summary>
    /// Custum Xml If Node Class
    /// </summary>
    public class If : CustomXmlBase
    {
        /// <summary>
        /// Gets the condition field.
        /// </summary>
        /// <value>The condition field.</value>
        public string ConditionField { get; private set; }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="If"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public If(XElement element)
            : base(element)
        {
            if (element.LastAttribute.Value != "If")
                throw new ArgumentException("Value of attribute 'element' must be 'If'.", "element");

            var propertiesNode = element.XPathSelectElement("./w:customXmlPr", Namespace);
            if (propertiesNode == null)
                return;

            ConditionField =
                propertiesNode.XPathSelectElement("./w:attr[@w:name='Condition']", Namespace).LastAttribute.Value;
            if (string.IsNullOrWhiteSpace(ConditionField))
                throw new ArgumentException("Property Condition of an If tag must be specified.", "ConditionField");

            propertiesNode.Remove();

            Content = element.Elements();
        }
    }
}