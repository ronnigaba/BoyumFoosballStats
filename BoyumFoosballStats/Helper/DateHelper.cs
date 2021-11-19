using System.Globalization;

namespace BoyumFoosballStats.Helper
{
    public static class DateHelper
    {
        public static int GetCurrentWeekByDate(DateTime date)
        {
            var calendar = new GregorianCalendar();
            var currentWeekByDate = calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            return currentWeekByDate;
        }
    }
}
