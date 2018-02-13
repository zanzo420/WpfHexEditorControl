using Microsoft.Win32;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexEditor.Sample.MVVM.ViewModels {
    public class ShellViewModel : BindableBase {
        public ShellViewModel() {
#if DEBUG
            var customBacks = new List<(long index, long length, Brush background)> {

                (0L,4L,Brushes.Yellow),

                (4L,4L,Brushes.Red),

                (8L,16L,Brushes.Brown)

            };

            for (int i = 0; i < 200; i++) {

                customBacks.Add((24 + i, 1, Brushes.Chocolate));

            }

            CustomBackgroundBlocks = customBacks;
#endif
        }


        private long _selectionStart;
        public long SelectionStart {
            get => _selectionStart;
            set => SetProperty(ref _selectionStart, value);
        }

        
        private long _focusPosition;
        public long FocusPosition {
            get => _focusPosition;
            set => SetProperty(ref _focusPosition, value);
        }


        private long _selectionLength;
        public long SelectionLength {
            get => _selectionLength;
            set => SetProperty(ref _selectionLength, value);
        }
        
        private IEnumerable<(long index, long length, Brush background)> _customBackgroundBlocks;
        public IEnumerable<(long index, long length, Brush background)> CustomBackgroundBlocks {
            get => _customBackgroundBlocks;
            set => SetProperty(ref _customBackgroundBlocks, value);
        }
        


        private DelegateCommand _loadedCommand;
        public DelegateCommand LoadedCommand => _loadedCommand ??
            (_loadedCommand = new DelegateCommand(
                () => {
#if DEBUG
                    //Stream = File.OpenRead("D://s.exe");
#endif
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


        //Set the focused position char as Selection Start
        private DelegateCommand _setAsStartCommand;
        public DelegateCommand SetAsStartCommand => _setAsStartCommand ??
            (_setAsStartCommand = new DelegateCommand(
                () => {
                    if(Stream == null) {
                        return;
                    }

                    //Check if FocusPosition is valid;
                    if(FocusPosition == -1) {
                        return;
                    }
                    if(FocusPosition >= (Stream?.Length ?? 0)) {
                        return;
                    }
                    
                    if (SelectionStart != -1 && SelectionLength != 0
                    && SelectionStart + SelectionLength - 1 > FocusPosition) {
                        SelectionLength = SelectionStart + SelectionLength - FocusPosition;
                    }
                    else {
                        SelectionLength = 1;
                    }

                    SelectionStart = FocusPosition;

                }
            ));


        //Set the focused position char as Selection End
        private DelegateCommand _setAsEndCommand;
        public DelegateCommand SetAsEndCommand => _setAsEndCommand ??
            (_setAsEndCommand = new DelegateCommand(
                () => {
                    if (Stream == null) {
                        return;
                    }

                    //Check if FocusPosition is valid;
                    if (FocusPosition == -1) {
                        return;
                    }
                    if (FocusPosition >= (Stream?.Length ?? 0)) {
                        return;
                    }

                    
                    if (SelectionStart != -1 && SelectionLength != 0
                    && SelectionStart < FocusPosition) {
                        SelectionLength = FocusPosition - SelectionStart + 1;
                    }
                    else {
                        SelectionStart = FocusPosition;
                        SelectionLength = 1;
                    }
                }
            ));

    }

    
}
