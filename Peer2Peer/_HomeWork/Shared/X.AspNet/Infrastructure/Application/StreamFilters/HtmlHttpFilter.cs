using System.IO;

namespace Host.Infrastructure.Application.StreamFilters
{
    internal class HtmlHttpFilter : StreamFilterBase
    {
        private string tempBuffer = string.Empty;

        public HtmlHttpFilter(Stream originalStream)
            : base(originalStream)
        {

        }

        // The Write method actually does the filtering.
        public override void Write(byte[] buffer, int offset, int count)
        {
            // Get buffer content
            string strBuffer = tempBuffer + System.Text.UTF8Encoding.UTF8.GetString(buffer, offset, count);
            tempBuffer = string.Empty;

            // Append the current buffer in the response
            var translated = TranslateStream(strBuffer);

            var index = translated.IndexOf("[[");
            if (index > 0)
            {
                tempBuffer = translated.Substring(index, translated.Length - index);
                translated = translated.Substring(0, index);
            }
           
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(translated);
            OriginalStream.Write(data, 0, data.Length);
        }

        public override void Flush()
        {
            // Append the current buffer in the response
            var translated = TranslateStream(tempBuffer);

            translated = CleanUp(translated);
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(translated);
            OriginalStream.Write(data, 0, data.Length);
            
            base.Flush();
        }
    }
}
