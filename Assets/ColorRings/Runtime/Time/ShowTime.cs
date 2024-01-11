using System;

public static class ShowTime
{
    public static string GetDay(TimeSpan timeSpan, string suffix = "D")
    {
        return $"{timeSpan.Days}{suffix}";
    }
    
    public static string GetDayHour(TimeSpan timeSpan)
    {
        return $"{timeSpan.Days}D:{timeSpan.Hours:D2}H";
    }

    public static string GetHourMinutes(TimeSpan timeSpan)
    {
        return $"{timeSpan.Hours:D2}H:{timeSpan.Minutes:D2}M";
    }
        
    public static string GetMinutesSeconds(TimeSpan timeSpan)
    {
        return $"{timeSpan.Minutes:D2}M:{timeSpan.Seconds:D2}S";
    }
}