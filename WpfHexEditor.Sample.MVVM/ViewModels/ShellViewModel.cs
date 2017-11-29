using Microsoft.Win32;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexEditor.Sample.MVVM.ViewModels {
    public class ShellViewModel : BindableBase {
        private DelegateCommand _openFileCommand;
        public DelegateCommand OpenFileCommand => _openFileCommand ??
            (_openFileCommand = new DelegateCommand(
                () => {
                    var fileDialog = new OpenFileDialog();

                    if (fileDialog.ShowDialog() != null) {
                        if (File.Exists(fileDialog.FileName)) {
                            Application.Current.MainWindow.Cursor = Cursors.Wait;

                            FileEditor.FileName = fileDialog.FileName;

                            Application.Current.MainWindow.Cursor = null;
                        }
                    }
                },
                () => FileEditor != null
            ));
        
        private DelegateCommand _submitChangesCommand;
        public DelegateCommand SubmitChangesCommand => _submitChangesCommand ??
            (_submitChangesCommand = new DelegateCommand(
                () => {
                    FileEditor?.SubmitChanges();
                }
            ));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand => _saveAsCommand ??
            (_saveAsCommand = new DelegateCommand(
                () => {
                    var fileDialog = new SaveFileDialog();

                    if (fileDialog.ShowDialog() != null) {
                        FileEditor.SubmitChanges(fileDialog.FileName, true);
                    }
                }
            ));

        private DelegateCommand _closeCommand;
        public DelegateCommand CloseCommand => _closeCommand ??
            (_closeCommand = new DelegateCommand(
                () => {
                    FileEditor?.CloseProvider();
                }
            ));

        private DelegateCommand _exitCommand;
        public DelegateCommand ExitCommand => _exitCommand ??
            (_exitCommand = new DelegateCommand(
                () => {
                    ExitRequest.Raise(new Notification());
                }
            ));
        public InteractionRequest<Notification> ExitRequest { get; } = new InteractionRequest<Notification>();



        public IFileEditable FileEditor { get; set; }
    }

    
}
