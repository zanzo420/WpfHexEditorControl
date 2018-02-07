using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TestDemoUI {
    public partial class TestDataLayer {
        public TestDataLayer() {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
            var blocks = new List<(int index, int length, Brush background)>();
            blocks.Add((0, 256, Brushes.Red));
            using (var fs = File.OpenRead("E://InstallShield2018ExpressComp.exe")) {
                var buffer = new byte[4096];
                fs.Read(buffer, 0, buffer.Length);
                hd.BackgroundBlocks = blocks;
                hd.Data = buffer;
                sd.Data = buffer;
            }
        }
    }
}
