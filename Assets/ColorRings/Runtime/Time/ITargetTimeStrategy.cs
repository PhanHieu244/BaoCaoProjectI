using System;
using ColorRings.Runtime.GameMode.AdvancedBoard;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;

public interface ITargetTimeStrategy
{
    DateTime EndTime { get; }
    TimeSpan RemainTime { get; }
    TimeSpan GetRemainTime(DateTime endTime);
}

[Serializable]
public class EndWeekTargetTime : ITargetTimeStrategy
{
    public DateTime EndTime => WeekEventScheduler.GetEndOfWeek();
    public TimeSpan RemainTime => WeekEventScheduler.GetTimeRemain();
    public TimeSpan GetRemainTime(DateTime endTime)
    {
        return WeekEventScheduler.GetTimeRemain(endTime);
    }
}

[Serializable]
public class ChristmasExpireTargetTime : ITargetTimeStrategy
{
    public DateTime EndTime => GetEndTime();
    public TimeSpan RemainTime => GetEndTime() - DateTime.UtcNow;
    public TimeSpan GetRemainTime(DateTime endTime)
    {
        return endTime - DateTime.UtcNow;
    }

    private DateTime GetEndTime()
    {
        var convertDate = new ConvertDate();
        var expireDate = convertDate.ParseStringToData(FalconConfig.Instance<IAPDateConfig>()
            .christmasPackExpireDate, out var canParse);
        if(!canParse) return DateTime.UtcNow.AddDays(-1);
        return expireDate;
    }
}

[Serializable]
public class NoAdsExpireTargetTime : ITargetTimeStrategy
{
    public DateTime EndTime => IAPSubscription.Instance.GetExpireDateTime(IAPKey.S_NO_ADS);
    public TimeSpan RemainTime => EndTime - DateTime.UtcNow;
    public TimeSpan GetRemainTime(DateTime endTime)
    {
        return endTime - DateTime.UtcNow;
    }
}