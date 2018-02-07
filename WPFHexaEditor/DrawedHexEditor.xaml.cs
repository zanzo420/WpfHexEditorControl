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
            //HexDataLayer.CellPadding = new Thickness(2, 0, 2, 0);
            //HexDataLayer.CellMargin =  new Thickness(0, 2, 0, 2);
            //HexDataLayer.CellPadding = new Thickness(0, 2, 0, 2);
            //StringDataLayer.CellMargin = new Thickness(0, 2, 0, 2);
            //StringDataLayer.CellPadding = new Thickness(0, 2, 0, 2);

            HexDataLayer.FontFamily = new FontFamily("Courier New");
            StringDataLayer.FontFamily = new FontFamily("Courier New");
            HexDataLayer.FontSize = 13;
            StringDataLayer.FontSize = 13;

            LinesOffsetInfoLayer.FontSize = 13;
            ColumnsOffsetInfoLayer.FontSize = 13;
            ColumnsOffsetInfoLayer.FontFamily = new FontFamily("Courier New");
            LinesOffsetInfoLayer.FontFamily = new FontFamily("Courier New");
            ColumnsOffsetInfoLayer.DataVisualType = Core.DataVisualType.Decimal;
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
        //To avoid endless looping of ScrollBar_ValueChanged and Position_PropertyChanged.
        private bool scrollBarValueUpdating = false;

        // Using a DependencyProperty as the backing store for Stream.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StreamProperty =
            DependencyProperty.Register(nameof(Stream), typeof(Stream), typeof(DrawedHexEditor),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,
                    Stream_PropertyChanged));

        private static void Stream_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is DrawedHexEditor ctrl)) return;

            //Close Old Stream,See the stream won't be closed actually;
            if (e.OldValue is Stream stream) {

            }

            //Open New Stream;
            if (e.NewValue is Stream newStream && newStream.CanRead) {
                ctrl.UpdateScrollBar();
                ctrl.UpdateColumnHeaders();
                ctrl.UpdateColumnHeaders();
                //Position PropertyChangedCallBack will update the view;
                ctrl.Position = 0;
                //UnSelectAll();

                //UpdateTblBookMark();
                //UpdateSelectionColor(FirstColor.HexByteData);

                ////Update count of byte
                //UpdateByteCount();

                ////Debug
                //Debug.Print("STREAM OPENED");
            }

        }
        
        #region These methods won't be invoked everytime scrolling.but only when stream is opened or closed.

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
        /// Update the position info panel at top of the control
        /// </summary>
        private void UpdateColumnHeaders() {
            ColumnsOffsetInfoLayer.StartStepIndex = 0;
            ColumnsOffsetInfoLayer.StepsCount = 16;
        }

        /// <summary>
        /// Update the position info panel at left of the control,see this won't change the content of the OffsetLines;
        /// </summary>
        private void UpdateOffsetLinesInfo() {
            LinesOffsetInfoLayer.StepLength = BytePerLine;
        }

        #endregion
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

       

#if DEBUG
        private Stopwatch watch = new Stopwatch();
#endif

        /// <summary>
        /// Refresh currentview of hexeditor
        /// </summary>
        /// <param name="controlResize"></param>
        /// <param name="refreshData"></param>
        public void RefreshView() {
            UpdateOffsetLinesContent();
            
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

            UpdateData();
        }

        #region  These methods will be invoked every time scrolling the content(scroll or position changed)(Refreshview calling);
        ///<see cref="RefreshView"/>

        /// <summary>
        /// Update the data and string stackpanels yo current view;
        /// </summary>
        private void UpdateData() {
            if (!(Stream?.CanRead??false)) {
                HexDataLayer.Data = null;
                StringDataLayer.Data = null;
                return;
            }

            Stream.Position = Position / BytePerLine * BytePerLine;
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
        }

        private void UpdateOffsetLinesContent() {
            if(Stream == null) {
                LinesOffsetInfoLayer.StartStepIndex = 0;
                LinesOffsetInfoLayer.StepsCount = 0;
            }
            
            LinesOffsetInfoLayer.StartStepIndex = Position / BytePerLine * BytePerLine;
            LinesOffsetInfoLayer.StepsCount = 
                (int)Math.Min(HexDataLayer.AvailableRowsCount ,
                (Stream.Length - Position) / BytePerLine + ((Stream.Length - Position) % BytePerLine > 0?1:0));
        }

        #endregion

        /// <summary>
        /// Obtain the max line for verticalscrollbar
        /// </summary>
        private long MaxLine => Stream.Length / BytePerLine;

        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (scrollBarValueUpdating) {
                return;
            }

            scrollBarValueUpdating = true;
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

            ctrl.UpdateColumnHeaders();
            ctrl.RefreshView();
        }

        #endregion
        #endregion
    
       
    }

}
