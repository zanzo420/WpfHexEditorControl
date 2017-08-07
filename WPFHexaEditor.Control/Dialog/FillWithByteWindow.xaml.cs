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

            CheckIfEnable();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckIfEnable();
        }

        private void CheckIfEnable()
        {
            OKButton.IsEnabled = ByteConverters.IsHexaValue(FillTextBox.Text);
        }

        /// <summary>
        /// Give the value in Byte
        /// </summary>
        public byte? Value
        {
            get
            {
                if (ByteConverters.IsHexaValue(FillTextBox.Text))
                {
                    return ByteConverters.HexToByte(FillTextBox.Text)[0];
                }
                else
                    return null;
            }
        }

        private void FillTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyValidator.IsHexKey(e.Key) ||
                KeyValidator.IsBackspaceKey(e.Key) ||
                KeyValidator.IsDeleteKey(e.Key) ||
                KeyValidator.IsEnterKey(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
