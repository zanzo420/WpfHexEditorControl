using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WpfHexaEditor.Core.Bytes;

namespace WpfHexaEditor
{
    public class HexDataLayer : DataLayerBase {
        public override Size CellSize => new Size(
                CellPadding.Top + CellPadding.Bottom + 2 * CharSize.Width,
                CharSize.Height + CellPadding.Left + CellPadding.Right);

        protected override void DrawByte(DrawingContext drawingContext, byte bt, Brush foreground, Point startPoint) {
            var chs = ByteConverters.ByteToHexCharArray(bt);
            for (int chIndex = 0; chIndex < 2; chIndex++) {
#if NET451
                var text = new FormattedText(
                    chs[chIndex].ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground);
                startPoint.X += CharSize.Width * chIndex;
                drawingContext.DrawText(text,
                    startPoint
                );

#endif
#if NET47
                var text = new FormattedText(
                    chs[chIndex].ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground, PixelPerDip);
                startPoint.X += CharSize.Width * chIndex;
                drawingContext.DrawText(text,
                    startPoint
                );
#endif

            }
        }

    }
}
