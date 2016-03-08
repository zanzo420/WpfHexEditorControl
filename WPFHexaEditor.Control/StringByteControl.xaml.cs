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
        private byte? _byte = null;
        private bool _isSelected = false;
        private bool _isByteModified = false;
        private bool _readOnlyMode;

        public event EventHandler Click;
        public event EventHandler MouseSelection;
        public event EventHandler StringByteModified;
        public event EventHandler MoveNext;

        public StringByteControl()
        {
            InitializeComponent();

            DataContext = this;
        }

        public long BytePositionInFile { get; set; } = -1;
        public bool StringByteFirstSelected { get; set; } = true;

        public byte? Byte
        {
            get
            {
                return this._byte;
            }
            set
            {
                this._byte = value;
                
                if (IsByteModified && InternalChange == false)
                    if (StringByteModified != null)
                        StringByteModified(this, new EventArgs());

                UpdateLabelFromByte();
                UpdateBinding();
            }
        }

        /// <summary>
        /// Updates somes bindings
        /// TEMPS METHOD
        /// TODO: Remplace by dependency property
        /// </summary>
        private void UpdateBinding()
        {
            BindingOperations.GetBindingExpression(this, UserControl.ToolTipProperty);
        }


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

        /// <summary>
        /// Update control label from byte property
        /// </summary>
        private void UpdateLabelFromByte()
        {
            if (_byte != null)
            {
                StringByteLabel.Content = Converters.ByteToChar(_byte.Value);
            }
            else
            {
                StringByteLabel.Content = "";
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                _isSelected = value;

                UpdateBackGround();
            }
        }

        /// <summary>
        /// Update Background
        /// </summary>
        private void UpdateBackGround()
        {
            if (_isSelected)
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
            if (_byte != null)
                if (!IsByteModified && !_isSelected)
                    this.Background = (SolidColorBrush)TryFindResource("MouseOverColor");

            if (e.LeftButton == MouseButtonState.Pressed)
                if (MouseSelection != null)
                    MouseSelection(this, e);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_byte != null)
                if (!IsByteModified && !_isSelected)
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
