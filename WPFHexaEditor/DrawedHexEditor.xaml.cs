using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHexaEditor.Core.Bytes;

namespace WpfHexaEditor {
    /// <summary>
    /// Interaction logic for DrawedHexEditor.xaml
    /// </summary>
    public partial class DrawedHexEditor : UserControl {
        #region DevBranch
        public DrawedHexEditor() {
            InitializeComponent();

            HexDataLayer.FontSize = 8;
            StringDataLayer.FontSize = 8;
            
        }
        
        /// <summary>
        /// Save the view byte buffer as a field. 
        /// To save the time when Scolling i do not building them every time when scolling.
        /// </summary>
        private byte[] _viewBuffer;
        private byte[] _viewBuffer2;
        //To avoid resigning buffer everytime and to notify the UI to rerender,
        //we're gonna switch from one to another while refreshing.
        private byte[] _realViewBuffer;
        /// <summary>
        /// Set the Stream are used by ByteProvider
        /// </summary>
        public Stream Stream {
            get => (Stream)GetValue(StreamProperty);
            set => SetValue(StreamProperty, value);
        }

        // Using a DependencyProperty as the backing store for Stream.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StreamProperty =
            DependencyProperty.Register(nameof(Stream), typeof(Stream), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,
                    Stream_PropertyChanged));

        private static void Stream_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is DrawedHexEditor ctrl)) return;
            if (e.NewValue is Stream stream) {
                ctrl.OpenStream(stream);
            }

        }

        
        /// <summary>
        /// Open file name
        /// </summary>
        private void OpenStream(Stream stream) {
            if (!stream.CanRead) return;
            Position = 0;

            //    _provider.ReadOnlyChanged += Provider_ReadOnlyChanged;
            //    _provider.DataCopiedToClipboard += Provider_DataCopied;
            //    _provider.ChangesSubmited += ProviderStream_ChangesSubmited;
            //    _provider.Undone += Provider_Undone;
            //    _provider.LongProcessChanged += Provider_LongProcessProgressChanged;
            //    _provider.LongProcessStarted += Provider_LongProcessProgressStarted;
            //    _provider.LongProcessCompleted += Provider_LongProcessProgressCompleted;
            //    _provider.LongProcessCanceled += Provider_LongProcessProgressCompleted;
            //    _provider.FillWithByteCompleted += Provider_FillWithByteCompleted;
            //    _provider.ReplaceByteCompleted += Provider_ReplaceByteCompleted;
            //    _provider.BytesAppendCompleted += Provider_BytesAppendCompleted;

            UpdateScrollBar();
            UpdateHeader();
            
            RefreshView();

            //UnSelectAll();

            //UpdateTblBookMark();
            //UpdateSelectionColor(FirstColor.HexByteData);

            ////Update count of byte
            //UpdateByteCount();

            ////Debug
            //Debug.Print("STREAM OPENED");
        }

        public long Position {
            get { return (long)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(-1L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    PositionProperty_Changed));

        private static void PositionProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(!(d is DrawedHexEditor ctrl)) {
                return;
            }

            var newPosition = (long)e.NewValue;

            if (!ctrl.scrollBarValueUpdating) {
                ctrl.scrollBarValueUpdating = true;
                ctrl.VerticalScrollBar.Value = ctrl.Position / ctrl.BytePerLine;
                ctrl.scrollBarValueUpdating = false;
            }
#if DEBUG
            ctrl.watch.Restart();
#endif
            ctrl.RefreshView();
            
#if DEBUG
            ctrl.watch.Stop();
            Debug.Print($"REFRESH TIME: {ctrl.watch.ElapsedMilliseconds} ms");
#endif
        }

        //To avoid endless looping of ScrollBar_ValueChanged and Position_PropertyChanged.
        private bool scrollBarValueUpdating = false;

        

        /// <summary>
        /// Update vertical scrollbar with file info
        /// </summary>
        private void UpdateScrollBar() {
            VerticalScrollBar.Visibility = Visibility.Collapsed;

            if (Stream != null) {
                VerticalScrollBar.Visibility = Visibility.Visible;
                VerticalScrollBar.SmallChange = 1;
                //VerticalScrollBar.LargeChange = ScrollLargeChange;
                VerticalScrollBar.Maximum = MaxLine - 1;
            }
        }

        

        /// <summary>
        /// Update the position info panel at left of the control
        /// </summary>
        private void UpdateHeader() {
            //Clear before refresh
            HexHeaderStackPanel.Children.Clear();

            if (Stream == null) return;

            //for (var i = HexHeaderStackPanel.Children.Count; i < BytePerLine; i++) {
            //    if (ByteSpacerPositioning == ByteSpacerPosition.Both ||
            //        ByteSpacerPositioning == ByteSpacerPosition.HexBytePanel)
            //        AddByteSpacer(HexHeaderStackPanel, i, true);

            //    var hlHeader = HighLightSelectionStart &&
            //                   GetColumnNumber(SelectionStart) == i &&
            //                   SelectionStart > -1;

            //    //Create control
            //    var headerLabel = new FastTextLine(this) {
            //        Height = LineHeight,
            //        AutoWidth = false,
            //        FontWeight = hlHeader ? FontWeights.Bold : FontWeights.Normal,
            //        Foreground = hlHeader ? ForegroundHighLightOffSetHeaderColor : ForegroundOffSetHeaderColor,
            //        RenderPoint = new Point(2, 0),
            //        ToolTip = $"Column : {i}"
            //    };

            //    #region Set text visual of header

            //    switch (DataStringVisual) {
            //        case DataVisualType.Hexadecimal:
            //            headerLabel.Text = ByteToHex((byte)i);
            //            headerLabel.Width = 20;
            //            break;
            //        case DataVisualType.Decimal:
            //            headerLabel.Text = i.ToString("d3");
            //            headerLabel.Width = 25;
            //            break;
            //    }

            //    #endregion

            //    //Add to stackpanel
            //    HexHeaderStackPanel.Children.Add(headerLabel);
            //}
        }

