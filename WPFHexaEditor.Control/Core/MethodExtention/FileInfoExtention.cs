//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System.IO;
using System.Threading;


namespace WPFHexaEditor.Core.MethodExtention
{
    public static class FileInfoExtention
    {
        /// <summary>
        /// Helper permetant de verifier si le fichier n'est pas verouiller par l'OS        
        /// </summary>        
        public static bool IsFileInUse(this FileInfo file)
        {
            FileStream stream = null;
            try
            {
                Thread.Sleep(500); //TEST CAR LORS DE COPIE, WINDOW N'EST PAS CAPABLE DE COPIER SONT FICHIER...
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //File non verouiller
            return false;
        }
    }
}
