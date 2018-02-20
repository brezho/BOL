using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace X.Documents.Word
{
    /// <summary>
    /// CustumXml Node Base Class
    /// </summary>
    public abstract class CustomXmlBase
    {
        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        protected XmlNamespaceManager Namespace { get; private set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public IEnumerable<XElement> Content { get; protected set; }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="CustomXmlBase"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        protected CustomXmlBase(XContainer element)
        {
            Namespace = new XmlNamespaceManager(new NameTable());
            Namespace.AddNamespace("w", DocxNamespaces.W);
            Content = element.Elements();
        }
    }
}