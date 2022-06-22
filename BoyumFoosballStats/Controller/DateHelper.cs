using System.Globalization;

namespace BoyumFoosballStats.Controller
{
    public static class DateHelper
    {
        public static string GetCurrentWeekByDate(DateTime date)
        {
            var calendar = new GregorianCalendar();
            var currentWeekByDate = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            return $"{date.Year}/{currentWeekByDate.ToString("00")}";
        }
    }
}
