using System;
using UnityEngine;

public class WinningStreakAuthor : MonoBehaviour
{
    [SerializeField] private TrayWinningStreakContainer tray;
    [SerializeField] private WinningStreakRewardData[] rewardPackages;
    
    private void Start()
    {
        Reward();
    }

    private void Reward()
    {
        GameDataManager.JoinWinningStreakProgress();
        Reward(GameDataManager.WinningStreak);   
    }

    private void Reward(int streak)
    {
        if (streak <= 0)
        {
            DevLog.Log("dont have streak");
            return;
        }
        //get index package reward
        int indexPackage = -1;
        int maxStreakAvailable = -1;
        for (int i = 0; i < rewardPackages.Length; i++)
        {
            var streakInPack = rewardPackages[i].streak;
            if (streakInPack > streak) continue;
            if(streakInPack < maxStreakAvailable) continue;
            maxStreakAvailable = streakInPack;
            indexPackage = i;
        }

        if (indexPackage < 0)
        {
            DevLog.Log("reward is not available");
            return;
        }
        DevLog.Log("id winning pack", indexPackage);
        //UIManager.onWinningStreakReward.Invoke(rewardPackages[indexPackage].powerUpTypes);
        tray.gameObject.SetActive(true);
        tray.Setup(rewardPackages[indexPackage].powerUpTypes);
    }
}

[Serializable]
public class WinningStreakRewardData
{
    public int streak;
    public PowerUpType[] powerUpTypes;
}
