using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// 2016 - Derek Tremblay (derektremblay666@gmail.com)
    /// WPF Hexadecimal editor control
    /// </summary>
    public partial class HexaEditor : UserControl
    {
        private string _fileName = "";
        private const double _lineInfoHeight = 22;
        private int _bytePerLine = 16;
        private Stream _file = null;
        private double _scrollLargeChange = 100;
        private bool _readOnlyMode = false;
        private long _selectionStart = -1;
        private long _selectionStop = -1;
        private bool _isHexDataVisible = true;
        private bool _isStringDataVisible = true;
        private bool _isVerticalScrollBarVisible = true;
        private bool _isHeaderVisible = true;
        private bool _isShowStatusBar = false;        
        
        /// <summary>
        /// ByteModified list for save/modify data
        /// </summary>
        private List<ByteModified> _byteModifiedList = new List<ByteModified>();

        public event EventHandler SelectionStartChanged;
        public event EventHandler SelectionStopChanged;
        public event EventHandler SelectionLenghtChanged;
        public event EventHandler DataCopied;

        public HexaEditor()
        {
            InitializeComponent();

            RefreshView(true);
        }

        /// <summary>
        /// Set or Get the file with the control will show hex
        /// </summary>
        public string FileName
        {
            get
            {
                return this._fileName;
            }

            set
            {
                //TODO: make open method
                this._fileName = value;
                
                if (File.Exists(value))
                {
                    CloseFile();
                    bool readOnlyMode = false;
                                        
                    try
                    {
                        _file = File.Open(value, FileMode.Open, FileAccess.ReadWrite, FileShare.Read); ;
                    }
                    catch
                    {
                        if (MessageBox.Show("The file is locked. Do you want to open it in read-only mode?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            _file = File.Open(value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            readOnlyMode = true;
                        }
                    }

                    RefreshView(true);

                    UnSelectAll();
                    
                    UpdateSelectionColorMode(FirstColor.HexByteData);

                    if (readOnlyMode)
                        ReadOnlyMode = true;                    
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
        }

        public int BytePerLine
        {
            get
            {
                return _bytePerLine;
            }
            set
            {
                _bytePerLine = value;

                RefreshView(true);                
            }
        }

        public double ScrollLargeChange {
            get
            {
                return _scrollLargeChange;
            }
            set
            {
                this._scrollLargeChange = value;

                UpdateVerticalScroll();
            }
        }

        /// <summary>
        /// Put the control on readonly mode.
        /// </summary>
        public bool ReadOnlyMode
        {
            get
            {
                return _readOnlyMode;
            }

            set
            {
                _readOnlyMode = value;

                RefreshView(false);
            }
        }

        /// <summary>
        /// Update de StatusPanel
        /// </summary>
        private void UpdateStatusPanel()
        {
            
        }

        /// <summary>
        /// Clear modification
        /// </summary>
        private void ClearBytesModifiedsList()
        {
            _byteModifiedList.Clear();
        }

        public bool HexDataVisibility
        {
            get
            {
                return _isHexDataVisible;
            }

            set
            {
                _isHexDataVisible = value;

                if (value)
                {
                    HexDataStackPanel.Visibility = Visibility.Visible;

                    if (HeaderVisibility)
                        HexHeaderStackPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    HexDataStackPanel.Visibility = Visibility.Collapsed;
                    HexHeaderStackPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        public bool StringDataVisibility
        {
            get
            {
                return _isStringDataVisible;
            }

            set
            {
                _isStringDataVisible = value;

                if (value)
                    StringDataStackPanel.Visibility = Visibility.Visible;
                else
                    StringDataStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        public bool VerticalScrollBarVisibility
        {
            get
            {
                return _isVerticalScrollBarVisible;
            }

            set
            {
                _isVerticalScrollBarVisible = value;

                if (value)
                    VerticalScrollBar.Visibility = Visibility.Visible;
                else
                    VerticalScrollBar.Visibility = Visibility.Collapsed;
            }
        }

        public bool HeaderVisibility
        {
            get
            {
                return _isHeaderVisible;
            }

            set
            {
                _isHeaderVisible = value;

                if (value)
                {
                    if (HexDataVisibility)
                        HexHeaderStackPanel.Visibility = Visibility.Visible;
                }
                else                
                    HexHeaderStackPanel.Visibility = Visibility.Collapsed;
                
            }
        }

        public bool StatusBarVisibility
        {
            get
            {
                return _isShowStatusBar;
            }

            set
            {
                _isShowStatusBar = value;
                
                if (value)
                    StatusBarGrid.Visibility = Visibility.Visible;
                else
                    StatusBarGrid.Visibility = Visibility.Collapsed;

                RefreshView(false);
            }
        }

        /// <summary>
        /// Refresh currentview of hexeditor
        /// </summary>
        /// <param name="ControlResize"></param>
        private void RefreshView(bool ControlResize = false)
        {
            UpdateLinesInfo();
            UpdateVerticalScroll();
            UpdateHexHeader();
            UpdateStringDataViewer(ControlResize);
            UpdateDataViewer(ControlResize);
            UpdateSelection();
            UpdateByteModified();
        }
        
        /// <summary>
        /// Update the selection of byte in hexadecimal panel
        /// </summary>
        private void UpdateSelectionColorMode(FirstColor coloring)
        {
            int stackIndex = 0;

            switch (coloring)
            {
                case FirstColor.HexByteData:
                    stackIndex = 0;
                    foreach (Label infolabel in LinesInfoStackPanel.Children)
                    {
                        foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                            byteControl.HexByteFirstSelected = true;

                        foreach (StringByteControl byteControl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                            byteControl.StringByteFirstSelected = false;

                        stackIndex++;
                    }
                    break;
                case FirstColor.StringByteData:
                    stackIndex = 0;
                    foreach (Label infolabel in LinesInfoStackPanel.Children)
                    {
                        foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                            byteControl.HexByteFirstSelected = false;

                        foreach (StringByteControl byteControl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                            byteControl.StringByteFirstSelected = true;

                        stackIndex++;
                    }
                    break;
            }
        }

        /// <summary>
        /// Update the selection of byte
        /// </summary>
        private void UpdateSelection()
        {
            int stackIndex = 0;
            foreach (Label infolabel in LinesInfoStackPanel.Children)
            {
                if (SelectionStart <= SelectionStop)
                {
                    //Stringbyte panel
                    foreach (StringByteControl byteControl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                        if (byteControl.BytePositionInFile >= SelectionStart && byteControl.BytePositionInFile <= SelectionStop)
                            byteControl.IsSelected = true;
                        else
                            byteControl.IsSelected = false;

                    //HexByte panel
                    foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                        if (byteControl.BytePositionInFile >= SelectionStart && byteControl.BytePositionInFile <= SelectionStop)
                            byteControl.IsSelected = true;
                        else
                            byteControl.IsSelected = false;
                }
                else 
                {
                    //Stringbyte panel
                    foreach (StringByteControl byteControl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                        if (byteControl.BytePositionInFile >= SelectionStop && byteControl.BytePositionInFile <= SelectionStart)
                            byteControl.IsSelected = true;
                        else
                            byteControl.IsSelected = false;

                    //HexByte panel
                    foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                        if (byteControl.BytePositionInFile >= SelectionStop && byteControl.BytePositionInFile <= SelectionStart)
                            byteControl.IsSelected = true;
                        else
                            byteControl.IsSelected = false;
                }
                stackIndex++;
            }
        }
        
        /// <summary>
        /// Check if the byte in parameter are modified and return original Bytemodified from list
        /// </summary>
        private ByteModified CheckIfIsByteModified(long bytePositionInFile)
        {
            foreach (ByteModified byteModified in _byteModifiedList)
            {
                if (byteModified.BytePositionInFile == bytePositionInFile)
                    return byteModified; //.GetCopy();
            }

            return null;
        }


        /// <summary>
        /// Update the dataviewer stackpanel
        /// </summary>
        private void UpdateDataViewer(bool ControlResize)
        {
            if (_file != null)
            {                
                if (ControlResize)
                {
                    HexDataStackPanel.Children.Clear();                    

                    foreach (Label infolabel in LinesInfoStackPanel.Children)
                    {
                        StackPanel dataLineStack = new StackPanel();
                        dataLineStack.Height = _lineInfoHeight;
                        dataLineStack.Orientation = Orientation.Horizontal;
                        
                        long position = Converters.HexLiteralToLong(infolabel.Content.ToString());

                        for (int i = 0; i < _bytePerLine; i++)
                        {
                            _file.Position = position + i;

                            if (_file.Position >= _file.Length)
                                break;

                            HexByteControl byteControl = new HexByteControl();

                            byteControl.BytePositionInFile = _file.Position;
                            byteControl.ReadOnlyMode = _readOnlyMode;
                            byteControl.MouseSelection += Control_Selected;
                            byteControl.Click += Control_Click;
                            byteControl.MoveNext += Control_MoveNext;
                            byteControl.ByteModified += Control_ByteModified;
                            byteControl.MoveUp += Control_MoveUp;

                            byteControl.Byte = (byte)_file.ReadByte();
                            
                            dataLineStack.Children.Add(byteControl);                                                        
                        }

                        HexDataStackPanel.Children.Add(dataLineStack);
                    }
                }
                else
                {
                    int stackIndex = 0;
                    foreach (Label infolabel in LinesInfoStackPanel.Children)
                    {
                        long position = Converters.HexLiteralToLong(infolabel.Content.ToString());

                        foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                        {
                            _file.Position = position++;
                                                        
                            if (_file.Position >= _file.Length)
                            {                                
                                byteControl.IsByteModified = false;
                                byteControl.BytePositionInFile = -1;
                                byteControl.ReadOnlyMode = _readOnlyMode;
                                byteControl.IsSelected = false;
                                byteControl.Byte = null; 
                            }
                            else
                            {
                                byteControl.IsByteModified = false;
                                byteControl.ReadOnlyMode = _readOnlyMode;
                                byteControl.BytePositionInFile = _file.Position;
                                byteControl.Byte = (byte)_file.ReadByte();                                
                            }
                        }

                        stackIndex++;                        
                    }
                }
            }
            else
            {
                HexDataStackPanel.Children.Clear();
            }
        }

        private void Control_MoveUp(object sender, EventArgs e)
        {
            long test = SelectionStart - BytePerLine;

            //TODO : Validation
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                if (test > -1)
                    SelectionStart -= BytePerLine;
                else
                    SelectionStart = 0;
            }
            else
            {
                if (test > -1)
                {
                    SelectionStart -= BytePerLine;
                    SelectionStop -= BytePerLine;
                }
            }
        }

        private void UpdateByteModified()
        {
            int stackIndex = 0;
            ByteModified byteModifiedCopy = null;
            foreach (ByteModified byteModified in _byteModifiedList)
            {
                stackIndex = 0;
                byteModifiedCopy = byteModified.GetCopy();

                foreach (Label infolabel in LinesInfoStackPanel.Children)
                {
                    foreach (StringByteControl byteControl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                    {
                        if (byteModifiedCopy.BytePositionInFile == byteControl.BytePositionInFile)
                        {
                            switch (byteModifiedCopy.Action)
                            {
                                case ByteAction.Modified:
                                    byteControl.InternalChange = true;
                                    byteControl.Byte = byteModifiedCopy.Byte;
                                    byteControl.IsByteModified = true;
                                    byteControl.InternalChange = false;
                                    break;
                            }
                        }
                    }

                    foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                    {
                        if (byteModifiedCopy.BytePositionInFile == byteControl.BytePositionInFile)
                        {
                            switch (byteModifiedCopy.Action)
                            {
                                case ByteAction.Modified:
                                    byteControl.InternalChange = true;
                                    byteControl.Byte = byteModifiedCopy.Byte;
                                    byteControl.IsByteModified = true;
                                    byteControl.InternalChange = false;
                                    break;
                            }
                        }
                    }

                    stackIndex++;
                }
            }
        }

        private void Control_ByteModified(object sender, EventArgs e)
        {
            HexByteControl ctrl = sender as HexByteControl;
            StringByteControl sbCtrl = sender as StringByteControl;

            if (sbCtrl != null)
                AddByteModified(sbCtrl.Byte, sbCtrl.BytePositionInFile, ByteAction.Modified);
            else if (ctrl != null)
                AddByteModified(ctrl.Byte, ctrl.BytePositionInFile, ByteAction.Modified);
        }

        /// <summary>
        /// Add/Modifiy a ByteModifed in the list of byte changed
        /// </summary>        
        private void AddByteModified(byte? @byte, long bytePositionInFile, ByteAction action)
        {
            ByteModified bytemodifiedOriginal = CheckIfIsByteModified(bytePositionInFile);

            if (bytemodifiedOriginal != null)
                _byteModifiedList.Remove(bytemodifiedOriginal);
            
            ByteModified byteModified = new ByteModified();

            byteModified.Byte = @byte;
            byteModified.BytePositionInFile = bytePositionInFile;
            byteModified.Action = action;

            _byteModifiedList.Add(byteModified);
        }
        
        private void SetFocusHexDataPanel(long bytePositionInFile)
        {
            int stackIndex = 0;
            foreach (Label infolabel in LinesInfoStackPanel.Children)
            {
                foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                {
                    if (byteControl.BytePositionInFile == bytePositionInFile)
                    {
                        byteControl.Focus();
                        return;
                    }
                }

                stackIndex++;
            }

            VerticalScrollBar.Value++;
            SetFocusHexDataPanel(bytePositionInFile);
        }

        private void SetFocusStringDataPanel(long bytePositionInFile)
        {
            int stackIndex = 0;
            foreach (Label infolabel in LinesInfoStackPanel.Children)
            {
                foreach (StringByteControl byteControl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                {
                    if (byteControl.BytePositionInFile == bytePositionInFile)
                    {
                        byteControl.Focus();
                        return;
                    }
                }

                stackIndex++;
            }

            VerticalScrollBar.Value++;
            SetFocusStringDataPanel(bytePositionInFile);
        }

        private void Control_Click(object sender, EventArgs e)
        {
            StringByteControl sbCtrl = sender as StringByteControl;
            HexByteControl ctrl = sender as HexByteControl;

            if (ctrl != null)
            {
                SelectionStart = ctrl.BytePositionInFile;
                SelectionStop = ctrl.BytePositionInFile;
                UpdateSelectionColorMode(FirstColor.HexByteData);
            }

            if (sbCtrl != null)
            {
                SelectionStart = sbCtrl.BytePositionInFile;
                SelectionStop = sbCtrl.BytePositionInFile;
                UpdateSelectionColorMode(FirstColor.StringByteData);
            }
        }

        private void Control_Selected(object sender, EventArgs e)
        {
            HexByteControl hbCtrl = sender as HexByteControl;
            StringByteControl sbCtrl = sender as StringByteControl;

            if (hbCtrl != null)
            {
                UpdateSelectionColorMode(FirstColor.HexByteData);
                SelectionStop = hbCtrl.BytePositionInFile;
            }

            if (sbCtrl != null)
            {
                UpdateSelectionColorMode(FirstColor.StringByteData);
                SelectionStop = sbCtrl.BytePositionInFile;
            }

            UpdateSelection();
        }

        /// <summary>
        /// Update the dataviewer stackpanel
        /// </summary>
        private void UpdateStringDataViewer(bool ControlResize)
        {
            if (_file != null)
            {                
                if (ControlResize)
                {
                    StringDataStackPanel.Children.Clear();

                    foreach (Label infolabel in LinesInfoStackPanel.Children)
                    {
                        StackPanel dataLineStack = new StackPanel();
                        dataLineStack.Height = _lineInfoHeight;
                        dataLineStack.Orientation = Orientation.Horizontal;
                        
                        long position = Converters.HexLiteralToLong(infolabel.Content.ToString());

                        for (int i = 0; i < _bytePerLine; i++)
                        {
                            _file.Position = position + i;
                            
                            if (_file.Position >= _file.Length)
                                break;
                                                        
                            StringByteControl sbCtrl = new StringByteControl();

                            sbCtrl.BytePositionInFile = _file.Position;
                            sbCtrl.StringByteModified += Control_ByteModified;
                            sbCtrl.ReadOnlyMode = _readOnlyMode;
                            sbCtrl.MoveNext += Control_MoveNext;
                            sbCtrl.MouseSelection += Control_Selected;
                            sbCtrl.Click += Control_Click;
                            sbCtrl.BytePositionInFile = _file.Position;
                            sbCtrl.MoveUp += Control_MoveUp;
                            sbCtrl.Byte = (byte)_file.ReadByte();
                                                        
                            dataLineStack.Children.Add(sbCtrl);
                        }

                        StringDataStackPanel.Children.Add(dataLineStack);
                    }
                }
                else
                {
                    int stackIndex = 0;
                    foreach (Label infolabel in LinesInfoStackPanel.Children)
                    {                        
                        long position = Converters.HexLiteralToLong(infolabel.Content.ToString());

                        foreach (StringByteControl sbCtrl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                        {
                            _file.Position = position++;

                            if (_file.Position >= _file.Length)
                            {
                                sbCtrl.Byte = null;
                                sbCtrl.BytePositionInFile = -1;
                                sbCtrl.IsByteModified = false;
                                sbCtrl.ReadOnlyMode = _readOnlyMode;
                                sbCtrl.IsSelected = false;
                            }
                            else
                            {
                                sbCtrl.InternalChange = true;
                                sbCtrl.Byte = (byte)_file.ReadByte();
                                sbCtrl.BytePositionInFile = _file.Position - 1;
                                sbCtrl.IsByteModified = false;
                                sbCtrl.ReadOnlyMode = _readOnlyMode;
                                sbCtrl.InternalChange = false;
                            }
                        }
                        
                        stackIndex++;                        
                    }
                }
            }
            else
            {
                StringDataStackPanel.Children.Clear();
            }
        }
        
        private void Control_MoveNext(object sender, EventArgs e)
        {
            HexByteControl hexByteCtrl = sender as HexByteControl;
            StringByteControl sbCtrl = sender as StringByteControl;

            if (sbCtrl != null)
            {
                sbCtrl.IsSelected = false;
                SetFocusStringDataPanel(sbCtrl.BytePositionInFile + 1);                
            }

            if (hexByteCtrl != null)
            {
                hexByteCtrl.IsSelected = false;
                SetFocusHexDataPanel(hexByteCtrl.BytePositionInFile + 1);            
            }

            if (hexByteCtrl != null || sbCtrl != null)
            {
                SelectionStart++;
                SelectionStop++;
                UpdateByteModified();
            }
        }

        /// <summary>
        /// Update the position info panel at left of the control
        /// </summary>
        public void UpdateHexHeader()
        {
            HexHeaderStackPanel.Children.Clear();

            if (_file != null)
            {
                for (int i = 0; i < _bytePerLine; i++)
                {
                    //Create control
                    Label LineInfoLabel = new Label();
                    LineInfoLabel.Height = _lineInfoHeight;
                    LineInfoLabel.Padding = new Thickness(0, 0, 10, 0);
                    LineInfoLabel.Foreground = Brushes.Gray;
                    LineInfoLabel.Width = 25;
                    LineInfoLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
                    LineInfoLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    LineInfoLabel.Content = Converters.ByteToHex((byte)i);
                    LineInfoLabel.ToolTip = $"Column : {i.ToString()}";

                    HexHeaderStackPanel.Children.Add(LineInfoLabel);
                }
            }
            else
            {

            }
        }

        /// <summary>
        /// Update the position info panel at left of the control
        /// </summary>
        public void UpdateLinesInfo()
        {
            LinesInfoStackPanel.Children.Clear();

            if (_file != null)
            {
                for(int i = 0; i < GetMaxVisibleLine(); i++)
                {
                    long fds = GetMaxVisibleLine();
                    //LineInfo
                    
                    long firstLineByte = ((long)VerticalScrollBar.Value + i) * _bytePerLine; 
                    string info = "0x" +  firstLineByte.ToString(Constant.HexLineInfoStringFormat, CultureInfo.InvariantCulture);

                    if (firstLineByte < _file.Length)
                    {
                        //Create control
                        Label LineInfoLabel = new Label();
                        LineInfoLabel.Height = _lineInfoHeight;
                        LineInfoLabel.Padding = new Thickness(0, 0, 10, 0);
                        LineInfoLabel.Foreground = Brushes.Gray;
                        LineInfoLabel.MouseDown += LineInfoLabel_MouseDown;
                        LineInfoLabel.MouseMove += LineInfoLabel_MouseMove;
                        LineInfoLabel.HorizontalContentAlignment = HorizontalAlignment.Right;
                        LineInfoLabel.VerticalContentAlignment = VerticalAlignment.Center;
                        LineInfoLabel.Content = info;
                        LineInfoLabel.ToolTip = $"Byte : {firstLineByte.ToString()}";  

                        LinesInfoStackPanel.Children.Add(LineInfoLabel);
                    }
                } 
            }
            else
            {

            }
        }

        private void LineInfoLabel_MouseMove(object sender, MouseEventArgs e)
        {
            Label line = sender as Label;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                SelectionStop = Converters.HexLiteralToLong(line.Content.ToString()) + BytePerLine - 1;
            }
        }

        private void LineInfoLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: Multiline selection

            Label line = sender as Label;

            SelectionStart = Converters.HexLiteralToLong(line.Content.ToString());
            SelectionStop = SelectionStart + BytePerLine - 1;
        }

        /// <summary>
        /// Close file and clear control
        /// ReadOnlyMode is reset to false
        /// </summary>
        public void CloseFile()
        {
            if (this._file != null)
            {
                this._file.Close();
                this._file = null;
                ReadOnlyMode = false;
                VerticalScrollBar.Value = 0;
            }

            ClearBytesModifiedsList();
            UnSelectAll();
            RefreshView();
        }

        /// <summary>
        /// Update vertical scrollbar with file info
        /// </summary>
        public void UpdateVerticalScroll()
        {
            VerticalScrollBar.Visibility = Visibility.Collapsed;

            if (_file != null)
            {
                //TODO : check if need to show
                VerticalScrollBar.Visibility = Visibility.Visible;

                VerticalScrollBar.SmallChange = 1;
                VerticalScrollBar.LargeChange = ScrollLargeChange;
                VerticalScrollBar.Maximum = GetMaxLine() - GetMaxVisibleLine() + 1;
            }     
                  
        }

        /// <summary>
        /// Set position of cursor
        /// </summary>
        public void SetPosition(long position, long byteLenght)
        {
            //TODO : selected hexbytecontrol
            SelectionStart = position;
            SelectionStop = position + byteLenght - 1;
            
            if (_file != null)
            {                
                VerticalScrollBar.Value = position / _bytePerLine;
            }
            else
                VerticalScrollBar.Value = 0;

            RefreshView(true);                      
        }

        public void SetPosition(long position)
        {
            SetPosition(position, 0);
        }

        /// <summary>
        /// Set position of cursor
        /// </summary>
        public void SetPosition(string HexLiteralPosition)
        {
            try
            {
                SetPosition(Converters.HexLiteralToLong(HexLiteralPosition));
            }
            catch
            {
                throw new InvalidCastException("Invalid hexa string");
            }
        }

        /// <summary>
        /// Set position of cursor
        /// </summary>
        public void SetPosition(string HexLiteralPosition, long byteLenght)
        {
            try
            {
                SetPosition(Converters.HexLiteralToLong(HexLiteralPosition), byteLenght);
            }
            catch
            {
                throw new InvalidCastException("Invalid hexa string");
            }
        }
               

        /// <summary>
        /// Obtain the max line for verticalscrollbar
        /// </summary>
        public long GetMaxLine()
        {
            if (_file != null)
                return _file.Length / _bytePerLine;
            else
                return 0;
        }

        /// <summary>
        /// Get the number of row visible in control 
        /// </summary>
        public long GetMaxVisibleLine()
        {
            return (long)(LinesInfoStackPanel.ActualHeight / _lineInfoHeight); // + 1; //TEMPS
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshView(true);
        }

        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RefreshView(false);
        }

        private void BottomRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                VerticalScrollBar.Value++;            
        }

        private void TopRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                VerticalScrollBar.Value--;            
        }

        #region Selection Property/Methods
        /// <summary>
        /// Set the start byte position of selection
        /// </summary>
        public long SelectionStart
        {
            get { return (long)GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }
                
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register("SelectionStart", typeof(long), typeof(HexaEditor),
                new FrameworkPropertyMetadata(-1L, new PropertyChangedCallback(SelectionStart_ChangedCallBack),
                    new CoerceValueCallback(SelectionStart_CoerceValueCallBack)));

        private static object SelectionStart_CoerceValueCallBack(DependencyObject d, object baseValue)
        {
            HexaEditor ctrl = d as HexaEditor;
            long value = (long)baseValue;

            if (value < -1)
                return -1L;

            if (ctrl._file == null)
                return -1L;
            else
                return baseValue;
        }

        private static void SelectionStart_ChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexaEditor ctrl = d as HexaEditor;
            
            ctrl.UpdateSelection();
            ctrl.UpdateStatusPanel();

            if (ctrl.SelectionStartChanged != null)
                ctrl.SelectionStartChanged(ctrl, new EventArgs());

            if (ctrl.SelectionLenghtChanged != null)
                ctrl.SelectionLenghtChanged(ctrl, new EventArgs());
        }
        
        /// <summary>
        /// Set the start byte position of selection
        /// </summary>
        public long SelectionStop
        {
            get { return (long)GetValue(SelectionStopProperty); }
            set { SetValue(SelectionStopProperty, value); }
        }

        public static readonly DependencyProperty SelectionStopProperty =
            DependencyProperty.Register("SelectionStop", typeof(long), typeof(HexaEditor),
                new FrameworkPropertyMetadata(-1L, new PropertyChangedCallback(SelectionStop_ChangedCallBack),
                    new CoerceValueCallback(SelectionStop_CoerceValueCallBack)));

        private static object SelectionStop_CoerceValueCallBack(DependencyObject d, object baseValue)
        {
            HexaEditor ctrl = d as HexaEditor;
            long value = (long)baseValue;

            if (value < -1)
                return -1L;

            if (ctrl._file == null)
                return -1L;

            if (value > ctrl._file.Length)
                return ctrl.FileName.Length;

            return baseValue;            
        }

        private static void SelectionStop_ChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexaEditor ctrl = d as HexaEditor;

            ctrl.UpdateSelection();
            ctrl.UpdateStatusPanel();

            if (ctrl.SelectionStopChanged != null)
                ctrl.SelectionStopChanged(ctrl, new EventArgs());

            if (ctrl.SelectionLenghtChanged != null)
                ctrl.SelectionLenghtChanged(ctrl, new EventArgs());            
        }

        /// <summary>
        /// Reset selection to -1
        /// </summary>
        public void UnSelectAll()
        {
            SelectionStart = -1;
            SelectionStop = -1;
        }

        /// <summary>
        /// Select the entire file
        /// If file are closed the selection will be set to -1
        /// </summary>
        public void SelectAll()
        {
            if (_file != null)
            {
                SelectionStart = 0;
                SelectionStop = _file.Length;
            }
            else
            {
                SelectionStart = -1;
                SelectionStop = -1;
            }

            UpdateSelection();
        }
                

        /// <summary>
        /// Get the lenght of byte are selected (base 1)
        /// </summary>
        public long SelectionLenght
        {
            get
            {
                if (SelectionStop == -1 || SelectionStop == -1)
                    return 0;
                else if (SelectionStart == SelectionStop)
                    return 1;
                else if (SelectionStart > SelectionStop)
                    return SelectionStart - SelectionStop + 1;
                else
                    return SelectionStop - SelectionStart + 1;
            }
        }
       
        #endregion

        #region Copy/Paste/Cut Methods
        /// <summary>
        /// Return true if Copy method could be invoked.
        /// </summary>
        public bool CanCopy()
        {

            if (SelectionLenght < 1 || _file == null)
                return false;

            return true;
        }

        /// <summary>
		/// Copies the current selection in the hex box to the Clipboard.
		/// </summary>
        private byte[] GetCopyData()
        {
            //Validation
            if (!CanCopy()) return new byte[0];
            if (SelectionStop == -1 || SelectionStop == -1) return new byte[0];

            //Variable
            long byteStartPosition = -1;            
            byte[] buffer = new byte[SelectionLenght];

            //Set start position
            if (SelectionStart == SelectionStop)
                byteStartPosition = SelectionStart;
            else if (SelectionStart > SelectionStop)
                byteStartPosition = SelectionStop;
            else
                byteStartPosition = SelectionStart;

            //set position
            _file.Position = byteStartPosition;

            _file.Read(buffer, 0, Convert.ToInt32(SelectionLenght));

            return buffer;
        }

        /// <summary>
        /// Copy to clipboard with default CopyPasteMode.ASCIIString
        /// </summary>
        public void CopyToClipboard()
        {
            CopyToClipboard(CopyPasteMode.ASCIIString);
        }

        /// <summary>
        /// Copy to clipboard
        /// </summary>        
        public void CopyToClipboard(CopyPasteMode copypastemode)
        {
            if (!CanCopy()) return;

            //Variables
            byte[] buffer = GetCopyData();
            string sBuffer = "";

            DataObject da = new DataObject();

            switch (copypastemode)
            {
                case CopyPasteMode.ASCIIString:
                    sBuffer = Converters.BytesToASCIIString(buffer);
                    da.SetText(sBuffer, TextDataFormat.Text);
                    break;
                case CopyPasteMode.HexaString:
                    sBuffer = Converters.ByteToHex(buffer);
                    da.SetText(sBuffer, TextDataFormat.Text);
                    break;
                case CopyPasteMode.Byte:
                    throw new NotImplementedException();
            }
            
            //set memorystream (BinaryData) clipboard data
            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer, 0, buffer.Length, false, true);
            da.SetData("BinaryData", ms);

            Clipboard.SetDataObject(da, true);

            if (DataCopied != null)
                DataCopied(this, new EventArgs());
        }

        #endregion
    }
}
