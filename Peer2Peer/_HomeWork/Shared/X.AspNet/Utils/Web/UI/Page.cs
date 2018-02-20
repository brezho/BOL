using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Web.UI
{
    public static class PageExtensions
    {
        public static string GetPostBackControlId(this Page page)
        {
            return page.Request.Form["__EVENTTARGET"];
        }

        public static string GetPostBackArguments(this Page page)
        {
            return page.Request.Form["__EVENTARGUMENT"];
        }

        public static Control GetPostBackControl(this Page page)
        {
            Control myControl = null;

            var controlId = page.GetPostBackControlId();

            if (((controlId != null) & (controlId != string.Empty)))
            {
                myControl = page.FindControl(controlId);
            }

            return myControl;
        }

        /// <summary>
        /// Loads the control.
        /// </summary>
        /// <param Name="page">The current page.</param>
        /// <param Name="userControlPath">The user control path.</param>
        /// <param Name="constructorParameters">The constructor parameters.</param>
        /// <returns>A new <see cref="Control"/> instance.</returns>
        public static System.Web.UI.Control LoadControl(this System.Web.UI.Page page, string userControlPath, params object[] constructorParameters)
        {
            var constParamTypes = new List<Type>();
            var constParamValues = new List<object>();
            foreach (object constParam in constructorParameters)
            {
                if (constParam != null)
                {
                    constParamTypes.Add(constParam.GetType());
                    constParamValues.Add(constParam);
                }
            }

            var control = page.LoadControl(userControlPath);
            ConstructorInfo constructor = control.GetType().BaseType.GetConstructor(constParamTypes.ToArray());

            if (constructor == null)
                throw new MemberAccessException("The requested constructor was not found on : " + control.GetType().BaseType.ToString());
            else
                constructor.Invoke(control, constParamValues.ToArray());

            // Finally return the fully initialized UC
            return control;
        }

    }
}
