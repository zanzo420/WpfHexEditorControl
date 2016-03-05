using System;
using System.Collections.Generic;
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
    /// Hexadecimal editor control
    /// </summary>
    public partial class HexaEditor : UserControl
    {
        private string _fileName = "";
        private const double _lineInfoHeight = 22;
        private int _bytePerLine = 16;
        private FileStream _file = null;
        private double _scrollLargeChange = 100;
        private bool _readOnlyMode = false;
        private long _selectionStart = -1;
        private long _selectionStop = -1;
        private bool _isHexDataVisible = true;
        private bool _isStringDataVisible = true;
        private bool _isVerticalScrollBarVisible = true;
        private bool _isHeaderVisible = true;
        private bool _isShowStatusBar = false;

        private List<ByteModified> _byteModifiedList = new List<ByteModified>();

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
                    _file = new FileStream(value, FileMode.Open, FileAccess.ReadWrite);

                    RefreshView(true);
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

                RefreshView(true);
            }
        }

        /// <summary>
        /// Index of position in file that the selection start
        /// </summary>
        public long SelectionStart
        {
            get
            {
                return _selectionStart;
            }

            set
            {
                if (_file == null)
                    _selectionStart = -1;
                else
                    _selectionStart = value;

                UpdateSelectionHexDataPanel();
                UpdateSelectionStringDataPanel();
            }
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

            UpdateSelectionHexDataPanel();
        }

        /// <summary>
        /// Clear modification
        /// </summary>
        private void ClearBytesModifiedsList()
        {
            _byteModifiedList.Clear();
        }

        /// <summary>
        /// Index of position in file that the selection stop
        /// </summary>
        public long SelectionStop
        {
            get
            {
                return _selectionStop;
            }

            set
            {
                if (_file != null)
                {
                    if (value > _file.Length)
                        _selectionStop = _file.Length;
                    else
                        _selectionStop = value;
                }
                else
                    _selectionStop = value;

                UpdateSelectionHexDataPanel();
                UpdateSelectionStringDataPanel();
            }
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

                RefreshView(true);
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
            UpdateSelectionHexDataPanel();
            UpdateSelectionStringDataPanel();
        }

        /// <summary>
        /// Update the selection of byte in hexadecimal panel
        /// </summary>
        private void UpdateSelectionHexDataPanel()
        {
            int stackIndex = 0;
            foreach (Label infolabel in LinesInfoStackPanel.Children)
            {                
                foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                {
                    if (byteControl.BytePositionInFile >= SelectionStart && byteControl.BytePositionInFile <= SelectionStop)
                        byteControl.IsSelected = true;
                    else
                        byteControl.IsSelected = false;

                    byteControl.IsByteModified = CheckIfIsByteModified(byteControl.BytePositionInFile) != null ? true : false;
                    
                }

                stackIndex++;                
            }
        }

        /// <summary>
        /// Update the selection of byte in string panel
        /// </summary>
        private void UpdateSelectionStringDataPanel()
        {
            int stackIndex = 0;
            foreach (Label infolabel in LinesInfoStackPanel.Children)
            {
                foreach (StringByteControl byteControl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                {
                    if (byteControl.BytePositionInFile >= SelectionStart && byteControl.BytePositionInFile <= SelectionStop)
                        byteControl.IsSelected = true;
                    else
                        byteControl.IsSelected = false;

                    byteControl.IsByteModified = CheckIfIsByteModified(byteControl.BytePositionInFile) != null ? true : false;

                }

                stackIndex++;
            }
        }

        /// <summary>
        /// Check if the byte in parameter are modified
        /// </summary>
        private ByteModified CheckIfIsByteModified(long bytePositionInFile)
        {
            foreach (ByteModified byteModified in _byteModifiedList)
            {
                if (byteModified.BytePositionInFile == bytePositionInFile)
                    return byteModified;
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
                ByteModified byteModified = null;

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
                            byteControl.MouseSelection += ByteControl_Selected;
                            byteControl.Click += ByteControl_Click;
                            byteControl.MoveNext += ByteControl_MoveNext;
                            byteControl.ByteModified += ByteControl_ByteModified;

                            byteModified = CheckIfIsByteModified(_file.Position);

                            if (byteModified != null)
                            {
                                switch (byteModified.Action)
                                {
                                    case ByteAction.Modified:
                                        byteControl.Byte = byteModified.Byte;
                                        byteControl.IsByteModified = true;
                                        break;
                                }                                
                            }
                            else
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
                        StackPanel dataLineStack = new StackPanel();
                        dataLineStack.Height = _lineInfoHeight;
                        dataLineStack.Orientation = Orientation.Horizontal;
                        

                        long position = Converters.HexLiteralToLong(infolabel.Content.ToString());

                        foreach (HexByteControl byteControl in ((StackPanel)HexDataStackPanel.Children[stackIndex]).Children)
                        {
                            _file.Position = position++;
                                                        
                            if (_file.Position >= _file.Length)
                            {                                
                                byteControl.IsByteModified = false;
                                byteControl.BytePositionInFile = -1;
                                byteControl.IsSelected = false;
                                byteControl.Byte = null; 
                            }
                            else
                            {
                                byteControl.IsByteModified = false;
                                byteControl.BytePositionInFile = _file.Position;

                                byteModified = CheckIfIsByteModified(_file.Position);
                                if (byteModified != null)
                                {
                                    switch (byteModified.Action)
                                    {
                                        case ByteAction.Modified:
                                            byteControl.Byte = byteModified.Byte;
                                            byteControl.IsByteModified = true;
                                            break;
                                    }
                                }
                                else
                                    byteControl.Byte = (byte)_file.ReadByte();                                
                            }                            
                        }

                        stackIndex++;
                        HexDataStackPanel.Children.Add(dataLineStack);
                    }
                }
            }
            else
            {
                HexDataStackPanel.Children.Clear();
            }
        }

        private void ByteControl_ByteModified(object sender, EventArgs e)
        {
            HexByteControl ctrl = sender as HexByteControl;

            AddByteModified(ctrl.Byte, ctrl.BytePositionInFile, ByteAction.Modified);
        }

        /// <summary>
        /// Add a bytemodifed in list of byte change
        /// </summary>        
        private void AddByteModified(byte? @byte, long bytePositionInFile, ByteAction action)
        {
            ByteModified bytemodified = CheckIfIsByteModified(bytePositionInFile);

            if (bytemodified == null)
            {
                ByteModified byteModified = new ByteModified();

                byteModified.Action = action;
                byteModified.Byte = @byte;
                byteModified.BytePositionInFile = bytePositionInFile;

                _byteModifiedList.Add(byteModified);
            }
            else
            {
                bytemodified.Byte = @byte;
                bytemodified.BytePositionInFile = bytePositionInFile;
                bytemodified.Action = action;
            }
        }

        private void ByteControl_MoveNext(object sender, EventArgs e)
        {
            HexByteControl ctrl = sender as HexByteControl;

            ctrl.IsSelected = false;

            SetFocus(ctrl.BytePositionInFile + 1);

            SelectionStart++;
            SelectionStop++;
            
        }

        private void SetFocus(long bytePositionInFile)
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
            SetFocus(bytePositionInFile);
        }

        private void ByteControl_Click(object sender, EventArgs e)
        {
            HexByteControl ctrl = sender as HexByteControl;

            SelectionStart = ctrl.BytePositionInFile;
            SelectionStop = ctrl.BytePositionInFile;
            
        }

        private void ByteControl_Selected(object sender, EventArgs e)
        {
            HexByteControl ctrl = sender as HexByteControl;

            SelectionStop = ctrl.BytePositionInFile;

            UpdateSelectionHexDataPanel();
            UpdateSelectionStringDataPanel();
        }

        /// <summary>
        /// Update the dataviewer stackpanel
        /// </summary>
        private void UpdateStringDataViewer(bool ControlResize)
        {
            if (_file != null)
            {
                ByteModified byteModified = null;

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

                            byteModified = CheckIfIsByteModified(_file.Position);
                            if (byteModified != null)
                            {
                                switch (byteModified.Action)
                                {
                                    case ByteAction.Modified:
                                        sbCtrl.Byte = byteModified.Byte;
                                        sbCtrl.BytePositionInFile = byteModified.BytePositionInFile;
                                        sbCtrl.IsByteModified = true;
                                        break;
                                }
                            }
                            else
                            {
                                sbCtrl.BytePositionInFile = _file.Position;
                                sbCtrl.Byte = (byte)_file.ReadByte();                                
                            }
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
                        StackPanel dataLineStack = new StackPanel();
                        dataLineStack.Height = _lineInfoHeight;
                        dataLineStack.Orientation = Orientation.Horizontal;
                        
                        long position = Converters.HexLiteralToLong(infolabel.Content.ToString());

                        try
                        {
                            foreach (StringByteControl sbCtrl in ((StackPanel)StringDataStackPanel.Children[stackIndex]).Children)
                            {
                                _file.Position = position++;

                                if (_file.Position >= _file.Length)
                                {
                                    sbCtrl.Byte = null;
                                }
                                else
                                {
                                    byteModified = CheckIfIsByteModified(_file.Position);
                                    if (byteModified != null)
                                    {
                                        switch (byteModified.Action)
                                        {
                                            case ByteAction.Modified:
                                                sbCtrl.Byte = byteModified.Byte;
                                                sbCtrl.BytePositionInFile = byteModified.BytePositionInFile;
                                                sbCtrl.IsByteModified = true;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        sbCtrl.Byte = (byte)_file.ReadByte();
                                        sbCtrl.BytePositionInFile = byteModified.BytePositionInFile;
                                    }
                                }                           
                            }
                        }
                        catch { }

                        stackIndex++;
                        HexDataStackPanel.Children.Add(dataLineStack);
                    }
                }
            }
            else
            {
                StringDataStackPanel.Children.Clear();
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
                    //TODO: Fix last line to EOF.
                    long firstLineByte = ((long)VerticalScrollBar.Value + i) * _bytePerLine; 
                    string info = "0x" +  firstLineByte.ToString(Constant.HexLineInfoStringFormat, Thread.CurrentThread.CurrentCulture);

                    if (firstLineByte < _file.Length)
                    {
                        //Create control
                        Label LineInfoLabel = new Label();
                        LineInfoLabel.Height = _lineInfoHeight;
                        LineInfoLabel.Padding = new Thickness(0, 0, 10, 0);
                        LineInfoLabel.Foreground = Brushes.Gray;
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

        /// <summary>
        /// Close file and clear control
        /// </summary>
        public void CloseFile()
        {
            if (this._file != null)
            {
                this._file.Close();
                this._file = null;
                VerticalScrollBar.Value = 0;
            }

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
        public void SetPosition(long position)
        {
            //TODO : selected hexbytecontrol
            SelectionStart = position;
            SelectionStop = position;


            if (_file != null)
            {
                VerticalScrollBar.Value = position / _bytePerLine;
            }
            else
                VerticalScrollBar.Value = 0;

            UpdateSelectionHexDataPanel();
            UpdateSelectionStringDataPanel();
        }

        /// <summary>
        /// Set position of cursor
        /// </summary>
        public void SetPosition(string HexLiteralPosition)
        {
            SetPosition(Converters.HexLiteralToLong(HexLiteralPosition));
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
    }
}
