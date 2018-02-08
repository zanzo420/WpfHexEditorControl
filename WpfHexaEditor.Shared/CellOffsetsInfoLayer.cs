using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor
{

    //To show Stream Offsets(left of HexEditor) and Column Index(top of HexEditor);
    public class CellOffsetsInfoLayer : FontControlBase, ICellsLayer, IOffsetsInfoLayer {
        public Thickness CellMargin { get; set; } = new Thickness(2);
        public Thickness CellPadding { get; set; } = new Thickness(2);
        //If datavisualtype is Hex,"ox" should be calculated.
        public virtual Size CellSize => new Size(
            ((DataVisualType == DataVisualType.Hexadecimal ? 2 : 0) +
                SavedBits) * CharSize.Width + CellPadding.Left + CellPadding.Right,
            CharSize.Height + CellPadding.Top + CellPadding.Bottom);

        public int SavedBits { get; set; } = 2;
        
        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(CellOffsetsInfoLayer),
                new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsRender));
        
        public long StartStepIndex {
            get { return (long)GetValue(StartStepIndexProperty); }
            set { SetValue(StartStepIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartStepIndexProperty =
            DependencyProperty.Register(nameof(StartStepIndex), typeof(long), typeof(CellOffsetsInfoLayer),
                new FrameworkPropertyMetadata(0L,FrameworkPropertyMetadataOptions.AffectsRender));
        
        public int StepsCount {
            get { return (int)GetValue(StepsProperty); }
            set { SetValue(StepsProperty, value); }
        }
        
        public DataVisualType DataVisualType { get; set; }

        // Using a DependencyProperty as the backing store for EndOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepsProperty =
            DependencyProperty.Register(nameof(StepsCount), typeof(int), typeof(CellOffsetsInfoLayer),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));



        public int StepLength {
            get { return (int)GetValue(StepLengthProperty); }
            set { SetValue(StepLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StepLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StepLengthProperty =
            DependencyProperty.Register(nameof(StepLength), typeof(int), typeof(CellOffsetsInfoLayer), new PropertyMetadata(1));

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);

            void DrawOneStep(long offSet,Point startPoint) {
                var str = string.Empty;
                switch (DataVisualType) {
                    case DataVisualType.Hexadecimal:
                        str = $"0x{ByteConverters.LongToHex(offSet, SavedBits)}";
                        break;
                    case DataVisualType.Decimal:
                        str = ByteConverters.LongToString(offSet, SavedBits);
                        break;
                    default:
                        break;
                }
#if NET451
                var text = new FormattedText(str, CultureInfo.CurrentCulture, 
                    FlowDirection.LeftToRight, TypeFace, FontSize, Foreground);
#endif
#if NET47
                var text = new FormattedText(str, CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize, Foreground, PixelPerDip);
#endif
                drawingContext.DrawText(text, startPoint);
            }

            void DrawSteps(Func<int,Point> getOffsetLocation) {
                
                for (int i = 0; i < StepsCount; i++) {
                    DrawOneStep(
                        i * StepLength + StartStepIndex,
                        getOffsetLocation(i)
                    );
                }
            }

            if(Orientation == Orientation.Horizontal) {
                DrawSteps(step => 
                    new Point(
                            (CellMargin.Left + CellMargin.Right + CellSize.Width) * step + CellMargin.Left + CellPadding.Left,
                            CellMargin.Top + CellPadding.Top
                    )
                );
                
            }
            else {
#if DEBUG
                //double lastY = 0;
#endif
                DrawSteps(step => new Point(
                            CellMargin.Left + CellPadding.Left,
                            (CellMargin.Top + CellMargin.Bottom + CellSize.Height) * step + CellMargin.Top + CellPadding.Top));
                      
                {
                    
                    
#if DEBUG
                    //if(lastY != pot.Y) {
                    //    lastY = pot.Y;
                    //    System.Diagnostics.Debug.WriteLine(lastY);
                    //}
#endif
                    //return pot;
                }
            
            }
        }

        protected override Size MeasureOverride(Size availableSize) {
            availableSize = base.MeasureOverride(availableSize);
            
            if (Orientation == Orientation.Horizontal) {
                availableSize.Height = CellMargin.Top + CellMargin.Bottom + CellSize.Height;
                if (double.IsInfinity(availableSize.Width)) {
                    availableSize.Width = 0;
                }
            }
            else {
                availableSize.Width = CellMargin.Left + CellMargin.Right + CellSize.Width;
                if (double.IsInfinity(availableSize.Height)) {
                    availableSize.Height = 0;
                }
            }
            
            return availableSize;
        }


        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftDownOnCell;
        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseLeftUpOnCell;
        public event EventHandler<(int cellIndex, MouseEventArgs e)> MouseMoveOnCell;
        public event EventHandler<(int cellIndex, MouseButtonEventArgs e)> MouseRightDownOnCell;
    }
    
}
