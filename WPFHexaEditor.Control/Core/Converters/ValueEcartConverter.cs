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
    /// Calcule l'ecart entre deux nombre et retorne la difference.
    /// Retourne un double
    /// Le nombre de digit est 2 par défaut
    /// </summary>
    public class ValueEcartConverter : IValueConverter
    {
        private int _baseValue = 0;
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

    
        [DefaultValue(0)]
        public int BaseValue
        {
            get
            {
                return _baseValue;
            }
            set
            {
                _baseValue = value;
            }
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = double.NaN;

            try
            {
                val = (double)value;

                return (_baseValue - val).Round(_precision);
            }
            catch
            {
                return null;
            }            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
