using System;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using ColorRings.Runtime.GameData;
using ColorRings.Runtime.GameMode.AdvancedBoard;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameDataManager : PersistentSingleton<GameDataManager>
{
    private static readonly string MaxLevelUnlockKey = "MaxLevelUnlock";
    private static readonly string HighScoreEndlessKey = "HighScore";
    private static readonly string HighScoreAdvancedKey = "HighScoreAdvanced";
    private static readonly string CurrentLevelIsBonusKey = "bonusKey";
    private static bool _currentLevelIsBonus;
    private static int _maxLevelUnlock;
    private static int _highScoreEndless;
    private static int _highScoreAdvancedEndless;
    private static int _soundState;
    private static int _musicState;
    private static int _vibrateState;

    private static int _currentScoreEndless;
    private static int _maxItemUnlock;
    private static int _timesPlay;
    
    private static int _destroyTileItemAmount;
    private static int _destroyRowItemAmount;
    private static int _destroyColItemAmount;
    private static int _swapItemAmount;

    private const string PlayerDataKey = "PlayerData_PUZZLE";
    private const string ClassicEndlessKey = "ClassicEndlessData_PUZZLE";
    private const string AdvancedEndlessKey = "ClassicEndlessData_PUZZLE";
    private const string PathHashSkin = "Skin Selector/HashSkinData";
    private const string PathShopSkin = "Skin Selector/SkinShopData";
    private static PlayerData _playerData;
    private static EndlessClassicData _endlessClassicData;
    private static HashSkinData _hashSkin;
    private static SkinShopData _skinShopData;

    public static int TimesPlay
    {
        get => _timesPlay;
        set
        {
            _timesPlay = value;
            if (IAPSubscription.Instance != null && IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS)) return;
            if (value < GameConst.TimesPlayShowAds) return;
            if (AdsManager.Instance.isShowedAdsBreak) return;
            AdsManager.Instance.ShowInter("ADS_WIN_LOSE");
            _timesPlay = 0;
        }
    }

    private void OnDrawGizmos()
    {
        _maxLevelUnlock = ObscuredPrefs.Get("MaxLevelUnlock", 0);
    }

    protected override void Awake()
    {
        base.Awake();
        LoadPlayerData();
        InitItemData();
        SetUpSkinData();
        InitPreciousData();
        LoadClassicEndlessData();
        _maxLevelUnlock = ObscuredPrefs.Get(MaxLevelUnlockKey, 0);
        _highScoreEndless = ObscuredPrefs.Get(HighScoreEndlessKey, 0);
        _highScoreAdvancedEndless = ObscuredPrefs.Get(HighScoreAdvancedKey, 0);
        _soundState = ObscuredPrefs.Get(GameConst.SoundState, 1);
        _musicState = ObscuredPrefs.Get(GameConst.MusicState, 1);
        _vibrateState = ObscuredPrefs.Get(GameConst.VibrateState, 1);
        _maxItemUnlock = ObscuredPrefs.Get(GameConst.MaxItemUnlock, 0);
    }

    public static void DeleteAll()
    {
        ObscuredPrefs.DeleteAll();
        PlayerPrefs.DeleteAll();
        ObscuredFilePrefs.DeleteAll();
    }

    #region HighScoreMode

    #region EndlessClassic

    public static int HighScoreEndLess
    {
        get => _endlessClassicData.highScore;
        set
        {
            if (value >= _endlessClassicData.maxPointAllTime)
            {
                _endlessClassicData.maxPointAllTime = value;
            }
            _endlessClassicData.highScore = value;
            SaveClassicEndlessData();
        }
    }
    
    public static int CurrentRewardHighScoreEndlessClassic
    {
        get => _endlessClassicData.currentRewardHighScore;
        set
        {
            _endlessClassicData.currentRewardHighScore = value;
            SaveClassicEndlessData();
        }
    }
    
    public static bool IsReceivedEndlessClassicGift(int id)
    {
        return _endlessClassicData.receivedReward.Length > id && _endlessClassicData.receivedReward[id] == 1;
    }
    
    public static void ReceiveEndlessClassicGift(int id)
    {
        if (_endlessClassicData.receivedReward.Length <= id)
        {
            DevLog.LogError("out of classic reward range", id);
            return;
        }
        _endlessClassicData.receivedReward[id] = 1;
        SaveClassicEndlessData();
    }

    public static void ResetClassicEndlessData()
    {
        var highScoreAllTime = _endlessClassicData.maxPointAllTime;
        _endlessClassicData = new EndlessClassicData
        {
            maxPointAllTime = highScoreAllTime
        };
        SaveClassicEndlessData();
    }

    private static void SaveClassicEndlessData()
    {
        DataHandle.Save(ClassicEndlessKey, _endlessClassicData);
    }
    
    private static void LoadClassicEndlessData()
    {
        _endlessClassicData = DataHandle.GetData<EndlessClassicData>(ClassicEndlessKey);
    }

    #endregion
    

    #endregion

    #region PlayerData

