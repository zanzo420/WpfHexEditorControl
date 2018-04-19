using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TestDemoUI {
    /// <summary>
    /// Interaction logic for TestPopup.xaml
    /// </summary>
    public partial class TestPopup : UserControl
    {
        public TestPopup()
        {
            InitializeComponent();
            hd.Data = new byte[] { 0x12, 0x12 };
            
            
            hd.MouseMoveOnCell += Hd_MouseMoveOnCell;
            hd.Background = Brushes.LightBlue;
            var blocks  = new List<(int index, int length, Brush background)>();
            blocks.Add((0, 2, Brushes.Orange));

            hd.BackgroundBlocks = blocks;
        }

        

        
        private void Hd_MouseMoveOnCell(object sender, (int cellIndex, MouseEventArgs e) arg) {
            var index = arg.cellIndex;
            var popPoint = hd.GetCellLocation(index);
            if(popPoint == null) {
                return;
            }
            
            //popusBottom.VerticalOffset = popPoint.Value.Y ;
            //popusBottom.HorizontalOffset = popPoint.Value.X;
            //popusBottom.IsOpen = true;
        }
        
        
    }
}
