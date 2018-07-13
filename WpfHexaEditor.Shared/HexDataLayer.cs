//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WpfHexaEditor.Core.Bytes;

namespace WpfHexaEditor
{
    public class HexDataLayer : DataLayerBase
    {
        public override Size CellSize => new Size
        (
            2 * CharSize.Width + CellPadding.Left + CellPadding.Right,
            CellPadding.Top + CellPadding.Bottom + CharSize.Height
        );

        protected void DrawByte(DrawingContext drawingContext, byte bt, Brush foreground, Point startPoint)
        {
            var chs = ByteConverters.ByteToHexCharArray(bt);

            for (var chIndex = 0; chIndex < 2; chIndex++)
            {
#if NET451
                var text = new FormattedText
                (
                    chs[chIndex].ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground
                );
#endif
                
#if NET47
                var text = new FormattedText
                (
                    chs[chIndex].ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground, PixelPerDip
                );
#endif

                startPoint.X += CharSize.Width * chIndex;

                drawingContext.DrawText
                (
                    text,
                    startPoint
                );

            }
        }

        protected override void DrawText(DrawingContext drawingContext) {
            if (Data == null)
                return;

            var index = 0;

            foreach (var bt in Data) {
                var col = index % BytePerLine;
                var row = index / BytePerLine;
                var foreground = Foreground;

                if (ForegroundBlocks != null)
                    foreach (var brushBlock in ForegroundBlocks) {
                        if (brushBlock.StartOffset <= index && brushBlock.StartOffset + brushBlock.Length - 1 >= index)
                            foreground = brushBlock.Brush;
                    }

                DrawByte(drawingContext, bt, foreground,
                    new Point
                    (
                        (CellMargin.Right + CellMargin.Left + CellSize.Width) * col + CellPadding.Left + CellMargin.Left,
                        (CellMargin.Top + CellMargin.Bottom + CellSize.Height) * row + CellPadding.Top + CellMargin.Top
                    )
                );

                index++;
            }
        }
    }


}
