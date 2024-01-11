using System.Collections.Generic;
using DG.Tweening;
using Falcon.FalconAnalytics.Scripts.Enum;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardCoinWin: PopUpContent
{
    [SerializeField] private Text levelText;
    [SerializeField] private Text coinText;
    [SerializeField] private Button bContinue, bReceiveCoin, bHome;
    [SerializeField] private Material grayMat;
    private List<IChallenge> _challenges = new();
    private int _coinRewardAmount;
    private bool _isRewardCoin;

    private void Awake()
    {
        bHome.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            var popup = Hub.Get<UIPause>(PopUpPath.POP_UP_WOOD__UI_PAUSE);
            Hub.Hide(popup.gameObject).Play().OnComplete(() =>
            {
                CanvasManager.Instance.SwitchSceneAnim(() =>
                {
                    GameLoader.Instance.LoadHome();
                });
            });
        });
        bContinue.onClick.AddListener(PlayNextLevel);
        SetUpCoinReward();
    }
    
    private void SetUpCoinReward()
    {
        bReceiveCoin.onClick.AddListener(ReceiveCoinReward);
        _coinRewardAmount = GetCoinReward();
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock + 1,FlowType.Source, "CoinWin", 
            $"CoinWin", "coin", _coinRewardAmount);
        GameDataManager.AddCoins(_coinRewardAmount);
    }

    private static int GetCoinReward()
    {
        return (int)(GameConst.CoinBase + (GameConst.CoinRate * (GameDataManager.MaxLevelUnlock + 1)) +
                     Random.Range(GameConst.CoinRange.start, GameConst.CoinRange.end + 1));
    }

    public void OnEnable()
    {
        levelText.text = $"LEVEL {GameDataManager.LevelToShow(GameDataManager.MaxLevelUnlock + 1)}";
        ++GameDataManager.TimesPlay;
        AudioManager.Instance.PlaySound(EventSound.Win);
        GameLoader.Instance.StopCountTimeAdsBreak();
        GameDataManager.MaxLevelUnlock++;
        coinText.text = _coinRewardAmount.ToString();
        _isRewardCoin = false;
    }

    public void OnDisable()
    {
        foreach (var challenge in _challenges)
        {
            Destroy(((Component)challenge)?.gameObject);
        }
        
        _challenges.Clear();
    }

    private void ReceiveCoinReward()
    {
        if (_isRewardCoin) return;
        AdsManager.Instance.ShowReward($"WIN_COIN_REWARD", () =>
        {
            Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock + 1,FlowType.Source, "CoinWin",
                $"CoinAdsReward", "coin", _coinRewardAmount);
            GameDataManager.AddCoins(_coinRewardAmount);
            _coinRewardAmount *= 2;
            coinText.text = _coinRewardAmount.ToString();
            bReceiveCoin.image.material = grayMat;
            _isRewardCoin = true;
        });
    }

    private void PlayNextLevel()
    {
        GameLoader.Instance.Load(GameDataManager.MaxLevelUnlock, gameObject);
        AudioManager.Instance.PlaySound(EventSound.Click);
    }

}