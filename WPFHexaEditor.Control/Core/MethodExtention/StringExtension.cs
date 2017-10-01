//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////


using System;
using System.Text.RegularExpressions;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class StringExtension
    {
        /// <summary>
        /// Indique si l'adresse email est valide
        /// </summary>        
        public static bool IsValidEmailAddress(this string s)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(s);
        }

        /// <summary>
        /// Retourne le nombre de mot contenue dans la string
        /// </summary>        
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Retourne True si le chaine est numerique
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string s)
        {
            long test = 0;

            try
            {
                test = Convert.ToInt64(s);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Retourne seulement un nombre sous forme de string contenue la string
        /// </summary>        
        public static string GetNumberInString(this string s)
        {
                return Regex.Match(s, @"\d+").Value;
        }
    }
}
