using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace X.AspNet.Infrastructure.Application.HttpHandlers
{
    class ControlLoader : HttpHandlerBase
    {
        public override void ProcessRequest(System.Web.HttpContext context)
        {
            var controlName = context.Request["controlName"];
            Page page = new Page();
            var ctl = page.LoadControl(controlName + ".ascx");
            page.Controls.Add(ctl);
            context.Server.Execute(page, context.Response.Output, true);
        }
    }
    class PageLoader : HttpHandlerBase
    {
        public override void ProcessRequest(System.Web.HttpContext context)
        {
            var controlName = context.Items["__ctrl"];
            BasePage page = new BasePage();
            var ctl = page.LoadControl((string)controlName );
            page.Controls.Add(ctl);
            context.Server.Execute(page, context.Response.Output, false);
            //context.Server.Execute(page, context.Response.Output, true);
        }
    }
}
