using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WpfHexEditor.Sample.MVVM.Contracts.Common;
using WpfHexEditor.Sample.MVVM.Contracts.ToolTip;

namespace WpfHexEditor.Sample.MVVM.ToolTip {
    public class ToolTipObjectItem : IToolTipObjectItem {
        private ContentControl _contentControl = new ContentControl();
        public object UIObject {
            get => _contentControl.Content;
            set => _contentControl.Content = value;
        }
    }
}
