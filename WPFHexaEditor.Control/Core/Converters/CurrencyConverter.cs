//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFHexaEditor.Core.Converters
{
    /// <summary>
    /// Convertir une string en dollars
    /// </summary>
    public class CurrencyConverter : IValueConverter
    {
        public enum Precision 
        {
            Digit_0,
            Digit_1,
            Digit_2,
            Digit_3,
            Digit_4
        }

        Precision _precision = Precision.Digit_2;

        public Precision precision
        {
            get
            {
                return _precision;
            }
            set
            {
                _precision = value;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (_precision)
            {
                case Precision.Digit_0:
                    return string.Format("{0:C0}", value);
                case Precision.Digit_1:
                    return string.Format("{0:C1}", value);
                case Precision.Digit_2:
                    return string.Format("{0:C2}", value);
                case Precision.Digit_3:
                    return string.Format("{0:C3}", value);
                case Precision.Digit_4:
                    return string.Format("{0:C4}", value);
                    //return string.Format("{0:C4}", value);
            }

            return string.Format("{0:C2}", value);
        }
                

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
