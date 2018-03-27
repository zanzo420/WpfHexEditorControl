using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexEditor.Sample.MVVM.Contracts {
    public interface IDialogService {
        string OpenFile();
    }

    public class DialogService:GenericServiceStaticInstance<IDialogService> {
    }

}
