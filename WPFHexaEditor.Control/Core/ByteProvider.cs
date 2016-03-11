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
        private Stream _file = null;

        /// <summary>
        /// ByteModified list for save/modify data
        /// </summary>
        private List<ByteModified> _byteModifiedList = new List<ByteModified>();
        private string _fileName;
        private bool _readOnlyMode = false;

        /// <summary>
        /// Occurs when data are copied to clipboard
        /// </summary>
        public event EventHandler DataCopied;

        /// <summary>
        /// Occurs when ReadOnlyChanged property change
        /// </summary>
        public event EventHandler ReadOnlyChanged;

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

                OpenFile(value);
            }
        }

        /// <summary>
        /// Open file 
        /// </summary>        
        public void OpenFile(string value)
        {
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

        private void CloseFile()
        {
            
        }



        //TODO : MAke class and implementing in hexaeditor

        //position
        //filename
        //filestream
        //openfile
        //closefile
        //byteaction list
        //addbyte
        //deletebyte
        //lenght
        //readonly
        //getbyte
        //canread / write
        //get change list
        //copy / paste / cut ...
        //undo / redo?
        //...           
    }
}
