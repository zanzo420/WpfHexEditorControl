using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHexaEditor.Core.Bytes;
using WpfHexEditor.Sample.MVVM.Contracts.App;
using WpfHexEditor.Sample.MVVM.Contracts.Hex;

namespace WpfHexEditor.Sample.MVVM.Hex
{
    [Export(typeof(IBytesToCharEncoding))]
    class ASCIIBytesToCharEncoding : IBytesToCharEncoding {
        public int BytePerChar => 1;

        public string GUID => Constants.BytesToCharEncodingGUID_ASCII;

        public string EncodingName => LanguageService.FindResourceString(Constants.BytesToCharEncodingName_ASCII);

        public int Sort => 8;

        public char Convert(byte[] bytesToConvert) {
            if (bytesToConvert == null)
                throw new ArgumentNullException(nameof(bytesToConvert));

            if (bytesToConvert.Length != BytePerChar)
                throw new InvalidOperationException($"The length {nameof(bytesToConvert)} doesn't match {nameof(BytePerChar)}.");

            return ByteConverters.ByteToChar(bytesToConvert[0]);
        }
    }

    [Export(typeof(IBytesToCharEncoding))]
    class UTF8BytesToCharEncoding : IBytesToCharEncoding {
        public int BytePerChar => 3;

        public string GUID => Constants.BytesToCharEncodingGUID_UTF8;

        public string EncodingName => LanguageService.FindResourceString(Constants.BytesToCharEncodingName_UTF8);

        public int Sort => 16;

        public char Convert(byte[] bytesToConvert) {
            if (bytesToConvert == null) {
                throw new ArgumentNullException(nameof(bytesToConvert));
            }

            if (bytesToConvert.Length != BytePerChar)
                throw new InvalidOperationException($"The length {nameof(bytesToConvert)} doesn't match {nameof(BytePerChar)}.");

            return Encoding.UTF8.GetChars(bytesToConvert)[0];
        }
    }

    [Export(typeof(IBytesToCharEncoding))]
    class UTF16BytesToCharEncoding : IBytesToCharEncoding {
        public int BytePerChar => 4;

        public string GUID => Constants.BytesToCharEncodingGUID_UTF16;

        public string EncodingName => LanguageService.FindResourceString(Constants.BytesToCharEncodingName_UTF16);

        public int Sort => 24;

        public char Convert(byte[] bytesToConvert) {
            if (bytesToConvert == null) {
                throw new ArgumentNullException(nameof(bytesToConvert));
            }

            if (bytesToConvert.Length != BytePerChar)
                throw new InvalidOperationException($"The length {nameof(bytesToConvert)} doesn't match {nameof(BytePerChar)}.");

            return Encoding.Unicode.GetChars(bytesToConvert)[0];
        }
    }

    [Export(typeof(IBytesToCharEncoding))]
    class GBKBytesToCharEncoding : IBytesToCharEncoding {
        public int BytePerChar => 2;

        public string GUID => Constants.BytesToCharEncodingGUID_GBK;

        public string EncodingName => LanguageService.FindResourceString(Constants.BytesToCharEncodingName_GBK);

        public int Sort => 24;

        private readonly Encoding _encoding = Encoding.GetEncoding("GBK");
        public char Convert(byte[] bytesToConvert) {
            if (bytesToConvert == null) {
                throw new ArgumentNullException(nameof(bytesToConvert));
            }

            if (bytesToConvert.Length != BytePerChar)
                throw new InvalidOperationException($"The length {nameof(bytesToConvert)} doesn't match {nameof(BytePerChar)}.");

            return Encoding.Unicode.GetChars(bytesToConvert)[0];
        }
    }


}
