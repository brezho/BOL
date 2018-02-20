using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml.Packaging;

namespace X.Documents.Word
{
    /// <summary>
    /// Docx Builder From Template
    /// </summary>
    public static class WordHelper
    {
        public static MemoryStream GetFileStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            MemoryStream memStream;
            using (Stream fileStream = File.Open(path, mode, access, share))
            {
                memStream = fileStream.LoadIntoMemoryStream();
            }
            return memStream;
        }
        public static MemoryStream LoadIntoMemoryStream(this Stream source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (!source.CanRead)
                throw new ArgumentException("source cannot be read (write only?)", "source");

            var initialCapacity = 32768;
            try
            {
                var templateSize = source.Length;
                initialCapacity = Convert.ToInt32(templateSize);
            }
            catch (NotSupportedException)
            {
            }

            var memStream = new MemoryStream(initialCapacity);
            source.CopyStream(memStream);
            return memStream;
        }
        public static void CopyStream(this Stream source, Stream target)
        {
            var srcPos = (source.CanSeek ? source.Position : long.MinValue);
            var destPos = (target.CanSeek ? target.Position : long.MinValue);

            const int bufSize = 0x1000;
            var buf = new byte[bufSize];
            int bytesRead;
            while ((bytesRead = source.Read(buf, 0, bufSize)) > 0)
            {
                target.Write(buf, 0, bytesRead);
            }

            var targetAsMemStream = target as MemoryStream;
            if (targetAsMemStream != null)
                targetAsMemStream.Capacity = Convert.ToInt32(targetAsMemStream.Length);

            if (target.CanSeek)
                target.Position = destPos;

            if (source.CanSeek)
                source.Position = srcPos;
        }

        public static void Transform(string sourceFile, string targetFile, Dictionary<string, object> data)
        {
            Transform(sourceFile, targetFile, data, null);
        }

        public static void Transform(string sourceFile, string targetFile, Dictionary<string, object> data, ITranslationProvider translationProvider)
        {
            //sourceFile.CanNotBeEmpty();
            //targetFile.CanNotBeEmpty();
            //data.CanNotBeNull();

            if (File.Exists(sourceFile))
            {
                MemoryStream _template;
                using (var templateStream = GetFileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    _template = templateStream.LoadIntoMemoryStream();
                }

                DataCollectionProcessor _processor = new DataCollectionProcessor(data);
                if (translationProvider != null) _processor.TranslationProvider = translationProvider;
                var outputStream = GenerateOutputStream(targetFile);

                try
                {
                    using (var source = _template.LoadIntoMemoryStream())
                    {
                        var docX = WordprocessingDocument.Open(source, true);
                        _processor.Process(docX);
                        docX.Close();

                        source.Seek(0, SeekOrigin.Begin);
                        source.CopyStream(outputStream);
                    }

                    outputStream.Close();
                    outputStream.Dispose();
                }
                catch (Exception)
                {
                    outputStream.Close();
                    outputStream.Dispose();

                    if (File.Exists(targetFile))
                    {
                        try
                        {
                            File.Delete(targetFile);
                        }
                        catch
                        {
                        }
                    }

                    throw;
                }
            }
        }


        private static Stream GenerateOutputStream(string outputFilePath)
        {
            try
            {
                Stream outputStream = File.Open(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                return outputStream;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("output file cannot be created (details: " + ex.Message + ")", "outputFilePath", ex);
            }
        }
    }
}