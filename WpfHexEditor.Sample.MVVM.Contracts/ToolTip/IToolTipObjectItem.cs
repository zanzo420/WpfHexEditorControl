using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHexEditor.Sample.MVVM.Contracts.Common;

namespace WpfHexEditor.Sample.MVVM.Contracts { 
  
    public interface IToolTipObjectItem : IUIObjectProvider {
        new object UIObject { get; set; }
    }
    
}
