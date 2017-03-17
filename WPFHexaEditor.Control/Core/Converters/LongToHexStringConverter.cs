using System;
using System.Globalization;
using System.Windows.Data;

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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}