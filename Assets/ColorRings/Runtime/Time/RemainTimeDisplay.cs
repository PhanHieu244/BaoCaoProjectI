using System;
using System.Collections;
using ColorRings.Runtime.GameMode.AdvancedBoard;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RemainTimeDisplay : MonoBehaviour
{
    [SerializeReference, SubclassSelector] protected ITargetTimeStrategy targetTimeStrategy = new EndWeekTargetTime();
    [SerializeField] private float secondsUpdateHM = 10f;
    [SerializeField] private int DaysSize = 50;
    [SerializeField] private int HMSize = 30;
    [SerializeField] private int MSSize = 30;
    public event Action OnExpireTime;
    protected Text timeText;
    
    private void Awake()
    {
        timeText = GetComponent<Text>();
    }

    private void OnEnable()
    {
        ShowTimeText();
    }

    private void ShowTimeText()
    {
        var remainTime = targetTimeStrategy.RemainTime;
        if (remainTime.Days > 0)
        {
            timeText.fontSize = DaysSize;
            ShowDays(remainTime);
            return;
        }

        if (remainTime.Hours > 0)
        {
            timeText.fontSize = HMSize;
            ShowHourMinute();
            return;
        }
        timeText.fontSize = MSSize;
        ShowMinuteSecond();
    }

    protected virtual void ShowDays(TimeSpan remainTime)
    {
        timeText.text = ShowTime.GetDay(remainTime);
    }

    private void ShowHourMinute()
    {
        StartCoroutine(IEShowHourMinute());
    }

    private IEnumerator IEShowHourMinute()
    {
        var endTime = targetTimeStrategy.EndTime;
        var wait = new WaitForSeconds(secondsUpdateHM);
        while (true)
        {
            DevLog.Log("UPDATE TIME");
            var remainTime = targetTimeStrategy.GetRemainTime(endTime);
            timeText.text = ShowTime.GetHourMinutes(remainTime);
            if(remainTime.Hours <= 0) break;
            yield return wait;
        }
        ShowTimeText();
    }
    
    private void ShowMinuteSecond()
    {
        StartCoroutine(IEShowMinuteSecond());
    }

    private IEnumerator IEShowMinuteSecond()
    {
        var wait = new WaitForSeconds(1f);
        while (true)
        {
            DevLog.Log("UPDATE TIME");
            var remainTime = targetTimeStrategy.RemainTime;
            timeText.text = ShowTime.GetMinutesSeconds(remainTime);
            if (remainTime.TotalSeconds < 1)
            {
                OnExpireTime?.Invoke();
                yield return wait;
                break;
            }
            if(remainTime.Days > 0) break; //break to show new target time
            yield return wait;
        }    
        ShowTimeText();
    }
}
