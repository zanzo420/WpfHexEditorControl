//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;

namespace WPFHexaEditor.Core.CharacterTable
{
    /// <summary>
    /// Objet représentant un DTE.
    ///
    /// Derek Tremblay 2003-2017
    /// </summary>
    public class DTE
    {
        /// <summary>Nom du DTE</summary>
        private string _Entry;

        #region Constructeurs

        /// <summary>
        /// Constructeur principal
        /// </summary>
        public DTE()
        {
            _Entry = "";
            Type = DTEType.Invalid;
            Value = "";
        }

        /// <summary>
        /// Contructeur permetant d'ajouter une entrée et une valeur
        /// </summary>
        /// <param name="Entry">Nom du DTE</param>
        /// <param name="Value">Valeur du DTE</param>
        public DTE(string entry, string value)
        {
            _Entry = entry;
            Value = value;
            Type = DTEType.DualTitleEncoding;
        }

        /// <summary>
        /// Contructeur permetant d'ajouter une entrée, une valeur et une description
        /// </summary>
        /// <param name="Entry">Nom du DTE</param>
        /// <param name="Value">Valeur du DTE</param>
        /// <param name="Description">Description du DTE</param>
        /// <param name="Type">Type de DTE</param>
        public DTE(string entry, string value, DTEType type)
        {
            _Entry = entry;
            Value = value;
            Type = type;
        }

        #endregion Constructeurs

        #region Propriétés

        /// <summary>
        /// Nom du DTE
        /// </summary>
        public string Entry
        {
            set
            {
                _Entry = value.ToUpper();
            }
            get
            {
                return _Entry;
            }
        }

        /// <summary>
        /// Valeur du DTE
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Type de DTE
        /// </summary>
        public DTEType Type { get; set; }

        #endregion Propriétés

        #region Méthodes

        /// <summary>
        /// Cette fonction permet de retourner le DTE sous forme : [Entry]=[Valeur]
        /// </summary>
        /// <returns>Retourne le DTE sous forme : [Entry]=[Valeur]</returns>
        public override string ToString()
        {
            if (Type != DTEType.EndBlock &&
                Type != DTEType.EndLine)
                return _Entry + "=" + Value;
            else
                return _Entry;
        }

        #endregion Méthodes

        #region Methodes Static

        public static DTEType TypeDTE(DTE DTEValue)
        {
            try
            {
                switch (DTEValue._Entry.Length)
                {
                    case 2:
                        if (DTEValue.Value.Length == 2)
                            return DTEType.ASCII;
                        else
                            return DTEType.DualTitleEncoding;

                    case 4: // >2
                        return DTEType.MultipleTitleEncoding;
                }
            }
            catch (IndexOutOfRangeException)
            {
                switch (DTEValue._Entry)
                {
                    case @"/":
                        return DTEType.EndBlock;

                    case @"*":
                        return DTEType.EndLine;
                        //case @"\":
                }
            }
            catch (ArgumentOutOfRangeException)
            { //Du a une entre qui a 2 = de suite... EX:  XX==
                return DTEType.DualTitleEncoding;
            }

            return DTEType.Invalid;
        }

        public static DTEType TypeDTE(string DTEValue)
        {
            try
            {
                switch (DTEValue)
                {
                    case @"<end>":
                        return DTEType.EndBlock;

                    case @"<ln>":
                        return DTEType.EndLine;
                        //case @"\":
                }

                if (DTEValue.Length == 1)
                    return DTEType.ASCII;
                else if (DTEValue.Length == 2)
                    return DTEType.DualTitleEncoding;
                else if (DTEValue.Length > 2)
                    return DTEType.MultipleTitleEncoding;
            }
            catch (ArgumentOutOfRangeException)
            { //Du a une entre qui a 2 = de suite... EX:  XX==
                return DTEType.DualTitleEncoding;
            }

            return DTEType.Invalid;
        }

        #endregion Methodes Static
    }
}