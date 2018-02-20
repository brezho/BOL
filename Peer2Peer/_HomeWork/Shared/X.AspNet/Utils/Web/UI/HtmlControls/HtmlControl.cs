using System.Collections.Generic;
using System.Linq;

namespace System.Web.UI.HtmlControls
{
    public static class HtmlControlExtensions
    {
        /// <summary>
        /// Adds the specified class(es) to the control.
        /// </summary>
        /// <param Name="control">The control.</param>
        /// <param Name="cssClass">The CSS class.</param>
        /// <returns></returns>
        public static HtmlControl AddClass(this HtmlControl control, string cssClass)
        {
            if (control.Attributes["class"] != null)
                control.Attributes.SetAttribute("class", (control.Attributes["class"] + " " + cssClass).Trim());
            else
                control.Attributes.SetAttribute("class", cssClass);

            return control;
        }


        /// <summary>
        /// Determines whether the control has the specified class.
        /// </summary>
        /// <param Name="control">The control.</param>
        /// <param Name="cssClass">The CSS class.</param>
        /// <returns>
        /// 	<c>true</c> if the specified control has class; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasClass(this HtmlControl control, string cssClass)
        {
            bool result = false;
            if (control.Attributes["class"] != null)
            {
                result = control.Attributes["class"].Split(' ').Any(x => x == cssClass);
            }
            return result;
        }

        /// <summary>
        /// Remove a single class, multiple classes, or all classes from the control.
        /// </summary>
        /// <param Name="control">The control.</param>
        /// <param Name="cssClass">The CSS class(es).</param>
        /// <returns></returns>
        public static HtmlControl RemoveClass(this HtmlControl control, string cssClass)
        {
            if (!cssClass.IsFilled())
            {
                if (control.Attributes["class"] != null)
                {
                    var removeClassSplit = cssClass.Split(' ');
                    var controlClassSplit = control.Attributes["class"].Split(' ');
                    control.Attributes.SetAttribute("class", controlClassSplit.Where(x => !removeClassSplit.Contains(x)).ToString(" ").Trim());
                }
            }
            else
            {
                // Remove all classes
                control.Attributes.SetAttribute("class", "");
            }

            return control;
        }
    }
}
