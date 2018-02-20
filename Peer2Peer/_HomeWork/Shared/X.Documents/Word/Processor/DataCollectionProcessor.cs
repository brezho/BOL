using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using DocumentFormat.OpenXml.Packaging;
using System.Diagnostics;
using System.Drawing;

namespace X.Documents.Word
{
    /// <summary>
    /// DataCollection  Docx Template Processor Class
    /// </summary>
    public class DataCollectionProcessor
    {
        #region Fields

        private static XmlNamespaceManager _namespace;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the frameworkDatabase.
        /// </summary>
        /// <value>The frameworkDatabase.</value>
        public Dictionary<string, object> Data { get; set; }

        public ITranslationProvider TranslationProvider { get; set; }


        private static XmlNamespaceManager Namespace
        {
            get
            {
                if (_namespace == null)
                {
                    _namespace = new XmlNamespaceManager(new NameTable());
                    _namespace.AddNamespace("w", DocxNamespaces.W);
                }
                return _namespace;
            }
        }

        #endregion

        #region Constructors

        ///// <summary>
        ///// Initializes a new concreteFeatureType of the <see cref="DataCollectionProcessor"/> class.
        ///// </summary>
        //public DataCollectionProcessor(WordHelper transformer)
        //{
        //    Data = new Dictionary<string, object>();
        //    Transformer = transformer;
        //}

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="DataCollectionProcessor"/> class.
        /// </summary>
        /// <param name="frameworkDatabase">The frameworkDatabase.</param>
        public DataCollectionProcessor(Dictionary<string, object> data)
        {
            if (data == null)
                data = new Dictionary<string, object>();

            Data = data;
        }

        public DataCollectionProcessor(Dictionary<string, object> data, ITranslationProvider translationProvider)
        {
            if (data == null)
                data = new Dictionary<string, object>();

            Data = data;
            TranslationProvider = translationProvider;
        }

        #endregion

        #region IWordTemplateProcessor Members

        /// <summary>
        /// Processes the specified doc.
        /// </summary>
        /// <param name="doc">The doc.</param>
        public void Process(WordprocessingDocument doc)
        {
            // Main
            ProcessOpenXmlPart(doc.MainDocumentPart, doc);

            // Headers
            var headerParts = doc.MainDocumentPart.HeaderParts.ToList();
            foreach (var t in headerParts)
            {
                ProcessOpenXmlPart(t, doc);
            }

            // Footers
            var footerParts = doc.MainDocumentPart.FooterParts.ToList();
            foreach (var t in footerParts)
            {
                ProcessOpenXmlPart(t, doc);
            }
        }

        #endregion

        #region Private Methods

