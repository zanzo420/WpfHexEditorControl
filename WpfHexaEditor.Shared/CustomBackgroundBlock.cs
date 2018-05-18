using System;
using System.Windows.Media;
using WpfHexaEditor.Core.Bytes;

namespace WpfHexaEditor
{
    internal class CustomBackgroundBlock
    {
        private long _length;

        public CustomBackgroundBlock() { }

        public CustomBackgroundBlock(long start, long length, SolidColorBrush color)
        {
            StartOffset = start;
            Length = length;
            Color = color;
        }

        public CustomBackgroundBlock(string start, long length, SolidColorBrush color)
        {
            var srt = ByteConverters.HexLiteralToLong(start);

            StartOffset = srt.success ? srt.position : throw new Exception("Can't convert this string to long");
            Length = length;
            Color = color;
        }

        /// <summary>
        /// Get or set the start offset
        /// </summary>
        public long StartOffset { get; set; }

        /// <summary>
        /// Get the stop offset
        /// </summary>
        public long StopOffset => StartOffset + Length - 1;

        /// <summary>
        /// Get or set the lenght of background block
        /// </summary>
        public long Length
        {
            get => _length;
            set => _length = value > 0 ? value : 1;
        }

        /// <summary>
        /// Description of background block
        /// </summary>
        public string Description { get; set; }

        public SolidColorBrush Color { get; set; } = Brushes.Transparent;
    }
}
