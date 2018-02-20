using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;

namespace X.Documents.Word
{
    class Picture : CustomXmlBase
    {
        /// <summary>
        /// Gets the value element.
        /// </summary>
        /// <value>The value element.</value>
        public XElement ValueElement { get; private set; }

        public string PictureNo { get; private set; }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public Picture(XElement element)
            : base(element)
        {
            if (element.LastAttribute.Value != "Picture")
                throw new ArgumentException("Value of attribute 'element' must be 'Picture'.", "element");

            ValueElement = element.XPathSelectElement(".//w:drawing", Namespace);
            if (ValueElement == null)
                return;

            var propertiesNode = element.XPathSelectElement("./w:customXmlPr", Namespace);
            if (propertiesNode == null)
                return;

            PictureNo =
                propertiesNode.XPathSelectElement("./w:attr[@w:name='PictureNo']", Namespace).LastAttribute.Value;
            if (string.IsNullOrWhiteSpace(PictureNo))
                throw new ArgumentException("Property 'PictureNo' of a Picture tag must be specified.", "PictureNo");

            propertiesNode.Remove();
            Content = element.Elements();

        }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="Field"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="frameworkDatabase">The frameworkDatabase.</param>
        public Picture(XElement element, object data)
            : base(element)
        {
            //if (element.LastAttribute.Value != "Picture")
            //    throw new ArgumentException("Value of attribute 'element' must be 'Picture'.", "element");

            //var isSimpleType = frameworkDatabase.GetType().IsPrimitive || frameworkDatabase.GetType().Equals(typeof (byte[] ));
            
            //ValueElement = element.XPathSelectElement(".//w:drawing", Namespace);
            
            
            //if (ValueElement == null)
            //    return;

            //Key = ValueElement.Value;
            
            //if (Key.IsNullOrEmpty())
            //    return;

            //Value = isSimpleType ? frameworkDatabase : frameworkDatabase.GetValue<byte[]>(Key);

            //ValueElement.Value = Value == null ? string.Empty : (string)Value;
        }

    }
}
