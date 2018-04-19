using System.ComponentModel.Composition;
using WpfHexEditor.Sample.MVVM.Contracts.ToolTip;

namespace WpfHexEditor.Sample.MVVM.ToolTip {
    [Export(typeof(IToolTipItemFactory))]
    public class ToolTipItemFactoryImpl : IToolTipItemFactory {
        public IToolTipDataItem CreateToolTipDataItem() => new ToolTipDataItem();

        public IToolTipObjectItem CreateToolTipObjectItem() => new ToolTipObjectItem();
    }
}