        private void ProcessOpenXmlPart(OpenXmlPart part, WordprocessingDocument doc)
        {
            try
            {
                using (var sr = new StreamReader(part.GetStream()))
                {
                    var xml = XElement.Parse(sr.ReadToEnd());
                    ProcessIfSections(xml);
                    ProcessIfNotSections(xml);
                    ProcessForEachSections(xml);
                    ProcessLabels(xml);
                    ProcessFieldSections(xml);
                    ProcessPictures(xml, doc);


                    using (var sw = new StreamWriter(part.GetStream(FileMode.Create)))
                    {
                        sw.Write(xml.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    var exToLog = ex;
                    while (exToLog.InnerException != null)
                    {
                        exToLog = exToLog.InnerException;
                    }
                    throw exToLog;
                }
                throw;
            }
        }

        #region ForEach Related

        private void ProcessForEachSections(XElement element)
        {
            var foreachs =
                element.XPathSelectElements(
                    ".//w:customXml[@w:element='ForEach'][not(ancestor::w:customXml[@w:element='ForEach'])]", Namespace)
                    .ToList();
            if (foreachs != null && foreachs.Count > 0)
            {
                foreach (var f in foreachs)
                {
                    ProcessForEach(f);
                }
                foreachs.Remove();
            }
        }

        private void ProcessForEachSections(XElement element, object parentDataRow)
        {
            var foreachs =
                element.XPathSelectElements(
                    ".//w:customXml[@w:element='ForEach'][not(.//w:customXml[@w:element='ForEach'])]", Namespace).ToList
                    ();
            if (foreachs != null && foreachs.Count > 0)
            {
                foreach (var f in foreachs)
                {
                    ProcessForEach(f, parentDataRow);
                }
                foreachs.Remove();
            }
        }

        private void ProcessForEach(XElement element)
        {
            var forEach = new ForEach(element);

            if ((!string.IsNullOrWhiteSpace(forEach.DataSourceName)) && Data.ContainsKey(forEach.DataSourceName))
            {

                var objectValue = Data[forEach.DataSourceName];
                if (!(objectValue is IEnumerable<object>))
                    throw new InvalidCastException(string.Format("Type of DataSource '{0}' must be IEnumerable<object>.",
                                                                 forEach.DataSourceName));

                var dataSource = objectValue as IEnumerable<object>;
                for (var index = dataSource.Count() - 1; index >= 0; index--)
                {
                    var newElement = new XElement(element);

                    ProcessIfSections(newElement, dataSource.ElementAt(index));
                    ProcessIfNotSections(newElement, dataSource.ElementAt(index));
                    ProcessForEachSections(newElement, dataSource.ElementAt(index));
                    ProcessFieldSections(newElement, dataSource.ElementAt(index));
                    ProcessLabels(newElement);

                    element.AddAfterSelf(newElement.Elements());
                }
            }
        }

        private void ProcessForEach(XElement element, object parentDataRow)
        {
            var forEach = new ForEach(element);

            var objectValue = parentDataRow.Hype().GetValue(forEach.DataSourceName);
            if (objectValue == null)
                return;

            if (!(objectValue is IEnumerable))
                return;

            var dataSource = objectValue as IEnumerable<object>;

            if (dataSource != null)
                for (var index = dataSource.Count() - 1; index >= 0; index--)
                {
                    var newElement = new XElement(element);

                    ProcessIfSections(newElement, dataSource.ElementAt(index));
                    ProcessIfNotSections(newElement, dataSource.ElementAt(index));
                    ProcessForEachSections(newElement, parentDataRow);
                    ProcessFieldSections(newElement, dataSource.ElementAt(index));
                    ProcessLabels(newElement);

                    element.AddAfterSelf(newElement.Elements());
                }
        }

        #endregion

        #region Field Related

        private static void ProcessFieldSections(XElement element, object dataRow)
        {
            var isSimpleType = dataRow.GetType().IsPrimitive || dataRow.GetType().Equals(typeof(string));
            var fields = element.XPathSelectElements(".//w:customXml[@w:element='Field']", Namespace);

            foreach (var entry in fields)
            {
                var field = new Field(entry, dataRow);

                if (isSimpleType)
                {
                    field.ValueElement.Value = dataRow == null ? string.Empty : (string)dataRow;
                }
                else
                {
                    string tmp = dataRow.Hype().GetValue<string>(field.Key);
                    field.ValueElement.Value = tmp == null ? string.Empty : tmp;
                }

                entry.AddBeforeSelf(field.Content);
            }
            fields.Remove();
        }

        private void ProcessFieldSections(XElement element)
        {
            var fields = element.XPathSelectElements(".//w:customXml[@w:element='Field']", Namespace);


            foreach (var entry in fields)
            {
                var field = new Field(entry);
                var stringValue = string.Empty;

                if (Data.ContainsKey(field.Key))
                {
                    if (Data[field.Key]!=null)
                    {
                        stringValue = Data[field.Key].Hype().ChangeType<string>();
                    }
                }
                else
                {
                    var key = Data.Keys.FirstOrDefault(k => field.Key.StartsWith(k + "."));
                    if (key != null)
                    {
                        var call = field.Key.Replace(key + ".", "");
                        var val = Data[key].Hype().EvaluateExpression(call);
                        stringValue = val == null ? string.Empty : val.Hype().ChangeType<string>(); ;
                    }
                    else continue; // jump to next field
                }

                field.ValueElement.Value = stringValue;
                entry.AddBeforeSelf(field.Content);
            }
            fields.Remove();
        }

        #endregion

        #region If Related

        private void ProcessIfSections(XElement element)
        {
            var ifs =
                element.XPathSelectElements(
                    ".//w:customXml[@w:element='If'][not(ancestor::w:customXml[@w:element='If']) and not(ancestor::w:customXml[@w:element='ForEach'])]",
                    Namespace).ToList();
            foreach (var entry in ifs)
            {
                ProcessIf(entry);
            }
            ifs.Remove();
        }

        private static void ProcessIfSections(XElement element, object dataRow)
        {
            var ifs =
                element.XPathSelectElements(
                    ".//w:customXml[@w:element='If'][not(ancestor::w:customXml[@w:element='If'])]", Namespace).ToList();
            foreach (var entry in ifs)
            {
                ProcessIf(entry, dataRow);
            }
            ifs.Remove();
        }

        private void ProcessIf(XElement element)
        {
            var ifElement = new If(element);

            if (!Data.ContainsKey(ifElement.ConditionField))
                throw new ArgumentException("Condition",
                                            string.Format(
                                                "Condition Field '{0}' could not be found in Word Template Processing Data.",
                                                ifElement.ConditionField));

            if (!Data[ifElement.ConditionField].GetType().Equals(typeof(bool)))
                throw new ArgumentException("Condition",
                                            string.Format("Type of Confirion Field '{0}' must be Boolean.",
                                                          ifElement.ConditionField));

            var checkValue = (bool)Data[ifElement.ConditionField];

            if (checkValue)
                element.AddAfterSelf(ifElement.Content);
        }

        private static void ProcessIf(XElement element, object dataRow)
        {
            var ifElement = new If(element);

            var objectValue = dataRow.Hype().GetValue(ifElement.ConditionField);
            if (objectValue == null)
                throw new ArgumentException("Condition",
                                            string.Format(
                                                "Condition Field '{0}' could not be found in Current Data OfficeRow.",
                                                ifElement.ConditionField));

            if (!(objectValue is bool))
                throw new ArgumentException("Condition",
                                            string.Format("Type of Confirion Field '{0}' must be Boolean.",
                                                          ifElement.ConditionField));

            var checkValue = (bool)objectValue;

            if (checkValue)
                element.AddAfterSelf(ifElement.Content);
        }

        #endregion

        #region IfNot Related

        private void ProcessIfNotSections(XElement element)
        {
            var ifNots =
                element.XPathSelectElements(
                    ".//w:customXml[@w:element='IfNot'][not(ancestor::w:customXml[@w:element='IfNot']) and not(ancestor::w:customXml[@w:element='ForEach'])]",
                    Namespace).ToList();
            foreach (var entry in ifNots)
            {
                ProcessIfNot(entry);
            }
            ifNots.Remove();
        }

        private static void ProcessIfNotSections(XElement element, object dataRow)
        {
            var ifNots =
                element.XPathSelectElements(
                    ".//w:customXml[@w:element='IfNot'][not(ancestor::w:customXml[@w:element='IfNot'])]", Namespace).
                    ToList();
            foreach (var entry in ifNots)
            {
                ProcessIfNot(entry, dataRow);
            }
            ifNots.Remove();
        }

        private void ProcessIfNot(XElement element)
        {
            var ifNot = new IfNot(element);

            if (!Data.ContainsKey(ifNot.ConditionField))
                throw new ArgumentException("Condition",
                                            string.Format(
                                                "Condition Field '{0}' could not be found in Word Template Processing Data.",
                                                ifNot.ConditionField));

            if (!Data[ifNot.ConditionField].GetType().Equals(typeof(bool)))
                throw new ArgumentException("Condition",
                                            string.Format("Type of Confirion Field '{0}' must be Boolean.",
                                                          ifNot.ConditionField));

            var checkValue = (bool)Data[ifNot.ConditionField];


            if (!checkValue)
                element.AddAfterSelf(ifNot.Content);
        }

        private static void ProcessIfNot(XElement element, object dataRow)
        {
            var ifNot = new IfNot(element);

            var objectValue = dataRow.Hype().GetValue(ifNot.ConditionField);
            if (objectValue == null)
                throw new ArgumentException("Condition",
                                            string.Format(
                                                "Condition Field '{0}' could not be found in Current Data OfficeRow.",
                                                ifNot.ConditionField));

            if (!(objectValue is bool))
                throw new ArgumentException("Condition",
                                            string.Format("Type of Confirion Field '{0}' must be Boolean.",
                                                          ifNot.ConditionField));

            var checkValue = (bool)objectValue;

            if (!checkValue)
                element.AddAfterSelf(ifNot.Content);
        }

        #endregion

        #region Label Related

        private void ProcessLabels(XElement xml)
        {
            var labels = xml.XPathSelectElements(".//w:customXml[@w:element='Label']", Namespace);
            foreach (var entry in labels)
            {
                var label = new Label(this, entry);
                entry.AddBeforeSelf(label.Content);
            }
            labels.Remove();
        }

        #endregion

        #region Picture Related

        private void ProcessPictures(XElement element, WordprocessingDocument doc)
        {
            var pictures = element.XPathSelectElements(".//w:customXml[@w:element='Picture']", Namespace);
            foreach (var entry in pictures)
            {
                var picture = new Picture(entry);

                if (!Data.ContainsKey(picture.PictureNo))
                    continue;

                string oldImageName = string.Format("/word/media/image{0}", picture.PictureNo);
                var imagePart = GetImagePart(doc, oldImageName);
                var newImageBytes = (byte[])Data[picture.PictureNo];
                if (newImageBytes != null)
                {
                    using (var writer = new BinaryWriter(imagePart.GetStream()))
                    {
                        writer.Write(newImageBytes);
                    }
                }
                entry.AddBeforeSelf(picture.Content);
            }
            pictures.Remove();
        }

        ImagePart GetImagePart(WordprocessingDocument doc, string imageName)
        {
            return doc.MainDocumentPart.ImageParts
                .Where(p => p.Uri.ToString().StartsWith(imageName)) // or EndsWith
                .First();
        }

        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        #endregion

        #endregion
    }
}