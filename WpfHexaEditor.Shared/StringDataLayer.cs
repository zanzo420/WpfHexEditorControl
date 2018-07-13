//////////////////////////////////////////////
// Apache 2.0  - 2018
// Author : Janus Tida
// Modified by : Derek Tremblay
//////////////////////////////////////////////

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor
{
    public class StringDataLayer : DataLayerBase
    {
        public override Size CellSize =>
            new Size(CellPadding.Right + CellPadding.Left + CharSize.Width,
                CharSize.Height + CellPadding.Top + CellPadding.Bottom);

        private byte[] _drawCharBuffer = null;
        protected override void DrawText(DrawingContext drawingContext) {
            if (Data == null)
                return;

            if (BytesToCharEncoding == null)
                return;
            
            if(_drawCharBuffer == null || _drawCharBuffer.Length != BytesToCharEncoding.BytePerChar) {
                _drawCharBuffer = new byte[BytesToCharEncoding.BytePerChar];
            }

            var firstVisibleBtIndex = (int)(BytesToCharEncoding.BytePerChar - DataOffsetInOriginalStream % BytesToCharEncoding.BytePerChar) % BytesToCharEncoding.BytePerChar;

            var charCount = (Data.Length - firstVisibleBtIndex) / BytesToCharEncoding.BytePerChar;
            
            for (int chIndex = 0; chIndex < charCount; chIndex++) {
                var btIndex = BytesToCharEncoding.BytePerChar * chIndex;
                var col = btIndex % BytePerLine;
                var row = btIndex / BytePerLine;
                var foreground = Foreground;

                if (ForegroundBlocks != null)
                    foreach (var block in ForegroundBlocks) {
                        if (block.StartOffset > btIndex || block.StartOffset + block.Length < btIndex) continue;

                        foreground = block.Brush;
                        break;
                    }
                
                Buffer.BlockCopy(Data, btIndex + firstVisibleBtIndex , _drawCharBuffer, 0, BytesToCharEncoding.BytePerChar);

#if NET451
                var text = new FormattedText(BytesToCharEncoding.Convert(_drawCharBuffer).ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground);
#endif

#if NET47
                var text = new FormattedText
                (
                    BytesToCharEncoding.Convert(_drawCharBuffer).ToString(), CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, TypeFace, FontSize,
                    foreground, PixelPerDip
                );
#endif
                drawingContext.DrawText(text,
                   new Point(
                       (CellMargin.Right + CellMargin.Left + CellSize.Width) * col + CellPadding.Left + CellMargin.Left,
                       (CellMargin.Top + CellMargin.Bottom + CellSize.Height) * row + CellPadding.Top + CellMargin.Top
                   )
               );
            }
        }
        
        public IBytesToCharEncoding BytesToCharEncoding {
            get { return (IBytesToCharEncoding)GetValue(BytesToCharEncodingProperty); }
            set { SetValue(BytesToCharEncodingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BytesToCharConverterProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BytesToCharEncodingProperty =
            DependencyProperty.Register(nameof(BytesToCharEncoding), typeof(IBytesToCharEncoding), typeof(StringDataLayer), new FrameworkPropertyMetadata(BytesToCharEncodings.ASCII, FrameworkPropertyMetadataOptions.AffectsRender));
        
    }
}
