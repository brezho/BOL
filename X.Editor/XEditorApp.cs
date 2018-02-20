using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.Application;
using X.Editor;
using X.Editor.Model;

namespace X.Application
{
    public abstract class XEditorApp : XApp
    {
        protected override sealed void OnInitialize(params string[] args)
        {
            base.OnInitialize(args);
            TypeCatalog.Instance.Add(new FileInfo(typeof(XEditorApp).Assembly.Location).Directory);

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            
        }
        protected override sealed void OnStart(params string[] args)
        {
            var frm = new Main(GetHierarchyProvider(args), args);
            System.Windows.Forms.Application.Run(frm);
        }

        protected virtual IHierarchyProvider GetHierarchyProvider(params string[] args)
        {
            return Config.GetProvider(args);
        }

    }
}
