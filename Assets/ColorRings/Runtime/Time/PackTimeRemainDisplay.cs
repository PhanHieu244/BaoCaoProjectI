using System;

public class PackTimeRemainDisplay : RemainTimeDisplay
{
    protected override void ShowDays(TimeSpan remainTime)
    {
        timeText.text = ShowTime.GetDay(remainTime, " days");
    }
}