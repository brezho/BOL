using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Host.Infrastructure.Application.StreamFilters
{
    internal class AjaxHttpFilter: StreamFilterBase 
    {
        private StringBuilder _responseHtml;
        private int _contentLength = 0;
        private bool _partOne = true;

        public AjaxHttpFilter(Stream originalStream)
            : base(originalStream)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // get buffer content
            string strBuffer = System.Text.UTF8Encoding.UTF8.GetString(buffer, offset, count);

            if (_partOne)
            {
                // determine the content length during first run
                if (_contentLength == 0)
                {
                    // Check for a valid Ajax header
                    Regex regEx = new Regex(@"^(?<length>\d+)\|[^\|]*\|[^\|]*\|", RegexOptions.Singleline | RegexOptions.Compiled);
                    Match m = regEx.Match(strBuffer);
                    if (m.Success)
                    {
                        // Read the length  
                        Group group = m.Groups["length"];
                        _contentLength = Convert.ToInt32(group.Value);

                        // initialise the StringBuilder (we  assume that translations increase
                        // the size by 20%
                        _responseHtml = new StringBuilder((int)(_contentLength * 1.2));
                    }
                    else
                    {
                        OriginalStream.Write(buffer, offset, count);
                        _partOne = false;
                        return;
                    }
                }

                // Add buffer to total buffer
                _responseHtml.Append(strBuffer);

                // Is all template received?
                if (IsAllDataReceived(_responseHtml.ToString()))
                {
                    //we have received all the template by now, so we can translate the content
                    string ajaxContent = _responseHtml.ToString();
                    // Translate the tokens in the html 
                    string translatedContent = RecursiveTranslateAjax(ajaxContent);
                    if (translatedContent != null)
                    {
                        byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(translatedContent);
                        // Write to the stream
                        OriginalStream.Write(data, 0, data.Length);
                    }

                    _partOne = false;
                }
            }
            else
            {
                // After the first part has been processed, just forward the other content to the browser.
                // this can also occur in multiple times if this 'rest'-template is totally larger than +-28K
                OriginalStream.Write(buffer, offset, count);
            } 
        }

        private bool IsAllDataReceived(string content)
        {

            bool success = true;

            Regex reg = new Regex(@"^(\d+)\|[^\|]*\|[^\|]*\|", RegexOptions.Singleline | RegexOptions.Compiled);
            Match m = reg.Match(content);
            if (m.Success)
            {
                int length = m.Groups[1].Value.ToInt(0);
                reg = new Regex(@"^(\d+)(\|[^\|]*\|[^\|]*\|)(.{" + length + @"})\|", RegexOptions.Singleline | RegexOptions.Compiled);
                m = reg.Match(content);

                //Check if the stream is cut, then its not yet completed
                if (m.Success)
                    return IsAllDataReceived(content.Substring(m.Length));
                else
                    return false;

            }
            return success;
        }

        private string RecursiveTranslateAjax(string content)
        {
            Regex reg = new Regex(@"^(\d+)\|[^\|]*\|[^\|]*\|", RegexOptions.Singleline | RegexOptions.Compiled);
            Match m = reg.Match(content);
            if (m.Success)
            {
                int length = m.Groups[1].Value.ToInt(0);
                reg = new Regex(@"^(\d+)(\|[^\|]*\|[^\|]*\|)(.{" + length + @"})\|", RegexOptions.Singleline | RegexOptions.Compiled);
                m = reg.Match(content);
                if (m.Success)
                {
                    string trans = TranslateStream(m.Groups[3].Value);
                    trans = CleanUp(trans);

                    return trans.Length + m.Groups[2].Value +
                        trans + "|" +
                        RecursiveTranslateAjax(content.Substring(m.Length));
                }
            }
            // if not Ajax, just translate everything,
            // it must be a normal PostBack or a string of some sort.
            return content;
        }
    }
}
