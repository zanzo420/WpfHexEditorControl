using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfHexaEditor
{
   public static class ToolTipExtension
    {
        private static Dictionary<FrameworkElement, Popup> _toolTipDics =
            new Dictionary<FrameworkElement, Popup>();
        
        public static UIElement GetOperatableToolTip(DependencyObject obj) {
            return (UIElement)obj.GetValue(OperatableToolTipProperty);
        }

        public static void SetOperatableToolTip(DependencyObject obj, FrameworkElement value) {
            obj.SetValue(OperatableToolTipProperty, value);
        }

        public static void SetToolTipOpen(this FrameworkElement elem,bool open,Point? point = null) {
            if (!_toolTipDics.ContainsKey(elem)) {
                throw new InvalidOperationException($"{nameof(_toolTipDics)} doesn't contain the {nameof(elem)}.");
            }

            if(point != null) {
                _toolTipDics[elem].VerticalOffset = point.Value.Y;
                _toolTipDics[elem].HorizontalOffset = point.Value.X;
            }
            
            _toolTipDics[elem].IsOpen = open;
        }
        
        // Using a DependencyProperty as the backing store for OperatableTool.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OperatableToolTipProperty =
            DependencyProperty.RegisterAttached("OperatableToolTip", typeof(FrameworkElement),
                typeof(ToolTipExtension), new PropertyMetadata(null, OperatableToolProperty_Changed));

        private static void OperatableToolProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is FrameworkElement elem)) return;

            if (_toolTipDics.ContainsKey(elem)) {
                _toolTipDics.Remove(elem);
            }

            if (!(e.NewValue is UIElement newElem)) return;

            var toolPop = new Popup {
                Child = newElem,
                PopupAnimation = PopupAnimation.Fade
            };

            toolPop.PlacementTarget = elem;
            toolPop.Placement = PlacementMode.Relative;
            toolPop.MouseLeave += Popup_MouseLeave;

            _toolTipDics.Add(elem, toolPop);
            
            elem.MouseDown  += FrameworkElem_MouseDown;
            elem.MouseUp    += FrameworkElem_MouseUp;
            elem.MouseEnter  += FrameworkElem_MouseEnter;
            elem.MouseLeave += FrameworkElem_MouseLeave;
            elem.Unloaded   += FrameworkElem_Unload;
            
            toolPop.SetBinding(FrameworkElement.DataContextProperty,
                new Binding(nameof(FrameworkElement.DataContext)) {
                    Source = elem
                }
            );
        }

        private static void FrameworkElem_MouseDown(object sender, MouseButtonEventArgs e) {
            if (!(sender is FrameworkElement elem)) return;

            if (!_toolTipDics.ContainsKey(elem)) return;

            if (GetAutoHide(elem)) {
                SetToolTipOpen(elem, false);
            }
        }

        private static void FrameworkElem_MouseUp(object sender, MouseButtonEventArgs e) {
            //if (!(sender is FrameworkElement elem)) return;

            //if (!_toolTipDics.ContainsKey(elem)) return;

            //var pop = _toolTipDics[elem];
            //_toolTipDics[elem] = pop;
        }

        private static void FrameworkElem_MouseLeave(object sender, MouseEventArgs e) {
            if (!(sender is FrameworkElement elem)) return;

            if (!_toolTipDics.ContainsKey(elem)) return;

            var pop = _toolTipDics[elem];

            if (pop.IsMouseOver) {
                return;
            }

            if (GetAutoHide(elem)) {
                SetToolTipOpen(elem, false);
            }
            
            _toolTipDics[elem] = pop;
        }

        private static void FrameworkElem_MouseEnter(object sender,MouseEventArgs e) {
            if (!(sender is FrameworkElement elem)) return;

            if (!_toolTipDics.ContainsKey(elem)) return;
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                return;
            }

            try {    
                var position = Mouse.GetPosition(elem);
                _toolTipDics[elem].VerticalOffset = position.Y;
                _toolTipDics[elem].HorizontalOffset = position.X;

                if (GetAutoShow(elem)) {
                    SetToolTipOpen(elem, true);
                }
            }
            catch(Exception ex) {
                throw;
            }
        }

        private static void FrameworkElem_Unload(object sender, RoutedEventArgs e) {
            if (!(sender is FrameworkElement elem)) return;

            elem.MouseDown  -= FrameworkElem_MouseDown;
            elem.MouseUp    -= FrameworkElem_MouseUp;
            elem.MouseEnter  -= FrameworkElem_MouseEnter;
            elem.MouseLeave -= FrameworkElem_MouseLeave;
            elem.Unloaded   -= FrameworkElem_Unload;

            if (_toolTipDics.ContainsKey(elem)) {
                _toolTipDics[elem].MouseLeave -= Popup_MouseLeave;
                _toolTipDics.Remove(elem);
            }
        }

        private static void Popup_MouseLeave(object sender, MouseEventArgs e) {
            if (!(sender is Popup pop)) return;

            foreach (var dic in _toolTipDics) {
                if(dic.Value == pop) {
                    if((pop.Child as FrameworkElement)?.ContextMenu?.IsOpen ?? false) {
                        return;
                    }
                    SetToolTipOpen(dic.Key, false);
                    break;
                }
            }
            
            
            
        }

        //This dp show the popup while the mouse entering the targetElem if set to true;
        public static bool GetAutoShow(DependencyObject obj) {
            return (bool)obj.GetValue(AutoShowProperty);
        }

        public static void SetAutoShow(DependencyObject obj, bool value) {
            obj.SetValue(AutoShowProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoShow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoShowProperty =
            DependencyProperty.RegisterAttached("AutoShow", typeof(bool), typeof(ToolTipExtension), new PropertyMetadata(true));


        //This dp hide the popup while the mouse leaving the targetElem if set to true;
        public static bool GetAutoHide(DependencyObject obj) {
            return (bool)obj.GetValue(AutoHideProperty);
        }

        public static void SetAutoHide(DependencyObject obj, bool value) {
            obj.SetValue(AutoHideProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoHide.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoHideProperty =
            DependencyProperty.RegisterAttached("AutoHide", typeof(bool), typeof(ToolTipExtension), new PropertyMetadata(true));


    } 

    //Simplified tooltip that loaded in visual tree.
    //Plz make sure the targetElem and tooltip are in the same container;
    //public class OperatableToolTip : Popup {
    //    public OperatableToolTip() {
    //        this.HorizontalAlignment = HorizontalAlignment.Left;
    //        this.VerticalAlignment = VerticalAlignment.Top;
            
    //    }

        
    //    //public static readonly DependencyProperty PlacementTargetProperty =
    //    //        DependencyProperty.Register(
    //    //                nameof(PlacementTarget),
    //    //                typeof(FrameworkElement),
    //    //                typeof(OperatableToolTip),
    //    //                new FrameworkPropertyMetadata(
    //    //                    null,
    //    //                    OnPlacementTargetChanged));

    //    ///// <summary>
    //    ///// The FrameworkElement relative to which the Popup will be displayed. If PlacementTarget is null (which
    //    ///// it is by default), the Popup is displayed relative to its visual parent.
    //    ///// </summary>
    //    //[Bindable(true), Category("Layout")]
    //    //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    //    //public FrameworkElement PlacementTarget {
    //    //    get { return (FrameworkElement)GetValue(PlacementTargetProperty); }
    //    //    set { SetValue(PlacementTargetProperty, value); }
    //    //}

    //    //private static void OnPlacementTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
    //    //    if (!(d is OperatableToolTip tTip)) return;

    //    //    if (e.OldValue is FrameworkElement oldElem) {
    //    //        oldElem.MouseDown  -= tTip.Target_MouseDown;
    //    //        oldElem.MouseLeave -= tTip.Target_MouseLeave;
    //    //        oldElem.MouseMove  -= tTip.Target_MouseMove;
    //    //        oldElem.MouseUp    -= tTip.Target_MouseUp;
    //    //        oldElem.MouseEnter -= tTip.Target_MouseEnter;
    //    //    }
            
    //    //    if (e.NewValue is FrameworkElement newElem) {
    //    //        newElem.MouseDown  += tTip.Target_MouseDown;
    //    //        newElem.MouseLeave += tTip.Target_MouseLeave;
    //    //        newElem.MouseMove  += tTip.Target_MouseMove;
    //    //        newElem.MouseUp    += tTip.Target_MouseUp;
    //    //        newElem.MouseEnter += tTip.Target_MouseEnter;
    //    //    }

    //    //}

    //    private bool _targetPressing;
    //    public bool TargetPressing => _targetPressing;

    //    private void Target_MouseDown(object d, MouseButtonEventArgs e) {
    //        _targetPressing = true;
    //        this.Visibility = Visibility.Hidden;
    //    }
        
    //    private void Target_MouseMove(object d, MouseEventArgs e) {
            
    //        if(PlacementTarget != null) {
    //            var position = Mouse.GetPosition(PlacementTarget);
    //            this.Margin = new Thickness(position.X, position.Y, 0, 0);
    //            this.Visibility = Visibility.Visible;
    //        }
            
    //    }

    //    private void Target_MouseEnter(object d, MouseEventArgs e) {
    //        if (e.LeftButton != MouseButtonState.Pressed) {
    //            _targetPressing = false;
    //        }
    //    }

    //    private void Target_MouseLeave(object d, MouseEventArgs e) {
    //        if (!this.IsMouseOver) {
    //            _targetPressing = false;
    //            this.Visibility = Visibility.Hidden;
    //        }
    //    }

    //    private void Target_MouseUp(object d, MouseButtonEventArgs e) {
    //        _targetPressing = false;
    //    }
    //}
    
}