#if UNITY_EDITOR
    [Button]
    private void UnlockFullItem()
    {
        _maxItemUnlock = 5;
    }
#endif
    private static void SavePlayerData()
    {
        DataHandle.Save(PlayerDataKey, _playerData);
    }
    
    private static void LoadPlayerData()
    {
        _playerData = DataHandle.GetData<PlayerData>(PlayerDataKey);
    }

    #region CoinData

    public static int Coin => _playerData.coin;
    public static Action<int> OnCoinChange;

    public static void AddCoins(int addAmount, bool isUpdateVisual = true)
    {
        _playerData.coin += addAmount;
        SavePlayerData();
        if (!isUpdateVisual) return;
        OnCoinChange?.Invoke(_playerData.coin);
    }

    public static void UpdateCoinVisual()
    {
        OnCoinChange?.Invoke(_playerData.coin);
    }
    
    public static bool CanSubCoins(int subAmount, bool isUpdateVisual = true)
    {
        if (_playerData.coin < subAmount) return false;
        _playerData.coin -= subAmount;
        SavePlayerData();
        if (isUpdateVisual) OnCoinChange?.Invoke(_playerData.coin);
        return true;
    }

    #endregion

    #region SkinPlayerData
    public static event Action<int> OnUnlockSkin;
    private static bool SkinItemIsUnlock(int id) => id < SkinAmount && _playerData.skinItemsUnlock[id] == 1;
    private static bool SkinItemIsExist(int id) => id < SkinAmount && _playerData.skinItemsUnlock[id] >= 0;
    public static int CurrentSkin => _playerData.currentSkin;
    public static int SkinAmount => _playerData.skinItemsUnlock.Length;
    public static int RewardSkinStreak => _playerData.skinStreak;
    public static SkinShopData SkinShopData => _skinShopData;
    public static void SelectSkin(int id)
    {
        if (!SkinAvailable(id)) _playerData.currentSkin = 0;
        _playerData.currentSkin = id;
        SavePlayerData();
    }
    
    private static void SetUpSkinData()
    {
        _hashSkin = Resources.Load<HashSkinData>(PathHashSkin);
        _skinShopData = Resources.Load<SkinShopData>(PathShopSkin);
        var hashCount = _hashSkin.ResourceSkinNames.Length;
        if (hashCount == SkinAmount) return;
        Array.Resize(ref _playerData.skinItemsUnlock, hashCount);
    }

    private static void RemoveSkin()
    {
        bool hasLock = false;
        foreach (var removeID in _hashSkin.UnAvailableInGame)
        {
            var value = _playerData.skinItemsUnlock[removeID];
            if(value == -1) continue;
            _playerData.skinItemsUnlock[removeID] = -1;
            hasLock = true;
        }

        if (_playerData.skinItemsUnlock[CurrentSkin] == -1)
        {
            _playerData.currentSkin = 0;
            hasLock = true;
        }

        if (!hasLock) return;
        SavePlayerData();
    }
    

    public static int IncreaseSkinStreak()
    {
        var streak = ++_playerData.skinStreak;
        if (streak >= GameConst.MaxSkinStreak) _playerData.skinStreak = 0;
        SavePlayerData();
        return streak;
    }

    public static bool IsRemainRewardSkin(out int idReward)
    {
        idReward = 0;
        var remainList = _skinShopData.RewardSkinsID.Where(rewardID => !SkinItemIsUnlock(rewardID)).ToList();
        if (remainList.Count <= 0) return false;
        idReward = remainList[Random.Range(0, remainList.Count)];
        return true;
    }

    private static bool IsRemainRewardSkin(out int idReward, out int remainAmount)
    {
        idReward = 0;
        var remainList = _skinShopData.RewardSkinsID.Where(rewardID => !SkinItemIsUnlock(rewardID)).ToList();
        remainAmount = remainList.Count;
        if (remainAmount <= 0) return false;
        idReward = remainList[Random.Range(0, remainList.Count)];
        return true;
    }

    public static Skin GetSkinByID(int id, bool isShow = false)
    {
        if (!isShow && !SkinAvailable(id)) id = 0;
        return Resources.Load<Skin>($"Skin Selector/Skin/{_hashSkin[id]}"); 
    }

    public static bool SkinAvailable(int id)
    {
        return id < SkinAmount && SkinItemIsUnlock(id);
    }
    
    public static void UnlockSkinByID(int id)
    {
        if (id > SkinAmount) return;
        if (!SkinItemIsExist(id)) return;
        _playerData.skinItemsUnlock[id] = 1;
        OnUnlockSkin?.Invoke(id);
        SavePlayerData();
        //ObscuredPrefs.Set(GameConst.SkinItemsUnlock, _skinItemsUnlock);
    }
    #endregion

    #region ItemData

    private static void InitItemData()
    {
        _destroyTileItemAmount = ObscuredPrefs.Get(PowerUpType.Tile.ToString(), 0);
        _destroyRowItemAmount = ObscuredPrefs.Get(PowerUpType.Row.ToString(), 0);
        _destroyColItemAmount = ObscuredPrefs.Get(PowerUpType.Line.ToString(), 0);
        _swapItemAmount = ObscuredPrefs.Get(PowerUpType.Swap.ToString(), 0);
    }
    
    public static int DestroyTileItemAmount
    {
        get => _destroyTileItemAmount;
        set
        {
            _destroyTileItemAmount = value;
            ObscuredPrefs.Set(PowerUpType.Tile.ToString(), value);
        }
    }
    public static int DestroyRowItemAmount
    {
        get => _destroyRowItemAmount;
        set
        {
            _destroyRowItemAmount = value;
            ObscuredPrefs.Set(PowerUpType.Row.ToString(), value);
        }
    }
    public static int DestroyColItemAmount
    {
        get => _destroyColItemAmount;
        set
        {
            _destroyColItemAmount = value;
            ObscuredPrefs.Set(PowerUpType.Line.ToString(), value);
        }
    }
    public static int SwapItemAmount
    {
        get => _swapItemAmount;
        set
        {
            _swapItemAmount = value;
            ObscuredPrefs.Set(PowerUpType.Swap.ToString(), value);
        }
    }

    #endregion

    #region WinningStreak
    
    public static int WinningStreak => _playerData.winningStreak;
    public static bool IsInWinningStreakProgress => _playerData.isInWinningStreakProgress > 0;

    public static void JoinWinningStreakProgress()
    {
        _playerData.isInWinningStreakProgress = 1;
        SavePlayerData();
    }

    private static void OutWinningStreakProgress()
    {
        _playerData.isInWinningStreakProgress = 0;
    }
    public static int CurrentWinningStreak { get; private set; }
    public static void SetupWinningStreak()
    {
        GameManager.Instance.OnWinGame -= IncreaseWinningStreak;
        GameManager.Instance.onOutGame -= ResetWinningStreak;
        GameManager.Instance.OnWinGame += IncreaseWinningStreak;
        GameManager.Instance.onOutGame += ResetWinningStreak;
        CurrentWinningStreak = WinningStreak;
    }

    private static void IncreaseWinningStreak()
    {
        OutWinningStreakProgress();
        _playerData.winningStreak++;
        DevLog.Log($"winning streak:-----", _playerData.winningStreak);
        SavePlayerData();
    }

    public static void ResetWinningStreak()
    {
        OutWinningStreakProgress();
        _playerData.winningStreak = 0;
        DevLog.Log($"winning streak:-----", _playerData.winningStreak);
        SavePlayerData();
    }

    #endregion

    #region OtherData
    public static int CurrentWeek => _playerData.currentWeek;

    private static void UpdateCurrentWeek(int week)
    {
        _playerData.currentWeek = week;
        SavePlayerData();
    }

    public static bool IsChangeWeek()
    {
        var week = WeekEventScheduler.GetWeekSinceStart();
        if (week == CurrentWeek) return false;
        UpdateCurrentWeek(week);
        return true;
    }

    #endregion
    #endregion

    #region LevelData
    public static int MaxLevelToShow => _maxLevelUnlock - CountLevelBonus(_maxLevelUnlock);
    public static string LevelToShow(int level) => IsLevelBonus(level) ? "Bonus" : (level - CountLevelBonus(level)).ToString();
    public static int MaxLevelUnlock
    {
        get
        {
            return _maxLevelUnlock;
        } 
        set
        {
            _maxLevelUnlock = value;
            ObscuredPrefs.Set(MaxLevelUnlockKey, value);
        }
        
    }

    public static int CountLevelBonus(int level)
    {
        return level / GameConst.BonusLevelRange;
    }

    public static bool IsLevelBonus(int level)
    {
        return level % GameConst.BonusLevelRange == 0;
    }

    public static void GetSpecialLevel(int nextLevelAmount ,out Queue<int> levelsBonus, out Queue<int> levelsRewardSkin)
    {
        levelsBonus = new Queue<int>();
        levelsRewardSkin = new Queue<int>();
        int count = RewardSkinStreak;
        int level = _maxLevelUnlock + 1;
        IsRemainRewardSkin(out var idReward, out var rewardAmount);
        for (int i = 0; i <= nextLevelAmount; i++)
        {
            if (IsLevelBonus(level))
            {
                levelsBonus.Enqueue(level);
                level++;
                continue;
            }
            count++;
            if (count == GameConst.MaxSkinStreak && levelsRewardSkin.Count < rewardAmount)
            {
                levelsRewardSkin.Enqueue(level);
                count = 0;
            }
            level++;
        }
    }

    
    
    public static int MaxItemUnlock
    {
        get => _maxItemUnlock;
        set
        {
            _maxItemUnlock = value;
            ObscuredPrefs.Set(GameConst.MaxItemUnlock, value);
        }
    }

