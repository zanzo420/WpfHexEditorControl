//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Windows.Data;
using System.Windows.Media;

namespace WPFHexaEditor.Core.Converters
{
    public class ValueToForegroundColorConverter: IValueConverter
    {
        private Color _color = Colors.Red;

        public Color ForeGroundColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);

            Double doubleValue = 0.0;

            if (value != null)
                Double.TryParse(value.ToString(), out doubleValue);
            
            if (doubleValue < 0)
                brush = new SolidColorBrush(_color);

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}