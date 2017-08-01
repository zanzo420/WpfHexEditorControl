using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFHexaEditor.Core;
using WPFHexaEditor.Core.Bytes;

namespace WPFHexaEditor.Control
{

    [TemplatePart(Name = FirstHexCharName, Type = typeof(TextBlock))]
    [TemplatePart(Name = SecondHexCharName, Type = typeof(TextBlock))]
    public partial class HexByteControl : System.Windows.Controls.Control
    {
        public const string FirstHexCharName = "FirstHexChar";
        public const string SecondHexCharName = "SecondHexChar";

        internal TextBlock FirstHexChar;
        internal TextBlock SecondHexChar;
        static HexByteControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexByteControl), new FrameworkPropertyMetadata(typeof(HexByteControl)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            FirstHexChar = GetTemplateChild(FirstHexCharName) as TextBlock;
            SecondHexChar = GetTemplateChild(SecondHexCharName) as TextBlock;
        }

        public HexByteControl(HexaEditor parent)
        {
            DataContext = this;
            KeyDown += UserControl_KeyDown;
            MouseDown += HexChar_MouseDown;
            MouseEnter += UserControl_MouseEnter;
            MouseLeave += UserControl_MouseLeave;
            Focusable = true;

            _parent = parent;
        }

        private bool _readOnlyMode = false;
        private KeyDownLabel _keyDownLabel = KeyDownLabel.FirstChar;
        private HexaEditor _parent;

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
        public event EventHandler CTRLZKey;
        public event EventHandler CTRLVKey;
        public event EventHandler CTRLCKey;
        public event EventHandler CTRLAKey;


        #region DependencyProperty

        /// <summary>
        /// Position in file
        /// </summary>
        public long BytePositionInFile
        {
            get { return (long)GetValue(BytePositionInFileProperty); }
            set { SetValue(BytePositionInFileProperty, value); }
        }

        public static readonly DependencyProperty BytePositionInFileProperty =
            DependencyProperty.Register("BytePositionInFile", typeof(long), typeof(HexByteControl), new PropertyMetadata(-1L));

        /// <summary>
        /// Action with this byte
        /// </summary>
        public ByteAction Action
        {
            get { return (ByteAction)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(ByteAction), typeof(HexByteControl),
                new FrameworkPropertyMetadata(ByteAction.Nothing,
                    new PropertyChangedCallback(Action_ValueChanged),
                    new CoerceValueCallback(Action_CoerceValue)));

        private static object Action_CoerceValue(DependencyObject d, object baseValue)
        {
            ByteAction value = (ByteAction)baseValue;

            if (value != ByteAction.All)
                return baseValue;
            else
                return ByteAction.Nothing;
        }

        private static void Action_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexByteControl ctrl = d as HexByteControl;

            if (e.NewValue != e.OldValue)
                ctrl.UpdateVisual();
        }

        /// <summary>
        /// Used for selection coloring
        /// </summary>
        public bool HexByteFirstSelected
        {
            get { return (bool)GetValue(HexByteFirstSelectedProperty); }
            set { SetValue(HexByteFirstSelectedProperty, value); }
        }

        public static readonly DependencyProperty HexByteFirstSelectedProperty =
            DependencyProperty.Register("HexByteFirstSelected", typeof(bool), typeof(HexByteControl), new PropertyMetadata(true));

        /// <summary>
        /// Byte used for this instance
        /// </summary>
        public byte? Byte
        {
            get { return (byte?)GetValue(ByteProperty); }
            set { SetValue(ByteProperty, value); }
        }

