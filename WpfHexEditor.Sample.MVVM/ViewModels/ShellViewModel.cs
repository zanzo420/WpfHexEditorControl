using Microsoft.Win32;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core;
using WpfHexEditor.Sample.MVVM.Contracts.App;
using WpfHexaEditor.Core.Interfaces;
using WpfHexEditor.Sample.MVVM.Helpers;
using WpfHexEditor.Sample.MVVM.Models;

namespace WpfHexEditor.Sample.MVVM.ViewModels {
    [Export]
    public partial class ShellViewModel : BindableBase {
        public ShellViewModel() {
            InitializeToolTips();


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


        private long _position;
        public long Position {
            get => _position;
            set => SetProperty(ref _position, value);
        }


        public ObservableCollection<(long index, long length, Brush background)> CustomBackgroundBlocks { get; set; } = new ObservableCollection<(long index, long length, Brush background)>();
        


        private DelegateCommand _loadedCommand;
        public DelegateCommand LoadedCommand => _loadedCommand ??
            (_loadedCommand = new DelegateCommand(
                () => {
#if DEBUG
                    Stream = File.OpenRead("E://FeiQ.1060559168.exe");
#endif
                }
            ));



        private DelegateCommand _testCommand;
        public DelegateCommand TestCommand => _testCommand ??
            (_testCommand = new DelegateCommand(
                () => {
#if DEBUG
                   if(Stream == null) {
                        return;
                   }
                    Position = 512;
            //        var customBacks = new ObservableCollection<(long index, long length, Brush background)> {

            //    (0L,4L,Brushes.Yellow),

            //    (4L,4L,Brushes.Red),

            //    (8L,16L,Brushes.Brown)

            //};

                    for (int i = 0; i < 200; i++) {

                        CustomBackgroundBlocks.Add((24 + i, 1, Brushes.Chocolate));

                    }

                    //CustomBackgroundBlocks = customBacks;
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
                    var fileName = DialogService.Current?.OpenFile();
                    if (string.IsNullOrEmpty(fileName)) {
                        return;
                    }

                    OpenFile(fileName);
                }
            ));

        private void OpenFile(string fileName) {
            if (Stream != null) {
                Stream.Close();
                Stream = null;
            }

            if (File.Exists(fileName)) {
                Application.Current.MainWindow.Cursor = Cursors.Wait;

                Stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite);

                Application.Current.MainWindow.Cursor = null;
            }
        }

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


        /// <summary>
        /// DragEventArg is not test-friendly,what we care is Data (IDataObject-typed) prop,
        /// That's why we set the command as IDataObject generic-typed;
        /// </summary>
        private DelegateCommand<IDataObject> _dropCommand;
        public DelegateCommand<IDataObject> DropCommand => _dropCommand ??
            (_dropCommand = new DelegateCommand<IDataObject>(
                dataObject => {
                    if(Stream != null) {
                        Stream.Dispose();
                        Stream = null;
                    }

                    if(dataObject == null) {
                        return;
                    }

                    try {
                        string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
                        if(files.Length == 0) {
                            return;
                        }
                        OpenFile(files[0]);
                    }
                    catch(Exception ex) {

                    }
                    //arg.Data.GetData()
                }
            ));


        

    }

    /// <summary>
    /// ToolTip
    /// </summary>
    public partial class ShellViewModel{
        private void InitializeToolTips() {
            _positionToolTip.KeyName = AppHelper.FindResourceString("OffsetTag");
            _valToolTip.KeyName = AppHelper.FindResourceString("ValueTag");

            DataToolTips.Add(_positionToolTip);
            DataToolTips.Add(_valToolTip);
        }

        private ToolTipItemDataModel _valToolTip = new ToolTipItemDataModel();
        private ToolTipItemDataModel _positionToolTip = new ToolTipItemDataModel();

        private long _hoverPosition;
        public long HoverPosition {
            get => _hoverPosition;
            set {
                SetProperty(ref _hoverPosition, value);
                RaisePropertyChanged(nameof(HoverByte));
                if (Stream?.CanRead ?? false) {
                    if(_hoverPosition >= Stream.Length) {
                        return;
                    }
                    Stream.Position = _hoverPosition;
                    _positionToolTip.Value = value.ToString();
                    _valToolTip.Value = Stream.ReadByte().ToString();
                }
                
            }
        }
        
        public byte HoverByte {
            get {
                if(Stream != null) {
                    Stream.Position = HoverPosition;
                    return (byte)Stream.ReadByte();
                }
                return 0;
            }
        }

        public ObservableCollection<ToolTipItemDataModel> DataToolTips { get; set; } = new ObservableCollection<ToolTipItemDataModel>();
        
        private ToolTipItemDataModel _selectedToolTipItem;
        public ToolTipItemDataModel SelectedToolTipItem {
            get => _selectedToolTipItem;
            set => SetProperty(ref _selectedToolTipItem, value);
        }
        
        private DelegateCommand _copyKeyCommand;
        public DelegateCommand CopyKeyCommand => _copyKeyCommand ??
            (_copyKeyCommand = new DelegateCommand(
                () => {
                    Clipboard.SetText(SelectedToolTipItem.KeyName);
                },
                () => SelectedToolTipItem != null
            )).ObservesProperty(() => SelectedToolTipItem);


        private DelegateCommand _copyValueCommand;
        public DelegateCommand CopyValueCommand => _copyValueCommand ??
            (_copyValueCommand = new DelegateCommand(
                () => {
                    Clipboard.SetText(SelectedToolTipItem.Value);
                },
                () => SelectedToolTipItem != null
            )).ObservesProperty(() => SelectedToolTipItem);
        
        private DelegateCommand _copyExpressionCommand;
        public DelegateCommand CopyExpressionCommand => _copyExpressionCommand ??
            (_copyExpressionCommand = new DelegateCommand(
                () => {
                    Clipboard.SetText($"{SelectedToolTipItem.KeyName}:{SelectedToolTipItem.Value}");
                },
                () => SelectedToolTipItem != null
            )).ObservesProperty(() => SelectedToolTipItem);

        //public ObservableCollection<(long position,long size,ToolTipItemDataModel toolTipItem)>
    }

    
}
