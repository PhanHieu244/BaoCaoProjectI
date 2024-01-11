using System;

public class RewardTimeRemainDisplay : RemainTimeDisplay
{
    protected override void ShowDays(TimeSpan remainTime)
    {
        timeText.text = ShowTime.GetDayHour(remainTime);
    }
}