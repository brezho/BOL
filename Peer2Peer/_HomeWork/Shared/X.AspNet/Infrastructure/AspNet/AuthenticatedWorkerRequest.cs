using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Host.Infrastructure.AspNet
{
    class AuthenticatedWorkerRequest : HttpWorkerRequest
    {
        HttpListenerContext _context = null;

        public AuthenticatedWorkerRequest(HttpListenerContext ctx)
            : base()
        {
            _context = ctx;
        }

        public override IntPtr GetUserToken()
        {
            return (_context.User != null && _context.User.Identity != null && _context.User.Identity.IsAuthenticated)
                ? ((WindowsIdentity)_context.User.Identity).Token
                : IntPtr.Zero;
        }

        public override string GetServerVariable(string name)
        {
            switch (name)
            {
                case "HTTPS":
                    return _context.Request.IsSecureConnection ? "on" : "off";
                case "HTTP_USER_AGENT":
                    return _context.Request.Headers["UserAgent"];
                case "LOGON_USER":
                    return (_context.User != null && _context.User.Identity != null && _context.User.Identity.IsAuthenticated)
                        ? _context.User.Identity.Name :
                        null;
                case "AUTH_TYPE":
                    return (_context.User != null && _context.User.Identity != null && _context.User.Identity.IsAuthenticated)
                        ? _context.User.Identity.AuthenticationType :
                        null;
            }

            return base.GetServerVariable(name);
        }


        public override string GetRawUrl()
        {
            return _context.Request.Url.PathAndQuery;
        }
        public override string GetHttpVerbName()
        {
            return _context.Request.HttpMethod;
        }
        public override string GetUriPath()
        {
            return _context.Request.Url.LocalPath;
        }
        public override string GetQueryString()
        {
            return _context.Request.Url.Query.TrimStart('?');
        }
        public override void SendStatus(int statusCode, string statusDescription)
        {
            _context.Response.StatusCode = statusCode;
            _context.Response.StatusDescription = statusDescription;
        }
        public override void SendUnknownResponseHeader(string name, string value)
        {
            _context.Response.Headers[name] = value;
        }
        public override void SendKnownResponseHeader(int index, string value)
        {
            var headerName = HttpWorkerRequest.GetKnownResponseHeaderName(index);
            if (!WebHeaderCollection.IsRestricted(headerName)) _context.Response.Headers[headerName] = value;
        }
        public override void SendResponseFromMemory(byte[] data, int length)
        {
            _context.Response.OutputStream.WriteAsync(data, 0, length);
            //_context.Response.OutputStream.Write(data, 0, length);
        }
        public override void FlushResponse(bool finalFlush)
        {
            _context.Response.OutputStream.Flush();
        }
        public override void EndOfRequest()
        {
            // might not want to close as it might be 
            // prevent Owin pipeline to function properly... 
            // to be followed up
            _context.Response.OutputStream.Close();
        }
        public override string GetRemoteAddress()
        {
            return _context.Request.RemoteEndPoint.Address.ToString();
        }

        public override string GetLocalAddress()
        {
            return _context.Request.LocalEndPoint.Address.ToString();
        }

        public override int GetLocalPort()
        {
            return _context.Request.LocalEndPoint.Port;
        }
        #region needed override

        public override int ReadEntityBody(byte[] buffer, int size)
        {
            return _context.Request.InputStream.Read(buffer, 0, size);
        }

        public override string GetKnownRequestHeader(int index)
        {
            return _context.Request.Headers[GetKnownRequestHeaderName(index)];
        }

        #endregion



        #region unused
        ///
        public override int GetRemotePort()
        {
            throw new NotImplementedException();
            //     return _context.Request.RemotePort.HasValue ? _context.Request.RemotePort.Value : 80;
        }
        public override string GetHttpVersion()
        {
            return string.Format("HTTP/{0}.{1}", _context.Request.ProtocolVersion.Major, _context.Request.ProtocolVersion.Minor);
        }



        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            throw new NotImplementedException();
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            throw new NotImplementedException();
        }
        #endregion unused


        #region uncertain
        //public override void CloseConnection()
        //{
        //    Console.WriteLine("CloseConnection");
        //    //_context.Close();
        //}

        //public override string GetAppPath()
        //{
        //    Console.WriteLine("GetAppPath");
        //    return _virtualDir;
        //}

        //public override string GetAppPathTranslated()
        //{
        //    Console.WriteLine("GetAppPathTranslated");
        //    return _physicalDir;
        //}


        //public override string GetUnknownRequestHeader(string name)
        //{
        //    Console.WriteLine("GetUnknownRequestHeader");
        //    return _context.Request.Headers[name];
        //}

        //public override string[][] GetUnknownRequestHeaders()
        //{
        //    Console.WriteLine("GetUnknownRequestHeaders");
        //    string[][] unknownRequestHeaders;


        //    var headers = _context.Request.Headers;
        //    List<string[]> headerPairs = new List<string[]>(headers.Count);

        //    foreach (var he in headers)
        //    {
        //        if (GetKnownRequestHeaderIndex(he.Key) == -1)
        //        {
        //            headerPairs.Add(new string[] { he.Key, he.Value.FirstOrDefault() ?? string.Empty });
        //        }
        //    }
        //    unknownRequestHeaders = headerPairs.ToArray();
        //    return unknownRequestHeaders;

        //}


        //public override string GetServerVariable(string name)
        //{
        //    Console.WriteLine("GetServerVariable");
        //    // TODO: vet this list
        //    switch (name)
        //    {
        //        case "HTTPS":
        //            return _context.Request.IsSecure ? "on" : "off";
        //        // return _context.Request.IsSecureConnection ? "on" : "off";
        //        case "HTTP_USER_AGENT":
        //            return _context.Request.Headers["UserAgent"];
        //        default:
        //            return null;
        //    }
        //}

        //public override string GetFilePath()
        //{
        //    Console.WriteLine("GetFilePath");
        //    // TODO: this is a hack
        //    string s = _context.Request.Uri.LocalPath;
        //    //string s = _context.Request.Url.LocalPath;
        //    if (s.IndexOf(".aspx") != -1) s = s.Substring(0, s.IndexOf(".aspx") + 5);
        //    else if (s.IndexOf(".asmx") != -1) s = s.Substring(0, s.IndexOf(".asmx") + 5);
        //    return s;
        //}

        //public override string GetFilePathTranslated()
        //{
        //    Console.WriteLine("GetFilePathTranslated");
        //    string s = GetFilePath();
        //    s = s.Substring(_virtualDir.Length);
        //    s = s.Replace('/', '\\');
        //    return _physicalDir + s;
        //}

        //public override string GetPathInfo()
        //{
        //    Console.WriteLine("GetPathInfo");
        //    string s1 = GetFilePath();
        //    string s2 = _context.Request.Uri.LocalPath;
        //    //string s2 = _context.Request.Url.LocalPath;
        //    if (s1.Length == s2.Length)
        //        return "";
        //    else
        //        return s2.Substring(s1.Length);
        //}

        #endregion

    }
}
