using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfHexaEditor.Core.Bytes;

namespace HexEditUnitTest
{
    [TestClass]
    public class ByteProviderTest
    {
        [TestMethod]
        public void GetByteCountTest()
        {
            var bp = new ByteProvider(@"C:\Test\TestFile.smc");

            var lenght = bp.Length;
            var byteCount = bp.GetByteCount().Sum();

            Assert.AreEqual(lenght, byteCount, "The number of byte need to be equal!");
        }
        

    }
}
