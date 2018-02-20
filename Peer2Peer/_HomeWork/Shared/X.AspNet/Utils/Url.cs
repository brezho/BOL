using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace X.AspNet.Utils
{
    /// <summary>
    /// Represents a URL
    /// </summary>
    public class Url
    {
        //HttpContext.Current.Request.Url

        #region Fields

        // string representation of the url
        private string _url;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="Url"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        public Url(string url)
        {
            _url = url;
        }

        public static Url Current
        {
            get
            {
                return new Url(HttpContext.Current.Request.Url.ToString());
            }
        }
        #endregion

        #region Properties

        private Pair PathAndQuery
        {
            get
            {
                if (!string.IsNullOrEmpty(_url))
                {
                    var urlSplitList = _url.Split('?');

                    if (urlSplitList.Length == 0 || urlSplitList.Length > 2)
                    {
                        throw new FormatException(string.Format("The format of the URL {0} is not valid.", _url));
                    }

                    if (urlSplitList.Length == 1)
                    {
                        return new Pair(urlSplitList[0], string.Empty);
                    }

                    if (urlSplitList.Length == 2)
                    {
                        return new Pair(urlSplitList[0], urlSplitList[1]);
                    }
                }

                return new Pair(string.Empty, string.Empty);
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The path.</value>
        public string Path
        {
            get { return PathAndQuery.First as string; }
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <value>The query.</value>
        public string Query
        {
            get { return PathAndQuery.Second as string; }
        }

        public string Extension
        {
            get
            {
                string result = null;
                var slashSplitted = Path.Split('/');
                if (slashSplitted.Length > 1)
                {
                    var dotSplitted = slashSplitted.Last().Split('.');
                    if (dotSplitted.Length > 1) result = dotSplitted.Last();
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the query parameters.
        /// </summary>
        /// <value>The list of query parameters.</value>
        public IEnumerable<QueryParameter> Queries
        {
            get
            {
                var queriesSplitList = Query.Split('&');

                return (from queryItem in queriesSplitList
                        select queryItem.Split('=')
                            into querySplit
                            where querySplit.Length > 1
                            select new QueryParameter(querySplit[0], querySplit[1]));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public Url AddQueryParameter(string key, string value)
        {
            RemoveQueryParameter(key);
            if (Query.Length > 0)
            {
                _url += string.Format("&{0}={1}", key, value);
            }
            else
            {
                _url += string.Format("?{0}={1}", key, value);
            }
            return this;
        }

        /// <summary>
        /// Adds a query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public Url AddQueryParameter(string key, object value)
        {
            return AddQueryParameter(key, value.ToString());
        }

        /// <summary>
        /// Removes a query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        public Url RemoveQueryParameter(string key)
        {
            var queries = Queries;
            if (queries.Count() > 0)
            {
                var urlBuilder = new StringBuilder(Path);
                urlBuilder.Append('?');

                var i = 0;
                foreach (var queryParameter in queries)
                {
                    if (queryParameter.Key != key)
                    {
                        urlBuilder.Append(queryParameter);
                        if (i < queries.Count() - 1)
                        {
                            urlBuilder.Append('&');
                        }
                    }
                    i++;
                }

                _url = urlBuilder.ToString();
                if (_url.EndsWith("?") || _url.EndsWith("&"))
                {
                    _url = _url.Substring(0, _url.Length - 1);
                }
            }
            return this;
        }

        /// <summary>
        /// Gets the query parameter value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetQueryParameterValue(string key)
        {
            var queryParameter = GetQueryParameter(key);
            return queryParameter == null ? string.Empty : queryParameter.Value;
        }

        /// <summary>
        /// Gets the query parameter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public QueryParameter GetQueryParameter(string key)
        {
            return Queries.FirstOrDefault(queryParameter => queryParameter.Key == key);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this concreteFeatureType.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this concreteFeatureType.
        /// </returns>
        public override string ToString()
        {
            return _url;
        }

        /// <summary>
        /// Resolves the URL (transforms the URL to absolute URL).
        /// </summary>
        /// <returns></returns>
        //public string ResolveUrl()
        //{
        //    if (_url.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
        //        _url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) ||
        //        _url.StartsWith("ftp://", StringComparison.InvariantCultureIgnoreCase) ||
        //        _url.StartsWith("sftp://", StringComparison.InvariantCultureIgnoreCase))
        //        return _url;

        //    if (_url.StartsWith("~"))
        //        _url = _url.Substring(1);
        //    _url = _url.PrependInCase("/");

        //    var protocol = UrlHelper.Scheme.AppendInCase("://");
        //    var host = UrlHelper.Host.EndsWith(@".")
        //                        ? UrlHelper.Host.Remove(UrlHelper.Host.Length - 1)
        //                        : UrlHelper.Host;
        //    var port = !UrlHelper.Port.IsNullOrEmpty()
        //                        ? UrlHelper.Port.PrependInCase(":")
        //                        : string.Empty;
        //    var application = !UrlHelper.Application.IsNullOrEmpty()
        //                        ? UrlHelper.Application.PrependInCase("/")
        //                        : string.Empty;

        //    var absolute = string.Concat(protocol, host, port, application, _url);
        //    return absolute;
        //}

        /// <summary>
        /// Resolves the URL (transforms the URL to absolute URL).
        /// </summary>
        public string ResolveClientUrl()
        {
            Control ctrl = new Control();
            return ctrl.ResolveClientUrl(_url);
        }

        #endregion
    }

    /// <summary>
    /// Represents a query parameter.
    /// </summary>
    public class QueryParameter
    {
        private KeyValuePair<string, string> _queryParameter;

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key
        {
            get { return _queryParameter.Key; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value
        {
            get { return _queryParameter.Value; }
        }

        /// <summary>
        /// Initializes a new concreteFeatureType of the <see cref="QueryParameter"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public QueryParameter(string key, string value)
        {
            _queryParameter = new KeyValuePair<string, string>(key, value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this concreteFeatureType.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this concreteFeatureType.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", _queryParameter.Key, _queryParameter.Value);
        }
    }
}