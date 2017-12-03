using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHexaEditor.Core.Interfaces {
    public interface IFileEditable {
        string FileName { get; set; }
        void CloseProvider();
        void SubmitChanges(string newfilename, bool overwrite = false);
        void SubmitChanges();

        
    }
}
