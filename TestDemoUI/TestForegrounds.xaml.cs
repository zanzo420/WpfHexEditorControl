using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using WpfHexaEditor.Core;

namespace TestDemoUI {
    /// <summary>
    /// Interaction logic for TestBlocks.xaml
    /// </summary>
    public partial class TestForegrounds : UserControl {
        public TestForegrounds() {
            InitializeComponent();
            editor.Stream = File.OpenRead("E://anli/Fat32.img");
            editor.CustomBackgroundBlocks = new List<BrushBlock> {
                new BrushBlock{StartOffset = 446, Length = 16, Brush = Brushes.Red }
            };
        }
    }
}
