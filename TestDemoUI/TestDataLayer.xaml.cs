using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core;
using static WpfHexaEditor.ToolTipExtension;

namespace TestDemoUI {
    public partial class TestDataLayer
    {
        public TestDataLayer() {
            InitializeComponent();
            hd.MouseMoveOnCell += Hd_MouseMoveOnCell;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
            var blocks = new List<BrushBlock>();
            blocks.Add(new BrushBlock { StartOffset = 0, Length = 256, Brush = Brushes.Red });
            using (var fs = File.OpenRead("E://InstallShield2018ExpressComp.exe")) {
                var buffer = new byte[4096];
                fs.Read(buffer, 0, buffer.Length);
                hd.BackgroundBlocks = blocks;
                hd.Data = buffer;
                sd.Data = buffer;
            }
        }

        
        private void Hd_MouseMoveOnCell(object sender, (int cellIndex, MouseEventArgs e) arg) {
            var index = arg.cellIndex;
            var popPoint = hd.GetCellLocation(index);
            if (popPoint == null) {
                return;
            }

            
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                return;
            }

            var pointValue = popPoint.Value;
            hd.SetToolTipOpen(true, new Point {
                X = pointValue.X + hd.CellMargin.Left + hd.CharSize.Width + hd.CellPadding.Left,
                Y = pointValue.Y + hd.CharSize.Height + hd.CellPadding.Top + hd.CellMargin.Top
            });
        }
    }
}
