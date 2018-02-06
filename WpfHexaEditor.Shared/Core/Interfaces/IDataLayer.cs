using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfHexaEditor.Core.Interfaces {
    public interface IDataLayer {
        byte[] Data { get; set; }
        IEnumerable<(int index, int length ,Brush background)> BackgroundBlocks { get; set; }
        IEnumerable<(int index, int length ,Brush foreground)> ForegroundBlocks { get; set; }

        Brush DefaultForeground { get; }
        double FontSize { get; set; }

        int BytePerLine { get; set; }
        int AvailableRowsCount { get; }
        
        Size CellSize { get; }
    }
}
