using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

namespace Core.Documents.Internal
{
    class OfficeParagraph
    {
        public XElement ParagraphElement { get; set; }
        public string StyleName { get; set; }
        public string Text { get; set; }
        public IEnumerable<OfficeComment> Comments()
        {
            XNamespace w = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            XElement p = ParagraphElement;
            var commentIds = p.Elements(w + "commentRangeStart")
                .Attributes(w + "id")
                .Select(c => (int)c);
            return commentIds
                .Select(i =>
                    new OfficeComment()
                    {
                        Id = i,
                        Author = Parent
                            .WordprocessingCommentsPart
                            .GetXDocument()
                            .Root
                            .Elements(w + "comment")
                            .Where(c => (int)c.Attribute(w + "id") == i)
                            .First()
                            .Attribute(w + "author")
                            .Value,
                        Text = Parent
                            .WordprocessingCommentsPart
                            .GetXDocument()
                            .Root
                            .Elements(w + "comment")
                            .Where(c => (int)c.Attribute(w + "id") == i)
                            .First()
                            .Descendants(w + "p")
                            .Select(run => run.Descendants(w + "t")
                                .StringConcatenate(e => (string)e) + "\n")
                            .StringConcatenate()
                            .Trim()
                    }
                );
        }
        public MainDocumentPart Parent { get; set; }
        public OfficeParagraph(MainDocumentPart parent) { Parent = parent; }
    }
}
