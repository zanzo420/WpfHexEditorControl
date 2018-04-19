using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexEditor.Sample.MVVM.Contracts.ToolTip { 
  
    public interface IToolTipObjectItem : IToolTipItem {
        new object UIObject { get; set; }
    }
    
}
