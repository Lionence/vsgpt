using Lionence.VSGPT.Control;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace Lionence.VSGPT.Window
{
    [Guid("bbc33f4a-54d9-44b6-a413-36efdbb66421")]
    public class ChatGPTEditorWindow : ToolWindowPane
    {
        public ChatGPTEditorWindow() : base(null)
        {
            Caption = "ChatGPT";
            Content = new ChatGPTEditorControl();
        }
    }
}
