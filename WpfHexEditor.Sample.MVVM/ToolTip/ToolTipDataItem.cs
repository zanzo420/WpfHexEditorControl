using WpfHexEditor.Sample.MVVM.Contracts.ToolTip;
using WpfHexEditor.Sample.MVVM.ToolTip.Models;

namespace WpfHexEditor.Sample.MVVM.ToolTip {
    class ToolTipDataItem : IToolTipDataItem {
        public ToolTipDataItem() {
            
        }
        private ToolTipDataItemViewModel _toolTipDataItemViewModel = new ToolTipDataItemViewModel();

        public string KeyName {
            get => _toolTipDataItemViewModel.KeyName;
            set => _toolTipDataItemViewModel.KeyName = value;
        }

        public string Value {
            get => _toolTipDataItemViewModel.Value;
            set => _toolTipDataItemViewModel.Value = value;
        }

        private Views.ToolTipDataItem _uiObject;
        public object UIObject {
            get {
                if(_uiObject == null) {
                    _uiObject = new Views.ToolTipDataItem {
                        DataContext = _toolTipDataItemViewModel
                    };
                }
                return _uiObject;
            }
        }
    }
}
