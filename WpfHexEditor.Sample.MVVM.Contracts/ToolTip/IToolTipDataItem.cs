using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHexEditor.Sample.MVVM.Contracts.Common;

namespace WpfHexEditor.Sample.MVVM.Contracts.ToolTip { 
    /// <summary>
    /// This tooltip item will show a Key (On the left) and a Value(On the right);
    /// </summary>
    public interface IToolTipDataItem : IToolTipItem {
        string KeyName { get; set; }
        string Value { get; set; }
    }
}
