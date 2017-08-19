//////////////////////////////////////////////
// Apache 2.0  - 2017
// Author       : Janus Tida
// Contributor  : Derek Tremblay
//////////////////////////////////////////////

namespace WPFHexaEditor.Core.Interface
{
    /// <summary>
    /// All byte control inherit from this interface.
    /// This interface is used to reduce the code when manipulate byte control
    /// </summary>
    internal interface IByteControl
    {
        long BytePositionInFile { get; set; }
        ByteAction Action { get; set; }
        byte? Byte { get; set; }
        bool IsFocus { get; set; }
        string HexString { get; }
        bool IsHighLight { get; set; }
        bool IsSelected { get; set; }
        bool FirstSelected { get; set; }
        bool ReadOnlyMode { get; set; }
        bool InternalChange { get; set; }
    }
}
