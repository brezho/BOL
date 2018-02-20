using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaBuilder.UIModel
{
    public partial class DockableWindow : WeifenLuo.WinFormsUI.Docking.DockContent, IInitialize
    {
        protected IdeEntity _ide;
        public DockableWindow()
        {
            InitializeComponent();
        }

        public virtual void Initialize(IdeEntity ide)
        {
            _ide = ide;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DockableWindow
            // 
            this.ClientSize = new System.Drawing.Size(294, 272);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DockableWindow";
            this.ResumeLayout(false);

        }
    }
}
