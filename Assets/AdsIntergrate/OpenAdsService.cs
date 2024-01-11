using System.Collections.Generic;
using AppsFlyerSDK;
using UnityEngine;

public class OpenAdsService : MonoBehaviour
{
    private static string MAX_SDK_KEY = "M4GLwqezVT2WDo75OWFGOV873pVg6-3S3Kpz8Rxe_-9CnHI9oXPB2TI5LpnRnqvr8hpH8kw7i4KTMcc891KCad";

    private void Start()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += AppOpenOnOnAdRevenuePaidEvent;

            AppOpenAdManager.ShowAdIfAvailable();
        };

#if UNITY_EDITOR
        MaxSdkUnityEditor.SetSdkKey(MAX_SDK_KEY);
        MaxSdkUnityEditor.InitializeSdk();
#else
        MaxSdk.SetSdkKey(MAX_SDK_KEY);
        MaxSdk.InitializeSdk();
#endif
    }

    private void AppOpenOnOnAdRevenuePaidEvent(string adUnit, MaxSdkBase.AdInfo adInfo)
    {
        Firebase.Analytics.Parameter[] adParameters =
        {
            new("ad_platform", "Max"),
            new("ad_source", adInfo.NetworkName),
            new("ad_unit_name", adInfo.AdUnitIdentifier),
            new("ad_format", "app_open"),
            new("currency", "USD"),

            new("value", adInfo.Revenue)
        };

        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", adParameters);

        var dic = new Dictionary<string, string>
        {
            { "ad_unit_name", adInfo.AdUnitIdentifier },
            { "ad_format", adInfo.AdFormat }
        };
        AppsFlyerAdRevenue.logAdRevenue(adInfo.NetworkName, AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
            adInfo.Revenue, "USD", dic);
    }

    private void OnAppOpenDismissedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
#if UNITY_EDITOR
        MaxSdkUnityEditor.LoadAppOpenAd(AppOpenAdManager.AppOpenAdUnitId);
#else
        MaxSdk.LoadAppOpenAd(AppOpenAdManager.AppOpenAdUnitId);
#endif
    }

    private void OnApplicationPause(bool pauseStatus)
    {
#if UA_BUILD
        return;
#endif
        if (!pauseStatus)
        {
            AppOpenAdManager.ShowAdIfAvailable();
        }
    }
}