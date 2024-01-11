using System.Collections;
using ColorRings.Runtime.UI;
using UnityEngine;

public class WinningStreakBar : BaseWinningStreakBar
{
    [SerializeField] protected BlockUI blockUI;
    
    private void OnEnable()
    {
        blockUI.Block();
        var currentStreak = GameDataManager.CurrentWinningStreak;
        DevLog.Log("current", currentStreak);
        SetUpBar(currentStreak);
        if (currentStreak < GameDataManager.WinningStreak)
        {
            StartCoroutine(IEIncrease());
            return;
        }
        StartCoroutine(IEDecrease());
    }

    private IEnumerator IEDecrease()
    {
        for (int i = minStreak - 1; i >= 0; i--)
        {
            while (progressBars[i].fillAmount > 0)
            {
                progressBars[i].fillAmount -= Time.deltaTime * duration;
                yield return null;
            }
        }
        blockUI.UnBlock();
    }

    private IEnumerator IEIncrease()
    {
        var streak = Mathf.Min(minStreak + 1, progressBars.Length) - 1;
        while (progressBars[streak].fillAmount < 1)
        {
            progressBars[streak].fillAmount += Time.deltaTime * duration;
            yield return null;
        }
        blockUI.UnBlock();
    }
}