using System.Collections.Generic;
using System.Linq;
using System;
namespace System.Web.UI.WebControls
{
    public static class WebControlExtensions
    {
        /// <summary>
        /// Adds the specified class(es) to the control.
        /// </summary>
        /// <param Name="control">The control.</param>
        /// <param Name="cssClass">The CSS class.</param>
        /// <returns></returns>
        public static WebControl AddClass(this WebControl control, string cssClass)
        {
            if (!control.HasClass(cssClass)) control.CssClass = (control.CssClass + " " + cssClass).Trim();
            //control.CssClass = (control.CssClass + " " + cssClass).Trim();
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
        public static bool HasClass(this WebControl control, string cssClass)
        {
            bool result = false;
            if (control.CssClass.IsFilled())
            {
                result = control.CssClass.Split(' ').Any(x => x == cssClass);
            }
            return result;
        }

        /// <summary>
        /// Remove a single class, multiple classes, or all classes from the control.
        /// </summary>
        /// <param Name="control">The control.</param>
        /// <param Name="cssClass">The CSS class(es).</param>
        /// <returns></returns>
        public static WebControl RemoveClass(this WebControl control, string cssClass)
        {
            cssClass.CanNotBeEmpty();

            if (control.CssClass.IsFilled())
            {
                var removeClassSplit = cssClass.Split(' ');
                var controlClassSplit = control.CssClass.Split(' ');
                control.CssClass = controlClassSplit.Where(x => !removeClassSplit.Contains(x)).ToString(" ").Trim();
            }

            return control;
        }

        public static WebControl RemoveClass(this WebControl control)
        {
            control.CssClass = "";
            return control;
        }

        public static IEnumerable<string> SelectedItems(this ListControl control)
        {
            return control.Items.Cast<ListItem>().Where(li => li.Selected).Select(li => li.Value);
        }
    }
}
