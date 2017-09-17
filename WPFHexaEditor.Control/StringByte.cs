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
using WPFHexaEditor.Core;
using WPFHexaEditor.Core.Bytes;
using WPFHexaEditor.Core.CharacterTable;
using WPFHexaEditor.Core.Interfaces;

namespace WPFHexaEditor
{
    internal class StringByte : TextBlock, IByteControl
    {
        //Global variable
        private HexEditor _parent;
        private TBLStream _TBLCharacterTable = null;

        //event
        public event EventHandler Click;
        public event EventHandler RightClick;
        public event EventHandler MouseSelection;
        public event EventHandler ByteModified;
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

        /// <summary>
        /// Default contructor
        /// </summary>
        /// <param name="parent"></param>
        public StringByte(HexEditor parent)
        {
            //Default properties
            Width = 10;
            Focusable = true;
            DataContext = this;
            Padding = new Thickness(0);
            TextAlignment = TextAlignment.Center;

            #region Binding tooltip
            LoadDictionary("/WPFHexaEditor;component/Resources/Dictionary/ToolTipDictionary.xaml");
            var txtBinding = new Binding()
            {
                Source = FindResource("ByteToolTip"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };

            /// <summary>
            /// Load ressources dictionnary
            /// </summary>
            /// <param name="url"></param>
            void LoadDictionary(string url)
            {
                var ttRes = new ResourceDictionary() { Source = new Uri(url, UriKind.Relative) };
                Resources.MergedDictionaries.Add(ttRes);
            }

            SetBinding(TextBlock.ToolTipProperty, txtBinding);
            #endregion

            //Event
            MouseEnter += UserControl_MouseEnter;
            MouseLeave += UserControl_MouseLeave;
            KeyDown += UserControl_KeyDown;
            MouseDown += StringByteLabel_MouseDown;
            ToolTipOpening += UserControl_ToolTipOpening;

            //Parent hexeditor
            _parent = parent;
        }


        #region DependencyProperty

        /// <summary>
        /// Position in file
        /// </summary>
        public long BytePositionInFile
        {
            get => (long)GetValue(BytePositionInFileProperty);
            set => SetValue(BytePositionInFileProperty, value);
        }

        public static readonly DependencyProperty BytePositionInFileProperty =
            DependencyProperty.Register(nameof(BytePositionInFile), typeof(long), typeof(StringByte), new PropertyMetadata(-1L));

        /// <summary>
        /// Used for selection coloring
        /// </summary>
        public bool FirstSelected
        {
            get => (bool)GetValue(FirstSelectedProperty);
            set => SetValue(FirstSelectedProperty, value);
        }

        public static readonly DependencyProperty FirstSelectedProperty =
            DependencyProperty.Register(nameof(FirstSelected), typeof(bool), typeof(StringByte), new PropertyMetadata(true));

        /// <summary>
        /// Byte used for this instance
        /// </summary>
        public byte? Byte
        {
            get => (byte?)GetValue(ByteProperty);
            set => SetValue(ByteProperty, value);
        }

        public static readonly DependencyProperty ByteProperty =
            DependencyProperty.Register(nameof(Byte), typeof(byte?), typeof(StringByte),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(Byte_PropertyChanged)));

        private static void Byte_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringByte ctrl)
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
        /// Next Byte of this instance (used for TBL/MTE decoding)
        /// </summary>
        public byte? ByteNext
        {
            get => (byte?)GetValue(ByteNextProperty);
            set => SetValue(ByteNextProperty, value);
        }

