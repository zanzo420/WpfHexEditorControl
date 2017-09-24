//////////////////////////////////////////////
// Apache 2.0  - 2013-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using WPFHexaEditor.Core.Bytes;

namespace WPFHexaEditor.Core.CharacterTable
{
    /// <summary>
    /// Cet objet représente un fichier Thingy TBL (entrée + valeur)
    /// </summary>
    public sealed class TblStream: IDisposable
    {
        /// <summary>Chemin vers le fichier (path)</summary>
        private string _fileName;

        /// <summary>Tableau de DTE représentant tous les les entrée du fichier</summary>
        private List<Dte> _dteList = new List<Dte>();

        /// <summary>Commentaire du fichier TBL</summary>
        //		private string _Commentaire = string.Empty;

        #region Constructeurs

        /// <summary>
        /// Constructeur permétant de chargé le fichier DTE
        /// </summary>
        /// <param name="fileName"></param>
        public TblStream(string fileName)
        {
            _dteList.Clear();

            //check if exist and load file
            if (File.Exists(fileName))
            {
                _fileName = fileName;
                Load();
            }
            else
                throw new FileNotFoundException();
        }

        /// <summary>
        /// Constructeur permétant de chargé le fichier DTE
        /// </summary>
        public TblStream()
        {
            _dteList.Clear();
            _fileName = string.Empty;
        }

        #endregion Constructeurs

        #region Indexer

        /// <summary>
        /// Indexeur permetant de travailler sur les DTE contenue dans TBL a la facons d'un tableau.
        /// </summary>
        public Dte this[int index]
        {   // declaration de indexer
            get
            {
                // verifie la limite de l'index
                if (index < 0 || index > _dteList.Count)
                    return null;  //throw new IndexOutOfRangeException("Cette item n'existe pas");
                return _dteList[index];
            }
            set
            {
                if (!(index < 0 || index >= _dteList.Count))
                    _dteList[index] = value;
            }
        }

        #endregion Indexer

        #region Méthodes

        /// <summary>
        /// Trouver une entré dans la table de jeu qui corestpond a la valeur hexa
        /// </summary>
        /// <param name="hex">Valeur hexa a rechercher dans la TBL</param>
        /// <param name="showSpecialValue">Afficher les valeurs de fin de block et de ligne</param>
        /// <returns></returns>
        public string FindMatch(string hex, bool showSpecialValue)
        {
            var rtn = "#";
            foreach (var dte in _dteList)
            {
                if (dte.Entry == hex)
                {
                    rtn = dte.Value;
                    break;
                }

                if (showSpecialValue)
                {
                    if (dte.Entry == "/" + hex)
                    {
                        rtn = "<end>";
                        break;
                    }
                    if (dte.Entry == "*" + hex)
                    {
                        rtn = "<ln>";
                        break;
                    }
                }
            }

            return rtn;
        }

        /// <summary>
        /// Trouver une entré dans la table de jeu qui corestpond a la valeur hexa
        /// </summary>
        /// <param name="hex">Valeur hexa a rechercher dans la TBL</param>
        /// <returns>Retourne le DTE/MTE trouvé. null si rien trouvé</returns>
        public Dte GetDte(string hex)
        {
            foreach (var dte in _dteList)
            {
                if (dte.Entry == hex)
                    return dte;

                if (dte.Entry == "/" + hex)
                    return dte;

                if (dte.Entry == "*" + hex)
                    return dte;
            }

            return null;
        }

        /// <summary>
        /// Trouver une entré dans la table de jeu qui corestpond a la valeur hexa
        /// </summary>
        /// <param name="hex">Valeur hexa a rechercher dans la TBL</param>
        public string FindMatch(string hex)
        {
            var rtn = "#";
            foreach (var dte in _dteList)
                if (dte.Entry == hex)
                {
                    rtn = dte.Value;
                    break;
                }

            return rtn;
        }

