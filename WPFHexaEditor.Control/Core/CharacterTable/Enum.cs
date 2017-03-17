//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

namespace WPFHexaEditor.Core.CharacterTable
{
    /// <summary>
    /// Type de DTE qui sera utilisé dans les classe de DTE
    ///
    /// Derek Tremblay 2003-2017
    /// </summary>
    public enum DTEType
    {
        Invalid = -1,
        ASCII = 0,
        Japonais,
        DualTitleEncoding,
        MultipleTitleEncoding,
        EndLine,
        EndBlock
    }
}