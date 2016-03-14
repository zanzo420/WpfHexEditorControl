using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using WPFHexaEditor.Control.Core;

namespace WPFHexaEditorExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TEMPS
            //HexEdit.FileName = @"C:\Test\NETwsw01.sys";
        }

        private void SetPositionButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SetPosition("0x20FF", 10);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.BytePerLine = 26;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() != null)
            {
                if (File.Exists(fileDialog.FileName))                    
                    HexEdit.FileName = fileDialog.FileName;
            }
        }

        private void CloseFileButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.CloseFile();
        }

        private void ReadOnlybutton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.ReadOnlyMode = !HexEdit.ReadOnlyMode;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SelectAll();
        }

        private void UnSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.UnSelectAll();
        }
        
        private void HexDataVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (HexEdit.HexDataVisibility == Visibility.Visible)
                HexEdit.HexDataVisibility = Visibility.Collapsed;
            else
                HexEdit.HexDataVisibility = Visibility.Visible;
        }

        private void HexHeaderVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (HexEdit.HeaderVisibility == Visibility.Visible)
                HexEdit.HeaderVisibility = Visibility.Collapsed;
            else
                HexEdit.HeaderVisibility = Visibility.Visible;
        }

        private void ScrollVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (HexEdit.VerticalScrollBarVisibility == Visibility.Visible)
                HexEdit.VerticalScrollBarVisibility = Visibility.Collapsed;
            else
                HexEdit.VerticalScrollBarVisibility = Visibility.Visible;
        }

        private void StringDataVisibilityButton_Click(object sender, RoutedEventArgs e)
        {

            if (HexEdit.StringDataVisibility == Visibility.Visible)
                HexEdit.StringDataVisibility = Visibility.Collapsed;
            else
                HexEdit.StringDataVisibility = Visibility.Visible;            
        }

        private void StatusBarVisibilityButton_Click(object sender, RoutedEventArgs e)
        {

            if (HexEdit.StatusBarVisibility == Visibility.Visible)
                HexEdit.StatusBarVisibility = Visibility.Collapsed;
            else
                HexEdit.StatusBarVisibility = Visibility.Visible;
        }
        
        private void HexEdit_SelectionLenghtChanged(object sender, EventArgs e)
        {
            SelectionLenghtLabel.Content = HexEdit.SelectionLenght;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (HexEdit.CanCopy())
            {
                HexEdit.CopyToClipboard();

                try
                {
                    if (Clipboard.ContainsText())
                        MessageBox.Show(Clipboard.GetText());
                }
                catch
                {
                    //Clipboard fail
                    if (Clipboard.ContainsText())
                        MessageBox.Show(Clipboard.GetText());
                }
            }
            else
                MessageBox.Show("Can't copy right now !");
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.Undo();
        }

        private void CopyToStreamButton_Click(object sender, RoutedEventArgs e)
        {
            MemoryStream ms = new MemoryStream();

            HexEdit.CopyToStream(ms, true);

            MessageBox.Show(ByteConverters.BytesToString(ms.ToArray()));
        }
    }
}
