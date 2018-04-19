using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHexEditor.Sample.MVVM.Contracts.ToolTip;

namespace WpfHexEditor.Sample.MVVM.ToolTip {
    [Export(typeof(IToolTipItemFactory))]
    public class ToolTipItemFactoryImpl : IToolTipItemFactory {
        public IToolTipDataItem CreateToolTipDataItem() => new ToolTipDataItem();

        public IToolTipObjectItem CreateToolTipObjectItem() => new ToolTipObjectItem();
    }
}
