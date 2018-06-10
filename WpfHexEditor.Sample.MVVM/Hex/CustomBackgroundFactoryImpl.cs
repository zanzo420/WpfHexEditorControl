using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHexaEditor.Core.Interfaces;
using WpfHexEditor.Sample.MVVM.Contracts.Hex;

namespace WpfHexEditor.Sample.MVVM.Hex {
    [Export(typeof(ICustomBackgroundFactory))]
    class CustomBackgroundFactoryImpl : ICustomBackgroundFactory {
        public BrushBlock CreateNew() => new CustomBackgroundBlock();
    }

}