        /// <summary>
        /// Trouver une entré dans la table de jeu qui corestpond a la valeur hexa
        /// </summary>
        /// <param name="hex">Valeur hexa a rechercher dans la TBL</param>
        /// <param name="showSpecialValue">Afficher les valeurs de fin de block et de ligne</param>
        /// <param name="notShowDte"></param>
        public string FindMatch(string hex, bool showSpecialValue, bool notShowDte)
        {
            var rtn = "#";
            foreach (var dte in _dteList)
            {
                if (dte.Entry == hex)
                {
                    if (notShowDte)
                    {
                        if (dte.Type == DteType.DualTitleEncoding)
                            break;

                        rtn = dte.Value;
                        break;
                    }
                    rtn = dte.Value;
                    break;
                }

                if (showSpecialValue)
                {
                    if (dte.Entry == "/" + hex)
                    {
                        rtn = "<end>";
                        break;
                    }

                    if (dte.Entry == "*" + hex)
                    {
                        rtn = "<ln>";
                        break;
                    }
                }
            }

            return rtn;
        }

        /// <summary>
        /// Convert data to TBL string. 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>
        /// Return string converted to TBL string representation.
        /// Return null on error
        /// </returns>
        public string ToTblString(byte[] data)
        {
            if (data != null)
            {
                var sb = new StringBuilder();

                for (var i = 0; i < data.Length; i++)
                {
                    if (i < data.Length - 1)
                    {
                        var mte = FindMatch(ByteConverters.ByteToHex(data[i]) + ByteConverters.ByteToHex(data[i + 1]), true);

                        if (mte != "#")
                        {
                            sb.Append(mte);
                            continue;
                        }
                    }

                    sb.Append(FindMatch(ByteConverters.ByteToHex(data[i]), true));
                }

                return sb.ToString();
            }

            return null;
        }

