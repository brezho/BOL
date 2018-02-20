using System;
using System.IO;
using System.Diagnostics;
using X.AspNet;
using X.AspNet.Services.Multilingual;
using X.AspNet.Utils;

namespace Host.Infrastructure.Application.StreamFilters
{
    internal abstract class StreamFilterBase : Stream
    {
        private long _position;
        private Stream _originalStream;

        protected Stream OriginalStream
        {
            get { return _originalStream; }
        }

        public StreamFilterBase(Stream originalStream)
        {
            _originalStream = originalStream;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Close()
        {
            _originalStream.Close();
        }

        public override void Flush()
        {
            _originalStream.Flush();
        }

        public override long Length
        {
            get { return 0; }
        }

        public override long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _originalStream.Seek(offset, origin);
        }

        public override void SetLength(long length)
        {
            _originalStream.SetLength(length);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _originalStream.Read(buffer, offset, count);
        }

        protected string GetTranslation(string resourceKey)
        {
            return Translations.GetTranslation(resourceKey);
        }

        protected string TranslateStream(string stream)
        {
            var browserBasedUrl = UrlHelper.Application.AppendInCase("/").PrependInCase("/");
            return Translations.Translate(stream).Replace("~/", browserBasedUrl);
        }

        protected string CleanUp(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            //BasePage currentPage = BasePage.Current;

            //if (currentPage != null)
            //{
                content = content.Replace("\t", "");
                while (content.IndexOf("  ") != -1)
                {
                    content = content.Replace("  ", " ");
                }
            //}

            return content;
        }

    }

}
