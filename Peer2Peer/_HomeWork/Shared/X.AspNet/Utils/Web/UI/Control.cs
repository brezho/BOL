using System.IO;
using System.Text;

namespace System.Web.UI
{
    public static class ControlExtensions
    {
        /// <summary>
        /// Searches from the control instance for a server control with the specified
        /// <paramref Name="id"/> parameter.
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="startingControl">The starting control.</param>
        /// <param Name="id">The identifier for the control to be found.</param>
        /// <returns>The specified control, or null if the specified control does not exist.</returns>
        public static T FindControl<T>(this Control startingControl, string id) where T : Control
        {
            return startingControl.FindPageControl<T>(id);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representing the server control content.
        /// </summary>
        /// <param Name="control">The control instance.</param>
        /// <returns>A <see cref="System.String"/> representing the server control content.</returns>
        public static string RenderControl(this Control control)
        {
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            HtmlTextWriter hw = new HtmlTextWriter(tw);

            control.RenderControl(hw);
            return sb.ToString();
        }

        private static T FindPageControl<T>(this Control startingControl, string id) where T : Control
        {

            T found = null;
            Page p = startingControl.Page;

            foreach (Control activeControl in p.Controls)
            {
                found = activeControl as T;
                if (found == null || (string.Compare(id, found.ID, true) != 0))
                {
                    found = FindChildControl<T>(activeControl, id);
                }
                if (found != null) { break; }
            }
            return found;
        }

        public static T FindChildControl<T>(this Control startingControl, string id) where T : Control
        {
           // return startingControl.FindControl(id) as T;
            T found = null;
            foreach (Control activeControl in startingControl.Controls)
            {
                found = activeControl as T;
                if ((found != null) && string.Equals(id, found.ID, StringComparison.InvariantCultureIgnoreCase)) break;
                else
                {
                    found = FindChildControl<T>(activeControl, id);
                    if (found != null) break;
                }
            }
            return found;
        }

        public static T FindChildControl<T>(this Control startingControl) where T : Control
        {

            T found = null;
            foreach (Control activeControl in startingControl.Controls)
            {
                found = activeControl as T;
                if (found == null)
                {
                    found = FindChildControl<T>(activeControl);
                }
                if (found != null) { break; }
            }
            return found;
        }
    }
}
