using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace DocumentFormat.OpenXml.Packaging
{
    public static class OpenXmlPartExtensions
    {
        public static XDocument GetXDocument(this OpenXmlPart part)
        {
            XDocument xdoc = part.Annotation<XDocument>();
            if (xdoc != null) return xdoc;
            using (StreamReader sr = new StreamReader(part.GetStream()))
            using (XmlReader xr = XmlReader.Create(sr))
                xdoc = XDocument.Load(xr);
            part.AddAnnotation(xdoc);
            return xdoc;
        }
    }
}
