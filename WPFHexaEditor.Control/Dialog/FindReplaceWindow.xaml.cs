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

        private void FindAllButton_Click(object sender, RoutedEventArgs e) =>
            _parent?.FindAll(GetAllByteFromHexEditor(), true);

        private byte[] GetAllByteFromHexEditor()
        {
            var cstream = new MemoryStream();
            FindHexEdit.CopyToStream(cstream, 0, FindHexEdit.Lenght - 1, true);
            return cstream.ToArray();
        }

        private void FindFirstButton_Click(object sender, RoutedEventArgs e) =>
            _parent?.FindFirst(GetAllByteFromHexEditor());

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void FindNextButton_Click(object sender, RoutedEventArgs e) =>
            _parent?.FindNext(GetAllByteFromHexEditor());

        private void FindLastButton_Click(object sender, RoutedEventArgs e) =>
            _parent?.FindLast(GetAllByteFromHexEditor());
    }
}
