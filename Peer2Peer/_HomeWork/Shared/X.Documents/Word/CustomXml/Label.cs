using System;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;

namespace X.Documents.Word
{
    /// <summary>
    /// Custum Xml Label Node Class
    /// </summary>
    public class Label : CustomXmlBase
    {
        private readonly string _key;
        private readonly string _translation;
        private readonly XElement _valueElement;

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="processor"></param>
        /// <param name="element">The element.</param>
        public Label(DataCollectionProcessor processor, XElement element)
            : base(element)
        {
            if (element.LastAttribute.Value != "Label")
                throw new ArgumentException("Value of attribute 'element' must be 'Label'.", "element");

            _valueElement = element.XPathSelectElement(".//w:t", Namespace);
            if (_valueElement == null)
                return;

            _key = _valueElement.Value;
            if (string.IsNullOrWhiteSpace(_key)) return;

            if (processor.TranslationProvider == null) _translation =_key;
            else _translation = processor.TranslationProvider.GetTranslation(_key);
            _valueElement.Value = _translation;
        }
    }
}