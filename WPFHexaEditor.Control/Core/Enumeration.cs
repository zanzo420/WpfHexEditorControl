using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFHexaEditor.Control.Core
{
    /// <summary>
    /// ByteAction used for ByteModified class
    /// </summary>
    public enum ByteAction
    {
        Nothing,
        Added,
        Deleted,
        Modified,

        /// <summary>
        /// Used in ByteProvirder for get list
        /// </summary>
        All 
    }

    /// <summary>
    /// Used for coloring mode of selection
    /// </summary>
    public enum FirstColor
    {
        HexByteData,
        StringByteData
    }

    /// <summary>
    /// Mode of Copy/Paste
    /// </summary>
    public enum CopyPasteMode
    {
        Byte,
        HexaString,
        ASCIIString
    }

    /// <summary>
    /// Used for check label are selected et next label to select...
    /// </summary>
    public enum KeyDownLabel
    {
        FirstChar,
        SecondChar,
        NextPosition
    }

    public enum ByteToString
    {
        /// <summary>
        /// Build-in convertion mode. (recommended)
        /// </summary>
        ByteToCharProcess,
        /// <summary>
        /// System.Text.Encoding.ASCII string encoder
        /// </summary>
        ASCIIEncoding
    }


}
