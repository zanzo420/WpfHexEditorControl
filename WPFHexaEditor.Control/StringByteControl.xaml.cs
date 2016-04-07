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
    /// <summary>
    /// Interaction logic for StringByteControl.xaml
    /// </summary>
    public partial class StringByteControl : UserControl
    {
        //private bool _isByteModified = false;
        private bool _readOnlyMode;

        public event EventHandler Click;
        public event EventHandler MouseSelection;
        public event EventHandler StringByteModified;
        public event EventHandler MoveNext;
        public event EventHandler MovePrevious;
        public event EventHandler MoveRight;
        public event EventHandler MoveLeft;
        public event EventHandler MoveUp;
        public event EventHandler MoveDown;
        public event EventHandler MovePageDown;
        public event EventHandler MovePageUp;
        public event EventHandler ByteDeleted;

        public StringByteControl()
        {
            InitializeComponent();

            DataContext = this;
        }

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
            DependencyProperty.Register("BytePositionInFile", typeof(long), typeof(StringByteControl), new PropertyMetadata(-1L));

        /// <summary>
        /// Used for selection coloring
        /// </summary>
        public bool StringByteFirstSelected
        {
            get { return (bool)GetValue(StringByteFirstSelectedProperty); }
            set { SetValue(StringByteFirstSelectedProperty, value); }
        }

        public static readonly DependencyProperty StringByteFirstSelectedProperty =
            DependencyProperty.Register("StringByteFirstSelected", typeof(bool), typeof(StringByteControl), new PropertyMetadata(true));

        /// <summary>
        /// Byte used for this instance
        /// </summary>
        public byte? Byte
        {
            get { return (byte?)GetValue(ByteProperty); }
            set { SetValue(ByteProperty, value); }
        }

        public static readonly DependencyProperty ByteProperty =
            DependencyProperty.Register("Byte", typeof(byte?), typeof(StringByteControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Byte_PropertyChangedCallBack)));

        private static void Byte_PropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringByteControl ctrl = d as StringByteControl;

            if (ctrl.Action != ByteAction.Nothing && ctrl.InternalChange == false)
                if (ctrl.StringByteModified != null)
                    ctrl.StringByteModified(ctrl, new EventArgs());

            ctrl.UpdateLabelFromByte();
        }
        
        /// <summary>
        /// Get or set if control as selected
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(StringByteControl), 
                new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsSelected_PropertyChangedCallBack)));

        private static void IsSelected_PropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringByteControl ctrl = d as StringByteControl;

            ctrl.UpdateBackGround();
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
            DependencyProperty.Register("IsHighLight", typeof(bool), typeof(StringByteControl),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(IsHighLight_PropertyChanged)));

        private static void IsHighLight_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringByteControl ctrl = d as StringByteControl;

            ctrl.UpdateBackGround();
        }

        /// <summary>
        /// Used to prevent StringByteModified event occurc when we dont want! 
        /// </summary>
        public bool InternalChange
        {
            get { return (bool)GetValue(InternalChangeProperty); }
            set { SetValue(InternalChangeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalChangeProperty =
            DependencyProperty.Register("InternalChange", typeof(bool), typeof(StringByteControl), new PropertyMetadata(false));
                
        /// <summary>
        /// Action with this byte
        /// </summary>
        public ByteAction Action
        {
            get { return (ByteAction)GetValue(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }
                
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register("Action", typeof(ByteAction), typeof(StringByteControl),
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
            StringByteControl ctrl = d as StringByteControl;

            ctrl.UpdateBackGround();
        }


        #endregion

        #region Standard property
        /// <summary>
        /// Get the hex string {00} representation of this byte
        /// </summary>
        public string HexString
        {
            get
            {
                if (Byte != null)
                    return ByteConverters.ByteToHex(Byte.Value);
                else
                    return "";
            }
        }
        #endregion

        /// <summary>
        /// Update control label from byte property
        /// </summary>
        private void UpdateLabelFromByte()
        {
            if (Byte != null)
            {
                StringByteLabel.Content = ByteConverters.ByteToChar(Byte.Value);
            }
            else
            {
                StringByteLabel.Content = "";
            }
        }
        
        /// <summary>
        /// Update Background
        /// </summary>
        private void UpdateBackGround()
        {

            if (IsSelected)
            {
                this.FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                StringByteLabel.Foreground = Brushes.White;

                if (StringByteFirstSelected)
                    this.Background = (SolidColorBrush)TryFindResource("FirstColor");
                else
                    this.Background = (SolidColorBrush)TryFindResource("SecondColor");
            }
            else if (IsHighLight)
            {
                this.FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                StringByteLabel.Foreground = Brushes.Black;

                this.Background = (SolidColorBrush)TryFindResource("HighLightColor");
            }
            else if (Action != ByteAction.Nothing)
            {
                switch (Action)
                {
                    case ByteAction.Modified:
                        this.FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        this.Background = (SolidColorBrush)TryFindResource("ByteModifiedColor");
                        StringByteLabel.Foreground = Brushes.Black;
                        break;
                    case ByteAction.Deleted:
                        this.FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        this.Background = (SolidColorBrush)TryFindResource("ByteDeletedColor");
                        StringByteLabel.Foreground = Brushes.Black;
                        break;
                }
                
            }
            else
            {
                this.FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                this.Background = Brushes.Transparent;
                StringByteLabel.Foreground = Brushes.Black;
            }
        }
        
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
        
        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyValidator.IsIgnoredKey(e.Key))
            {
                e.Handled = true;
                return;
            }
            else if (KeyValidator.IsUpKey(e.Key))
            {
                e.Handled = true;
                if (MoveUp != null)
                    MoveUp(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsDownKey(e.Key))
            {
                e.Handled = true;
                if (MoveDown != null)
                    MoveDown(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsLeftKey(e.Key))
            {
                e.Handled = true;
                if (MoveLeft != null)
                    MoveLeft(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsRightKey(e.Key))
            {
                e.Handled = true;
                if (MoveRight != null)
                    MoveRight(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsPageDownKey(e.Key))
            {
                e.Handled = true;
                if (MovePageDown != null)
                    MovePageDown(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsPageUpKey(e.Key))
            {
                e.Handled = true;
                if (MovePageUp != null)
                    MovePageUp(this, new EventArgs());

                return;
            }
            else if (KeyValidator.IsDeleteKey(e.Key))
            {
                if (!ReadOnlyMode)
                {
                    e.Handled = true;
                    if (ByteDeleted != null)
                        ByteDeleted(this, new EventArgs());

                    return;
                }
            }
            else if (KeyValidator.IsBackspaceKey(e.Key))
            {
                if (!ReadOnlyMode)
                {
                    e.Handled = true;
                    if (ByteDeleted != null)
                        ByteDeleted(this, new EventArgs());

                    if (MovePrevious != null)
                        MovePrevious(this, new EventArgs());

                    return;
                }
            }
            
                        

            //MODIFY ASCII... 
            //TODO : MAKE BETTER KEYDETECTION AND EXPORT IN KEYVALIDATOR
            if (!ReadOnlyMode)
            {                
                bool isok = false;

                if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                {
                    if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        StringByteLabel.Content = ByteConverters.ByteToChar((byte)KeyInterop.VirtualKeyFromKey(e.Key));
                        isok = true;
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        isok = true;
                        StringByteLabel.Content = ByteConverters.ByteToChar((byte)KeyInterop.VirtualKeyFromKey(e.Key)).ToString().ToLower();
                    }
                }
                else
                {
                    if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        StringByteLabel.Content = ByteConverters.ByteToChar((byte)KeyInterop.VirtualKeyFromKey(e.Key)).ToString().ToLower();
                        isok = true;
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        isok = true;
                        StringByteLabel.Content = ByteConverters.ByteToChar((byte)KeyInterop.VirtualKeyFromKey(e.Key));    
                    }
                }

                //Move focus event
                if (isok)
                    if (MoveNext != null)
                    {
                        Action = ByteAction.Modified;
                        Byte = ByteConverters.CharToByte(StringByteLabel.Content.ToString()[0]);

                        MoveNext(this, new EventArgs());
                    }
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (Action != ByteAction.Modified &&
                    Action != ByteAction.Deleted &&
                    Action != ByteAction.Added && 
                    !IsSelected && !IsHighLight)
                    this.Background = (SolidColorBrush)TryFindResource("MouseOverColor");

            if (e.LeftButton == MouseButtonState.Pressed)
                if (MouseSelection != null)
                    MouseSelection(this, e);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (Action != ByteAction.Modified &&
                    Action != ByteAction.Deleted &&
                    Action != ByteAction.Added &&
                    !IsSelected && !IsHighLight)
                    this.Background = Brushes.Transparent;
        }

        private void StringByteLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Focus();

                if (Click != null)
                    Click(this, e);
            }
        }
    }
}
