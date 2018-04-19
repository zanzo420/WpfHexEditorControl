using System.Runtime.CompilerServices;
using WpfHexEditor.Sample.MVVM.Contracts.Common;

namespace WpfHexEditor.Sample.MVVM.Contracts.App {
    public interface ILoggerService {
        void WriteLine(string msg);
        void WriteCallerLine(string msg, [CallerMemberName] string callerName = null);
    }
    public class LoggerService:GenericServiceStaticInstance<ILoggerService> {

    }
}
