using System;
using System.Collections.Generic;
using System.IO;
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
