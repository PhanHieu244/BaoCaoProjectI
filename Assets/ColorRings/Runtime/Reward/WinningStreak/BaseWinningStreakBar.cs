using UnityEngine;
using UnityEngine.UI;

public class BaseWinningStreakBar : MonoBehaviour
{
    [SerializeField] protected float duration = 1f;
    [SerializeField] protected Image[] progressBars;
    protected int minStreak;

    protected void SetUpBar(int currentStreak)
    {
        minStreak = Mathf.Min(currentStreak, progressBars.Length);
        for (int i = 0; i < minStreak ; i++)
        {
            progressBars[i].fillAmount = 1;
        }
        for (int i = minStreak; i < progressBars.Length; i++)
        {
            progressBars[i].fillAmount = 0;
        }
    }

}