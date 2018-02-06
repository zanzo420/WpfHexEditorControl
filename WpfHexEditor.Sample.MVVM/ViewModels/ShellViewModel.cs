using Microsoft.Win32;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System.IO;
using System.Windows;
using System.Windows.Input;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexEditor.Sample.MVVM.ViewModels {
    public class ShellViewModel : BindableBase {
        public ShellViewModel() {
            
        }


        private DelegateCommand _loadedCommand;
        public DelegateCommand LoadedCommand => _loadedCommand ??
            (_loadedCommand = new DelegateCommand(
                () => {
                    //Stream = File.OpenRead("D://s.exe");
                }
            ));


        private Stream _stream;
        public Stream Stream {
            get => _stream;
            set => SetProperty(ref _stream, value);
        }


        #region File_Menu
        private DelegateCommand _openFileCommand;
        public DelegateCommand OpenFileCommand => _openFileCommand ??
            (_openFileCommand = new DelegateCommand(
                () => {
                    
                    var fileDialog = new OpenFileDialog();

                    if (fileDialog.ShowDialog() != null) {

                        if (Stream != null) {
                            Stream.Close();
                            Stream = null;
                        }

                        if (File.Exists(fileDialog.FileName)) {
                            Application.Current.MainWindow.Cursor = Cursors.Wait;

                            Stream = File.Open(fileDialog.FileName, FileMode.Open, FileAccess.ReadWrite);

                            Application.Current.MainWindow.Cursor = null;
                        }
                    }
                }
            ));

        private DelegateCommand _submitChangesCommand;
        public DelegateCommand SubmitChangesCommand => _submitChangesCommand ??
            (_submitChangesCommand = new DelegateCommand(
                () => {
                    //FileEditor?.SubmitChanges();
                }
            ));

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand => _saveAsCommand ??
            (_saveAsCommand = new DelegateCommand(
                () => {
                    var fileDialog = new SaveFileDialog();

                    if (fileDialog.ShowDialog() != null) {
                        //FileEditor.SubmitChanges(fileDialog.FileName, true);
                    }
                }
            ));

        private DelegateCommand _closeCommand;
        public DelegateCommand CloseCommand => _closeCommand ??
            (_closeCommand = new DelegateCommand(
                () => {
                    //FileEditor?.CloseProvider();
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

        #endregion

        #region Edit_Menu

        private DelegateCommand _setReadOnlyCommand;
        public DelegateCommand SetReadOnlyCommand =>
            _setReadOnlyCommand ?? (_setReadOnlyCommand = new DelegateCommand(
                () => {

                }
            ));

        private DelegateCommand _undoCommand;
        public DelegateCommand UndoCommand => _undoCommand ?? (_undoCommand = new DelegateCommand(
            () => {
                //FileEditor?.Undo();
            }
        ));

        private DelegateCommand<CopyPasteMode?> _copyToClipBoardCommand;
        public DelegateCommand<CopyPasteMode?> CopyToClipBoardCommand => 
            _copyToClipBoardCommand ?? 
            (_copyToClipBoardCommand = new DelegateCommand<CopyPasteMode?>(
                mode => {
                    if(mode != null) {
                        //FileEditor?.CopyToClipboard(mode.Value);
                    }
                }
            ));


        #endregion
        

    }

    
}