#endregion

    #region HightScore
    public static int HighScoreAdvancedEndLess
    {
        get => _highScoreAdvancedEndless;
        set
        {
            _highScoreAdvancedEndless = value;
            ObscuredPrefs.Set(HighScoreAdvancedKey, value);
        }
    }

    #endregion

    #region PreciousData

    private const string RingBreakKey = "ringBreak";
    private const string RingBreakAddKey = "ringBreakAdd";
    private const string TreasureIDKey = "treasureID";
    private static int _ringBreakAdd;
    private static int _ringBreakAmount;
    private static int _treasureID;

    private void InitPreciousData()
    {
        _ringBreakAmount = ObscuredPrefs.Get(RingBreakKey, 0);
        _ringBreakAdd = ObscuredPrefs.Get(RingBreakAddKey, 0);
        _treasureID = ObscuredPrefs.Get(TreasureIDKey, 0);
    }

    public static void ResetPreciousData()
    {
        RingBreakAmount = 0;
        TreasureID = 0;
        RingBreakAdd = 0;
    }
    
    
    public static int TreasureID
    {
        get => _treasureID;
        set
        {
            _treasureID = value;
            ObscuredPrefs.Set(TreasureIDKey, value);
        }
    }
    
    public static int RingBreakAdd
    {
        get => _ringBreakAdd;
        set
        {
            _ringBreakAdd = value;
            ObscuredPrefs.Set(RingBreakAddKey, value);
        }
    }

    public static int RingBreakAmount
    {
        get => _ringBreakAmount;
        set
        {
            _ringBreakAmount = value;
            ObscuredPrefs.Set(RingBreakKey, value);
        }
    }
    
    #endregion

    #region Sound-Music-Vibrate

    public static int SoundState
    {
        get => _soundState;

        set
        {
            _soundState = value;
            ObscuredPrefs.Set(GameConst.SoundState, value);
        }
    }
    
    public static int MusicState
    {
        get => _musicState;

        set
        {
            _musicState = value;
            ObscuredPrefs.Set(GameConst.MusicState, value);
        }
    }
    
    public static int VibrateState
    {
        get => _vibrateState;

        set
        {
            _vibrateState = value;
            ObscuredPrefs.Set(GameConst.VibrateState, value);
        }
    }

    #endregion
    
}