        public static readonly DependencyProperty ByteNextProperty =
            DependencyProperty.Register(nameof(ByteNext), typeof(byte?), typeof(StringByte),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ByteNext_PropertyChanged)));

        private static void ByteNext_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //if (d is StringByte ctrl)
            //    if (e.NewValue != e.OldValue)
            //    {
            //        ctrl.UpdateLabelFromByte();
            //        //ctrl.UpdateVisual();
            //    }
        }

        /// <summary>
        /// Get or set if control as selected
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(StringByte),
                new FrameworkPropertyMetadata(false, new PropertyChangedCallback(IsSelected_PropertyChangedCallBack)));

        private static void IsSelected_PropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringByte ctrl)
                if (e.NewValue != e.OldValue)
                    ctrl.UpdateVisual();
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
            DependencyProperty.Register(nameof(IsHighLight), typeof(bool), typeof(StringByte),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(IsHighLight_PropertyChanged)));

        private static void IsHighLight_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringByte ctrl)
                if (e.NewValue != e.OldValue)
                    ctrl.UpdateVisual();
        }

        /// <summary>
        /// Used to prevent StringByteModified event occurc when we dont want!
        /// </summary>
        public bool InternalChange
        {
            get => (bool)GetValue(InternalChangeProperty);
            set => SetValue(InternalChangeProperty, value);
        }

        // Using a DependencyProperty as the backing store for InternalChange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalChangeProperty =
            DependencyProperty.Register(nameof(InternalChange), typeof(bool), typeof(StringByte), new PropertyMetadata(false));

        /// <summary>
        /// Action with this byte
        /// </summary>
        public ByteAction Action
        {
            get => (ByteAction)GetValue(ActionProperty);
            set => SetValue(ActionProperty, value);
        }

        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.Register(nameof(Action), typeof(ByteAction), typeof(StringByte),
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
            if (d is StringByte ctrl)
                if (e.NewValue != e.OldValue)
                    ctrl.UpdateVisual();
        }

        #endregion DependencyProperty

        #region Characters tables

        /// <summary>
        /// Show or not Multi Title Enconding (MTE) are loaded in TBL file
        /// </summary>
        public bool TBLShowMTE
        {
            get => (bool)GetValue(TBLShowMTEProperty);
            set => SetValue(TBLShowMTEProperty, value);
        }

        // Using a DependencyProperty as the backing store for TBLShowMTE.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBLShowMTEProperty =
            DependencyProperty.Register(nameof(TBLShowMTE), typeof(bool), typeof(StringByte), 
                new FrameworkPropertyMetadata(true, 
                    new PropertyChangedCallback(TBLShowMTE_PropetyChanged)));

        private static void TBLShowMTE_PropetyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringByte ctrl)
                ctrl.UpdateLabelFromByte();
        }

        /// <summary>
        /// Type of caracter table are used un hexacontrol.
        /// For now, somes character table can be readonly but will change in future
        /// </summary>
        public CharacterTableType TypeOfCharacterTable
        {
            get => (CharacterTableType)GetValue(TypeOfCharacterTableProperty);
            set => SetValue(TypeOfCharacterTableProperty, value);
        }

        // Using a DependencyProperty as the backing store for TypeOfCharacterTable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeOfCharacterTableProperty =
            DependencyProperty.Register(nameof(TypeOfCharacterTable), typeof(CharacterTableType), typeof(StringByte),
                new FrameworkPropertyMetadata(CharacterTableType.ASCII,
                    new PropertyChangedCallback(TypeOfCharacterTable_PropertyChanged)));

        private static void TypeOfCharacterTable_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StringByte ctrl)
                ctrl.UpdateLabelFromByte();
        }

        public TBLStream TBLCharacterTable
        {
            get => _TBLCharacterTable;
            set => _TBLCharacterTable = value;
        }

        #endregion Characters tables

        /// <summary>
        /// Update control label from byte property
        /// </summary>
        internal void UpdateLabelFromByte()
        {
            if (Byte != null)
            {
                switch (TypeOfCharacterTable)
                {
                    case CharacterTableType.ASCII:
                        Text = ByteConverters.ByteToChar(Byte.Value).ToString();
                        Width = 12;
                        break;

                    case CharacterTableType.TBLFile:
                        ReadOnlyMode = !_TBLCharacterTable.AllowEdit;

                        if (_TBLCharacterTable != null)
                        {
                            string content = "#";

                            if (TBLShowMTE)
                                if (ByteNext.HasValue)
                                {
                                    string MTE = (ByteConverters.ByteToHex(Byte.Value) + ByteConverters.ByteToHex(ByteNext.Value));
                                    content = _TBLCharacterTable.FindMatch(MTE, true);
                                }

                            if (content == "#")
                                content = _TBLCharacterTable.FindMatch(ByteConverters.ByteToHex(Byte.Value), true);

                            Text = content;

                            //TODO: CHECK FOR AUTO ADAPT TO CONTENT AND FONTSIZE
                            switch (DTE.TypeDTE(content))
                            {
                                case DTEType.DualTitleEncoding:
                                    Width = 10 + content.Length * 2.2D;
                                    break;
                                case DTEType.MultipleTitleEncoding:
                                    Width = 10 + content.Length * 4.2D + (FontSize / 2);
                                    break;
                                case DTEType.EndLine:
                                    Width = 24;
                                    break;
                                case DTEType.EndBlock:
                                    Width = 34;
                                    break;
                                default:
                                    Width = 10;
                                    break;
                            }
                        }
                        else
                            goto case CharacterTableType.ASCII;
                        break;
                }
            }
            else
                Text = "";
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

                if (FirstSelected)
                    Background = _parent.SelectionFirstColor;
                else
                    Background = _parent.SelectionSecondColor;
            }
            else if (IsHighLight)
            {
                FontWeight = _parent.FontWeight;
                Foreground = _parent.Foreground;

                Background = _parent.HighLightColor;
            }
            else if (Action != ByteAction.Nothing)
            {
                switch (Action)
                {
                    case ByteAction.Modified:
                        FontWeight = FontWeights.Bold;
                        Background = _parent.ByteModifiedColor;
                        Foreground = _parent.Foreground;
                        break;

                    case ByteAction.Deleted:
                        FontWeight = FontWeights.Bold;
                        Background = _parent.ByteDeletedColor;
                        Foreground = _parent.Foreground;
                        break;
                }
            }
            else //TBL COLORING
            {
                FontWeight = _parent.FontWeight;
                Background = Brushes.Transparent;
                Foreground = _parent.Foreground;

                if (TypeOfCharacterTable == CharacterTableType.TBLFile)
                    switch (DTE.TypeDTE(Text))
                    {
                        case DTEType.DualTitleEncoding:
                            Foreground = _parent.TBLDTEColor;
                            break;
                        case DTEType.MultipleTitleEncoding:
                            Foreground = _parent.TBLMTEColor;
                            break;
                        case DTEType.EndLine:
                            Foreground = _parent.TBLEndLineColor;
                            break;
                        case DTEType.EndBlock:
                            Foreground = _parent.TBLEndBlockColor;
                            break;
                        default:
                            Foreground = _parent.TBLDefaultColor;
                            break;
                    }
            }

            UpdateAutoHighLiteSelectionByteVisual();
        }

        private void UpdateAutoHighLiteSelectionByteVisual()
        {
            //Auto highlite selectionbyte
            if (_parent.AllowAutoHightLighSelectionByte && _parent.SelectionByte != null)
                if (Byte == _parent.SelectionByte && !IsSelected)
                    Background = _parent.AutoHighLiteSelectionByteBrush;
        }

        /// <summary>
        /// Get or set if control as in read only mode
        /// </summary>
        public bool ReadOnlyMode { get; set; } = false;

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            #region Key validation and launch event if needed
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

                    if (BytePositionInFile > 0)
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
            #endregion

            //MODIFY ASCII...
            if (!ReadOnlyMode)
            {
                bool isok = false;

                if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
                {
                    if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        Text = KeyValidator.GetCharFromKey(e.Key).ToString();
                        isok = true;
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        isok = true;
                        Text = KeyValidator.GetCharFromKey(e.Key).ToString().ToLower(); 
                    }
                }
                else
                {
                    if (Keyboard.Modifiers != ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        Text = KeyValidator.GetCharFromKey(e.Key).ToString().ToLower(); 
                        isok = true;
                    }
                    else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key != Key.RightShift && e.Key != Key.LeftShift)
                    {
                        isok = true;
                        Text = KeyValidator.GetCharFromKey(e.Key).ToString();
                    }
                }

                //Move focus event
                if (isok)
                    if (MoveNext != null)
                    {
                        Action = ByteAction.Modified;
                        Byte = ByteConverters.CharToByte(Text.ToString()[0]);

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
                    Background = _parent.MouseOverColor;

            UpdateAutoHighLiteSelectionByteVisual();

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

            UpdateAutoHighLiteSelectionByteVisual();
        }

        private void StringByteLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Focus();

                Click?.Invoke(this, e);
            }

            if (e.RightButton == MouseButtonState.Pressed)            
                RightClick?.Invoke(this, e);            
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
            ByteNext = null;
        }
    }
}
