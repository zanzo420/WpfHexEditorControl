//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////


using System.Windows.Controls;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class CheckBoxExtention
    {
        /// <summary>
        /// Desactive un checkbox et baisse son opacité a 50%
        /// </summary>
        /// <param name="checkbox"></param>
        public static void DisableCheckBox(this CheckBox checkbox)
        {
            checkbox.Opacity = 0.5;
            checkbox.IsEnabled = false;
        }

        /// <summary>
        /// Active un checkbox et ajuste son opacité a 100%
        /// </summary>
        /// <param name="checkbox"></param>
        public static void EnableCheckBox(this CheckBox checkbox)
        {
            checkbox.Opacity = 1;
            checkbox.IsEnabled = true;
        }
    }
}
