namespace WPFHexaEditor.Core
{
    /// <summary>
    /// BookMark class
    /// </summary>
    public class BookMark
    {        
        public ScrollMarker Marker { get; set; } = ScrollMarker.Nothing;
        public long BytePositionInFile { get; set; } = 0;
    }
}
