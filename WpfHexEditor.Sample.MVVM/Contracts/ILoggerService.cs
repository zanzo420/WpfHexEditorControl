using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexEditor.Sample.MVVM.Contracts {
    public interface ILoggerService {
        void WriteLine(string msg);
        void WriteCallerLine(string msg, [CallerMemberName] string callerName = null);
    }
    public class LoggerService:GenericServiceStaticInstance<ILoggerService> {

    }
}
