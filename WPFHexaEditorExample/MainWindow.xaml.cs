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
        private object fileName;

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
            HexEdit.SetPosition("0x20FF");
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
            HexEdit.ReadOnlyMode = true;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SelectAll();
        }

        private void UnSelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.UnSelectAll();
        }
    }
}
