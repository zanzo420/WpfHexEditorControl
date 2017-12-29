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
        MemoryStream ms = new MemoryStream(10);

        public FindReplaceWindow()
        {
            InitializeComponent();

            //TEST

            ms.WriteByte(0);
            FindHexEdit.Stream = ms;
        }
    }
}
