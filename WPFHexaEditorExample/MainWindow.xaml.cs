//////////////////////////////////////////////
// Apache 2.0  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
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
            ReadOnly,
            ScrollVisibility
        }

        public MainWindow()
        {
            InitializeComponent();

            UpdateAllSettings();
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() != null)
            {
                if (File.Exists(fileDialog.FileName))
                {
                    Application.Current.MainWindow.Cursor = Cursors.Wait;

                    HexEdit.FileName = fileDialog.FileName;

                    Application.Current.MainWindow.Cursor = null;
                }
            }
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;
            ///HexEdit.SaveTBLFile();
            HexEdit.SubmitChanges();
            Application.Current.MainWindow.Cursor = null;
        }

        private void CloseFileMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.CloseProvider();
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
            UpdateSetting(SettingEnum.ScrollVisibility);
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

        private void ShowScrollMenu_Click(object sender, RoutedEventArgs e)
        {
            UpdateSetting(SettingEnum.ScrollVisibility);
        }

        private void SetPositionMenu_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GOPosition_Click(object sender, RoutedEventArgs e)
        {
            if (long.TryParse(PositionText.Text, out var position))
                HexEdit.SetPosition(position, 1);
            else
                MessageBox.Show("Enter long value.");

            ViewMenu.IsSubmenuOpen = false;
        }

        private void GOHexPosition_Click(object sender, RoutedEventArgs e)
        {
            var (success, position) = ByteConverters.HexLiteralToLong(PositionHexText.Text);

            if (success && position > 0)
                HexEdit.SetPosition(position, 1);
            else
                MessageBox.Show("Enter hexa value.");
            
            ViewMenu.IsSubmenuOpen = false;
        } 

        private void PositionHexText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ByteConverters.IsHexaValue(PositionHexText.Text).success)
                GoPositionHexaButton.IsEnabled = true;
            else
                GoPositionHexaButton.IsEnabled = false;
        }

        private void PositionText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (long.TryParse(PositionText.Text, out var position))
                GoPositionButton.IsEnabled = true;
            else
                GoPositionButton.IsEnabled = false;
        }

        private void UndoMenu_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.Undo();
        }

        private void SetBookMarkButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SetBookMark();
        }

        private void DeleteBookmark_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.ClearScrollMarker(ScrollMarker.Bookmark);
        }

        private void FindText_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void FindFirstButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void FindPreviousButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void FindAllSelection_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.FindAllSelection(true);
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SelectAll();
        }

        private void FindNextButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CTableASCIIButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.TypeOfCharacterTable = CharacterTableType.Ascii;
            CTableAsciiButton.IsChecked = true;
            CTableTblButton.IsChecked = false;
            CTableTblDefaultAsciiButton.IsChecked = false;
        }

        private void CTableTBLButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() != null)
            {
                if (File.Exists(fileDialog.FileName))
                {
                    Application.Current.MainWindow.Cursor = Cursors.Wait;

                    HexEdit.LoadTblFile(fileDialog.FileName);
                    HexEdit.TypeOfCharacterTable = CharacterTableType.TblFile;
                    CTableAsciiButton.IsChecked = false;
                    CTableTblButton.IsChecked = true;
                    CTableTblDefaultAsciiButton.IsChecked = false;

                    Application.Current.MainWindow.Cursor = null;
                }
            }
        }

        private void CTableTBLDefaultASCIIButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Cursor = Cursors.Wait;

            HexEdit.TypeOfCharacterTable = CharacterTableType.TblFile;
            HexEdit.LoadDefaultTbl();
            CTableAsciiButton.IsChecked = false;
            CTableTblButton.IsChecked = false;
            CTableTblDefaultAsciiButton.IsChecked = true;

            Application.Current.MainWindow.Cursor = null;
        }

        private void SaveAsMenu_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog();

            if (fileDialog.ShowDialog() != null)
            {
                HexEdit.SubmitChanges(fileDialog.FileName, true);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (HexEdit.DataStringVisual == DataVisualType.Decimal)
            //    HexEdit.DataStringVisual = DataVisualType.Hexadecimal;
            //else
            //    HexEdit.DataStringVisual = DataVisualType.Decimal;

            HexEdit.SetBytePerLineToFit();
        }

        private void TextBlock_GotFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}