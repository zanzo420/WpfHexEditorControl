using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFHexaEditor.Control.Core
{
    /// <summary>
    /// Used for interaction with file
    /// </summary>
    public class ByteProvider
    {
        //Global variable
        private List<ByteModified> _byteModifiedList = new List<ByteModified>();
        private string _fileName = string.Empty;
        private FileStream _file = null;
        private bool _readOnlyMode = false;
        private bool _isUndoEnabled = true;

        //Event
        public event EventHandler DataCopied;
        public event EventHandler ReadOnlyChanged;
        public event EventHandler FileClosed;
        public event EventHandler PositionChanged;
        public event EventHandler Undone;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ByteProvider()
        {

        }

        /// <summary>
        /// Construct new ByteProvider with filename and try to open file
        /// </summary>
        public ByteProvider(string filename)
        {
            FileName = filename;
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

                OpenFile();
            }
        }

        /// <summary>
        /// Open file 
        /// </summary>        
        public void OpenFile()
        {
            if (File.Exists(FileName))
            {
                CloseFile();

                bool readOnlyMode = false;

                try
                {
                    _file = File.Open(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read); ;
                }
                catch
                {
                    if (MessageBox.Show("The file is locked. Do you want to open it in read-only mode?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        _file = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        readOnlyMode = true;
                    }
                }

                if (readOnlyMode)
                    ReadOnlyMode = true;
            }
            else
            {
                throw new FileNotFoundException();
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

                //Launch event
                if (ReadOnlyChanged != null)
                    ReadOnlyChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Close file and clear control
        /// ReadOnlyMode is reset to false
        /// </summary>
        public void CloseFile()
        {
            if (IsOpen)
            {
                this._file.Close();
                this._file = null;
                ReadOnlyMode = false;

                if (FileClosed != null)
                    FileClosed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Get the lenght of file. Return -1 if file is close.
        /// </summary>
        public long Length
        {
            get
            {
                if (IsOpen)
                    return _file.Length;

                return -1;
            }
        }

        /// <summary>
        /// Get or Set position in file. Return -1 when file is closed
        /// </summary>
        public long Position
        {
            get
            {
                if (IsOpen)
                    return _file.Position;

                return -1;
            }
            set
            {
                if (IsOpen)
                {
                    _file.Position = value;

                    if (PositionChanged != null)
                        PositionChanged(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Get if file is open
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (_file != null)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Get if file is open
        /// </summary>
        public static bool CheckIsOpen(ByteProvider provider)
        {
            if (provider != null)
                if (provider.IsOpen)
                    return true;

            return false;
        }

        /// <summary>
        /// Readbyte at position if file CanRead. Return -1 is file is closed of EOF.
        /// </summary>
        /// <returns></returns>
        public int ReadByte()
        {
            if (IsOpen)
                if (_file.CanRead)
                    return _file.ReadByte();

            return -1;
        }

        /// <summary>
        /// Clear modification
        /// </summary>
        public void ClearBytesModifiedsList()
        {
            if (_byteModifiedList != null)
                _byteModifiedList.Clear();
        }

        /// <summary>
        /// Check if the byte in parameter are modified and return original Bytemodified from list
        /// </summary>
        public ByteModified CheckIfIsByteModified(long bytePositionInFile, ByteAction action = ByteAction.Modified)
        {
            foreach (ByteModified byteModified in _byteModifiedList)
            {
                if (action != ByteAction.All)
                {
                    if (byteModified.BytePositionInFile == bytePositionInFile &&
                        byteModified.IsValid == true &&
                        byteModified.Action == action)
                        return byteModified;
                }
                else
                {
                    if (byteModified.BytePositionInFile == bytePositionInFile &&
                        byteModified.IsValid == true)
                        return byteModified;
                }
            }

            return null;
        }

        /// <summary>
        /// Add/Modifiy a ByteModifed in the list of byte have changed
        /// </summary>        
        public void AddByteModified(byte? @byte, long bytePositionInFile)
        {
            ByteModified bytemodifiedOriginal = CheckIfIsByteModified(bytePositionInFile, ByteAction.Modified);

            if (bytemodifiedOriginal != null)
                _byteModifiedList.Remove(bytemodifiedOriginal);

            ByteModified byteModified = new ByteModified();
            
            byteModified.Byte = @byte;
            byteModified.BytePositionInFile = bytePositionInFile;
            byteModified.Action = ByteAction.Modified;

            _byteModifiedList.Add(byteModified);
        }

        /// <summary>
        /// Add/Modifiy a ByteModifed in the list of byte have deleted
        /// </summary>        
        public void AddByteDeleted(long bytePositionInFile, long length)
        {
            long position = bytePositionInFile;

            for (int i = 0; i < length; i++)
            {
                ByteModified bytemodifiedOriginal = CheckIfIsByteModified(position, ByteAction.All);

                if (bytemodifiedOriginal != null)
                    _byteModifiedList.Remove(bytemodifiedOriginal);

                ByteModified byteModified = new ByteModified();
                
                byteModified.Byte = new byte();
                byteModified.BytePositionInFile = position;
                byteModified.Action = ByteAction.Deleted;

                _byteModifiedList.Add(byteModified);

                position++;
            }
        }
        /// <summary>
        /// Return an IEnumerable ByteModified have action set to Modified
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ByteModified> ByteModifieds(ByteAction action)
        {
            foreach (ByteModified byteModified in _byteModifiedList)
                if (action != ByteAction.All)
                {
                    if (byteModified.Action == ByteAction.Modified)
                        yield return byteModified;
                }
                else
                    yield return byteModified;
        }



        #region Copy/Paste/Cut Methods
        /// <summary>
        /// Get the lenght of byte are selected (base 1)
        /// </summary>
        public long GetSelectionLenght(long selectionStart, long selectionStop)
        {
                if (selectionStop == -1 || selectionStop == -1)
                    return 0;
                else if (selectionStart == selectionStop)
                    return 1;
                else if (selectionStart > selectionStop)
                    return selectionStart - selectionStop + 1;
                else
                    return selectionStop - selectionStart + 1;            
        }

        /// <summary>
        /// Return true if Copy method could be invoked.
        /// </summary>
        public bool CanCopy(long selectionStart, long selectionStop)
        {
            if (GetSelectionLenght(selectionStart, selectionStop) < 1 || !IsOpen)
                return false;

            return true;
        }

        /// <summary>
		/// Copies the current selection in the hex box to the Clipboard.
		/// </summary>
        private byte[] GetCopyData(long selectionStart, long selectionStop, bool excludeByteAction)
        {
            //Validation
            if (!CanCopy(selectionStart, selectionStop)) return new byte[0];
            if (selectionStop == -1 || selectionStop == -1) return new byte[0];

            //Variable
            long byteStartPosition = -1;
            List<byte> bufferList = new List<byte>();

            //Set start position
            if (selectionStart == selectionStop)
                byteStartPosition = selectionStart;
            else if (selectionStart > selectionStop)
                byteStartPosition = selectionStop;
            else
                byteStartPosition = selectionStart;

            //set position
            _file.Position = byteStartPosition;

            //Exclude byte deleted from copy
            if (excludeByteAction)
            {
                byte[] buffer = new byte[GetSelectionLenght(selectionStart, selectionStop)];
                _file.Read(buffer, 0, Convert.ToInt32(GetSelectionLenght(selectionStart, selectionStop)));
                return buffer;
            }
            else
            {
                for (int i = 0; i < GetSelectionLenght(selectionStart, selectionStop); i++)
                {
                    ByteModified byteModified = CheckIfIsByteModified(_file.Position, ByteAction.All);

                    if (byteModified == null)
                    {
                        bufferList.Add((byte)_file.ReadByte());
                        continue;
                    }
                    else
                    {
                        switch (byteModified.Action)
                        {
                            case ByteAction.Added:
                                //TODO : IMPLEMENTING ADD BYTE
                                break;
                            case ByteAction.Deleted:
                                //NOTHING TODO we dont want to add deleted byte
                                break;
                            case ByteAction.Modified:
                                if (byteModified.IsValid)
                                    bufferList.Add(byteModified.Byte.Value);
                                break;
                        }
                    }

                    _file.Position++;
                }
            }
            
            return bufferList.ToArray();
        }
        
        /// <summary>
        /// Copy to clipboard
        /// </summary>        
        public void CopyToClipboard(CopyPasteMode copypastemode, long selectionStart, long selectionStop, bool excludeByteAction = false)
        {
            if (!CanCopy(selectionStart, selectionStop)) return;

            //Variables
            byte[] buffer = GetCopyData(selectionStart, selectionStop, excludeByteAction);
            string sBuffer = "";

            DataObject da = new DataObject();

            switch (copypastemode)
            {
                case CopyPasteMode.ASCIIString:
                    sBuffer = Converters.BytesToString(buffer);
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

        #endregion Copy/Paste/Cut Methods

        #region Undo / Redo

        /// <summary>
        /// Undo last byteaction
        /// </summary>
        public void Undo()
        {
            if (CanUndo())
            {
                _byteModifiedList.RemoveAt(_byteModifiedList.Count - 1);

                if (Undone != null)
                    Undone(this, new EventArgs());
            }
        }

        /// <summary>
        /// Get or set for indicate if control CanUndo
        /// </summary>
        public bool IsUndoEnabled
        {
            get { return _isUndoEnabled; }
            set { this._isUndoEnabled = value; }
        }

        /// <summary>
        /// Check if the control can undone to a previous value
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {

            if (IsUndoEnabled)
                return _byteModifiedList.Count > 0;
            else
                return false;
        }
        #endregion Undo / Redo


        //TODO : Make class and implementing in hexaeditor

        //byteaction list
        //addbyte
        //deletebyte
        //getbyte
        //canread / write
        //get change list
        //paste / cut ...
        //undo / redo?
        //...           
    }
}
