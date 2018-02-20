using Host.Default.Helpers.HttpFilters;
using Host.Infrastructure.Application;
using X.AspNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace X.AspNet
{
    public class BasePage : System.Web.UI.Page
    {
        public static BasePage Current
        {
            get
            {
                return HttpContext.Current.Handler as BasePage;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (MasterPageFile == null) MasterPageFile = WebApp.Current.BasePageUrl;
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Add the control to the PlaceHolderContent (with ID=Content) of the master page
            if (CurrentControl != null) Master.FindControl("Content").Controls.Add(CurrentControl);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.Header.Controls.Add(new LiteralControl("<meta Name=\"version\" content=\"" + WebApp.Current.Version() + "\" />"));
            Page.Header.Controls.Add(new LiteralControl(@"<!--[if lt IE 9]><script src=""https://oss.maxcdn.com/html5shiv/3.7.2/html5shiv.min.js""></script><script src=""https://oss.maxcdn.com/respond/1.4.2/respond.min.js""></script><![endif]-->"));
            RegisterStyleSheetFile("~/Css.ashx");
            RegisterClientScriptFile("~/Js.ashx");
        }
        protected void RegisterStyleSheetFile(string path)
        {
            var ctrl = new HtmlGenericControl("link");

            path = UrlHelper.Application + path.TrimStart('~');
            path = ScriptsUtils.AddResourceMark(path);

            ctrl.Attributes.Add("href", path.PrependInCase("/"));
            ctrl.Attributes.Add("type", "text/css");
            ctrl.Attributes.Add("rel", "stylesheet");

            if (path.EndsWith(".less", StringComparison.OrdinalIgnoreCase))
            {
                ctrl.Attributes["rel"] += "/less";
            }

            Page.Header.Controls.Add(ctrl);
        }

        protected void RegisterClientScriptFile(string path)
        {
            path = UrlHelper.Application + path.TrimStart('~');
            path = ScriptsUtils.AddResourceMark(path);
            ScriptManager.GetCurrent(Page).Scripts.Add(new ScriptReference(path.PrependInCase("/")));
        }

        string CurrentControlVirtualPath
        {
            get
            {
                var res = HttpContext.Current.Items["__ctrl"] as string;
                return res;
            }
        }

        UserControl currentModule = null;

        public UserControl CurrentControl
        {
            get
            {
                EnsureControlIsLoaded();
                return currentModule;
            }
        }

        void EnsureControlIsLoaded()
        {
            if ((currentModule == null) && this.CurrentControlVirtualPath != null)
            {
                currentModule = LoadControl(this.CurrentControlVirtualPath) as UserControl;

                if (currentModule == null) throw new Exception("Control must inherit " + typeof(UserControl).Name);

                SetProperties(currentModule);
            }
        }

        void SetProperties(UserControl control)
        {
            Type t = control.GetType();

            Url url = new Url(Request.Url.OriginalString);
            foreach (var queryParam in url.Queries)
            {
                System.Reflection.PropertyInfo pi = t.GetProperty(queryParam.Key);
                if (pi != null && pi.CanWrite)
                {
                    pi.SetValue(control, queryParam.Value, null);
                }
            }
        }

    }
}
