//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////


using System;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class DecimalExtension
    {
        public static decimal round(this decimal s, int digit)
        {
            return Math.Round(s, digit);
        }

    }
}
