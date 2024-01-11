using System;

namespace ColorRings.Runtime.GameMode.AdvancedBoard
{
    public static class WeekEventScheduler
    {
        private static readonly DateTime StartTime = 
            new DateTime(2023, 11, 27, 0, 0, 0, DateTimeKind.Utc);

        public static int GetWeekSinceStart()
        {
            var days = GetDaySinceStart();
            return days / 7;
        }

        public static int GetDaySinceStart()
        {
            var now = DateTime.UtcNow;
            TimeSpan elapsedTime = now - StartTime;
            return (int) elapsedTime.TotalDays;
        }

        public static TimeSpan GetTimeRemain(DateTime endTimeUtc)
        {
            var now = DateTime.UtcNow;
            return endTimeUtc - now;
        }
        
        public static TimeSpan GetTimeRemain()
        {
            var now = DateTime.UtcNow;
            return GetEndOfWeek() - now;
        }

        public static DateTime GetEndOfWeek()
        {
            var endOfWeek = StartTime;
            return endOfWeek.AddDays(7 * (GetWeekSinceStart() + 1));
        }
    }
}