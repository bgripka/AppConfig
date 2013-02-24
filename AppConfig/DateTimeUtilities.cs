using System;
using System.Collections.Generic;
using System.Text;

namespace AppConfig
{
    public class DateTimeUtilities
    {
        public static string GetRelativeTimeDescription(DateTime dateTime)
        {
            TimeSpan timeElapsed = DateTime.Now - dateTime;

            if (timeElapsed.Ticks < 0)
                throw new NotSupportedException("DateTime values that are in the future are not supported.");

            if (timeElapsed.TotalSeconds < 60)
                return "Moments Ago";
            else if (timeElapsed.TotalMinutes == 1)
                return "1 Minute Ago";
            else if (timeElapsed.TotalHours < 1)
                return timeElapsed.TotalMinutes.ToString("#0") + " Minutes Ago";
            else if (timeElapsed.TotalMinutes < 120)
                return " 1 Hour and " + timeElapsed.Minutes + " Minutes Ago";
            else if (timeElapsed.TotalHours < 24 && dateTime.Date == DateTime.Now.Date)
                return "About " + timeElapsed.TotalHours.ToString("#0") + " Hours Ago";
            else if (timeElapsed.TotalHours < 48 && dateTime.Date == DateTime.Now.Date.AddDays(-1))
                return "Yesterday";
            else if (timeElapsed.TotalDays < 7)
                return dateTime.DayOfWeek.ToString();
            else if (timeElapsed.TotalDays < 14)
                return "Last " + dateTime.DayOfWeek.ToString();
            else
                return dateTime.ToString("D");
        }
    }
}
