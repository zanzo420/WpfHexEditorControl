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
                hexEditor1.FileName = fileDialog.FileName;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK && File.Exists(fileDialog.FileName))
            {
                hexEditor1.LoadTblFile(fileDialog.FileName);
                hexEditor1.TypeOfCharacterTable = CharacterTableType.TblFile;
            }
        }
    }
}
