using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

using WPFHexaEditor.Core.Bytes;

namespace WPFHexaEditor.Core.Converters
{
    /// <summary>
    /// Used to convert long value to hexadecimal string like this 0xFFFFFFFF.
    /// </summary>
    public class LongToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var LongValue = Int32.Parse(value.ToString());

            return "0x" + LongValue.ToString(ConstantReadOnly.HexLineInfoStringFormat, CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType,  object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
