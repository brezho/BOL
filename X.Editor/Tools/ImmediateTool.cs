using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using X.Editor.Model;

namespace X.Editor.Tools
{
    partial class X { }
    public class ImmediateTool : ToolWindowBase
    {
        TextBox OutBox;

        public string Prompt { get; set; }
        public ImmediateTool(IEditorShell main)
            : base(main)
        {
            Text = "Immediate";
            Prompt = "";
            OutBox = new TextBox();
            OutBox.Multiline = true;
            OutBox.Dock = DockStyle.Fill;
            OutBox.ScrollBars = ScrollBars.Vertical;

            OutBox.KeyDown += (s, kpa) =>
            {
                HandleKeyPress(kpa);
            };
            ClearAll();
            Controls.Add(OutBox);
        }

        private void HandleKeyPress(KeyEventArgs kpa)
        {
            switch (kpa.KeyCode)
            {
                case Keys.Escape:
                    if (kpa.Control) ClearAll();
                    return;
                case Keys.Enter:
                    kpa.SuppressKeyPress = true;
                    var cmd = GetCommandText();
                    OutBox.Text += Environment.NewLine;
                    if (cmd.Length > 0)
                    {
                        var result = InterpretCommand(cmd);
                        if (!string.IsNullOrWhiteSpace(result)) AppendResult(result);
                    }
                    AppendPrompt();
                    return;

                case Keys.Left:
                    if (OutBox.SelectionStart <= inputStartPosition)
                    {
                        kpa.SuppressKeyPress = true;
                    }
                    return;

                case Keys.Right:
                    // do nothing, normal behaviour quite alright
                    return;

                case Keys.Up:
                    // Need to recall History
                    ClearCommandText();
                    kpa.SuppressKeyPress = true;
                    return;

                case Keys.ControlKey:
                    return;

                case Keys.C:
                    if (kpa.Control)
                    {
                        return;
                    }
                    break;
            }

            // handle positioning (in case user has set the caret out of typing area)
            if (OutBox.SelectionStart < inputStartPosition)
            {
                OutBox.SelectionStart = inputStartPosition + GetCommandText().Length;
            }

        }

        string PromptDisplayText { get { return " " + Prompt + "> "; } }


        int inputStartPosition = 0;
        private void AppendResult(string result)
        {
            var txt = result.AppendInCase(Environment.NewLine);
            OutBox.Text = OutBox.Text.Append(txt);
        }

        private string GetCommandText()
        {
            var lastLine = OutBox.Text.Split(Environment.NewLine).LastOrDefault();
            var cmdText = lastLine.RemoveStart(PromptDisplayText);
            return cmdText;
        }

        string InterpretCommand(string commandText)
        {
            var interpretation = Shell.Hierarchy.InterpretCommand(commandText);
            if (interpretation.Exception != null)
            {
                return interpretation.Exception.Format();
            }
            return interpretation.Result;
        }

        void ClearAll()
        {
            OutBox.Clear();
            AppendPrompt();
        }
        void ClearCommandText()
        {
            var lastLine = OutBox.Text.Split(Environment.NewLine).LastOrDefault();
            OutBox.Text = OutBox.Text.RemoveEnd(lastLine);
            AppendPrompt();
        }

        private void AppendPrompt()
        {
            OutBox.Text += PromptDisplayText;
            OutBox.SelectionStart = OutBox.Text.Length;
            inputStartPosition = OutBox.Text.Length;
            OutBox.ScrollToCaret();
        }

        //internal void InquireCaretPosition(out int line, out int column)
        //{
        //    line = 0;
        //    column = 0;
        //    int caret = OutBox.SelectionStart;
        //    int iLine = OutBox.GetLineFromCharIndex(caret);
        //    if (iLine < 0) iLine = 0;
        //    line = iLine;
        //    int firstChar = OutBox.GetFirstCharIndexFromLine(iLine);
        //    if (firstChar < 0) firstChar = 0;
        //    column = caret - firstChar;
        //}
    }
}
