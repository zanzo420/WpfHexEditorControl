//////////////////////////////////////////////
// Apache 2.0  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

namespace WPFHexaEditor.Core.Bytes
{
    public class ByteModified
    {
        /// <summary>
        /// Byte mofidied
        /// </summary>
        public byte? Byte { get; set; } = null;

        /// <summary>
        /// Action have made in this byte
        /// </summary>
        public ByteAction Action { get; set; } = ByteAction.Nothing;

        /// <summary>
        /// Get of Set te position in file
        /// </summary>
        public long BytePositionInFile { get; set; } = -1;

        /// <summary>
        /// Number of byte to undo when this byte is reach
        /// </summary>
        public long UndoLenght { get; set; } = 1;

        /// <summary>
        /// Check if the object is valid and data can be used for action
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (BytePositionInFile > -1 && Action != ByteAction.Nothing && Byte != null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Clear object
        /// </summary>
        public void Clear()
        {
            Byte = null;
            Action = ByteAction.Nothing;
            BytePositionInFile = -1;
        }

        /// <summary>
        /// Copy Current instance to another
        /// </summary>
        /// <returns></returns>
        public ByteModified GetCopy()
        {
            ByteModified newByteModified = new ByteModified();
            object copied = null;

            newByteModified.Action = Action;
            newByteModified.Byte = Byte; //.Value;
            newByteModified.BytePositionInFile = BytePositionInFile;

            copied = newByteModified;

            return (ByteModified)copied;
        }

        public override string ToString()
        {
            return $"ByteModified - Action:{Action} Position:{BytePositionInFile} Byte:{Byte}";
        }

        /// <summary>
        /// Get if file is open
        /// </summary>
        public static bool CheckIsValid(ByteModified byteModified)
        {
            if (byteModified != null)
                if (byteModified.IsValid)
                    return true;

            return false;
        }
    }
}