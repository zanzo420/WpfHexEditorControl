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
    /// Interaction logic for HexControl.xaml
    /// </summary>
    public partial class HexByteControl : UserControl
    {   
        private bool _isSelected = false;
        private bool _readOnlyMode = false;
        private KeyDownLabel _keyDownLabel = KeyDownLabel.FirstChar;

        public event EventHandler ByteModified;
        public event EventHandler MouseSelection;
        public event EventHandler Click;
        public event EventHandler MoveNext;
        public event EventHandler MoveRight;
        public event EventHandler MoveLeft;
        public event EventHandler MoveUp;
        public event EventHandler MoveDown;
        public event EventHandler MovePageDown;
        public event EventHandler MovePageUp;
        public event EventHandler ByteDeleted;

        public HexByteControl()
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

            ctrl.UpdateBackGround();
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
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Byte_PropertyChangedCallBack)));

        private static void Byte_PropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexByteControl ctrl = d as HexByteControl;

            if (ctrl.Action != ByteAction.Nothing && ctrl.InternalChange == false)
                if (ctrl.ByteModified != null)
                    ctrl.ByteModified(ctrl, new EventArgs());

            ctrl.UpdateLabelFromByte();
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
            get
            {
                return ((string)FirstHexChar.Content + (string)SecondHexChar.Content).ToString();
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

                _keyDownLabel = KeyDownLabel.FirstChar;

                UpdateBackGround();
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
                FirstHexChar.Foreground = Brushes.White;
                SecondHexChar.Foreground = Brushes.White;

                if (HexByteFirstSelected)
                    this.Background = (SolidColorBrush)TryFindResource("FirstColor"); 
                else
                    this.Background = (SolidColorBrush)TryFindResource("SecondColor");
            }
            else if (Action != ByteAction.Nothing)
            {
                switch (Action)
                {
                    case ByteAction.Modified:
                        this.FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        this.Background = (SolidColorBrush)TryFindResource("ByteModifiedColor");
                        FirstHexChar.Foreground = Brushes.Black;
                        SecondHexChar.Foreground = Brushes.Black;
                        break;
                    case ByteAction.Deleted:
                        this.FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        this.Background = (SolidColorBrush)TryFindResource("ByteDeletedColor");
                        FirstHexChar.Foreground = Brushes.Black;
                        SecondHexChar.Foreground = Brushes.Black;
                        break;
                }
            }
            else
            {
                this.FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                this.Background = Brushes.Transparent;
                FirstHexChar.Foreground = Brushes.Black;
                SecondHexChar.Foreground = Brushes.Black;
            }
        }

        private void UpdateLabelFromByte()
        {
            if (Byte != null)
            {
                string hexabyte = ByteConverters.ByteToHex(Byte.Value);

                FirstHexChar.Content = hexabyte.Substring(0, 1);
                SecondHexChar.Content = hexabyte.Substring(1, 1);
            }
            else
            {
                FirstHexChar.Content = "";
                SecondHexChar.Content = "";
            }
        }

        private void HexChar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Focus();

                if (Click != null)
                    Click(this, e);
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (KeyValidator.IsUpKey(e.Key))
            {
                e.Handled = true;
                if (MoveUp != null)
                    MoveUp(this, new EventArgs());

                return;
            }
            else if(KeyValidator.IsDownKey(e.Key))
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
                e.Handled = true;
                if (ByteDeleted != null)
                    ByteDeleted(this, new EventArgs());

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
                            FirstHexChar.Content = key;
                            _keyDownLabel = KeyDownLabel.SecondChar;
                            Action = ByteAction.Modified;
                            Byte = ByteConverters.HexToByte(FirstHexChar.Content.ToString() + SecondHexChar.Content.ToString())[0];
                            break;
                        case KeyDownLabel.SecondChar:
                            SecondHexChar.Content = key;
                            _keyDownLabel = KeyDownLabel.NextPosition;

                            Action = ByteAction.Modified;                            
                            Byte = ByteConverters.HexToByte(FirstHexChar.Content.ToString() + SecondHexChar.Content.ToString())[0];
                            
                            //Move focus event
                            if (MoveNext != null)
                                MoveNext(this, new EventArgs());
                            break;
                    }
                }            
        }
        
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (Action != ByteAction.Modified &&
                    Action != ByteAction.Deleted &&
                    Action != ByteAction.Added &&
                    !IsSelected)
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
                    !IsSelected)
                    this.Background = Brushes.Transparent;
        }
    }
}
