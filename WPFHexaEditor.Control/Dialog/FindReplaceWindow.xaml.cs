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
        private readonly MemoryStream _findMs = new MemoryStream(1);
        private readonly MemoryStream _replaceMs = new MemoryStream(1);
        private readonly HexEditor _parent;

        public FindReplaceWindow(HexEditor parent)
        {
            InitializeComponent();

            //Parent hexeditor for "binding" search
            _parent = parent;

            //Initialize stream and hexeditor
            _findMs.WriteByte(0);
            _replaceMs.WriteByte(0);
            FindHexEdit.Stream = _findMs;
            ReplaceHexEdit.Stream = _replaceMs;
        }

        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            SaveStream();
        }

        /// <summary>
        /// Save the stream before working with
        /// </summary>
        private void SaveStream()
        {
            FindHexEdit.SubmitChanges();
            ReplaceHexEdit.SubmitChanges();
        }

        private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FindAllButton_Click(object sender, RoutedEventArgs e)
        {
            SaveStream();

            _parent?.FindAll(_findMs.ToArray(), true);

            Close();            
        }
    }
}
