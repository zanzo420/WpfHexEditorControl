//////////////////////////////////////////////
// Apache 2.0  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
// Contributor: Janus Tida
//////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using WpfHexaEditor.Core;
using WpfHexaEditor.Core.Bytes;
using WpfHexaEditor.Core.Interfaces;

namespace WpfHexaEditor
{
    internal class HexByte : TextBlock, IByteControl
    {

        //global class variables
        private KeyDownLabel _keyDownLabel = KeyDownLabel.FirstChar;
        private readonly HexEditor _parent;

        //Events
        public event EventHandler ByteModified;
        public event EventHandler MouseSelection;
        public event EventHandler Click;
        public event EventHandler RightClick;
        public event EventHandler MoveNext;
        public event EventHandler MovePrevious;
        public event EventHandler MoveRight;
        public event EventHandler MoveLeft;
        public event EventHandler MoveUp;
        public event EventHandler MoveDown;
        public event EventHandler MovePageDown;
        public event EventHandler MovePageUp;
        public event EventHandler ByteDeleted;
        public event EventHandler EscapeKey;
        public event EventHandler CtrlzKey;
        public event EventHandler CtrlvKey;
        public event EventHandler CtrlcKey;
        public event EventHandler CtrlaKey;

        public HexByte(HexEditor parent)
        {
            //Parent hexeditor
            _parent = parent;

            //Default properties
            DataContext = this;
            Focusable = true;
            TextAlignment = TextAlignment.Left;
            Padding = new Thickness(2, 0, 0, 0);

            #region Binding tooltip
            LoadDictionary("/WPFHexaEditor;component/Resources/Dictionary/ToolTipDictionary.xaml");
            var txtBinding = new Binding
            {
                Source = FindResource("ByteToolTip"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };

            // Load ressources dictionnary
            void LoadDictionary(string url)
            {
                var ttRes = new ResourceDictionary { Source = new Uri(url, UriKind.Relative) };
                Resources.MergedDictionaries.Add(ttRes);
            }

            SetBinding(ToolTipProperty, txtBinding);
            #endregion
                        
            //Event
            KeyDown += UserControl_KeyDown;
            MouseDown += HexChar_MouseDown;
            MouseEnter += UserControl_MouseEnter;
            MouseLeave += UserControl_MouseLeave;
            ToolTipOpening += UserControl_ToolTipOpening;
            
            //Update width
            UpdateDataVisualWidth();
        }



        #region DependencyProperty

        /// <summary>
        /// Position in file
        /// </summary>
        //public long BytePositionInFile
        //{
        //    get => (long)GetValue(BytePositionInFileProperty);
        //    set => SetValue(BytePositionInFileProperty, value);
        //}

        //public static readonly DependencyProperty BytePositionInFileProperty =
        //    DependencyProperty.Register(nameof(BytePositionInFile), typeof(long), typeof(HexByte), new PropertyMetadata(-1L));
        public long BytePositionInFile { get; set; } = -1L;

        /// <summary>
        /// Action with this byte
        /// </summary>
        public ByteAction Action
        {
            get => (ByteAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register(nameof(Action), typeof(ByteAction), typeof(HexByte),
                new FrameworkPropertyMetadata(ByteAction.Nothing,
                    Action_ValueChanged,
                    Action_CoerceValue));

        private static object Action_CoerceValue(DependencyObject d, object baseValue)
        {
            var value = (ByteAction)baseValue;

            return value != ByteAction.All ? baseValue : ByteAction.Nothing;
        }

        private static void Action_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HexByte ctrl && e.NewValue != e.OldValue) ctrl.UpdateVisual();
        }

        /// <summary>
        /// Used for selection coloring
        /// </summary>
        //public bool FirstSelected
        //{
        //    get => (bool)GetValue(FirstSelectedProperty);
        //    set => SetValue(FirstSelectedProperty, value);
        //}

        //public static readonly DependencyProperty FirstSelectedProperty =
        //    DependencyProperty.Register(nameof(FirstSelected), typeof(bool), typeof(HexByte), new PropertyMetadata(true));
        public bool FirstSelected { get; set; } = false;

        /// <summary>
        /// Byte used for this instance
        /// </summary>
        public byte? Byte
        {
            get => (byte?)GetValue(ByteProperty);
            set => SetValue(ByteProperty, value);
        }

        public static readonly DependencyProperty ByteProperty =
            DependencyProperty.Register(nameof(Byte), typeof(byte?), typeof(HexByte),
                new FrameworkPropertyMetadata(null, Byte_PropertyChanged));

        private static void Byte_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HexByte ctrl)
                if (e.NewValue != null)
                {
                    if (e.NewValue != e.OldValue)
                    {
                        if (ctrl.Action != ByteAction.Nothing && ctrl.InternalChange == false)
                            ctrl.ByteModified?.Invoke(ctrl, new EventArgs());

                        ctrl.UpdateLabelFromByte();
                    }
                }
                else
                    ctrl.UpdateLabelFromByte();
        }

