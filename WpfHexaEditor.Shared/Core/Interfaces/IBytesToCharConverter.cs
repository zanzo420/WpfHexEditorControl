//////////////////////////////////////////////
// Apache 2.0  - 2017
// Author       : Janus Tida
// Contributor  : Derek Tremblay
//////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace WpfHexaEditor.Core.Interfaces
{
    /// <summary>
    /// The instances that implement this interface maybe use in StringDataLayer and StringByteControl,aiming at varies of charsets.
    /// </summary>
    public interface IBytesToCharEncoding
    {
        char Convert(byte[] bytesToConvert);
        int BytePerChar { get; }
    }
}
