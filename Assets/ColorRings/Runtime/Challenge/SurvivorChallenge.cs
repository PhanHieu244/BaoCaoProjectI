using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SurvivorChallenge : Challenge<SurvivorChallenge>, IChallenge, ILimitStrategy
{
    [SerializeField] private Text timeText;
    private float _secondsTimeLife;
    private bool _stopCount;
    public event Action OnOutOfTime;
    
    private void Start()
    {
        GameManager.Instance.OnLoseGame += (() => _stopCount = true);
        InGamePowerUpManager.Instance.OnRevival += tileCoords => OnRevival();
        StartCoroutine(IEStartChallenge());
    }

    private IEnumerator IEStartChallenge()
    {
        _stopCount = true;
        UpdateUI();
        yield return new WaitForSeconds(1.1f);
        _stopCount = false;
    }

    private void OnRevival()
    {
        _secondsTimeLife += GameConst.TimeRevivalInBonusLevel;
        StartCoroutine(IEStartChallenge());
    }

    public void Set(SurvivalData challengeData)
    {
        _secondsTimeLife = challengeData.secondsTimeLife;
    }

    public bool IsOutOfLimit()
    {
        var isOutTime = _secondsTimeLife <= 0;
        _stopCount = isOutTime || _stopCount;
        return isOutTime;
    }

    private void FixedUpdate()
    {
        if (_stopCount) return;
        CountDown();
        if (Time.frameCount % 10 != 0) return;
        UpdateUI();
    }

    private void CountDown()
    {
        _secondsTimeLife -= Time.fixedDeltaTime;
        if (!IsOutOfLimit()) return;
        _stopCount = true;
        OnOutOfTime?.Invoke();
    }

    private void UpdateUI()
    {
        timeText.text = GetTextTime();
    }

    private string GetTextTime()
    {
        var time = ((int)_secondsTimeLife);
        var minutes = time / 60;
        var seconds = time % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    public bool IsComplete()
    {
        return false;
    }

    public void SetComplete()
    {
        
    }

    [Serializable]
    public class SurvivalData : IChallengeData
    {
        public int secondsTimeLife;
    }
}

public interface ILimitStrategy
{
    bool IsOutOfLimit();
}
