using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets a DateTime representing the first day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime First(this DateTime current)
        {
            DateTime first = current.AddDays(1 - current.Day);
            return first;
        }

        /// <summary>
        /// Gets a DateTime representing the last day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime Last(this DateTime current)
        {
            var daysInMonth = DateTime.DaysInMonth(current.Year, current.Month);

            DateTime last = current.First().AddDays(daysInMonth - 1);
            return last;
        }

        /// <summary>
        /// Gets a DateTime representing the first day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime FirstNextMonth(this DateTime current)
        {
            return current.Last().AddDays(1);
        }

        /// <summary>
        /// Gets a DateTime representing the first day in the current month
        /// </summary>
        /// <param name="current">The current date</param>
        /// <returns></returns>
        public static DateTime LastNextMonth(this DateTime current)
        {
            return current.FirstNextMonth().Last();
        }


        public static DateTime? EndOfDay(this DateTime? dateTime)
        {
            return dateTime == null ? null : (DateTime?) dateTime.Value.EndOfDay();
        }

        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

         public static DateTime? StartOfDay(this DateTime? dateTime)
         {
             return dateTime == null ? null : (DateTime?)dateTime.Value.StartOfDay();
         }

        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        public static string ToDDMMyyyHHmm(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm");
        }

        public static string ToDDMMyyyHHmm(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.GetValueOrDefault().ToDDMMyyyHHmm() : "";
        }

        public static string ToDDMMyyy(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }

        public static string ToDDMMyyyWithSlash(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }

        public static string ToDDMMyyy(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.GetValueOrDefault().ToDDMMyyy() : "";
        }

        public static string ToddMMMyyyy(this DateTime? value, string defaultValue="")
        {
            return value.HasValue ? value.Value.ToddMMMyyyy() : defaultValue;
        }

        public static string ToddMMMyyyy(this DateTime dateTime)
        {
            return dateTime.ToString("dd.MMM.yyyy");
        }

        public static string FormatLongDatePatternWithTime(this DateTime dateTime)
        {
            return dateTime.ToString("f");
        }

        public static string FormatLongDatePatternWithTime(this DateTime? dateTime)
        {
            return dateTime == null ? string.Empty : dateTime.Value.FormatLongDatePatternWithTime();
        }

        public static string FormatLongDatePattern(this DateTime dateTime)
        {
            return dateTime.ToString("D");
        }

        public static string FormatLongDatePattern(this DateTime? dateTime)
        {
            return dateTime == null ? string.Empty : dateTime.Value.FormatLongDatePattern();
        }

        public static string FormatDDMMMYYYY(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MMM/yyyy");
        }

        public static string FormatDDMMMYYYYHHmm(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MMM/yyyy HH:mm");
        }

         public static string FormatDDMMMYYYYHHmm(this DateTime? dateTime)
         {
             return dateTime.HasValue ? dateTime.Value.FormatDDMMMYYYYHHmm() : "";
         }

        public static string FormatDDMMMYYYY(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.FormatDDMMMYYYY() : "";
        }

        public static TimeSpan ToTimeSpan(this DateTime dateTime)
        {
            return dateTime.TimeOfDay;
        }

        public static TimeSpan? ToTimeSpan(this DateTime? dateTime)
        {
            return dateTime == null ? null : (TimeSpan?)ToTimeSpan(dateTime.Value);
        }

        public static DateTime ToDateTime(this TimeSpan timeSpan)
        {
            var currentDateTime = DateTime.Now;
            return new DateTime(currentDateTime.Year,currentDateTime.Month, currentDateTime.Day, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        public static DateTime? ToDateTime(this TimeSpan? timeSpan)
        {
            return timeSpan == null ? null : (DateTime?) ToDateTime(timeSpan.Value);
        }

        private static string s_Am = "am";
        private static string s_Pm = "pm";

        public static String ToHHmm(this TimeSpan timeSpan)
        {
            return "{0}:{1} {2}".F((timeSpan.Hours > 12 || timeSpan.Hours == 0 ? Math.Abs(timeSpan.Hours - 12) : timeSpan.Hours).ToString("0"), timeSpan.Minutes.ToString("0#"), timeSpan.Hours > 11  ? s_Pm : s_Am);
        }

        public static String ToHHmm(this TimeSpan? timeSpan)
        {
            return timeSpan == null ? null : timeSpan.Value.ToHHmm();
        }

        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Now.Date;
        }

        public static bool IsToday(this DateTime? dateTime)
        {
            return dateTime.HasValue ? IsToday(dateTime.Value) : false;
        }

        public static bool IsTomorrow(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Now.Date.AddDays(1);
        }

        public static bool IsTomorrow(this DateTime? dateTime)
        {
            return dateTime.HasValue ? IsTomorrow(dateTime.Value) : false;
        }

        public static DateTime FromUnixTime(this int unixTime)
        {
            return FromUnixTime((long) unixTime);
        }

        public static DateTime FromUnixTime(this long unixTime)
        {
            return (new DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(unixTime);
        }
        
        public static string TimeToDate(this DateTime dateTime)
        {
            var today = DateTime.Now;
            var timeDifference = dateTime - today;
            var returnValue = "";
            if (timeDifference.Days > 0)
                returnValue += "{0} days".F(timeDifference.Days);

            if(timeDifference.Hours > 0)
                returnValue += (returnValue.Length > 0 ? ", " : "") + "{0} hrs".F(timeDifference.Hours);

            if (timeDifference.Minutes > 0)
                returnValue += (returnValue.Length > 0 ? ", " : "") + "{0} mins".F(timeDifference.Minutes);
            return returnValue;
        }

        public static string TimeToDate(this DateTime? dateTime)
        {
            return dateTime == null ? string.Empty : dateTime.Value.TimeToDate();
        }
       
    }
}
