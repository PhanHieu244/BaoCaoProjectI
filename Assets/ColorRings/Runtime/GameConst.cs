using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConst : MonoBehaviour
{
    
    public const string SoundState = "SoundState";
    public const string MusicState = "MusicState";
    public const string VibrateState = "VibrateState";
    public const string SkinItemsUnlock = "SkinItemsUnlock";

    public const string MaxItemUnlock = "MaxItemUnlock";
    public const string BoardShapeResource = "BoardShape";
    public const string EndlessLevelResource = "Endless";
    public const string AdvancedEndlessLevelResource = "Advanced Endless";

    public const int TimesPlayShowAds = 1;
    public const float SecondAdsBreak = 80;

    public const int CoinBaseRewardInBonusLevel = 2;
    public const int CoinRangeToIncreaseByLevel = 50;
    public const int TimeRevivalInBonusLevel = 20;

    public const int TimesSpawnEndless = 30;
    public const int MaxSkinStreak = 5;
    public const int BonusLevelRange = 11;
    
    public const int CoinBase = 10;
    public const float CoinRate = 0.1f;
    public static readonly RangeInt CoinRange = new RangeInt(3, 7);

    public const float TimeCheckInternet = 3f;
}
