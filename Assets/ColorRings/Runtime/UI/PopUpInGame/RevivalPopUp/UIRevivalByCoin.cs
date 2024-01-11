using System;
using System.Collections;
using DG.Tweening;
using Falcon.FalconAnalytics.Scripts.Enum;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRevivalByCoin : PopUpContent
{
    [SerializeField] private Button bCoinRevival;
    [SerializeField] private Button bAdsRevival;
    [SerializeField] private Button bLose;
    [SerializeField] private Text coinAmount;
    [SerializeField] private Material grey;
    protected IRevivalStrategy revivalStrategy;
    private const int TileDestroyAmount = 4;
    private const int FirstCoinValue = 500;
    private const int AddCoinValue = 100;
    private int _coinRevival;

    private void Awake()
    {
        bCoinRevival.onClick.RemoveAllListeners();
        bCoinRevival.onClick.AddListener(DestroyByCoin);
        bAdsRevival.onClick.RemoveAllListeners();
        bAdsRevival.onClick.AddListener(DestroyByAds);
        bLose.onClick.RemoveAllListeners();
        bLose.onClick.AddListener(() =>
        {
            StartCoroutine(SwitchUILose());
        });

        GameDataManager.OnCoinChange += OnCoinDataChange;
    }

    protected virtual void OnEnable()
    {
        _coinRevival = GetCoinToRevival();
        coinAmount.text = _coinRevival.ToString();
        if (_coinRevival > GameDataManager.Coin)
        {
            //todo st when dont have enough money
            bCoinRevival.image.material = grey;
        }
        SetUpRevivalStrategy();
    }

    protected void OnDestroy()
    {
        GameDataManager.OnCoinChange -= OnCoinDataChange;
    }

    private void SetUpRevivalStrategy()
    {
        var revivalManager = new RevivalStrategyManager();
        revivalStrategy = revivalManager.GetRevivalStrategy(GameManager.Instance.Level.ModeGameType);
    }
    
    private int GetCoinToRevival()
    {
        var loseCount = GameManager.Instance.LoseCount;
        if (loseCount == 1)
        {
            return FirstCoinValue;
        }

        return FirstCoinValue * loseCount + AddCoinValue;
    }

    protected virtual IEnumerator SwitchUILose()
    {
        yield return Hub.Hide(gameObject).Play()
            .OnComplete(ShowLose).WaitForCompletion();
    }

    protected virtual void ShowLose()
    {
        Hub.Show(Hub.Get<PopUpContent>(revivalStrategy.LosePath).gameObject).Play();
    }

    protected virtual void DestroyByCoin()
    {
        var coinToRevival = GetCoinToRevival();
        if (!GameDataManager.CanSubCoins(coinToRevival))
        {
            return;
        }
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Sink, "CoinRevival", 
            revivalStrategy.ItemIdResourceLog, "coin", coinToRevival);
        Revival();
    }

    private void DestroyByAds()
    {
        AdsManager.Instance.ShowReward(revivalStrategy.AdWhere, Revival);
    }
    
    protected virtual void Revival()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        Hub.Hide(gameObject).Play()
            .OnComplete(() =>
            {
                InGamePowerUpManager.Instance.DestroyRandomTile(TileDestroyAmount);
            });
    }

    private void OnCoinDataChange(int i)
    {
        var coinToRevival = GetCoinToRevival();
        if (coinToRevival <= GameDataManager.Coin)
        {
            bCoinRevival.image.material = null;
            return;
        }

        bCoinRevival.image.material = grey;
    }
}

public class RevivalStrategyManager
{
    public IRevivalStrategy GetRevivalStrategy(ModeGameType modeGameType) => modeGameType switch
    {
        ModeGameType.Normal => new AdventureRevivalStrategy(),
        ModeGameType.Hard => new AdventureRevivalStrategy(),
        ModeGameType.SuperHard => new AdventureRevivalStrategy(),
        ModeGameType.Endless => new EndlessClassicRevivalStrategy(),
        ModeGameType.AdvancedEndless => new EndlessAdvancedRevivalStrategy(),
        ModeGameType.Bonus => new BonusRevivalStrategy(),
        _ => throw new ArgumentOutOfRangeException(nameof(modeGameType), modeGameType, null)
    };
}

public interface IRevivalStrategy
{
    ModeGameType GameMode { get; }
    string LosePath { get; }
    string ItemIdResourceLog { get; }
    string AdWhere { get; }
}

public class AdventureRevivalStrategy : IRevivalStrategy
{
    public ModeGameType GameMode => ModeGameType.Normal;
    public string LosePath => PopUpPath.POP_UP_WOOD__UI_LOSECHALLENGE;
    public string ItemIdResourceLog => "AdventureCoinRevival";
    public string AdWhere => "Adventure_Revival";
} 

public class BonusRevivalStrategy : IRevivalStrategy
{
    public ModeGameType GameMode => ModeGameType.Bonus;
    public string LosePath => PopUpPath.POP_UP_WOOD__UI_BONUS_WIN;
    public string ItemIdResourceLog => "BonusCoinRevival";
    public string AdWhere => "Bonus_Revival";
} 

public class EndlessClassicRevivalStrategy : IRevivalStrategy
{
    public ModeGameType GameMode => ModeGameType.Endless;
    public string LosePath => PopUpPath.POP_UP_WOOD__UI_ENDLESSCLASSIC_LOSE;
    public string ItemIdResourceLog => "EndlessCoinRevival";
    public string AdWhere => "Classic_Revival";
} 

public class EndlessAdvancedRevivalStrategy : IRevivalStrategy
{
    public ModeGameType GameMode => ModeGameType.AdvancedEndless;
    public string LosePath => PopUpPath.POP_UP_WOOD__UI_ENDLESSCLASSIC_LOSE;
    public string ItemIdResourceLog => "AdvancedCoinRevival_" + BoardShapeEndlessManager.BoardID;
    public string AdWhere => "Advanced_Revival";
} 