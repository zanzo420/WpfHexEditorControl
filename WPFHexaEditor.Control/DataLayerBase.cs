using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor {
    public abstract class DataLayerBase : FrameworkElement,IDataLayer {
        public byte[] Data {
            get { return (byte[])GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data), 
                typeof(byte[]), 
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender
                    //Data_PropertyChanged
                )
            );

        //private static void Data_PropertyChanged(DependencyObject d,
        //    DependencyPropertyChangedEventArgs e) {
        //    if(!(d is DataLayerBase ctrl)){
        //        return;
        //    }
            
        //    if(e.OldValue is INotifyCollectionChanged oldCollection) {
        //        oldCollection.CollectionChanged -= ctrl.RefreshRender;
        //    }

        //    if(e.NewValue is INotifyCollectionChanged newCollection) {
        //        newCollection.CollectionChanged += ctrl.RefreshRender;
        //    }
        //}

        private void RefreshRender(object sender, NotifyCollectionChangedEventArgs e) => this.InvalidateVisual();

        public IEnumerable<(int index, int length, Brush foreground)> ForegroundBlocks {
            get { return (IEnumerable<(int index, int length, Brush background)>)GetValue(ForegroundBlocksProperty); }
            set { SetValue(ForegroundBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForegroundBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundBlocksProperty =
            DependencyProperty.Register(nameof(ForegroundBlocks),typeof(IEnumerable<(int index, int length, Brush foreground)>),
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));
        
        public IEnumerable<(int index, int length, Brush background)> BackgroundBlocks {
            get { return (IEnumerable<(int index, int length, Brush foreground)>)GetValue(BackgroundBlocksProperty); }
            set { SetValue(BackgroundBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackgroundBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundBlocksProperty =
            DependencyProperty.Register(nameof(BackgroundBlocks), typeof(IEnumerable<(int index, int length, Brush background)>),
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));


        public Brush DefaultForeground {
            get { return (Brush)GetValue(DefaultForegroundProperty); }
            set { SetValue(DefaultForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultForegroundProperty =
            DependencyProperty.Register(nameof(DefaultForeground), typeof(Brush),
                typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));


        public double FontSize {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    12.0D,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));



        public FontFamily FontFamily {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register(nameof(FontFamily), typeof(FontFamily), typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    new FontFamily("Microsoft YaHei"),
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));



        public int BytePerLine {
            get { return (int)GetValue(BytePerLineProperty); }
            set { SetValue(BytePerLineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BytePerLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BytePerLineProperty =
            DependencyProperty.Register(nameof(BytePerLine), typeof(int), typeof(DataLayerBase),
                new FrameworkPropertyMetadata(
                    16,
                    FrameworkPropertyMetadataOptions.AffectsRender
                ));
        
        protected double cellMargin = 6;
        protected double lineMargin = 6;
        //Get the width of every char text;
        protected double chSquareSize {
            get {
                //Cuz "D" may hold the "widest" size;
                var typeface = new Typeface(FontFamily, new FontStyle(), new FontWeight(), new FontStretch());
                var measureText = new FormattedText(
                            "D", System.Globalization.CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight, typeface, FontSize, Brushes.Black, VisualTreeHelper.GetDpi(this).PixelsPerDip);
                return measureText.Width;
            }
        }

        public int AvailableRowsCount => (int)(this.ActualHeight / CellSize.Height);

        public abstract Size CellSize { get; }
        
        protected virtual void DrawBackgrounds(DrawingContext drawingContext) {
            if (BackgroundBlocks == null) {
                return;
            }

            foreach (var (index, length, background) in BackgroundBlocks) {
                for (int i = 0; i < length; i++) {
                    var col = (index + i) % BytePerLine;
                    var row = (index + i) / BytePerLine;
                    drawingContext.DrawRectangle(
                        background,
                        null,
                        new Rect {
                            X = col * CellSize.Width,
                            Y = row * CellSize.Height + 1,
                            Height = CellSize.Height - 2,
                            Width = CellSize.Width
                        }
                    );
                }
            }
        }

        protected double PixelPerDip => (_pixelPerDip ?? (_pixelPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip)).Value;
        private double? _pixelPerDip;
        
        protected Typeface TypeFace => _typeface??(_typeface = new Typeface(FontFamily, new FontStyle(), new FontWeight(), new FontStretch()));
        private Typeface _typeface;

        protected virtual void DrawText(DrawingContext drawingContext) {
            if (Data == null) {
                return;
            }

            
            var index = 0;
            foreach (var bt in Data) {
                var col = index % BytePerLine;
                var row = index / BytePerLine;
                
                Brush foreground = DefaultForeground;
                if (ForegroundBlocks != null) {
                    foreach (var tuple in ForegroundBlocks) {
                        if (tuple.index <= index && tuple.index + tuple.length >= index) {
                            foreground = tuple.foreground;
                            break;
                        }
                    }
                }

                DrawByte(drawingContext, bt, foreground, new Point(
                        CellSize.Width * col ,
                        CellSize.Height * row + lineMargin - 2.5
                ));

                index++;
            }

        }
        protected abstract void DrawByte(DrawingContext drawingContext,byte bt,Brush foreground,Point startPoint);
        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            DrawBackgrounds(drawingContext);
            DrawText(drawingContext);
        }

        protected override Size MeasureOverride(Size availableSize) {
            availableSize.Width = CellSize.Width * BytePerLine;
            if (double.IsInfinity(availableSize.Height)) {
                availableSize.Height = 512;
            }
            return availableSize;
            //return base.MeasureOverride(availableSize);
        }
    }

    public class HexDataLayer : DataLayerBase {
        public override Size CellSize => 
            (_cellSize ?? (_cellSize = new Size(cellMargin + 2 * chSquareSize, chSquareSize + lineMargin * 2))).Value;
        private Size? _cellSize;

        protected override void DrawByte(DrawingContext drawingContext, byte bt, Brush foreground, Point startPoint) {
            var chs = ByteConverters.ByteToHexCharArray(bt);
            for (int chIndex = 0; chIndex < 2; chIndex++) {

                var text = new FormattedText(
                    chs[chIndex].ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground,PixelPerDip);
                startPoint.X += chSquareSize * chIndex;
                drawingContext.DrawText(text,
                    startPoint
                );
            }
        }
    }

    public class StringDataLayer : DataLayerBase {
        public override Size CellSize =>
            (_cellSize ?? (_cellSize = new Size(cellMargin + chSquareSize, chSquareSize + lineMargin * 2))).Value;
        private Size? _cellSize;
        protected override void DrawByte(DrawingContext drawingContext, byte bt , Brush foreground, Point startPoint) {
            var ch = ByteConverters.ByteToChar(bt);
            var text = new FormattedText(ch.ToString(), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, TypeFace, FontSize,
                foreground, PixelPerDip);
            drawingContext.DrawText(text, startPoint);
        }
    }
}