        /// <summary>
        /// Used to prevent ByteModified event occurc when we dont want! 
        /// </summary>
        //public bool InternalChange
        //{
        //    get => (bool)GetValue(InternalChangeProperty);
        //    set => SetValue(InternalChangeProperty, value);
        //}

        //// Using a DependencyProperty as the backing store for InternalChange.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty InternalChangeProperty =
        //    DependencyProperty.Register(nameof(InternalChange), typeof(bool), typeof(HexByte), new PropertyMetadata(false));

        public bool InternalChange { get; set; } = false;
        #endregion

        /// <summary>
        /// Get or set if control as in read only mode
        /// </summary>
        public bool ReadOnlyMode { get; set; } = false;

        /// <summary>
        /// Get or Set if control as selected
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(HexByte),
                new FrameworkPropertyMetadata(false,
                    IsSelected_PropertyChange));


        private static void IsSelected_PropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HexByte ctrl && e.NewValue != e.OldValue)
            {
                ctrl._keyDownLabel = KeyDownLabel.FirstChar;
                ctrl.UpdateVisual();
            }
        }

        /// <summary>
        /// Get of Set if control as marked as highlighted
        /// </summary>                        
        public bool IsHighLight
        {
            get => (bool)GetValue(IsHighLightProperty);
            set => SetValue(IsHighLightProperty, value);
        }

        public static readonly DependencyProperty IsHighLightProperty =
            DependencyProperty.Register(nameof(IsHighLight), typeof(bool), typeof(HexByte),
                new FrameworkPropertyMetadata(false,
                    IsHighLight_PropertyChanged));

        private static void IsHighLight_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HexByte ctrl && e.NewValue != e.OldValue)
            {
                ctrl._keyDownLabel = KeyDownLabel.FirstChar;
                ctrl.UpdateVisual();
            }
        }

        /// <summary>
        /// Update Background,foreground and font property
        /// </summary>
        public void UpdateVisual()
        {
            FontFamily = _parent.FontFamily;

            if (IsSelected)
            {
                FontWeight = _parent.FontWeight;
                Foreground = _parent.ForegroundContrast;

                Background = FirstSelected ? _parent.SelectionFirstColor : _parent.SelectionSecondColor;
            }
            else if (IsHighLight)
            {
                FontWeight = _parent.FontWeight;
                Foreground = _parent.Foreground;
                Background = _parent.HighLightColor;
            }
            else if (Action != ByteAction.Nothing)
            {
                FontWeight = FontWeights.Bold;
                Foreground = _parent.Foreground;

                switch (Action)
                {
                    case ByteAction.Modified:
                        Background = _parent.ByteModifiedColor;
                        break;
                    case ByteAction.Deleted:
                        Background = _parent.ByteDeletedColor;
                        break;
                }
            }
            else
            {
                FontWeight = _parent.FontWeight;
                Background = Brushes.Transparent;
                Foreground = _parent.Foreground;
            }

            UpdateAutoHighLiteSelectionByteVisual();
        }

        private void UpdateAutoHighLiteSelectionByteVisual()
        {
            //Auto highlite selectionbyte
            if (_parent.AllowAutoHightLighSelectionByte && _parent.SelectionByte != null && Byte == _parent.SelectionByte && !IsSelected)
                Background = _parent.AutoHighLiteSelectionByteBrush;
        }


        internal void UpdateLabelFromByte()
        {
            if (Byte != null)
            {
                switch (_parent.DataStringVisual)
                {
                    case DataVisualType.Hexadecimal:
                        var chArr = ByteConverters.ByteToHexCharArray(Byte.Value);
                        Text = new string(chArr);
                        break;
                    case DataVisualType.Decimal:                        
                        Text = Byte.Value.ToString("d3");
                        break;
                }                
            }
            else            
                Text = string.Empty;            
        }

        private void HexChar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Focus();
                Click?.Invoke(this, e);
            }

            if (e.RightButton == MouseButtonState.Pressed)            
                RightClick?.Invoke(this, e);            
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            #region Key validation and launch event if needed
            if (KeyValidator.IsUpKey(e.Key))
            {
                e.Handled = true;
                MoveUp?.Invoke(this, new EventArgs());

                return;
            }
            if (KeyValidator.IsDownKey(e.Key))
            {
                e.Handled = true;
                MoveDown?.Invoke(this, new EventArgs());

                return;
            }
            if (KeyValidator.IsLeftKey(e.Key))
            {
                e.Handled = true;
                MoveLeft?.Invoke(this, new EventArgs());

                return;
            }
            if (KeyValidator.IsRightKey(e.Key))
            {
                e.Handled = true;
                MoveRight?.Invoke(this, new EventArgs());

                return;
            }
            if (KeyValidator.IsPageDownKey(e.Key))
            {
                e.Handled = true;
                MovePageDown?.Invoke(this, new EventArgs());

                return;
            }
            if (KeyValidator.IsPageUpKey(e.Key))
            {
                e.Handled = true;
                MovePageUp?.Invoke(this, new EventArgs());

                return;
            }
            if (KeyValidator.IsDeleteKey(e.Key))
            {
                if (!ReadOnlyMode)
                {
                    e.Handled = true;
                    ByteDeleted?.Invoke(this, new EventArgs());

                    return;
                }
            }
            else if (KeyValidator.IsBackspaceKey(e.Key))
            {
                e.Handled = true;
                ByteDeleted?.Invoke(this, new EventArgs());

                MovePrevious?.Invoke(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsEscapeKey(e.Key))
            {
                e.Handled = true;
                EscapeKey?.Invoke(this, new EventArgs());
                return;
            }
            else if (KeyValidator.IsCtrlZKey(e.Key))
            {
                e.Handled = true;
                CtrlzKey?.Invoke(this, new EventArgs());
                return;
            }
            else if (KeyValidator.IsCtrlVKey(e.Key))
            {
                e.Handled = true;
                CtrlvKey?.Invoke(this, new EventArgs());
                return;
            }
            else if (KeyValidator.IsCtrlCKey(e.Key))
            {
                e.Handled = true;
                CtrlcKey?.Invoke(this, new EventArgs());
                return;
            }
            else if (KeyValidator.IsCtrlAKey(e.Key))
            {
                e.Handled = true;
                CtrlaKey?.Invoke(this, new EventArgs());
                return;
            }

            #endregion

            //MODIFY BYTE
            if (!ReadOnlyMode && KeyValidator.IsHexKey(e.Key))
                switch (_parent.DataStringVisual)
                {
                    case DataVisualType.Hexadecimal:

                        #region Edit hexadecimal value 

                        string key;
                        key = KeyValidator.IsNumericKey(e.Key) ? KeyValidator.GetDigitFromKey(e.Key).ToString() : e.Key.ToString().ToLower();

                        //Update byte
                        var byteValueCharArray = ByteConverters.ByteToHexCharArray(Byte.Value);
                        switch (_keyDownLabel)
                        {
                            case KeyDownLabel.FirstChar:
                                byteValueCharArray[0] = key.ToCharArray()[0];
                                _keyDownLabel = KeyDownLabel.SecondChar;
                                Action = ByteAction.Modified;
                                Byte = ByteConverters.HexToByte(
                                    byteValueCharArray[0] + byteValueCharArray[1].ToString())[0];
                                break;
                            case KeyDownLabel.SecondChar:
                                byteValueCharArray[1] = key.ToCharArray()[0];
                                _keyDownLabel = KeyDownLabel.NextPosition;

                                Action = ByteAction.Modified;
                                Byte = ByteConverters.HexToByte(
                                    byteValueCharArray[0] + byteValueCharArray[1].ToString())[0];

                                //Move focus event
                                MoveNext?.Invoke(this, new EventArgs());
                                break;
                        }

                        #endregion

                        break;
                    case DataVisualType.Decimal:

                        //Not editable at this moment, maybe in future

                        break;
                }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Byte != null && Action != ByteAction.Modified && Action != ByteAction.Deleted && Action != ByteAction.Added && !IsSelected && !IsHighLight)
                Background = _parent.MouseOverColor;

            UpdateAutoHighLiteSelectionByteVisual();

            if (e.LeftButton == MouseButtonState.Pressed)
                MouseSelection?.Invoke(this, e);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Byte != null && Action != ByteAction.Modified && Action != ByteAction.Deleted && Action != ByteAction.Added && !IsSelected && !IsHighLight)
                Background = Brushes.Transparent;

            UpdateAutoHighLiteSelectionByteVisual();
        }

        private void UserControl_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (Byte == null)
                e.Handled = true;
        }

        /// <summary>
        /// Clear control
        /// </summary>
        public void Clear()
        {
            BytePositionInFile = -1;
            Byte = null;
            Action = ByteAction.Nothing;
            IsHighLight = false;
            IsSelected = false;
        }

        public void UpdateDataVisualWidth()
        {
            switch (_parent.DataStringVisual)
            {
                case DataVisualType.Decimal:
                    Width = 25;
                    break;
                case DataVisualType.Hexadecimal:
                    Width = 20;
                    break;
            }
        }
    }
}
