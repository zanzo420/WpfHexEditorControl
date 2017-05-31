//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Globalization;
using System.Windows.Data;
using System.ComponentModel;
using WPFHexaEditor.Core.MethodExtention;

namespace WPFHexaEditor.Core.Converters
{

    /// <summary>
    /// Arrondir un nombre avec une quantité paramétrable de digits après la virgule.
    /// Retourne un double
    /// Le nombre de digit est 2 par défaut
    /// </summary>
    public class DecimalToDoubleRoundConverter : IValueConverter
    {
        private int _precision;

    
        [DefaultValue(2)]
        public int Precision
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
            double val = double.NaN;

            try
            {
                val = decimal.ToDouble((decimal)value);
            }
            catch { }


            if (val != double.NaN)
                return val.Round(_precision);
            else
                return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
