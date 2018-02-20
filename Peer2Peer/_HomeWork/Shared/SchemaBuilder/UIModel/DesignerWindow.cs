using SchemaBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchemaBuilder.UIModel.Designers
{
    public class DesignerWindow : DockableWindow
    {
        IView _view;
        public IView View
        {
            get { return _view; }
            set
            {
                _view = value;
                OnDataObjectSet();
            }
        }
       
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _ide.OnViewClosing(View, e);
            base.OnClosing(e);
        }

        protected internal virtual void OnDataObjectSet()
        {
            this.TabText = View.Model.Name;
            this.Controls.Add(View.InnerControl);
            View.InnerControl.Dock = DockStyle.Fill;
        }

    }
}
