using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using UnityEngine;

public class AppOpenAdManager
{
#if UNITY_ANDROID
    private const string AD_UNIT_ID = "212a24a43be9bbdc";
#elif UNITY_IOS
    private const string AD_UNIT_ID = "212a24a43be9bbdc";
#else
    private const string AD_UNIT_ID = "unexpected_platform";
#endif
    public static string AppOpenAdUnitId => AD_UNIT_ID;

    public static void ShowAdIfAvailable()
    {
        if (IAPCacheData.isPurchaseSucceed)
        {
            IAPCacheData.ResetPurChaseSucceed();
            return;
        }
        if (IAPSubscription.Instance != null && IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS)) return;
        if (!FalconConfig.Instance<OpenAdsConfig>().isShowOpenAds) return;
#if UNITY_EDITOR
        if (MaxSdkUnityEditor.IsAppOpenAdReady(AppOpenAdUnitId))
            MaxSdkUnityEditor.ShowAppOpenAd(AppOpenAdUnitId);
        else
            MaxSdkUnityEditor.LoadAppOpenAd(AppOpenAdUnitId);
#else
        if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
            MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
        else
            MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
#endif
    }
}