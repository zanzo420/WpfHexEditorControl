using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFHexaEditor.Control.Core
{
    public static class ConstantReadOnly
    {
        public static readonly string HexLineInfoStringFormat = "x8";
        public static readonly string HexStringFormat = "x";

        public const long LARGE_FILE_LENGTH = 268435456L;
        public const int COPY_BLOCK_SIZE = 32768;
        public const int FIND_BLOCK_SIZE = 8192;
        
    }
}
