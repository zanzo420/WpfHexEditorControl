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
using WPFHexaEditor.Core;
using WPFHexaEditor.Core.Bytes;
using WPFHexaEditorExample.Properties;

namespace WPFHexaEditorExample
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum SettingEnum
        {
            HeaderVisibility,
            ReadOnly
        }

        public MainWindow()
        {
            InitializeComponent();

            UpdateAllSettings();
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() != null)
            {
                if (File.Exists(fileDialog.FileName))
                    HexEdit.FileName = fileDialog.FileName;
                else
                    MessageBox.Show("File not found!", Settings.Default.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
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
            UpdateSetting(SettingEnum.ReadOnly);
        }

        private void ShowHeaderMenu_Click(object sender, RoutedEventArgs e)
        {
            UpdateSetting(SettingEnum.HeaderVisibility);
        }

        private void UpdateSetting(SettingEnum setting)
        {
            switch (setting)
            {
                case SettingEnum.HeaderVisibility:
                    if (!Settings.Default.HeaderVisibility)
                        HexEdit.HeaderVisibility = Visibility.Collapsed;
                    else
                        HexEdit.HeaderVisibility = Visibility.Visible;

                    Settings.Default.HeaderVisibility = HexEdit.HeaderVisibility == Visibility.Visible;
                    break;
                case SettingEnum.ReadOnly:
                    HexEdit.ReadOnlyMode = Settings.Default.ReadOnly; 
                    
                    HexEdit.ClearAllChange();
                    HexEdit.RefreshView();
                    break;
            }
        }

        private void UpdateAllSettings()
        {
            UpdateSetting(SettingEnum.HeaderVisibility);
            UpdateSetting(SettingEnum.ReadOnly);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Settings.Default.Save();
        
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CopyHexaMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.CopyToClipboard(CopyPasteMode.HexaString);
        }

        private void CopyStringMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.CopyToClipboard();
        }

        private void DeleteSelectionMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.DeleteSelection();
        }
    }
}
