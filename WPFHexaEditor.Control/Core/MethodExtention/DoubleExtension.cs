//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class DoubleExtension
    {
        public static double Round(this double s, int digit = 2)
        {
            return Math.Round(s, digit);
        }

    }
}
