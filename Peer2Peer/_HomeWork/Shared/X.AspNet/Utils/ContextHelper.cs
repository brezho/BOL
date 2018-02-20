using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace X.AspNet.Utils
{
    /// <summary>
    /// Represents an advanced context to store key/value pairs. 
    /// This class can be used to store frameworkDatabase for Web and Windows applications.
    /// </summary>
    public static class ContextHelper
    {
        #region Public methods

        /// <summary>
        /// Gets the value specified its <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">The type of the returned value</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The value corresponding to the specified <paramref name="key"/>.</returns>
        public static T Get<T>(string key)
        {
            return Get<T>(key, () => default(T));
        }

        public static T Get<T>(string key, T defaultValue)
        {
            return Get<T>(key, () => defaultValue);
        }


        public static T Get<T>(string key, Func<T> defaultValueDelegate)
        {
            var item = Get(key);

            if (item.IsNull())
            {
                item = defaultValueDelegate();
                if (!item.IsNull()) Set(key, item);
            }
            return (T)item;
        }

        static object Get(string key)
        {
            if (HttpContext.Current != null)
            {
                var value = GetWebRequestContext(key);
                return value ?? GetWebSessionContext(key);
            }
            return GetThreadScopedContext(key);
        }

        /// <summary>
        /// Sets the value (<paramref name="obj"/>) for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="obj">The object value.</param>
        public static void Set(string key, object obj)
        {
            Set(key, obj, false);
        }

        public static void Set(string key, object obj, bool isPersistent)
        {
            if (HttpContext.Current != null)
            {
                if (isPersistent) SetWebSessionContext(key, obj);
                else SetWebRequestContext(key, obj);
            }
            else SetThreadScopedContext(key, obj);
        }

        /// <summary>
        /// Removes the value for the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Remove(string key)
        {
            if (HttpContext.Current != null)
            {
                RemoveWebRequestContext(key);
                RemoveWebSessionContext(key);
            }
            else RemoveThreadScopedContext(key);
        }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns>The name of the current user.</returns>
        public static string GetUserName()
        {
            return GetUserIdentity().Name;
        }

        public static IIdentity GetUserIdentity()
        {
            return (HttpContext.Current.IsNotNull()
                && HttpContext.Current.User.IsNotNull()
                && HttpContext.Current.User.Identity.IsNotNull())
                       ? HttpContext.Current.User.Identity
                       : Thread.CurrentPrincipal.IsNotNull() && Thread.CurrentPrincipal.Identity.IsAuthenticated
                        ? Thread.CurrentPrincipal.Identity
                        : WindowsIdentity.GetCurrent();
        }
        #endregion

        public static string BinPath()
        {
            return (HttpContext.Current != null) ? HttpContext.Current.Server.MapPath("~").AppendInCase("\\") + "bin" : AppDomain.CurrentDomain.BaseDirectory;
        }

        #region Private Methods

        private static object GetWebRequestContext(string key)
        {
            return HttpContext.Current.Items.Contains(GetHttpContextKey(key))
                       ? HttpContext.Current.Items[GetHttpContextKey(key)]
                       : null;
        }

        private static object GetWebSessionContext(string key)
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session[key] != null)
                return HttpContext.Current.Session[key];
            return null;
        }

        private static void SetWebRequestContext(string key, object obj)
        {
            if (HttpContext.Current.Items[GetHttpContextKey(key)] == null)
                HttpContext.Current.Items.Add(GetHttpContextKey(key), obj);
            HttpContext.Current.Items[GetHttpContextKey(key)] = obj;
        }

        private static void SetWebSessionContext(string key, object obj)
        {
            if (HttpContext.Current.Session[key] == null)
                HttpContext.Current.Session.Add(key, obj);
            HttpContext.Current.Session[key] = obj;
        }

        private static void RemoveWebRequestContext(string key)
        {
            HttpContext.Current.Items.Remove(GetHttpContextKey(key));
        }

        private static void RemoveWebSessionContext(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }

        private static object GetThreadScopedContext(string key)
        {
            var dataStoreSlot = Thread.GetNamedDataSlot(GetThreadContextKey(key));
            return Thread.GetData(dataStoreSlot);
        }

        private static void SetThreadScopedContext(string key, object obj)
        {
            var dataStoreSlot = Thread.GetNamedDataSlot(GetThreadContextKey(key)) ??
                                Thread.AllocateNamedDataSlot(GetThreadContextKey(key));
            Thread.SetData(dataStoreSlot, obj);
        }

        private static void RemoveThreadScopedContext(string key)
        {
            Thread.FreeNamedDataSlot(GetThreadContextKey(key));
        }

        private static string GetThreadContextKey(string key)
        {
            return String.Format("{0}_{1}", "__" + key + "_", Thread.CurrentContext.ContextID);
        }

        private static string GetHttpContextKey(string key)
        {
            return "__" + key + "_" + HttpContext.Current.GetHashCode().ToString("x") + Thread.CurrentContext.ContextID;
        }

        internal static string GetSessionKey(string key)
        {
            return (HttpContext.Current != null) ? HttpContext.Current.Session.SessionID + "_" + key : GetThreadContextKey(key);
        }

        #endregion
    }
}
