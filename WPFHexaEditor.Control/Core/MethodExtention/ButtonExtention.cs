//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////


using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class ButtonExtention
    {
        /// <summary>
        /// Desactive un boutton et baisse son opacité a 50%
        /// </summary>
        /// <param name="button"></param>
        public static void DisableButton(this Button button)
        {
            button.Opacity = 0.5;
            button.IsEnabled = false;
        }

        /// <summary>
        /// Active un boutton et ajuste son opacité a 100%
        /// </summary>
        /// <param name="button"></param>
        public static void EnableButton(this Button button)
        {
            button.Opacity = 1;
            button.IsEnabled = true;
        }
    }


    public static class ToggleButtonExtention
    {
        /// <summary>
        /// Desactive un boutton et baisse son opacité a 50%
        /// </summary>
        /// <param name="button"></param>
        public static void DisableButton(this ToggleButton button)
        {
            button.Opacity = 0.5;
            button.IsEnabled = false;
        }

        /// <summary>
        /// Active un boutton et ajuste son opacité a 100%
        /// </summary>
        /// <param name="button"></param>
        public static void EnableButton(this ToggleButton button)
        {
            button.Opacity = 1;
            button.IsEnabled = true;
        }
    }
}
