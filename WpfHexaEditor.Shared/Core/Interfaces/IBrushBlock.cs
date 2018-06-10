using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace WpfHexaEditor.Core.Interfaces
{
    public interface IBrushBlock
    {
        long StartOffset { get; set; }
        long Length { get; set; }
        Brush Brush { get; set; }
    }
}
