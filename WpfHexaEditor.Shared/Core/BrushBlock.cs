using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace WpfHexaEditor.Core.Interfaces
{
    public class BrushBlock
    {
        public long StartOffset { get; set; }
        public long Length { get; set; }
        public Brush Brush { get; set; }
    }
}
