using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFHexaEditor.Core.Bytes;

namespace WPFHexaEditor.Core
{
    public class BookMark
    {
        ScrollMarker _marker = ScrollMarker.Nothing;
        ByteModified _byteModified = new ByteModified();

        public ScrollMarker Marker
        {
            get
            {
                return _marker;
            }

            set
            {
                _marker = value;
            }
        }

        public ByteModified ByteModified
        {
            get
            {
                return _byteModified;
            }

            set
            {
                _byteModified = value;
            }
        }
    }
}
