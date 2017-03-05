using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFHexaEditor.Core;
using WPFHexaEditor.Core.Bytes;
using WPFHexaEditor.Core.TBL;

namespace WPFHexaEditor.Control
{
    /// <summary>
    /// Interaction logic for StringByteControl.xaml
    /// </summary>
    public partial class StringByteControl : UserControl
    {
        //private bool _isByteModified = false;
        private bool _readOnlyMode;
        private TBLStream _TBLCharacterTable = null;

        public event EventHandler Click;
        public event EventHandler RightClick;
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
        public event EventHandler EscapeKey;

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
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Byte_PropertyChanged)));

        private static void Byte_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringByteControl ctrl = d as StringByteControl;

            if (e.NewValue != e.OldValue)
            {
                if (ctrl.Action != ByteAction.Nothing && ctrl.InternalChange == false)
                    ctrl.StringByteModified?.Invoke(ctrl, new EventArgs());

                ctrl.UpdateLabelFromByte();
                ctrl.UpdateHexString();

                ctrl.UpdateBackGround();

            }
        }

        /// <summary>
        /// Next Byte of this instance (used for TBL/MTE decoding)
        /// </summary>
        public byte? ByteNext
        {
            get { return (byte?)GetValue(ByteNextProperty); }
            set { SetValue(ByteNextProperty, value); }
        }

        public static readonly DependencyProperty ByteNextProperty =
            DependencyProperty.Register("ByteNext", typeof(byte?), typeof(StringByteControl),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ByteNext_PropertyChanged)));

        private static void ByteNext_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringByteControl ctrl = d as StringByteControl;

            if (e.NewValue != e.OldValue)
            {
                //if (ctrl.Action != ByteAction.Nothing && ctrl.InternalChange == false)
                //    ctrl.StringByteModified?.Invoke(ctrl, new EventArgs());

                ctrl.UpdateLabelFromByte();
                //ctrl.UpdateHexString();

                ctrl.UpdateBackGround();

            }
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

            if (e.NewValue != e.OldValue)
                ctrl.UpdateBackGround();
        }
        
        /// <summary>
        /// Get the hex string {00} representation of this byte
        /// </summary>
        public string HexString
        {
            get { return (string)GetValue(HexStringProperty); }
            internal set { SetValue(HexStringProperty, value); }
        }

        public static readonly DependencyProperty HexStringProperty =
            DependencyProperty.Register("HexString", typeof(string), typeof(StringByteControl), 
                new FrameworkPropertyMetadata(string.Empty));
        
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

            if (e.NewValue != e.OldValue)
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

            if (e.NewValue != e.OldValue)
                ctrl.UpdateBackGround();
        }
        #endregion

        #region Characters tables
        /// <summary>
        /// Type of caracter table are used un hexacontrol. 
        /// For now, somes character table can be readonly but will change in future
        /// </summary>
        public CharacterTable TypeOfCharacterTable
        {
            get { return (CharacterTable)GetValue(TypeOfCharacterTableProperty); }
            set { SetValue(TypeOfCharacterTableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeOfCharacterTable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeOfCharacterTableProperty =
            DependencyProperty.Register("TypeOfCharacterTable", typeof(CharacterTable), typeof(StringByteControl),
                new FrameworkPropertyMetadata(CharacterTable.ASCII,
                    new PropertyChangedCallback(TypeOfCharacterTable_PropertyChanged)));


        private static void TypeOfCharacterTable_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringByteControl ctrl = d as StringByteControl;

            ctrl.UpdateLabelFromByte();
            ctrl.UpdateHexString();
        }

        public TBLStream TBLCharacterTable
        {
            get
            {
                return _TBLCharacterTable;
            }
            set
            {
                _TBLCharacterTable = value;
            }
        }

        /// <summary>
        /// Get or set the color of DTE in string panel.
        /// </summary>
        public SolidColorBrush TBL_DTEColor
        {
            get { return (SolidColorBrush)GetValue(TBL_DTEColorProperty); }
            set { SetValue(TBL_DTEColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TBL_DTEColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBL_DTEColorProperty =
            DependencyProperty.Register("TBL_DTEColor", typeof(SolidColorBrush), typeof(StringByteControl),
                new FrameworkPropertyMetadata(Brushes.Red,
                    new PropertyChangedCallback(TBLColor_Changed)));

        private static void TBLColor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StringByteControl ctrl = d as StringByteControl;
                        
            ctrl.UpdateBackGround();
        }

        /// <summary>
        /// Get or set the color of MTE in string panel.
        /// </summary>
        public SolidColorBrush TBL_MTEColor
        {
            get { return (SolidColorBrush)GetValue(TBL_MTEColorProperty); }
            set { SetValue(TBL_MTEColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TBL_DTEColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBL_MTEColorProperty =
            DependencyProperty.Register("TBL_MTEColor", typeof(SolidColorBrush), typeof(StringByteControl),
                new FrameworkPropertyMetadata(Brushes.Red,
                    new PropertyChangedCallback(TBLColor_Changed)));

        /// <summary>
        /// Get or set the color of EndBlock in string panel.
        /// </summary>
        public SolidColorBrush TBL_EndBlockColor
        {
            get { return (SolidColorBrush)GetValue(TBL_EndBlockColorProperty); }
            set { SetValue(TBL_EndBlockColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TBL_DTEColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBL_EndBlockColorProperty =
            DependencyProperty.Register("TBL_EndBlockColor", typeof(SolidColorBrush), typeof(StringByteControl),
                new FrameworkPropertyMetadata(Brushes.Blue,
                    new PropertyChangedCallback(TBLColor_Changed)));

        /// <summary>
        /// Get or set the color of EndBlock in string panel.
        /// </summary>
        public SolidColorBrush TBL_EndLineColor
        {
            get { return (SolidColorBrush)GetValue(TBL_EndLineColorProperty); }
            set { SetValue(TBL_EndLineColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TBL_DTEColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBL_EndLineColorProperty =
            DependencyProperty.Register("TBL_EndLineColor", typeof(SolidColorBrush), typeof(StringByteControl),
                new FrameworkPropertyMetadata(Brushes.Blue,
                    new PropertyChangedCallback(TBLColor_Changed)));

        /// <summary>
        /// Get or set the color of EndBlock in string panel.
        /// </summary>
        public SolidColorBrush TBL_DefaultColor
        {
            get { return (SolidColorBrush)GetValue(TBL_DefaultColorProperty); }
            set { SetValue(TBL_DefaultColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TBL_DTEColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBL_DefaultColorProperty =
            DependencyProperty.Register("TBL_DefaultColor", typeof(SolidColorBrush), typeof(StringByteControl),
                new FrameworkPropertyMetadata(Brushes.Black,
                    new PropertyChangedCallback(TBLColor_Changed)));

        #endregion Characters tables

        /// <summary>
        /// Update control label from byte property
        /// </summary>
        private void UpdateLabelFromByte()
        {
            if (Byte != null)
            {
                switch (TypeOfCharacterTable)
                {
                    case CharacterTable.ASCII:
                        StringByteLabel.Content = ByteConverters.ByteToChar(Byte.Value);
                        Width = 12;
                        break;
                    case CharacterTable.TBLFile:
                        ReadOnlyMode = true;

                        if (_TBLCharacterTable != null)
                        {
                            string content = "#";

                            if (ByteNext.HasValue)
                            {
                                string MTE = (ByteConverters.ByteToHex(Byte.Value) + ByteConverters.ByteToHex(ByteNext.Value)).ToUpper();
                                content = _TBLCharacterTable.FindTBLMatch(MTE, true);
                            }

                            if (content == "#")
                                content = _TBLCharacterTable.FindTBLMatch(ByteConverters.ByteToHex(Byte.Value).ToUpper().ToUpper(), true);

                            StringByteLabel.Content = content;

                            //Adapt width to content... NOT COMPLETED. 
                            //TODO: CHECK FOR AUTO ADAPT TO CONTENT AND FONTSIZE
                            switch (DTE.TypeDTE(content))
                            {
                                case DTEType.DualTitleEncoding:
                                    Width = 12 + content.Length * 2.2D;
                                    break;
                                case DTEType.MultipleTitleEncoding:
                                    Width = 12 + content.Length * 4.2D + (FontSize / 2);                                    
                                    break;
                                case DTEType.EndLine:
                                    Width = 24;
                                    break;
                                case DTEType.EndBlock:
                                    Width = 32;
                                    break;
                                default:
                                    Width = 12;
                                    break;
                            }
                        }
                        else
                            goto case CharacterTable.ASCII;
                        break;
                }                
            }
            else
                StringByteLabel.Content = "";            
        }

        private void UpdateHexString()
        {
            if (Byte != null)
                HexString = $"0x{ByteConverters.ByteToHex(Byte.Value)}";
            else
                HexString = string.Empty;
        }
        
        /// <summary>
        /// Update Background
        /// </summary>
        private void UpdateBackGround()
        {
            if (IsSelected)
            {
                FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                StringByteLabel.Foreground = Brushes.White;

                if (StringByteFirstSelected)
                    Background = (SolidColorBrush)TryFindResource("FirstColor");
                else
                    Background = (SolidColorBrush)TryFindResource("SecondColor");

                return;
            }
            else if (IsHighLight)
            {
                FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                StringByteLabel.Foreground = Brushes.Black;

                Background = (SolidColorBrush)TryFindResource("HighLightColor");

                return;
            }
            else if (Action != ByteAction.Nothing)
            {
                switch (Action)
                {
                    case ByteAction.Modified:
                        FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        Background = (SolidColorBrush)TryFindResource("ByteModifiedColor");
                        StringByteLabel.Foreground = Brushes.Black;
                        break;
                    case ByteAction.Deleted:
                        FontWeight = (FontWeight)TryFindResource("BoldFontWeight");
                        Background = (SolidColorBrush)TryFindResource("ByteDeletedColor");
                        StringByteLabel.Foreground = Brushes.Black;
                        break;
                }

                return;
            }
            else //TBL COLORING
            {
                FontWeight = (FontWeight)TryFindResource("NormalFontWeight");
                Background = Brushes.Transparent;
                StringByteLabel.Foreground = Brushes.Black;
                
                if (TypeOfCharacterTable == CharacterTable.TBLFile)
                    switch (DTE.TypeDTE((string)StringByteLabel.Content))
                    {
                        case DTEType.DualTitleEncoding:
                            StringByteLabel.Foreground = TBL_DTEColor;
                            break;
                        case DTEType.MultipleTitleEncoding:
                            StringByteLabel.Foreground = TBL_MTEColor;
                            break;
                        case DTEType.EndLine:
                            StringByteLabel.Foreground = TBL_EndLineColor;
                            break;
                        case DTEType.EndBlock:
                            StringByteLabel.Foreground = TBL_EndBlockColor;
                            break;
                        default:
                            StringByteLabel.Foreground = TBL_DefaultColor;
                            break;
                    }
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
                if (!ReadOnlyMode)
                {
                    e.Handled = true;
                    ByteDeleted?.Invoke(this, new EventArgs());

                    MovePrevious?.Invoke(this, new EventArgs());

                    return;
                }
            }
            else if (KeyValidator.IsEscapeKey(e.Key))
            {
                e.Handled = true;
                EscapeKey?.Invoke(this, new EventArgs());
                return;
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
                    Background = (SolidColorBrush)TryFindResource("MouseOverColor");

            if (e.LeftButton == MouseButtonState.Pressed)
                MouseSelection?.Invoke(this, e);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (Byte != null)
                if (Action != ByteAction.Modified &&
                    Action != ByteAction.Deleted &&
                    Action != ByteAction.Added &&
                    !IsSelected && !IsHighLight)
                    Background = Brushes.Transparent;
        }

        private void StringByteLabel_MouseDown(object sender, MouseButtonEventArgs e)
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
    }
}
