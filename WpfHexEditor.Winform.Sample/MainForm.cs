using System;
using System.IO;
using System.Windows.Forms;
using WpfHexaEditor.Core;

namespace WpfHexEditor.Winform.Sample
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK && File.Exists(fileDialog.FileName))
                hexEditor.FileName = fileDialog.FileName;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK && File.Exists(fileDialog.FileName))
            {
                hexEditor.LoadTblFile(fileDialog.FileName);
                hexEditor.TypeOfCharacterTable = CharacterTableType.TblFile;
            }
        }
    }
}
