using System;
using CodeStage.AntiCheat.Storage;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;

public class IAPCacheData
{
    private const string BeginnerPackKey = "beginner_puzzle";
    private const string XmasPackKey = "Xmas_puzzle";
    public static bool isPurchaseSucceed;
    public static event Action OnBuyIap;
    public static bool BeginnerPackAvailable => ObscuredPrefs.Get(BeginnerPackKey, 0) == 0; 
    public static void ConsumeBeginnerPack()
    {
        ObscuredPrefs.Set(BeginnerPackKey, 1);
        OnBuyIap?.Invoke();
    }
    public static bool ChristmasPackAvailable => ObscuredPrefs.Get(XmasPackKey, 0) == 0 
                                                 && !IsExpire(FalconConfig.Instance<IAPDateConfig>().christmasPackExpireDate);
    public static void ConsumeChristmasPack()
    {
        ObscuredPrefs.Set(XmasPackKey, 1);
        OnBuyIap?.Invoke();
    }

    public static void ResetPurChaseSucceed()
    {
        isPurchaseSucceed = false;
    }
    
    private static bool IsExpire(string time)
    {
        var convertDate = new ConvertDate();
        var expireDate = convertDate.ParseStringToData(time, out var canParse);
        if (!canParse) return true;
        return DateTime.UtcNow > expireDate;
    }
}

