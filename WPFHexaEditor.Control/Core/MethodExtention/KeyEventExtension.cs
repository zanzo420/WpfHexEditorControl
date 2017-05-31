//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System.Windows.Input;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class KeyEventExtension
    {
        public static bool IsNumericKey(this KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                return true;
            else
                return false;
        }
    }
}
