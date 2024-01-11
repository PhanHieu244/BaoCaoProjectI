using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeReference, SubclassSelector] private IWinLoseUIStrategy _winLoseUIStrategy = new MainWinLoseUIStrategy();
    public static Action onCountPowerUpChange;
    public static Action<PowerUpType> onWinningStreakReward;
    
    private void Awake()
    {
        Initialize();
    }
    
    private void Initialize()
    {
        _winLoseUIStrategy.Initialize();
        GameManager.Instance.OnWinGame += OnWinGame;
        GameManager.Instance.OnLoseGame += OnLoseGame;
    }

    private void OnLoseGame()
    {
        StartCoroutine(_winLoseUIStrategy.OnLoseGame());
    }

    private void OnWinGame()
    {
        StartCoroutine(_winLoseUIStrategy.OnWinGame());
    }
}