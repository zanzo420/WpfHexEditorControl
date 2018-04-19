using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestDemoUI {
    /// <summary>
    /// Interaction logic for TestBlocks.xaml
    /// </summary>
    public partial class TestForegrounds : UserControl {
        public TestForegrounds() {
            InitializeComponent();
            editor.Stream = File.OpenRead("E://anli/Fat32.img");
            editor.CustomBackgroundBlocks = new List<(long index, long length, Brush background)> {
                (446,16,Brushes.Red),
                (446 + 16,18,Brushes.Coral)
            };
        }
    }
}
