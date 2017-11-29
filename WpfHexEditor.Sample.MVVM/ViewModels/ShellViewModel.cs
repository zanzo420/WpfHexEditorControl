using Microsoft.Win32;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

                            FileName = fileDialog.FileName;

                            Application.Current.MainWindow.Cursor = null;
                        }
                    }
                }
            ));

        private string _fileName;
        public string FileName {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private DelegateCommand _submitChangesCommand;
        public DelegateCommand SubmitChangesCommand => _submitChangesCommand ??
            (_submitChangesCommand = new DelegateCommand(
                () => {
                    SubmitChangesRequest.Raise(new Notification());
                }
            ));
        public InteractionRequest<Notification> SubmitChangesRequest { get; } = new InteractionRequest<Notification>();

        private DelegateCommand _closeCommand;
        public DelegateCommand CloseCommand => _closeCommand ??
            (_closeCommand = new DelegateCommand(
                () => {
                    CloseRequest.Raise(null);
                }
            ));
        public InteractionRequest<Notification> CloseRequest { get; } = new InteractionRequest<Notification>();
    }
}
