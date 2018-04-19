using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfHexEditor.Sample.MVVM.Contracts.Common;

namespace WpfHexEditor.Sample.MVVM.Contracts.App {
    public interface ILoggerService {
        void WriteLine(string msg);
        void WriteCallerLine(string msg, [CallerMemberName] string callerName = null);
    }
    public class LoggerService:GenericServiceStaticInstance<ILoggerService> {

    }
}
