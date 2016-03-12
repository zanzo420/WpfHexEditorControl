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
            HexEdit.HexDataVisibility = !HexEdit.HexDataVisibility;
        }

        private void HexHeaderVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.HeaderVisibility = !HexEdit.HeaderVisibility;
        }

        private void ScrollVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.VerticalScrollBarVisibility = !HexEdit.VerticalScrollBarVisibility;
        }

        private void StringDataVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.StringDataVisibility = !HexEdit.StringDataVisibility;
        }

        private void StatusBarVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.StatusBarVisibility = !HexEdit.StatusBarVisibility;
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

                if (Clipboard.ContainsText())
                    MessageBox.Show(Clipboard.GetText());
            }
            else
                MessageBox.Show("Can't copy right now !");
        }
    }
}