        /// <summary>
        /// Chargé le fichier dans l'objet
        /// </summary>
        /// <returns>Retoune vrai si le fichier est bien charger</returns>
        private void Load()
        {
            //Vide la collection
            _dteList.Clear();
            //ouverture du fichier

            if (!File.Exists(_fileName))
            {
                var fs = File.Create(_fileName);
                fs.Close();
            }

            StreamReader tblFile;
            try
            {
                tblFile = new StreamReader(_fileName, Encoding.ASCII);
            }
            catch
            {
                return;
            }

            if (tblFile.BaseStream.CanRead)
            {
                //lecture du fichier jusqua la fin et séparation par ligne
                char[] sepEndLine = { '\n' }; //Fin de ligne
                char[] sepEqual = { '=' }; //Fin de ligne

                //build strings line
                var textFromFile = new StringBuilder(tblFile.ReadToEnd());
                textFromFile.Insert(textFromFile.Length, '\r');
                textFromFile.Insert(textFromFile.Length, '\n');
                var lines = textFromFile.ToString().Split(sepEndLine);

                //remplir la collection de DTE : this._DTE
                foreach (var line in lines)
                {
                    var info = line.Split(sepEqual);

                    //ajout a la collection (ne prend pas encore en charge le Japonais)
                    Dte dte;
                    try
                    {
                        switch (info[0].Length)
                        {
                            case 2:
                                dte = info[1].Length == 2 ? new Dte(info[0], info[1].Substring(0, info[1].Length - 1), DteType.Ascii) : new Dte(info[0], info[1].Substring(0, info[1].Length - 1), DteType.DualTitleEncoding);
                                break;
                            case 4: // >2
                                dte = new Dte(info[0], info[1].Substring(0, info[1].Length - 1), DteType.MultipleTitleEncoding);
                                break;
                            default:
                                continue;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        switch (info[0].Substring(0, 1))
                        {
                            case @"/":
                                dte = new Dte(info[0].Substring(0, info[0].Length - 1), string.Empty, DteType.EndBlock);
                                break;

                            case @"*":
                                dte = new Dte(info[0].Substring(0, info[0].Length - 1), string.Empty, DteType.EndLine);
                                break;
                            //case @"\":
                            default:
                                continue;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    { //Du a une entre qui a 2 = de suite... EX:  XX==
                        dte = new Dte(info[0], "=", DteType.DualTitleEncoding);
                    }

                    _dteList.Add(dte);
                }

                //Load bookmark
                BookMarks.Clear();
                foreach (var line in lines)
                {
                    try
                    {
                        if (line.Substring(0, 1) == "(")
                        {
                            var fav = new BookMark();
                            var lineSplited = line.Split(')');
                            fav.Description = lineSplited[1].Substring(0, lineSplited[1].Length - 1);

                            lineSplited = line.Split('h');
                            fav.BytePositionInFile = ByteConverters.HexLiteralToLong(lineSplited[0].Substring(1, lineSplited[0].Length - 1)).position;
                            fav.Marker = ScrollMarker.TblBookmark;
                            BookMarks.Add(fav);
                        }
                    }
                    catch
                    {
                        //Nothing to add if error
                    }
                }

                tblFile.Close();
            }
        }

        /// <summary>
        /// Enregistrer dans le fichier
        /// </summary>
        /// <returns>Retourne vrai si le fichier à été bien enregistré</returns>
        public bool Save()
        {
            //ouverture du fichier
            var myFile = new FileStream(_fileName, FileMode.Create, FileAccess.Write);
            var tblFile = new StreamWriter(myFile, Encoding.ASCII);

            if (tblFile.BaseStream.CanWrite)
            {
                //Save tbl set
                foreach (var dte in _dteList)
                    if (dte.Type != DteType.EndBlock && dte.Type != DteType.EndLine)
                        tblFile.WriteLine(dte.Entry + "=" + dte.Value);
                    else
                        tblFile.WriteLine(dte.Entry);

                //Save bookmark
                tblFile.WriteLine();
                foreach (var mark in BookMarks)
                    tblFile.WriteLine(mark.ToString());

                //Ecriture de 2 saut de ligne a la fin du fichier.
                //(obligatoire pour certain logiciel utilisant les TBL)
                tblFile.WriteLine();
                tblFile.WriteLine();
            }

            //Ferme le fichier TBL
            tblFile.Close();

            return true;
        }

        /// <summary>
        /// Ajouter un element a la collection
        /// </summary>
        /// <param name="dte">objet DTE a ajouter fans la collection</param>
        public void Add(Dte dte) => _dteList.Add(dte);

        /// <summary>
        /// Effacer un element de la collection a partir d'un objet DTE
        /// </summary>
        /// <param name="dte"></param>
        public void Remove(Dte dte) => _dteList.Remove(dte);

        /// <summary>
        /// Effacer un element de la collection avec son index dans la collection
        /// </summary>
        /// <param name="index">Index de l'element a effacer</param>
        public void Remove(int index) => _dteList.RemoveAt(index);

        /// <summary>
        /// Recherche un élément dans la TBL
        /// </summary>
        /// <param name="dte">Objet DTE a rechercher dans la TBL</param>
        /// <returns>Retourne la position ou ce trouve cette élément dans le tableau</returns>
        public int Find(Dte dte) => _dteList.BinarySearch(dte);
        
        /// <summary>
        /// Recherche un élément dans la TBL
        /// </summary>
        /// <param name="entry">Entrée sous forme hexadécimal (XX)</param>
        /// <param name="value">Valeur de l'entré</param>
        /// <returns>Retourne la position ou ce trouve cette élément dans le tableau</returns>
        public int Find(string entry, string value) => _dteList.BinarySearch(new Dte(entry, value));

        /// <summary>
        /// Recherche un élément dans la TBL
        /// </summary>
        /// <param name="entry">Entrée sous forme hexadécimal (XX)</param>
        /// <param name="value">Valeur de l'entré</param>
        /// <param name="type">Type de DTE</param>
        /// <returns>Retourne la position ou ce trouve cette élément dans le tableau</returns>
        public int Find(string entry, string value, DteType type) => _dteList.BinarySearch(new Dte(entry, value, type));

        #endregion Méthodes

        #region Propriétés

        /// <summary>
        /// Chemin d'acces au fichier (path)
        /// La fonction load doit etre appeler pour rafraichir la fonction
        /// </summary>
        [ReadOnly(true)]
        public string FileName
        {
            get => _fileName;
            internal set
            {
                _fileName = value;
                Load();
            }
        }

        /// <summary>
        /// Total d'élement dans l'objet TBL
        /// </summary>
        public int Length => _dteList.Count;

        /// <summary>
        /// Avoir acess au Bookmark
        /// </summary>
        [Browsable(false)]
        public List<BookMark> BookMarks { get; set; } = new List<BookMark>();

        /// <summary>
        /// Obtenir le total d'entré DTE dans la Table
        /// </summary>
        public int TotalDte => _dteList.Count(l => l.Type == DteType.DualTitleEncoding);

        /// <summary>
        /// Obtenir le total d'entré MTE dans la Table
        /// </summary>
        public int TotalMte => _dteList.Count(l => l.Type == DteType.MultipleTitleEncoding);

        /// <summary>
        /// Obtenir le total d'entré ASCII dans la Table
        /// </summary>
        public int TotalAscii => _dteList.Count(l => l.Type == DteType.Ascii);

        /// <summary>
        /// Obtenir le total d'entré Invalide dans la Table
        /// </summary>
        public int TotalInvalid => _dteList.Count(l => l.Type == DteType.Invalid);

        /// <summary>
        /// Obtenir le total d'entré Japonais dans la Table
        /// </summary>
        public int TotalJaponais => _dteList.Count(l => l.Type == DteType.Japonais);

        /// <summary>
        /// Obtenir le total d'entré Fin de ligne dans la Table
        /// </summary>
        public int TotalEndLine => _dteList.Count(l => l.Type == DteType.EndLine);

        /// <summary>
        /// Obtenir le total d'entré Fin de Block dans la Table
        /// </summary>
        public int TotalEndBlock => _dteList.Count(l => l.Type == DteType.EndBlock);

        /// <summary>
        /// Renvoi le caractere de fin de block
        /// </summary>
        public string EndBlock
        {
            get
            {
                foreach (var dte in _dteList)
                    if (dte.Type == DteType.EndBlock)
                        return dte.Entry;

                return string.Empty;
            }
        }

        /// <summary>
        /// Renvoi le caractere de fin de ligne
        /// </summary>
        public string EndLine
        {
            get
            {
                foreach (var dte in _dteList)
                    if (dte.Type == DteType.EndLine)
                        return dte.Entry;

                return string.Empty;
            }
        }

        /// <summary>
        /// Enable/Disable Readonly on control.
        /// </summary>
        public bool AllowEdit { get; set; }

        #endregion Propriétés

        #region Build default TBL
        public static TblStream CreateDefaultAscii(DefaultCharacterTableType type = DefaultCharacterTableType.Ascii)
        {
            var tbl = new TblStream();

            switch (type)
            {
                case DefaultCharacterTableType.Ascii:
                    for (byte i = 0; i < 255; i++)
                    {
                        var dte = new Dte(ByteConverters.ByteToHex(i).ToUpper(), $"{ByteConverters.ByteToChar(i)}");
                        tbl.Add(dte);
                    }
                    break;
            }

            tbl.AllowEdit = true;
            return tbl;
        }

        #endregion

        #region IDisposable Support
        private bool _disposedValue; // Pour détecter les appels redondants

        void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _dteList = null;
                }

                _disposedValue = true;
            }
        }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
        }
        #endregion
    }
}