using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHexEditor.Sample.MVVM.Contracts;

namespace WpfHexEditor.Sample.MVVM.Common {
    [Export(typeof(IDialogService))]
    public class DialogService : IDialogService {
        public string OpenFile() {
            var fileDialog = new OpenFileDialog();
            if(fileDialog.ShowDialog() == true) {
                return fileDialog.FileName;
            }

            return null;
        }
    }
}
