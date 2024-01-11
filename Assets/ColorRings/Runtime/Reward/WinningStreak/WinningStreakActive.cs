using UnityEngine;

public class WinningStreakActive : MonoBehaviour
{
    private const int LevelActive = 20;
    public static bool isActive = false;

    private void OnEnable()
    {
        if (!ConditionActiveWinningStreak())
        {
            isActive = false;
            return;
        }
        if (GameDataManager.IsInWinningStreakProgress)
        {
            GameDataManager.ResetWinningStreak();
        }
        isActive = true;
        DevLog.Log("WinningStreak", "Active");
        GameDataManager.SetupWinningStreak();   
    }

    private void OnDisable()
    {
        isActive = false;
    }

    private static bool ConditionActiveWinningStreak()
    {
        return LevelActive <= GameDataManager.MaxLevelUnlock;
    }
    
    public static bool IsActiveWinningStreak(int level)
    {
        return LevelActive <= level;
    }
}