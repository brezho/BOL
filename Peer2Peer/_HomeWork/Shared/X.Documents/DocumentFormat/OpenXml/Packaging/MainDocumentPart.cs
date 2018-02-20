using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Core.Documents.Internal;

namespace DocumentFormat.OpenXml.Packaging
{
    public static class MainDocumentPartExtensions
    {
        public static string DefaultStyle(this MainDocumentPart mainDocument)
        {
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            return (string)mainDocument
                    .StyleDefinitionsPart
                    .GetXDocument()
                    .Root
                    .Elements(w + "style")
                    .Where(style => (string)style.Attribute(w + "type") == "paragraph" &&
                      (string)style.Attribute(w + "default") == "1")
                    .First()
                    .Attribute(w + "styleId");
        }

        static IEnumerable<OfficeParagraph> Paragraphs(this MainDocumentPart mainDocument)
        {
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            XName r = w + "r";
            XName ins = w + "ins";
            string defaultStyle = mainDocument.DefaultStyle();
            return
                from p in mainDocument.GetXDocument()
                    .Root.Element(w + "body").Descendants(w + "p")
                let styleNode = p.Elements(w + "pPr").Elements(w + "pStyle")
                    .FirstOrDefault()
                select new OfficeParagraph(mainDocument)
                {
                    ParagraphElement = p,
                    StyleName = styleNode != null ?
                        (string)styleNode.Attribute(w + "val") :
                        defaultStyle,
                    // in the following query, need to select both the r and ins elements
                    // to assemble the text properly for paragraphs that have tracked changes.
                    Text = p.Elements()
                        .Where(z => z.Name == r || z.Name == ins)
                        .Descendants(w + "t")
                        .StringConcatenate(element => (string)element)
                };
        }
    }
}
