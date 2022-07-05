using System;

namespace UDO.LOB.DependentMaintenance
{
    public static class DateTimeExtensions
    {
        public static DateTime TodayNoon
        {
            get
            {
                var today = DateTime.Today;

                return today.AddHours(12);
            }
        }
    }
}