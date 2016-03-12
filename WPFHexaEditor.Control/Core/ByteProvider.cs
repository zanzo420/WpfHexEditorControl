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

        //Event
        public event EventHandler DataCopied;
        public event EventHandler ReadOnlyChanged;
        public event EventHandler FileClosed;
        public event EventHandler PositionChanged;

        /// <summary>
        /// Constructor
        /// </summary>
        public ByteProvider()
        {

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
            if (this._file != null)
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
        public long Lenght
        {
            get
            {
                if (_file != null)
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
                if (_file != null)
                    return _file.Position;

                return -1;
            }
            set
            {
                if (_file != null)
                {
                    _file.Position = value;

                    if (FileClosed != null)
                        FileClosed(this, new EventArgs());
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
        /// Readbyte at position if file CanRead. Return -1 is file is closed of EOF.
        /// </summary>
        /// <returns></returns>
        public int ReadByte()
        {
            if (_file != null)
                if (_file.CanRead)
                    return _file.ReadByte();

            return -1;
        }

        /// <summary>
        /// Clear modification
        /// </summary>
        private void ClearBytesModifiedsList()
        {
            if (_byteModifiedList != null)
                _byteModifiedList.Clear();
        }

        /// <summary>
        /// Check if the byte in parameter are modified and return original Bytemodified from list
        /// </summary>
        private ByteModified CheckIfIsByteModified(long bytePositionInFile)
        {
            foreach (ByteModified byteModified in _byteModifiedList)
            {
                if (byteModified.BytePositionInFile == bytePositionInFile && byteModified.IsValid == true)
                    return byteModified; //.GetCopy();
            }

            return null;
        }

        /// <summary>
        /// Add/Modifiy a ByteModifed in the list of byte changed
        /// </summary>        
        public void AddByteModified(byte? @byte, long bytePositionInFile)
        {
            ByteModified bytemodifiedOriginal = CheckIfIsByteModified(bytePositionInFile);

            if (bytemodifiedOriginal != null)
                _byteModifiedList.Remove(bytemodifiedOriginal);

            ByteModified byteModified = new ByteModified();

            //TODO: Add action type (deleted, add...)
            byteModified.Byte = @byte;
            byteModified.Lenght = 1;
            byteModified.BytePositionInFile = bytePositionInFile;
            byteModified.Action = ByteAction.Modified;

            _byteModifiedList.Add(byteModified);
        }

        //TODO : Make class and implementing in hexaeditor

        //byteaction list
        //addbyte
        //deletebyte
        //getbyte
        //canread / write
        //get change list
        //copy / paste / cut ...
        //undo / redo?
        //...           
    }
}
