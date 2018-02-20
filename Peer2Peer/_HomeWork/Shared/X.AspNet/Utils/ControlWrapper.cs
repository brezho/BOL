using Host.Infrastructure.VPP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace X.AspNet.Utils
{

    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public abstract class ControlWrapper<T> : Panel, INamingContainer
     where T : UserControl
    {
        private T _item;

        public T Item
        {
            get
            {
                EnsureChildControls();
                return _item;
            }
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page.InitComplete += new EventHandler(Page_InitComplete);
            if (!DesignMode)
            {
                EnsureChildControls();
                SetControlProperties();
            }
        }

        void Page_LoadComplete(object sender, EventArgs e)
        {
            SetControlProperties();
        }

        void Page_InitComplete(object sender, EventArgs e)
        {
            this.Hype().WireEventsTo(Item);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if (_item.IsNull())
            {
                var ascxType = typeof(T);
                var virtualPath = AssemblyScanner.GetVirtualPath(ascxType.FullName + ".ascx");
                _item = (T)this.Page.LoadControl(virtualPath);
                _item.ID = "virt";
                this.Controls.Add(_item);
                SetControlProperties();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            EnsureChildControls();
            SetControlProperties();
        }

        private void SetControlProperties()
        {
            System.Reflection.FastObjectAccessor.CloneMembers(this, Item, new string[] { "EnableTheming", "SkinID" });
        }
    }
}

