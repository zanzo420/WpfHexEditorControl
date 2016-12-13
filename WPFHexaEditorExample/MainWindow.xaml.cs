using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFHexaEditorExample
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() != null)
            {
                if (File.Exists(fileDialog.FileName))
                    HexEdit.FileName = fileDialog.FileName;
                else
                    MessageBox.Show("File not found!", Properties.Settings.Default.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SubmitChanges();
        }

        private void CloseFileMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.CloseFile();
        }

        private void SetReadOnlyMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.ReadOnlyMode = !HexEdit.ReadOnlyMode;            
            SetReadOnlyMenu.IsChecked = HexEdit.ReadOnlyMode;

            HexEdit.ClearAllChange();
            HexEdit.RefreshView();
        }
    }
}
