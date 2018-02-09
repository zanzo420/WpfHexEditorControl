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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor {
    /// <summary>
    /// Interaction logic for DrawedHexEditor.xaml
    /// </summary>
    public partial class DrawedHexEditor : UserControl {
        #region DevBranch
        public DrawedHexEditor() {
            InitializeComponent();
            this.FontSize = 8;
            this.FontFamily = new FontFamily("Courier New");
            this.DataVisualType = DataVisualType.Decimal;

            CellMargion = new Thickness(0,1,0,1);
            CellPadding = new Thickness(2);
            var customBacks = new List<(long index, long length, Brush background)> {
                (0L,4L,Brushes.Yellow),
                (4L,4L,Brushes.Red),
                (8L,16L,Brushes.Brown)
            };
            for (int i = 0; i < 200; i++) {
                customBacks.Add((24 + i, 1, Brushes.Chocolate));
            }
            CustomBackgroundBlocks = customBacks;
            //SelectionStart = 0;
            //SelectionLength = 1048576;

            InitilizeEvents();
            
        }

        //Cuz xaml designer's didn't support valuetuple,events subscribing will be executed in code-behind.
        private void InitilizeEvents() {
            this.SizeChanged += delegate { UpdateContent(); };

            void initialCellsLayer(ICellsLayer layer) {
                layer.MouseLeftDownOnCell += DataLayer_MouseLeftDownOnCell;
                layer.MouseLeftUpOnCell += DataLayer_MouseLeftUpOnCell;
                layer.MouseMoveOnCell += DataLayer_MouseMoveOnCell;
                layer.MouseRightDownOnCell += DataLayer_MouseRightDownOnCell;
            }

            initialCellsLayer(HexDataLayer);
            initialCellsLayer(StringDataLayer);
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

        private int MaxVisibleLength {
            get {
                if(Stream == null) {
                    return 0;
                }

                return (int)Math.Min(HexDataLayer.AvailableRowsCount * BytePerLine,
                Stream.Length - Position / BytePerLine * BytePerLine);
            }
        }
            

        /// <summary>
        /// Obtain the max line for verticalscrollbar
        /// </summary>
        private long MaxLine => Stream.Length / BytePerLine;

#if DEBUG
        private Stopwatch watch = new Stopwatch();
#endif

        private List<(int index, int length, Brush background)> dataBackgroundBlocks = new List<(int index, int length, Brush background)>();

        //To avoid endless looping of ScrollBar_ValueChanged and Position_PropertyChanged.
        private bool _scrollBarValueUpdating = false;
        //Remember the position in which the mouse last clicked.
        private long? _lastMouseDownPosition;

        #region EventSubscriber handlers;
        private void Control_MouseWheel(object sender, MouseWheelEventArgs e) {
            if(Stream != null) {
                if (e.Delta > 0) //UP
                    VerticalScrollBar.Value -= e.Delta / 120 * (int)MouseWheelSpeed;

                if (e.Delta < 0) //Down
                    VerticalScrollBar.Value += e.Delta / 120 * -(int)MouseWheelSpeed;
            }
        }
        
        private void DataLayer_MouseLeftDownOnCell(object sender, (int cellIndex, MouseButtonEventArgs e) arg) {
            if(arg.cellIndex >= MaxVisibleLength) {
                return;
            }

            _lastMouseDownPosition = Position / BytePerLine * BytePerLine + arg.cellIndex;
            FocusPosition = _lastMouseDownPosition.Value;
        }
        
        private void DataLayer_MouseRightDownOnCell(object sender, (int cellIndex, MouseButtonEventArgs e) arg) {
            
        }

        private void DataLayer_MouseMoveOnCell(object sender, (int cellIndex, MouseEventArgs e) arg) {
            if(arg.e.LeftButton != MouseButtonState.Pressed) {
                return;
            }

#if DEBUG
            //arg.cellIndex = 15;
            //_lastMouseDownPosition = 0;
#endif
            //Operate Selection;
            if (_lastMouseDownPosition == null) {
                return;
            }
            
            var cellPosition = Position / BytePerLine * BytePerLine + arg.cellIndex;
            if (_lastMouseDownPosition.Value == cellPosition) {
                return;
            }

            var length = Math.Abs(cellPosition - _lastMouseDownPosition.Value) + 1;
            SelectionStart = Math.Min(cellPosition, _lastMouseDownPosition.Value);
            SelectionLength = length;
        }

        private void DataLayer_MouseLeftUpOnCell(object sender, (int cellIndex, MouseButtonEventArgs e) arg) {
            _lastMouseDownPosition = null;
        }

        #endregion
       
        private static void Stream_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is DrawedHexEditor ctrl)) return;
            //These methods won't be invoked everytime scrolling.but only when stream is opened or closed.
            ctrl.UpdateInfoes();

            //Position PropertyChangedCallBack will update the content;
            ctrl.Position = 0;

            ctrl.SelectionStart = -1;
            ctrl.SelectionLength = 0;
            
            //UpdateTblBookMark();
            //UpdateSelectionColor(FirstColor.HexByteData);

            ////Update count of byte
            //UpdateByteCount();

            ////Debug
            //Debug.Print("STREAM OPENED");

        }

        /// <summary>
        /// This method won't be while scrolling,but only when stream is opened or closed,byteperline changed(UpdateInfo);
        /// </summary>
        private void UpdateInfoes() {
            UpdateScrollBarInfo();
            UpdateColumnHeaderInfo();
            UpdateOffsetLinesInfo();
        }

        #region These methods won't be invoked everytime scrolling.but only when stream is opened or closed,byteperline changed(UpdateInfo).

        /// <summary>
        /// Update vertical scrollbar with file info
        /// </summary>
        private void UpdateScrollBarInfo() {
            VerticalScrollBar.Visibility = Visibility.Collapsed;

            if (Stream != null) {
                VerticalScrollBar.Visibility = Visibility.Visible;
                VerticalScrollBar.SmallChange = 1;
                //VerticalScrollBar.LargeChange = ScrollLargeChange;
                VerticalScrollBar.Maximum = MaxLine - 1;
            }
        }

        /// <summary>
        /// Update the position info panel at top of the control
        /// </summary>
        private void UpdateColumnHeaderInfo() {
            ColumnsOffsetInfoLayer.StartStepIndex = 0;
            ColumnsOffsetInfoLayer.StepsCount = 16;
        }

        /// <summary>
        /// Update the position info panel at left of the control,see this won't change the content of the OffsetLines;
        /// </summary>
        private void UpdateOffsetLinesInfo() {
            if(Stream == null) {
                return;
            }
            LinesOffsetInfoLayer.DataVisualType = DataVisualType;
            LinesOffsetInfoLayer.StepLength = BytePerLine;
            if(DataVisualType == DataVisualType.Hexadecimal) {
                LinesOffsetInfoLayer.SavedBits = ByteConverters.GetHexBits(Stream.Length);
            }
            else {
                LinesOffsetInfoLayer.SavedBits = ByteConverters.GetDecimalBits(Stream.Length);
            }
        }

        //This will affect how a linesinfo and columnsinfo index change.
        public DataVisualType DataVisualType {
            get { return (DataVisualType)GetValue(DataVisualTypeProperty); }
            set { SetValue(DataVisualTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataVisualType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataVisualTypeProperty =
            DependencyProperty.Register(nameof(DataVisualType), 
                typeof(DataVisualType), typeof(DrawedHexEditor),
                new PropertyMetadata(DataVisualType.Hexadecimal, DataVisualTypeProperty_Changed));

        private static void DataVisualTypeProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is DrawedHexEditor ctrl)) {
                return;
            }

            ctrl.LinesOffsetInfoLayer.DataVisualType = (DataVisualType)e.NewValue;
            ctrl.ColumnsOffsetInfoLayer.DataVisualType = (DataVisualType)e.NewValue;
            ctrl.UpdateContent();
        }

        #endregion


        public long Position {
            get { return (long)GetValue(PositionProperty); }
            set {
                SetValue(PositionProperty, value);
#if DEBUG
                watch.Restart();
#endif
                UpdateContent();
#if DEBUG
                watch.Stop();
                Debug.Print($"REFRESH TIME: {watch.ElapsedMilliseconds} ms");
#endif
            }
        
        }

        // Using a DependencyProperty as the backing store for Position.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(-1L, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        
        
        public IEnumerable<(long index,long length,Brush background)> CustomBackgroundBlocks {
            get { return (IEnumerable<(long index,long length,Brush background)>)GetValue(CustomBackgroundBlocksProperty); }
            set { SetValue(CustomBackgroundBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomBackgroundBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomBackgroundBlocksProperty =
            DependencyProperty.Register(nameof(CustomBackgroundBlocks), 
                typeof(IEnumerable<(long index,long length,Brush background)>), 
                typeof(DrawedHexEditor),
                new PropertyMetadata(null));

        public Thickness CellMargion {
            set {
                HexDataLayer.CellMargin = value;
                StringDataLayer.CellMargin = value;
                LinesOffsetInfoLayer.CellMargin   = new Thickness(0, value.Top, 0, value.Bottom);
                ColumnsOffsetInfoLayer.CellMargin = new Thickness(value.Left, 0, value.Right, 0);
            }
        }

        public Thickness CellPadding {
            set {
                HexDataLayer.CellPadding = value;
                StringDataLayer.CellPadding = value;
                LinesOffsetInfoLayer.CellPadding = new Thickness(0, value.Top, 0, value.Bottom);
                ColumnsOffsetInfoLayer.CellPadding = new Thickness(value.Left, 0, value.Right, 0);
            }
        }
        
        /// <summary>
        /// Refresh currentview of hexeditor
        /// </summary>
        /// <param name="controlResize"></param>
        /// <param name="refreshData"></param>
        public void UpdateContent() {
            UpdateOffsetLinesContent();
            UpdateScrollBarContent();
            //Update visual of byte control
            //UpdateByteModified();
            
            //UpdateHighLight();
            //UpdateStatusBar();
            //UpdateVisual();
            //UpdateFocus();

            //CheckProviderIsOnProgress();

            //if (controlResize) {
            //    UpdateScrollMarkerPosition();
            //    UpdateHeader(true);
            //}
            
            UpdateBackgroundBlocks();

            UpdateDataContent();
        }

        

        #region  These methods will be invoked every time scrolling the content(scroll or position changed)(Refreshview calling);
        ///<see cref="UpdateContent"/>
        /// <summary>
        /// Update the hex and string layer you current view;
        /// </summary>
        private void UpdateDataContent() {
            if (!(Stream?.CanRead??false)) {
                HexDataLayer.Data = null;
                StringDataLayer.Data = null;
                return;
            }

            Stream.Position = Position / BytePerLine * BytePerLine;
            
            if (_viewBuffer == null || _viewBuffer.Length != MaxVisibleLength) {
                _viewBuffer = new byte[MaxVisibleLength];
            }
            if (_viewBuffer2 == null || _viewBuffer2.Length != MaxVisibleLength) {
                _viewBuffer2 = new byte[MaxVisibleLength];
            }
            _realViewBuffer = _realViewBuffer == _viewBuffer ? _viewBuffer2 : _viewBuffer;
            
            Stream.Read(_realViewBuffer, 0, MaxVisibleLength);
         
            HexDataLayer.Data = _realViewBuffer;
            StringDataLayer.Data = _realViewBuffer;
        }

        private void UpdateOffsetLinesContent() {
            if(Stream == null) {
                LinesOffsetInfoLayer.StartStepIndex = 0;
                LinesOffsetInfoLayer.StepsCount = 0;
                return;
            }
            
            LinesOffsetInfoLayer.StartStepIndex = Position / BytePerLine * BytePerLine;
            LinesOffsetInfoLayer.StepsCount = 
                (int)Math.Min(HexDataLayer.AvailableRowsCount , 
                MaxVisibleLength / BytePerLine + (MaxVisibleLength % BytePerLine != 0?1:0 ));
        }

        private void UpdateScrollBarContent() {
            if (!_scrollBarValueUpdating) {
                _scrollBarValueUpdating = true;
                VerticalScrollBar.Value = Position / BytePerLine;
                _scrollBarValueUpdating = false;
            }
        }
        #region Data Backgrounds
        private void UpdateBackgroundBlocks() {
            //ClearBackgroundBlocks;
            HexDataLayer.BackgroundBlocks = null;
            StringDataLayer.BackgroundBlocks = null;

            dataBackgroundBlocks.Clear();

            AddCustomBackgroundBlocks();
            AddSelectionBackgroundBlocks();
            AddFocusPositionBlock();

            HexDataLayer.BackgroundBlocks = dataBackgroundBlocks;
            StringDataLayer.BackgroundBlocks = dataBackgroundBlocks;
        }

        private void AddBackgroundBlock(long index, long length, Brush background) {
            if (Stream == null) {
                return;
            }
            
            //Check whether Selection is in sight;
            if (!(index + length >= Position && index < Position + MaxVisibleLength)) {
                return;
            }

            var maxIndex = Math.Max(index, Position);
            var minEnd = Math.Min(index + length, Position + MaxVisibleLength);

            dataBackgroundBlocks.Add(((int)(maxIndex - Position), (int)(minEnd - maxIndex), background));
        }
       
        private void AddSelectionBackgroundBlocks() => AddBackgroundBlock(SelectionStart, SelectionLength, SelectionBrush);
       
        private void AddCustomBackgroundBlocks() {
            if (CustomBackgroundBlocks != null) {
                foreach (var (index, length, background) in CustomBackgroundBlocks) {
                    AddBackgroundBlock(index, length,background);
                }
            }
        }

        private void AddFocusPositionBlock() {
            if(FocusPosition >= 0) {
                AddBackgroundBlock(FocusPosition, 1, FocusBrush);
            }
        }
        #endregion

        private void UpdateForegroundBlocks() {

        }

        #endregion

        

        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (_scrollBarValueUpdating) {
                return;
            }

            _scrollBarValueUpdating = true;
            Position = (long)e.NewValue * BytePerLine;
            _scrollBarValueUpdating = false;
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

            ctrl.UpdateInfoes();

            ctrl.UpdateContent();
        }

        #endregion

        public long SelectionStart {
            get { return (long)GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register(nameof(SelectionStart), typeof(long), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(-1L, SelectionStart_PropertyChanged));

        private static void SelectionStart_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(!(d is DrawedHexEditor ctrl)) {
                return;
            }

            ctrl.UpdateBackgroundBlocks();
        }
        
        public long SelectionLength {
            get { return (long)GetValue(SelectionLengthProperty); }
            set { SetValue(SelectionLengthProperty, value); }
        }


        // Using a DependencyProperty as the backing store for SelectionLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionLengthProperty =
            DependencyProperty.Register(nameof(SelectionLength), typeof(long), typeof(DrawedHexEditor), new PropertyMetadata(0L, SelectionLengthProperty_Changed));

        private static void SelectionLengthProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(!(d is DrawedHexEditor ctrl)) {
                return;
            }

            ctrl.UpdateBackgroundBlocks();
        }

        public long FocusPosition {
            get { return (long)GetValue(FocusPositionProperty); }
            set { SetValue(FocusPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusPositionProperty =
            DependencyProperty.Register(nameof(FocusPosition), typeof(long), typeof(DrawedHexEditor),
                new PropertyMetadata(-1L, FocusPositionProperty_Changed));

        private static void FocusPositionProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if(!(d is DrawedHexEditor ctrl)) {
                return;
            }

            ctrl.UpdateBackgroundBlocks();
        }



        public Brush FocusBrush {
            get { return (Brush)GetValue(FocusBrushProperty); }
            set { SetValue(FocusBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusBrushProperty =
            DependencyProperty.Register(nameof(FocusBrush), typeof(Brush), typeof(DrawedHexEditor), new PropertyMetadata(Brushes.Blue));



        public Brush SelectionBrush {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectionBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register(nameof(SelectionBrush), typeof(Brush), typeof(DrawingBrush), new PropertyMetadata(Brushes.Red));


        public MouseWheelSpeed MouseWheelSpeed {
            get { return (MouseWheelSpeed)GetValue(MouseWheelSpeedProperty); }
            set { SetValue(MouseWheelSpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseWheelSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseWheelSpeedProperty =
            DependencyProperty.Register(nameof(MouseWheelSpeed), typeof(MouseWheelSpeed), typeof(DrawedHexEditor), new PropertyMetadata(MouseWheelSpeed.Normal));


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


        #endregion

    }

}
