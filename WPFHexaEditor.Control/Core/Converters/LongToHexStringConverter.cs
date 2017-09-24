//////////////////////////////////////////////
// Apache 2.0  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFHexaEditor.Core.Converters
{
    /// <summary>
    /// Used to convert long value to hexadecimal string like this 0xFFFFFFFF.
    /// </summary>
    public sealed class LongToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defaultRtn = "0x00000000";

            if (value != null)
            {
                if (Int64.TryParse(value.ToString(), out var longValue))
                {
                    if (longValue > -1)
                        return "0x" + longValue.ToString(ConstantReadOnly.HexLineInfoStringFormat, CultureInfo.InvariantCulture).ToUpper();
                    return defaultRtn;
                }
                return defaultRtn;
            }
            return defaultRtn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}