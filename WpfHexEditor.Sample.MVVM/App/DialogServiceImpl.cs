using Microsoft.Win32;
using System.ComponentModel.Composition;
using WpfHexEditor.Sample.MVVM.Contracts.App;

namespace WpfHexEditor.Sample.MVVM.App {
    [Export(typeof(IDialogService))]
    public class DialogServiceImpl : IDialogService {
        public string OpenFile() {
            var fileDialog = new OpenFileDialog();
            if(fileDialog.ShowDialog() == true) {
                return fileDialog.FileName;
            }

            return null;
        }
    }
}
