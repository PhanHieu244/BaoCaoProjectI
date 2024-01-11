using System;
using System.Collections;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;

public interface IWinLoseUIStrategy
{
    void Initialize();
    IEnumerator OnWinGame();
    IEnumerator OnLoseGame();
}

public abstract class WinLoseUIStrategy : IWinLoseUIStrategy
{
    public abstract void Initialize();
    public abstract IEnumerator OnWinGame();
    public abstract IEnumerator OnLoseGame();
}

[Serializable]
public class MainWinLoseUIStrategy : WinLoseUIStrategy
{
    [SerializeField] private WinUIType winUIType = WinUIType.REWARD_POWERUP;
    [SerializeField] private LoseUIType loseUIType = LoseUIType.REVIVAL_COIN_NORMAL;
    private string _winUIPath;
    private string _loseUIPath;
    private ParticleSystem _currentFireWork;
    private ParticleSystem _currentConfetti;
    
    public override void Initialize()
    {
        SetUpUIPath();
        if (_currentFireWork != null)
        {
            PoolManager.Instance["FireWork"].Release(_currentFireWork);
        }
        
        if (_currentConfetti != null)
        {
            PoolManager.Instance["Confetti"].Release(_currentConfetti);
        }

        Hub.Hide(Hub.Get<PopUpContent>(PopUpPath.POP_UP_UI_ADSBREAK).gameObject).Play();
    }

    private void SetUpUIPath()
    {
        _winUIPath = winUIType switch
        {
            //WinUIType.REWARD_POWERUP => PopUpPath.POP_UP_WOOD__UI_WIN,
            WinUIType.REWARD_COIN => PopUpPath.POP_UP_WOOD__UI_REWARDCOINWIN,
            _ => PopUpPath.POP_UP_WOOD__UI_REWARDCOINWIN
        };

        _loseUIPath = loseUIType switch
        {
            //LoseUIType.REVIVAL_CLASSIC => PopUpPath.POP_UP_WOOD__UI_REVIVAL,
            LoseUIType.REVIVAL_COIN_NORMAL => PopUpPath.POP_UP_WOOD__UI__NORMALMODE_REVIVALBYCOIN,
            LoseUIType.REVIVAL_COIN_ENDLESSCLASSIC => PopUpPath.POP_UP_WOOD__UI__ENDLESSCLASSIC_REVIVAL,
            _ => PopUpPath.POP_UP_WOOD__UI__NORMALMODE_REVIVALBYCOIN
        };
    }

    public override IEnumerator OnLoseGame()
    {
        GameLog.Instance.OnLose();
        yield return Hub.Show(Hub.Get<PopUpContent>(_loseUIPath).gameObject).Play();
    }

    public override IEnumerator OnWinGame()
    {
        yield return ShowWinPopUp();
        GameLog.Instance.OnWin();
    }
    
    private IEnumerator ShowWinPopUp()
    {
        AudioManager.Instance.PlaySound(EventSound.LevelComplete);
        yield return ShowTextWin();
        yield return Hub.Show(Hub.Get<PopUpContent>(_winUIPath).gameObject).Play();
        
    }
    
    private IEnumerator ShowTextWin()
    {
        var transform = GameManager.Instance.transform;
        _currentFireWork = PoolManager.Instance["FireWork"].Get<ParticleSystem>(transform);
        _currentFireWork.Play();
        _currentConfetti = PoolManager.Instance["Confetti"].Get<ParticleSystem>(transform);
        _currentConfetti.Play();
        yield return Hub.Show(Hub.Get<PopUpContent>(PopUpPath.POP_UP_WOOD__UI_LEVELCOMPLETE).gameObject).Play();
        yield return new WaitForSeconds(3f);
        yield return Hub.Hide(Hub.Get<PopUpContent>(PopUpPath.POP_UP_WOOD__UI_LEVELCOMPLETE).gameObject).Play();
        yield return new WaitForSeconds(.5f);
    }

    public enum WinUIType
    {
        REWARD_POWERUP = 0,
        REWARD_COIN
    }
    
    public enum LoseUIType
    {
        REVIVAL_CLASSIC = 0,
        REVIVAL_COIN_NORMAL,
        REVIVAL_COIN_ENDLESSCLASSIC,
    }
}