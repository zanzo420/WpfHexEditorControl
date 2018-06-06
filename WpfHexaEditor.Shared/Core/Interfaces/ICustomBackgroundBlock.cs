using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace WpfHexaEditor.Core.Interfaces
{
    public interface ICustomBackgroundBlock
    {
        long StartOffset { get; set; }
        long Length { get; set; }
        Brush Background { get; set; }
    }
}
