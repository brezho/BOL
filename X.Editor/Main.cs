using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using X.Editor.Documents;
using X.Editor.Tools.Commander;
using X.Editor.Tools.Explorer;
using X.Editor.Tools.Output;
using X.Editor.Tools.PropertyGrid;
using X.Editor.Model;
using System.Helpers;

namespace X.Editor
{
    class MainSuppress { }
    public partial class Main : System.Windows.Forms.Form, IEditorShell
    {
        public event EventHandler<HirarchyChangedEventArgs> HierarchyChanged;

        public static Font Consolas975 = new System.Drawing.Font("Consolas", 9.75f);

        private Hierarchy _hierarchy;
        public Hierarchy Hierarchy
        {
            get { return _hierarchy; }
            private set
            {
                if (value != _hierarchy)
                {
                    _hierarchy = value;
                  //  EventsHelper.Fire(HierarchyChanged, this, new HirarchyChangedEventArgs(value));
                    if (HierarchyChanged != null) HierarchyChanged(this, new HirarchyChangedEventArgs(value));
                }
            }
        }
        public CommandsList Commands { get; private set; }

        public void Trace(string message)
        {
            output.Write(message);
        }

        public void TraceLine(string message)
        {
            Trace(message + Environment.NewLine);
        }
        public void Trace(string message, params object[] args)
        {
            Trace(string.Format(message, args));
        }

        ExplorerTool explorer;
        CommanderTool commander;
        OutputTool output;
        PropertyGridTool propertyGrid;

        public Main(IHierarchyProvider hierarchyProvider, params string[] args)
        {
            InitializeComponent();

            Commands = new CommandsList();

            explorer = new ExplorerTool(this);
            commander = new CommanderTool(this);
            output = new OutputTool(this);
            propertyGrid = new PropertyGridTool(this);

            Hierarchy = hierarchyProvider.CreateHierarchy(this);
            var startingNode = Hierarchy.Nodes().FirstOrDefault();

            Hierarchy.ActivationRequested += (ars, ara) =>
            {
                var window = DockPanel.GetOpenDocumentWindows().Where(x => x.Node.Equals(ara)).FirstOrDefault();
                if (window != null) window.Activate();
                else
                {
                    var editor = ara.GetEditor(typeof(Control));
                    if (editor != null)
                    {
                        var newWindow = new DocumentWindowBase(this, ara);
                        newWindow.Show(this.DockPanel, DockState.Document);
                        newWindow.Controls.Add(editor as Control);
                        editor.ActivateIn(newWindow);
                    }
                }
            };


            commander.Show(DockPanel, DockState.DockLeft);
            explorer.Show(DockPanel, DockState.DockLeft);
            propertyGrid.Show(DockPanel, DockState.DockRight);
            output.Show(DockPanel, DockState.DockBottom);

            if (startingNode != null)
            {
                startingNode.Select();
                Hierarchy.Activate(startingNode);
            }


            //Commands.Add("Show Output", () => { output.Show(this.DockPanel, DockState.DockBottom); }, "Show the output panel");
            //  Commands.Add("Show Explorer", () => { explorer.Show(this.DockPanel, DockState.DockLeft); }, "Show the explorer panel");
            // Commands.Add("Show Properties", () => { propertyGrid.Show(this.DockPanel, DockState.DockRight); }, "Show the selected object properties panel");

            //       RegisterQuickieText();

            SetupKeyHook();
            GoFullscreen(false);
        }

        internal void SetupKeyHook()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, a) =>
            {
                if (a.KeyCode != Keys.ControlKey
                    && a.KeyCode != Keys.Menu
                    && a.KeyCode != Keys.ShiftKey)
                {
                    var key = new UserInput() { Alt = a.Alt, Control = a.Control, Shift = a.Shift, Key = a.KeyCode.ToString() };
                    a.Handled = this.Hierarchy.HandleUserInput(key);
                    if (a.Handled)
                    {
                        TraceLine(this.Hierarchy.SelectedNode.NodeDataAdapter.GetDisplayName() + " " + key.ToString());
                    }
                }
            };
        }

        private void GoFullscreen(bool fullscreen)
        {
            ShowIcon = false;
            Text = "X.Edit";
            if (fullscreen)
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                Bounds = Screen.PrimaryScreen.Bounds;
            }
            else
            {
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }

        WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel;


        private void InitializeComponent()
        {
            DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            //   DockPanel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;

            DockPanel.Theme = new VS2015BlueTheme();

            Controls.Add(DockPanel);
            Font = Consolas975;

            IsMdiContainer = true;
            Name = "Main";
            Text = "Main";
        }

        void RegisterQuickieText()
        {
            DockPanel.Click += (s, a) =>
            {
                var tb = new System.Windows.Forms.TextBox();
                var f = new System.Windows.Forms.Form();

                tb.KeyDown += (kps, kpa) =>
                {
                    if (kpa.KeyCode == Keys.Escape) f.Close();
                    if (kpa.KeyCode == Keys.Enter)
                    {
                        var command = tb.Text.ToLowerInvariant();
                        // Do some more intelligent stuff
                        // than this:

                        if (command.StartsWith("remove"))
                        {
                            var id = command.Split(' ')[1];
                            Hierarchy.Remove(long.Parse(id));
                        }

                        if (command == "com")
                        {
                            this.Commands.Add("Hey " + this.Commands.Count(), (n, x) => { MessageBox.Show(n.NodeDataAdapter.GetDisplayName()); }, "Nice one buddy");
                        }
                        if (command == "comr")
                        {
                            var first = this.Commands.First();
                            //  if (first != null) Hierarchy.Commands.RemoveItem(first);
                        }
                        f.Close();
                    }
                };


                tb.Width = 300;
                tb.Font = Consolas975;
                f.WindowState = FormWindowState.Normal;
                f.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                f.Controls.Add(tb);
                f.Show();
                f.SetDesktopLocation(Cursor.Position.X, Cursor.Position.Y);
                f.ClientSize = tb.Size;

            };

        }

        public bool AskApproval(string title, string message)
        {
            return MessageBox.Show(message, title, MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK;
        }

        public void ShowModal(string title, string text)
        {
            MessageBox.Show(text, title);
        }
    }
}