#if DEBUG
        private Stopwatch watch = new Stopwatch();
#endif

        /// <summary>
        /// Refresh currentview of hexeditor
        /// </summary>
        /// <param name="controlResize"></param>
        /// <param name="refreshData"></param>
        public void RefreshView() {


            //UpdateLinesOffSet();
            
            UpdateViewers();

            //Update visual of byte control
            //UpdateByteModified();
            //UpdateSelection();
            //UpdateHighLight();
            //UpdateStatusBar();
            //UpdateVisual();
            //UpdateFocus();

            //CheckProviderIsOnProgress();

            //if (controlResize) {
            //    UpdateScrollMarkerPosition();
            //    UpdateHeader(true);
            //}

        }

        

        /// <summary>
        /// Update the data and string stackpanels yo current view;
        /// </summary>
        private void UpdateViewers() {
            if (!(Stream?.CanRead??false)) {
                return;
            }

            Stream.Position = Position;
            var bufferlength = (int)Math.Min(HexDataLayer.AvailableRowsCount * BytePerLine,Stream.Length - Stream.Position);
            if (_viewBuffer == null || _viewBuffer.Length != bufferlength) {
                _viewBuffer = new byte[bufferlength];
            }
            if (_viewBuffer2 == null || _viewBuffer2.Length != bufferlength) {
                _viewBuffer2 = new byte[bufferlength];
            }
            _realViewBuffer = _realViewBuffer == _viewBuffer ? _viewBuffer2 : _viewBuffer;
            
            Stream.Read(_realViewBuffer, 0, bufferlength);
         
            HexDataLayer.Data = _realViewBuffer;
            StringDataLayer.Data = _realViewBuffer;

            //#region Control need to resize
            //    if (_viewBuffer != null) {
            //        if (_viewBuffer.Length < bufferlength) {
            //            BuildDataLines(MaxVisibleLine);
            //            _viewBuffer = new byte[bufferlength];
            //        }
            //    }
            //    else {
            //        _viewBuffer = new byte[bufferlength];
            //        BuildDataLines(MaxVisibleLine);
            //    }

            //#endregion


            //if (LinesInfoStackPanel.Children.Count == 0) return;

            //var startPosition = HexLiteralToLong((LinesInfoStackPanel.Children[0] as FastTextLine).Tag.ToString()).position;
            //_provider.Position = startPosition;
            //var readSize = _provider.Read(_viewBuffer, 0, bufferlength);
            //var index = 0;

            //#region HexByte refresh

            //TraverseHexBytes(byteControl => {
            //    byteControl.Action = ByteAction.Nothing;
            //    byteControl.ReadOnlyMode = ReadOnlyMode;

            //    byteControl.InternalChange = true;

            //    if (index < readSize && _priLevel == curLevel) {
            //        byteControl.Byte = _viewBuffer[index];
            //        byteControl.BytePositionInFile = startPosition + index;
            //    }
            //    else
            //        byteControl.Clear();

            //    byteControl.InternalChange = false;
            //    index++;
            //});

            //#endregion

            //index = 0;

            //#region StringByte refresh

            //TraverseStringBytes(sbCtrl => {
            //    sbCtrl.Action = ByteAction.Nothing;
            //    sbCtrl.ReadOnlyMode = ReadOnlyMode;

            //    sbCtrl.InternalChange = true;
            //    sbCtrl.TblCharacterTable = _tblCharacterTable;
            //    sbCtrl.TypeOfCharacterTable = TypeOfCharacterTable;

            //    if (index < readSize) {
            //        sbCtrl.Byte = _viewBuffer[index];
            //        sbCtrl.BytePositionInFile = startPosition + index;
            //        sbCtrl.ByteNext = index < readSize - 1 ? (byte?)_viewBuffer[index + 1] : null;
            //    }
            //    else
            //        sbCtrl.Clear();

            //    sbCtrl.InternalChange = false;
            //    index++;
            //});

            //#endregion

        }

        /// <summary>
        /// Obtain the max line for verticalscrollbar
        /// </summary>
        private long MaxLine => Stream.Length / BytePerLine;

        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (scrollBarValueUpdating) {
                return;
            }

            scrollBarValueUpdating = true;
            //var datas = new List<byte>();
            //for (int i = 0; i < 16; i++) {
            //    for (byte j = 16; j < 32; j++) {
            //        datas.Add(j);
            //    }
            //}
            //HexDataLayer.Data = datas.ToArray();
            //StringDataLayer.Data = datas.ToArray();
            Position = (long)e.NewValue * BytePerLine;
            scrollBarValueUpdating = false;
        }

        private void BottomRectangle_MouseDown(object sender, MouseButtonEventArgs e) {

        }

        private void BottomRectangle_MouseEnter(object sender, MouseEventArgs e) {

        }

        private void BottomRectangle_MouseLeave(object sender, MouseEventArgs e) {

        }

        private void TopRectangle_MouseDown(object sender, MouseButtonEventArgs e) {

        }

        private void TopRectangle_MouseEnter(object sender, MouseEventArgs e) {

        }

        private void TopRectangle_MouseLeave(object sender, MouseEventArgs e) {

        }

        private void CancelLongProcessButton_Click(object sender, RoutedEventArgs e) {

        }
        #endregion


        #region DependencyPorperties
        #region BytePerLine property/methods

        /// <summary>
        /// Get or set the number of byte are show in control
        /// </summary>
        public int BytePerLine {
            get => (int)GetValue(BytePerLineProperty);
            set => SetValue(BytePerLineProperty, value);
        }

        public static readonly DependencyProperty BytePerLineProperty =
            DependencyProperty.Register("BytePerLine", typeof(int), typeof(HexEditor),
                new PropertyMetadata(16,BytePerLine_PropertyChanged));

        private static void BytePerLine_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is DrawedHexEditor ctrl) || e.NewValue == e.OldValue) return;
            ctrl.HexDataLayer.BytePerLine = (int)e.NewValue;
            ctrl.StringDataLayer.BytePerLine = (int)e.NewValue;
        }

        #endregion
        #endregion
    
       
    }

}
