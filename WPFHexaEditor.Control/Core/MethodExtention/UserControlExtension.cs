//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class UserControlExtension
    {
        /// <summary>
        /// Déplace le focus sur le prochain control vers la droite dans l'ordre de tabulation
        /// </summary>        
        public static void MoveFocusNext(this UserControl ctrl)
        {
            TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

            if (keyboardFocus != null)
                keyboardFocus.MoveFocus(tRequest);                        
        }

        /// <summary>
        /// Déplace le focus sur le prochain control vers la gauche dans l'ordre de tabulation
        /// </summary>        
        public static void MoveFocusPrevious(this UserControl ctrl)
        {
            TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Previous);
            UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

            if (keyboardFocus != null)
                keyboardFocus.MoveFocus(tRequest);
        }
    }
}
