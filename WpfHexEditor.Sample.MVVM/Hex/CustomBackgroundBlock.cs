using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfHexaEditor.Core.Interfaces;
using WpfHexEditor.Sample.MVVM.Contracts.Hex;

namespace WpfHexEditor.Sample.MVVM.Hex {
    class CustomBackgroundBlock : BrushBlock {
        public long StartOffset { get ; set ; }
        public long Length { get ; set ; }
        public Brush Background { get; set; }
    }

    


}
