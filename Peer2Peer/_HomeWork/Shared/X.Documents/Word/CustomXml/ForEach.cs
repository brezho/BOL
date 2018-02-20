using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;

namespace X.Documents.Word
{
    /// <summary>
    /// Custum Xml ForEach Node Class
    /// </summary>
    public class ForEach : CustomXmlBase
    {
        /// <summary>
        /// Gets the name of the frameworkDatabase source.
        /// </summary>
        /// <value>The name of the frameworkDatabase source.</value>
        public string DataSourceName { get; private set; }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="ForEach"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public ForEach(XElement element)
            : base(element)
        {
            if (element.LastAttribute.Value != "ForEach")
                throw new ArgumentException("Value of attribute 'element' must be 'ForEach'.", "element");

            var propertiesNode = element.XPathSelectElement("./w:customXmlPr", Namespace);
            if (propertiesNode == null)
                return;

            DataSourceName =
                propertiesNode.XPathSelectElement("./w:attr[@w:name='DataSource']", Namespace).LastAttribute.Value;
            if (string.IsNullOrWhiteSpace(DataSourceName))
                throw new ArgumentException("Property 'DataSource' of a ForEach tag must be specified.", "DataSource");

            propertiesNode.Remove();
            Content = element.Elements();
        }
    }
}