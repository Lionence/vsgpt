using System.Windows;
using System.Windows.Controls;

namespace Lionence.VSGPT.Control
{
    public partial class ChatGPTEditorControl : UserControl
    {
        public ChatGPTEditorControl()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                string.Format(System.Globalization.CultureInfo.CurrentUICulture, "Invoked '{0}'", this.ToString()),
                "ToolWindow1");
        }
    }
}