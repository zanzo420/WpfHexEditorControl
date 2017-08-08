using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFHexaEditor.Core;
using WPFHexaEditor.Core.Bytes;

namespace WPFHexaEditor.Control.Dialog
{
    /// <summary>
    /// This Window is used to give a hex value for fill the selection with.
    /// </summary>
    public partial class FillWithByteWindow : Window
    {
        public FillWithByteWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }       

    }
}
