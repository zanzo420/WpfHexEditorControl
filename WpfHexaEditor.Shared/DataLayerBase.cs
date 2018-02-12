using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor {
    public abstract class DataLayerBase : FontControlBase, IDataLayer, ICellsLayer {
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
                    //DataProperty_Changed
                )
            );

        //private static void DataProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        //    if(!(d is DataLayerBase ctrl)) {
        //        return;
        //    }
        //}

        private void RefreshRender(object sender, NotifyCollectionChangedEventArgs e) => this.InvalidateVisual();

        public IEnumerable<(int index, int length, Brush foreground)> ForegroundBlocks {
            get { return (IEnumerable<(int index, int length, Brush background)>)GetValue(ForegroundBlocksProperty); }
            set { SetValue(ForegroundBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForegroundBlocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForegroundBlocksProperty =
            DependencyProperty.Register(nameof(ForegroundBlocks), typeof(IEnumerable<(int index, int length, Brush foreground)>),
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

        public Thickness CellPadding { get; set; } = new Thickness(2);
        public Thickness CellMargin { get; set; } = new Thickness(2);

        public int AvailableRowsCount => (int)(this.ActualHeight / CellSize.Height);

        public abstract Size CellSize { get; }

        private (int index, Brush background)[] _drawedRects;
        private (int index, Brush background)[] DrawedRects {
            get {
                if(Data == null) {
                    return null;
                }

                if(_drawedRects == null || _drawedRects.Length < Data.Length) {
                    _drawedRects = new(int index, Brush background)[Data.Length];
                }

                return _drawedRects;
            }
        }


        protected virtual void DrawBackgrounds(DrawingContext drawingContext) {
            if (BackgroundBlocks == null) {
                return;
            }

            if(DrawedRects == null) {
                return;
            }

            if(Data == null) {
                return;
            }

            for (int i = 0; i < Data.Length; i++) {
                DrawedRects[i].background = Brushes.Transparent;
            }
            
#if DEBUG
            //double lastY = 0;
#endif
            foreach (var (index, length, background) in BackgroundBlocks) {
                for (int i = 0; i < length; i++) {
                    DrawedRects[index + i].background = background;
#if DEBUG
                    //if(this is HexDataLayer && lastY != rect.Y) {
                    //    lastY = rect.Y;
                    //    System.Diagnostics.Debug.WriteLine(rect.Y);
                    //}
#endif
                }
            }

            for (int i = 0; i < Data.Length; i++) {
                var col = i % BytePerLine;
                var row = i / BytePerLine;

                drawingContext.DrawRectangle(
                    DrawedRects[i].background,
                    null,
                    new Rect {
                        X = col * (CellMargin.Right + CellMargin.Left + CellSize.Width) + CellMargin.Left,
                        Y = row * (CellMargin.Top + CellMargin.Bottom + CellSize.Height) + CellMargin.Top,
                        Height = CellSize.Height,
                        Width = CellSize.Width
                    }
                );
            }
           
        }

        protected virtual void DrawText(DrawingContext drawingContext) {
            if (Data == null) {
                return;
            }


            var index = 0;
            foreach (var bt in Data) {
                var col = index % BytePerLine;
                var row = index / BytePerLine;

                Brush foreground = Foreground;
                if (ForegroundBlocks != null) {
                    foreach (var tuple in ForegroundBlocks) {
                        if (tuple.index <= index && tuple.index + tuple.length >= index) {
                            foreground = tuple.foreground;
                            break;
                        }
                    }
                }

                DrawByte(drawingContext, bt, foreground,
                    new Point(
                        (CellMargin.Right + CellMargin.Left + CellSize.Width) * col + CellPadding.Left + CellMargin.Left,
                        (CellMargin.Top + CellMargin.Bottom + CellSize.Height) * row + CellPadding.Top + CellMargin.Top
                    )
                );

                index++;
            }

        }
        protected abstract void DrawByte(DrawingContext drawingContext, byte bt, Brush foreground, Point startPoint);
        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            DrawBackgrounds(drawingContext);
            DrawText(drawingContext);
        }

        protected override Size MeasureOverride(Size availableSize) {
            availableSize = base.MeasureOverride(availableSize);
            availableSize.Width = (CellSize.Width + CellMargin.Left + CellMargin.Right) * BytePerLine;
            if (double.IsInfinity(availableSize.Height)) {
                availableSize.Height = 0;
            }
            
            return availableSize;
        }

        public event EventHandler<(int cellIndex,MouseButtonEventArgs e)> MouseLeftDownOnCell;
        public event EventHandler<(int cellIndex,MouseButtonEventArgs e)> MouseLeftUpOnCell;
        public event EventHandler<(int cellIndex,MouseEventArgs e)> MouseMoveOnCell;
        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseRightDownOnCell;
        
        private int? GetIndexFromMouse(MouseEventArgs e) {
            if (Data == null) {
                return null;
            }

            var location = e.GetPosition(this);
            var col = (int)(location.X / (CellMargin.Left + CellMargin.Right + CellSize.Width));
            var row = (int)(location.Y / (CellMargin.Top + CellMargin.Bottom + CellSize.Height));
            if (row * BytePerLine + col < Data.Length) {
                return row * BytePerLine + col;
            }
            return null;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnPreviewMouseLeftButtonDown(e);
       
            if (Data == null) {
                return;
            }

            var index = GetIndexFromMouse(e);
            if(index != null) {
                MouseLeftDownOnCell?.Invoke(this,(index.Value,e));
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
            base.OnMouseUp(e);
            var index = GetIndexFromMouse(e);
            if(index != null) {
                MouseLeftUpOnCell?.Invoke(this, (index.Value, e));
            }
        }
        
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            var index = GetIndexFromMouse(e);
            if(index != null) {
                MouseMoveOnCell?.Invoke(this, (index.Value, e));
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
            base.OnMouseRightButtonDown(e);
            var index = GetIndexFromMouse(e);
            if(index != null) {
                MouseRightDownOnCell?.Invoke(this, (index.Value, e));
            }
        }
    }
    
}
