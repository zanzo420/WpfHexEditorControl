//////////////////////////////////////////////
// MIT License  - 2016-2017
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
    ///
    /// Derek Tremblay 2003-2017
    /// </summary>
    public class TBLStream
    {
        /// <summary>Chemin vers le fichier (path)</summary>
        private string _FileName;

        /// <summary>Tableau de DTE représentant tous les les entrée du fichier</summary>
        private List<DTE> _DTEList = new List<DTE>();

        /// <summary>Commentaire du fichier TBL</summary>
        //		private string _Commentaire = "";

        #region Constructeurs

        /// <summary>
        /// Constructeur permétant de chargé le fichier DTE
        /// </summary>
        /// <param name="FileName"></param>
        public TBLStream(string FileName)
        {
            _DTEList.Clear();

            //check if exist and load file
            if (File.Exists(FileName))
            {
                _FileName = FileName;
                Load();
            }
            else
                throw new FileNotFoundException();
        }

        /// <summary>
        /// Constructeur permétant de chargé le fichier DTE
        /// </summary>
        /// <param name="FileName"></param>
        public TBLStream()
        {
            _DTEList.Clear();
            _FileName = "";
        }

        #endregion Constructeurs

        #region Indexer

        /// <summary>
        /// Indexeur permetant de travailler sur les DTE contenue dans TBL a la facons d'un tableau.
        /// </summary>
        public DTE this[int index]
        {   // declaration de indexer
            get
            {
                // verifie la limite de l'index
                if (index < 0 || index > _DTEList.Count)
                    throw new IndexOutOfRangeException("Cette item n'existe pas");
                else
                    return _DTEList[index];
            }
            set
            {
                if (!(index < 0 || index >= _DTEList.Count))
                    _DTEList[index] = value;
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
        public string FindTBLMatch(string hex, bool showSpecialValue)
        {
            string rtn = "#";
            foreach (DTE dte in _DTEList)
            {
                if (dte.Entry == hex)
                {
                    rtn = dte.Value;
                    break;
                }

                if (showSpecialValue)
                {
                    if (dte.Entry == ("/" + hex))
                    {
                        rtn = "<end>";
                        break;
                    }
                    else if (dte.Entry == ("*" + hex))
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
        /// <param name="showSpecialValue">Afficher les valeurs de fin de block et de ligne</param>
        /// <returns>Retourne le DTE/MTE trouvé. null si rien trouvé</returns>
        public DTE GetDTE(string hex)
        {
            foreach (DTE dte in _DTEList)
            {
                if (dte.Entry == hex)
                    return dte;

                if (dte.Entry == ("/" + hex))
                    return dte;
                else if (dte.Entry == ("*" + hex))
                    return dte;
            }

            return null;
        }

        /// <summary>
        /// Trouver une entré dans la table de jeu qui corestpond a la valeur hexa
        /// </summary>
        /// <param name="hex">Valeur hexa a rechercher dans la TBL</param>
        /// <param name="showSpecialValue">Afficher les valeurs de fin de block et de ligne</param>
        /// <returns></returns>
        public string FindTBLMatch(string hex)
        {
            string rtn = "#";
            foreach (DTE dte in _DTEList)
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
        /// <returns></returns>
        public string FindTBLMatch(string hex, bool showSpecialValue, bool NotShowDTE)
        {
            string rtn = "#";
            foreach (DTE dte in _DTEList)
            {
                if (dte.Entry == hex)
                {
                    if (NotShowDTE)
                    {
                        if (dte.Type == DTEType.DualTitleEncoding)
                            break;
                        else
                        {
                            rtn = dte.Value;
                            break;
                        }
                    }
                    else
                    {
                        rtn = dte.Value;
                        break;
                    }
                }

                if (showSpecialValue)
                {
                    if (dte.Entry == ("/" + hex))
                    {
                        rtn = "<end>";
                        break;
                    }
                    else if (dte.Entry == ("*" + hex))
                    {
                        rtn = "<ln>";
                        break;
                    }
                }
            }

            return rtn;
        }

        /// <summary>
        /// Chargé le fichier dans l'objet
        /// </summary>
        /// <returns>Retoune vrai si le fichier est bien charger</returns>
        private bool Load()
        {
            //Vide la collection
            _DTEList.Clear();
            //ouverture du fichier

            if (!File.Exists(_FileName))
            {
                FileStream fs = File.Create(_FileName);
                fs.Close();
            }

            StreamReader TBLFile = new StreamReader(_FileName, Encoding.ASCII);

            if (TBLFile.BaseStream.CanRead)
            {
                //lecture du fichier jusqua la fin et séparation par ligne
                char[] sepEndLine = { '\n' }; //Fin de ligne
                char[] sepEqual = { '=' }; //Fin de ligne

                //build strings line
                StringBuilder textFromFile = new StringBuilder(TBLFile.ReadToEnd());
                textFromFile.Insert(textFromFile.Length, '\r');
                textFromFile.Insert(textFromFile.Length, '\n');
                string[] lines = textFromFile.ToString().Split(sepEndLine);

                //remplir la collection de DTE : this._DTE
                foreach (string line in lines)
                {
                    string[] info = line.Split(sepEqual);

                    //ajout a la collection (ne prend pas encore en charge le Japonais)
                    DTE dte = new DTE();
                    try
                    {
                        switch (info[0].Length)
                        {
                            case 2:
                                if (info[1].Length == 2)
                                    dte = new DTE(info[0], info[1].Substring(0, info[1].Length - 1), DTEType.ASCII);
                                else
                                    dte = new DTE(info[0], info[1].Substring(0, info[1].Length - 1), DTEType.DualTitleEncoding);
                                break;

                            case 4: // >2
                                dte = new DTE(info[0], info[1].Substring(0, info[1].Length - 1), DTEType.MultipleTitleEncoding);
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
                                dte = new DTE(info[0].Substring(0, info[0].Length - 1), "", DTEType.EndBlock);
                                break;

                            case @"*":
                                dte = new DTE(info[0].Substring(0, info[0].Length - 1), "", DTEType.EndLine);
                                break;
                            //case @"\":
                            default:
                                continue;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    { //Du a une entre qui a 2 = de suite... EX:  XX==
                        dte = new DTE(info[0], "=", DTEType.DualTitleEncoding);
                    }

                    _DTEList.Add(dte);
                }

                //Load bookmark
                BookMarks.Clear();
                BookMark fav = null;
                string[] lineSplited;

                foreach (string line in lines)
                {
                    try
                    {
                        if (line.Substring(0, 1) == "(")
                        {
                            fav = new BookMark();
                            lineSplited = line.Split(new char[] { ')' });
                            fav.Description = lineSplited[1].Substring(0, lineSplited[1].Length - 1);

                            lineSplited = line.Split(new char[] { 'h' });
                            fav.BytePositionInFile = ByteConverters.HexLiteralToLong(lineSplited[0].Substring(1, lineSplited[0].Length - 1));
                            fav.Marker = ScrollMarker.TBLBookmark;
                            BookMarks.Add(fav);
                        }
                    }
                    catch { } //Nothing to add if error
                }

                TBLFile.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Enregistrer dans le fichier
        /// </summary>
        /// <returns>Retourne vrai si le fichier à été bien enregistré</returns>
        public bool Save()
        {
            //ouverture du fichier
            FileStream myFile = new FileStream(_FileName, FileMode.Create, FileAccess.Write);
            StreamWriter TBLFile = new StreamWriter(myFile, Encoding.ASCII);

            if (TBLFile.BaseStream.CanWrite)
            {
                //Save tbl set
                foreach (DTE dte in _DTEList)
                    if (dte.Type != DTEType.EndBlock && dte.Type != DTEType.EndLine)
                        TBLFile.WriteLine(dte.Entry + "=" + dte.Value);
                    else
                        TBLFile.WriteLine(dte.Entry);

                //Save bookmark
                TBLFile.WriteLine();
                foreach (BookMark mark in BookMarks)
                    TBLFile.WriteLine(mark.ToString());

                //Ecriture de 2 saut de ligne a la fin du fichier.
                //(obligatoire pour certain logiciel utilisant les TBL)
                TBLFile.WriteLine();
                TBLFile.WriteLine();
            }

            //Ferme le fichier TBL
            TBLFile.Close();

            return true;
        }

        /// <summary>
        /// Ajouter un element a la collection
        /// </summary>
        /// <param name="dte">objet DTE a ajouter fans la collection</param>
        public void Add(DTE dte)
        {
            _DTEList.Add(dte);
        }

        /// <summary>
        /// Effacer un element de la collection a partir d'un objet DTE
        /// </summary>
        /// <param name="dte"></param>
        public void Remove(DTE dte)
        {
            _DTEList.Remove(dte);
        }

        /// <summary>
        /// Effacer un element de la collection avec son index dans la collection
        /// </summary>
        /// <param name="index">Index de l'element a effacer</param>
        public void Remove(int index)
        {
            _DTEList.RemoveAt(index);
        }

        /// <summary>
        /// Recherche un élément dans la TBL
        /// </summary>
        /// <param name="dte">Objet DTE a rechercher dans la TBL</param>
        /// <returns>Retourne la position ou ce trouve cette élément dans le tableau</returns>
        public int Find(DTE dte)
        {
            return _DTEList.BinarySearch(dte);
        }

        /// <summary>
        /// Recherche un élément dans la TBL
        /// </summary>
        /// <param name="Entry">Entrée sous forme hexadécimal (XX)</param>
        /// <param name="Value">Valeur de l'entré</param>
        /// <returns>Retourne la position ou ce trouve cette élément dans le tableau</returns>
        public int Find(string Entry, string Value)
        {
            DTE dte = new DTE(Entry, Value);
            return _DTEList.BinarySearch(dte);
        }

        /// <summary>
        /// Recherche un élément dans la TBL
        /// </summary>
        /// <param name="Entry">Entrée sous forme hexadécimal (XX)</param>
        /// <param name="Value">Valeur de l'entré</param>
        /// <param name="Type">Type de DTE</param>
        /// <returns>Retourne la position ou ce trouve cette élément dans le tableau</returns>
        public int Find(string Entry, string Value, DTEType Type)
        {
            DTE dte = new DTE(Entry, Value, Type);
            return _DTEList.BinarySearch(dte);
        }

        #endregion Méthodes

        #region Propriétés

        /// <summary>
        /// Chemin d'acces au fichier (path)
        /// La fonction load doit etre appeler pour rafraichir la fonction
        /// </summary>
        [ReadOnly(true)]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            internal set
            {
                _FileName = value;

                Load();
            }
        }

        /// <summary>
        /// Total d'élement dans l'objet TBL
        /// </summary>
        public int Length
        {
            get
            {
                return _DTEList.Count;
            }
        }

        /// <summary>
        /// Avoir acess au Bookmark
        /// </summary>
        [Browsable(false)]
        public List<BookMark> BookMarks { get; set; } = new List<BookMark>();

        /// <summary>
        /// Obtenir le total d'entré DTE dans la Table
        /// </summary>
        public int TotalDTE
        {
            get
            {
                return _DTEList.Count(l => l.Type == DTEType.DualTitleEncoding);
            }
        }

        /// <summary>
        /// Obtenir le total d'entré MTE dans la Table
        /// </summary>
        public int TotalMTE
        {
            get
            {
                return _DTEList.Count(l => l.Type == DTEType.MultipleTitleEncoding);
            }
        }

        /// <summary>
        /// Obtenir le total d'entré ASCII dans la Table
        /// </summary>
        public int TotalASCII
        {
            get
            {
                return _DTEList.Count(l => l.Type == DTEType.ASCII);
            }
        }

        /// <summary>
        /// Obtenir le total d'entré Invalide dans la Table
        /// </summary>
        public int TotalInvalid
        {
            get
            {
                return _DTEList.Count(l => l.Type == DTEType.Invalid);
            }
        }

        /// <summary>
        /// Obtenir le total d'entré Japonais dans la Table
        /// </summary>
        public int TotalJaponais
        {
            get
            {
                return _DTEList.Count(l => l.Type == DTEType.Japonais);
            }
        }

        /// <summary>
        /// Obtenir le total d'entré Fin de ligne dans la Table
        /// </summary>
        public int TotalEndLine
        {
            get
            {
                return _DTEList.Count(l => l.Type == DTEType.EndLine);
            }
        }

        /// <summary>
        /// Obtenir le total d'entré Fin de Block dans la Table
        /// </summary>
        public int TotalEndBlock
        {
            get
            {
                return _DTEList.Count(l => l.Type == DTEType.EndBlock);
            }
        }

        /// <summary>
        /// Renvoi le caractere de fin de block
        /// </summary>
        public string EndBlock
        {
            get
            {
                foreach (DTE dte in _DTEList)
                    if (dte.Type == DTEType.EndBlock)
                        return dte.Entry;

                return "";
            }
        }

        /// <summary>
        /// Renvoi le caractere de fin de ligne
        /// </summary>
        public string EndLine
        {
            get
            {
                foreach (DTE dte in _DTEList)
                    if (dte.Type == DTEType.EndLine)
                        return dte.Entry;

                return "";
            }
        }

        /// <summary>
        /// Enable/Disable Readonly on control.
        /// </summary>
        public bool AllowEdit { get; set; } = false;

        #endregion Propriétés

        #region Build default TBL
        public static TBLStream CreateDefaultASCII(DefaultCharacterTableType type = DefaultCharacterTableType.ASCII)
        {
            TBLStream tbl = new TBLStream();

            switch (type)
            {
                case DefaultCharacterTableType.ASCII:
                    for (byte i = 0; i < 255; i++)
                    {
                        DTE dte = new DTE(ByteConverters.ByteToHex(i).ToUpper(), $"{ByteConverters.ByteToChar(i)}");
                        tbl.Add(dte);
                    }
                    break;
            }

            tbl.AllowEdit = true;
            return tbl;
        }
        #endregion
    }
}