using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexaEditor.Core {
    public abstract class GenericStaticInstance<T> where T:class,new() {
        private static T _staticInstance;
        public static T StaticInstance => _staticInstance ?? (_staticInstance = new T());
    }
}