        public static readonly DependencyProperty ByteProperty =
            DependencyProperty.Register("Byte", typeof(byte?), typeof(HexByteControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Byte_PropertyChanged)));

        private static void Byte_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexByteControl ctrl = d as HexByteControl;

            if (e.NewValue != e.OldValue)
            {
                if (ctrl.Action != ByteAction.Nothing && ctrl.InternalChange == false)
                {
                    ctrl.ByteModified?.Invoke(ctrl, new EventArgs());
                }

                ctrl.UpdateLabelFromByte();
                ctrl.UpdateHexString();
            }
        }

        /// <summary>
        /// Used to prevent ByteModified event occurc when we dont want! 
        /// </summary>
        public bool InternalChange
        {
            get { return (bool)GetValue(InternalChangeProperty); }
            set { SetValue(InternalChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalChangeProperty =
            DependencyProperty.Register("InternalChange", typeof(bool), typeof(HexByteControl), new PropertyMetadata(false));

        #endregion

        public bool ReadOnlyMode
        {
            get
            {
                return _readOnlyMode;
            }
            set
            {
                _readOnlyMode = value;
            }
        }

        /// <summary>
        /// Get the hex string representation of this byte
        /// </summary>
        public string HexString
        {
            get { return (string)GetValue(HexStringProperty); }
            internal set { SetValue(HexStringProperty, value); }
        }

        public static readonly DependencyProperty HexStringProperty =
            DependencyProperty.Register("HexString", typeof(string), typeof(HexByteControl),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Get or Set if control as selected
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(HexByteControl),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(IsSelected_PropertyChange)));


        private static void IsSelected_PropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexByteControl ctrl = d as HexByteControl;

            if (e.NewValue != e.OldValue)
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
            get { return (bool)GetValue(IsHighLightProperty); }
            set { SetValue(IsHighLightProperty, value); }
        }

        public static readonly DependencyProperty IsHighLightProperty =
            DependencyProperty.Register("IsHighLight", typeof(bool), typeof(HexByteControl),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(IsHighLight_PropertyChanged)));

        private static void IsHighLight_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexByteControl ctrl = d as HexByteControl;

            if (e.NewValue != e.OldValue)
            {
                ctrl._keyDownLabel = KeyDownLabel.FirstChar;
                ctrl.UpdateVisual();
            }
        }

        public bool IsFocus
        {
            get { return (bool)GetValue(IsFocusProperty); }
            set { SetValue(IsFocusProperty, value); }
        }

        public static readonly DependencyProperty IsFocusProperty =
            DependencyProperty.Register("IsFocus", typeof(bool), typeof(HexByteControl),
                new FrameworkPropertyMetadata(false, IsFocus_PropertyChanged));

        private static void IsFocus_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexByteControl ctrl = d as HexByteControl;
            if (e.NewValue != e.OldValue)
            {
                ctrl._keyDownLabel = KeyDownLabel.FirstChar;
                ctrl.UpdateVisual();
            }
        }

        /// <summary>
        /// Update Background,foreground and font property
        /// </summary>
        internal void UpdateVisual()
        {
            if (IsFocus)
            {
                FirstHexChar.Foreground = Brushes.White;
                SecondHexChar.Foreground = Brushes.White;
                Background = Brushes.Black;
            }
            else if (IsSelected)
            {
                FontWeight = _parent.FontWeight;
                FirstHexChar.Foreground = _parent.ForegroundContrast;
                SecondHexChar.Foreground = _parent.ForegroundContrast;

                if (HexByteFirstSelected)
                    Background = _parent.SelectionFirstColor;
                else
                    Background = _parent.SelectionSecondColor;
            }
            else if (IsHighLight)
            {
                FontWeight = _parent.FontWeight;
                FirstHexChar.Foreground = _parent.Foreground;
                SecondHexChar.Foreground = _parent.Foreground;

                Background = _parent.HighLightColor;
            }
            else if (Action != ByteAction.Nothing)
            {
                switch (Action)
                {
                    case ByteAction.Modified:
                        FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        Background = _parent.ByteModifiedColor;
                        FirstHexChar.Foreground = _parent.Foreground;
                        SecondHexChar.Foreground = _parent.Foreground;
                        break;

                    case ByteAction.Deleted:
                        FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        Background = _parent.ByteDeletedColor;
                        FirstHexChar.Foreground = _parent.Foreground;
                        SecondHexChar.Foreground = _parent.Foreground;
                        break;
                }
            }
            else
            {
                FontWeight = _parent.FontWeight;
                Background = Brushes.Transparent;
                FirstHexChar.Foreground = _parent.Foreground;
                SecondHexChar.Foreground = _parent.Foreground;
            }
        }

        private void UpdateLabelFromByte()
        {
            if (Byte != null)
            {
                string hexabyte = ByteConverters.ByteToHex(Byte.Value);

                FirstHexChar.Text = hexabyte.Substring(0, 1);
                SecondHexChar.Text = hexabyte.Substring(1, 1);
            }
            else
            {
                FirstHexChar.Text = "";
                SecondHexChar.Text = "";
            }
        }

        private void HexChar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Focus();
                Click?.Invoke(this, e);
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                RightClick?.Invoke(this, e);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyValidator.IsUpKey(e.Key))
            {
                e.Handled = true;
                MoveUp?.Invoke(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsDownKey(e.Key))
            {
                e.Handled = true;
                MoveDown?.Invoke(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsLeftKey(e.Key))
            {
                e.Handled = true;
                MoveLeft?.Invoke(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsRightKey(e.Key))
            {
                e.Handled = true;
                MoveRight?.Invoke(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsPageDownKey(e.Key))
            {
                e.Handled = true;
                MovePageDown?.Invoke(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsPageUpKey(e.Key))
            {
                e.Handled = true;
                MovePageUp?.Invoke(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsDeleteKey(e.Key))
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
                CTRLZKey?.Invoke(this, new EventArgs());
                return;
            }
            else if (KeyValidator.IsCtrlVKey(e.Key))
            {
                e.Handled = true;
                CTRLVKey?.Invoke(this, new EventArgs());
                return;
            }
            else if (KeyValidator.IsCtrlCKey(e.Key))
            {
                e.Handled = true;
                CTRLCKey?.Invoke(this, new EventArgs());
                return;
            }
            else if (KeyValidator.IsCtrlAKey(e.Key))
            {
                e.Handled = true;
                CTRLAKey?.Invoke(this, new EventArgs());
                return;
            }

            //MODIFY BYTE
            if (!ReadOnlyMode)
                if (KeyValidator.IsHexKey(e.Key))
                {
                    string key;
                    if (KeyValidator.IsNumericKey(e.Key))
                        key = KeyValidator.GetDigitFromKey(e.Key).ToString();
                    else
                        key = e.Key.ToString().ToLower();

                    switch (_keyDownLabel)
                    {
                        case KeyDownLabel.FirstChar:
                            FirstHexChar.Text = key;
                            _keyDownLabel = KeyDownLabel.SecondChar;
                            Action = ByteAction.Modified;
                            Byte = ByteConverters.HexToByte(FirstHexChar.Text.ToString() + SecondHexChar.Text.ToString())[0];
                            break;
                        case KeyDownLabel.SecondChar:
                            SecondHexChar.Text = key;
                            _keyDownLabel = KeyDownLabel.NextPosition;

                            Action = ByteAction.Modified;
                            Byte = ByteConverters.HexToByte(FirstHexChar.Text.ToString() + SecondHexChar.Text.ToString())[0];

                            //Move focus event
                            MoveNext?.Invoke(this, new EventArgs());
                            break;
                    }
                }
        }

        private void UpdateHexString()
        {
            HexString = ((string)FirstHexChar.Text + (string)SecondHexChar.Text); //.ToString();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (Action != ByteAction.Modified &&
                    Action != ByteAction.Deleted &&
                    Action != ByteAction.Added &&
                    !IsSelected && !IsHighLight && !IsFocus)
                    Background = _parent.MouseOverColor;

            if (e.LeftButton == MouseButtonState.Pressed)
                MouseSelection?.Invoke(this, e);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (Action != ByteAction.Modified &&
                    Action != ByteAction.Deleted &&
                    Action != ByteAction.Added &&
                    !IsSelected && !IsHighLight && !IsFocus)
                    Background = Brushes.Transparent;
        }

    }
}
