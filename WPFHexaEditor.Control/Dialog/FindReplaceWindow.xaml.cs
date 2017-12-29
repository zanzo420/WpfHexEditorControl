using System;
using System.IO;
using System.Windows;

namespace WpfHexaEditor.Dialog
{
    /// <summary>
    /// Logique d'interaction pour FindReplaceWindow.xaml
    /// </summary>
    public partial class FindReplaceWindow
    {
        public FindReplaceWindow()
        {
            InitializeComponent();

            //TEST
            MemoryStream ms = new MemoryStream(10);
            ms.WriteByte(0);
            FindHexEdit.Stream = ms;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
