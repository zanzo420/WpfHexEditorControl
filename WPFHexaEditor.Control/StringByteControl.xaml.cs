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
using WPFHexaEditor.Control.Core;

namespace WPFHexaEditor.Control
{
    /// <summary>
    /// Interaction logic for StringByteControl.xaml
    /// </summary>
    public partial class StringByteControl : UserControl
    {
        private bool _isByteModified = false;
        private bool _readOnlyMode;

        public event EventHandler Click;
        public event EventHandler MouseSelection;
        public event EventHandler StringByteModified;
        public event EventHandler MoveNext;
        public event EventHandler MoveRight;
        public event EventHandler MoveLeft;
        public event EventHandler MoveUp;
        public event EventHandler MoveDown;

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

            if (ctrl.IsByteModified && ctrl.InternalChange == false)
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
        #endregion

        #region Standard property
        /// <summary>
        /// Get the hex string representation of this byte
        /// </summary>
        public string HexString
        {
            get
            {
                if (Byte != null)
                    return Converters.ByteToHex(Byte.Value);
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
                StringByteLabel.Content = Converters.ByteToChar(Byte.Value);
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
            else if (_isByteModified)
            {
                this.FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                this.Background = (SolidColorBrush)TryFindResource("ByteModifiedColor");
                StringByteLabel.Foreground = Brushes.Black;
            }
            else
            {
                this.FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                this.Background = Brushes.Transparent;
                StringByteLabel.Foreground = Brushes.Black;
            }
        }

        public bool IsByteModified
        {
            get
            {
                return this._isByteModified;
            }
            set
            {
                this._isByteModified = value;

                UpdateBackGround();
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

        public bool InternalChange { get; set; } = false;

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyValidator.IsUpKey(e.Key))
            {
                e.Handled = true;
                if (MoveUp != null)
                    MoveUp(this, new EventArgs());

                return;
            }


            if (!ReadOnlyMode)
            {
                //TODO : MAKE BETTER KEYDETECTION AND EXPORT IN KEYVALIDATOR
                bool isok = false;

                if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                {
                    StringByteLabel.Content = Converters.ByteToChar((byte)KeyInterop.VirtualKeyFromKey(e.Key)).ToString().ToLower();//e.Key.ToString();
                    isok = true;
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                {
                    isok = true;
                    StringByteLabel.Content = Converters.ByteToChar((byte)KeyInterop.VirtualKeyFromKey(e.Key));//e.Key.ToString();    
                }

                //Move focus event
                if (isok)
                    if (MoveNext != null)
                    {
                        IsByteModified = true;
                        Byte = Converters.CharToByte(StringByteLabel.Content.ToString()[0]);

                        MoveNext(this, new EventArgs());
                    }
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (!IsByteModified && !IsSelected)
                    this.Background = (SolidColorBrush)TryFindResource("MouseOverColor");

            if (e.LeftButton == MouseButtonState.Pressed)
                if (MouseSelection != null)
                    MouseSelection(this, e);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (!IsByteModified && !IsSelected)
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
