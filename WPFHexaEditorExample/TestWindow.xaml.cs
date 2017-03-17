//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using WPFHexaEditor.Core.Bytes;

namespace WPFHexaEditorExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
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
            long position = 0;
            if (long.TryParse(PositionText.Text, out position))
                HexEdit.SetPosition(position, 1);
            else
                MessageBox.Show("Enter long value.");
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
            HexEdit.Close();
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

            //ms.Position = 0;
            //File.WriteAllBytes(@"c:\test\test.exe", ms.ToArray());

            MessageBox.Show(ByteConverters.BytesToString(ms.ToArray()));
        }

        private void SelectionByteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"SelectionByteArray ToString \n\n {ByteConverters.BytesToString(HexEdit.SelectionByteArray)}");
            MessageBox.Show($"SelectionString \n\n {HexEdit.SelectionString}");
            MessageBox.Show($"SelectionHexa \n\n {HexEdit.SelectionHexa}");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SubmitChanges();
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Findtextbox.Text != "")
                HexEdit.FindFirst(Findtextbox.Text);
            else
                MessageBox.Show("Enter string to find");
        }

        private void Findtextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                findHexLabel.Content = ByteConverters.StringToHex(Findtextbox.Text);
            }
            catch { }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (Findtextbox.Text != "")
                HexEdit.FindNext(Findtextbox.Text);
            else
                MessageBox.Show("Enter string to find");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (Findtextbox.Text != "")
                HexEdit.FindLast(Findtextbox.Text);
            else
                MessageBox.Show("Enter string to find");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.FindAllSelection(true);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            HexEdit.SetBookMark();
        }
    }
}