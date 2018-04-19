using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexEditor.Sample.MVVM.Contracts.Common {
    public interface IUIObjectProvider {
        object UIObject { get; }
    }
}
