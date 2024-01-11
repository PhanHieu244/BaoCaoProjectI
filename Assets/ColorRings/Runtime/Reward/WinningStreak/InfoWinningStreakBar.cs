using System;

public class InfoWinningStreakBar : BaseWinningStreakBar
{
    private void OnEnable()
    {
        if (GameDataManager.IsInWinningStreakProgress)
        {
            GameDataManager.ResetWinningStreak();
        }
        var currentStreak = GameDataManager.WinningStreak;
        SetUpBar(currentStreak);
    }
}