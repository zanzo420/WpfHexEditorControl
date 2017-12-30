using System.IO;
using System.Windows;

namespace WpfHexaEditor.Dialog
{
    /// <summary>
    /// Logique d'interaction pour FindReplaceWindow.xaml
    /// </summary>
    public partial class FindWindow
    {
        private readonly MemoryStream _findMs = new MemoryStream(1);
        private readonly HexEditor _parent;

        public FindWindow(HexEditor parent)
        {
            InitializeComponent();

            //Parent hexeditor for "binding" search
            _parent = parent;

            //Initialize stream and hexeditor
            _findMs.WriteByte(0);
            FindHexEdit.Stream = _findMs;
        }

        /// <summary>
        /// Save the stream before working with
        /// </summary>
        private void SaveStream() => FindHexEdit.SubmitChanges();

        private void FindAllButton_Click(object sender, RoutedEventArgs e)
        {
            SaveStream();
            _parent?.FindAll(_findMs.ToArray(), true);
        }

        private void FindFirstButton_Click(object sender, RoutedEventArgs e)
        {
            SaveStream();
            _parent?.FindFirst(_findMs.ToArray());
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
        
        private void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
            SaveStream();
            _parent?.FindNext(_findMs.ToArray());
        }

        private void FindLastButton_Click(object sender, RoutedEventArgs e)
        {
            SaveStream();
            _parent?.FindLast(_findMs.ToArray());
        }
    }
}
