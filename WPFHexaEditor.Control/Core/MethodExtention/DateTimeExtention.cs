//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;


namespace WPFHexaEditor.Core.MethodExtention
{
    public static class DateTimeExtention
    {
        /// <summary>
        /// Permet d'obternir un DateTime ne comportant que la date.
        /// </summary>        
        public static DateTime ToShortDateTime(this DateTime s)
        {
            return DateTime.Parse(s.ToShortDateString());
        }

        /// <summary>
        /// Méthode d'extension pour trouver la première journée de la semaine
        /// </summary>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Ajout des jours de travail a la date
        /// </summary>
        /// <param name="originalDate"></param>
        /// <param name="workDays"></param>
        /// <returns></returns>
        public static DateTime AddWorkdays(this DateTime originalDate, int workDays)
        {
            DateTime tmpDate = originalDate;
            while (workDays > 0)
            {
                tmpDate = tmpDate.AddDays(1);
                if (tmpDate.DayOfWeek < DayOfWeek.Saturday &&
                    tmpDate.DayOfWeek > DayOfWeek.Sunday &&
                    !tmpDate.IsHoliday())
                    workDays--;
            }
            return tmpDate;
        }

        /// <summary>
        /// Enleve des jours de travail a la date
        /// </summary>
        /// <param name="originalDate"></param>
        /// <param name="workDays"></param>
        /// <returns></returns>
        public static DateTime SubtractWorkdays(this DateTime originalDate, int workDays)
        {
            DateTime tmpDate = originalDate;
            while (workDays > 0)
            {
                tmpDate = tmpDate.AddDays(-1);
                if (tmpDate.DayOfWeek < DayOfWeek.Saturday &&
                    tmpDate.DayOfWeek > DayOfWeek.Sunday &&
                    !tmpDate.IsHoliday())
                    workDays--;
            }
            return tmpDate;
        }

        /// <summary>
        /// Détermine si la date est un jour de fête...
        /// TODO: Charger les dates a partir de la base de donnée.
        /// </summary>
        /// <param name="originalDate"></param>
        /// <returns>FALSE car non implementer pour le moment</returns>
        public static bool IsHoliday(this DateTime originalDate)
        {
            return false;
        }        
    }
}
