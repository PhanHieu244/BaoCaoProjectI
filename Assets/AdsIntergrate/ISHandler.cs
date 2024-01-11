using System;
using System.Collections;
using System.Collections.Generic;
using AppsFlyerSDK;
using Falcon.FalconAnalytics.Scripts.Enum;
using UnityEngine;

public class ISHandler : MonoBehaviour
{
#if UNITY_ANDROID
    [SerializeField] private string androidKey;
#elif UNITY_IOS
    [SerializeField] private string iosKey;
#endif

    [Tooltip("If the banner fire onAdLoadFailedEvent then try again after this duration")] [SerializeField]
    private bool loadBanner = true, loadInter = true, loadRewarded = true;

#if UNITY_EDITOR
    [Header("Only works on editor")] [SerializeField]
    private bool _isRewardedSucceed = true;
#endif


    private bool _isInitSuccessful, _rewardedHasClosed, _rewardedHasRewarded;
    private Action _onRewardedSuccess, _onRewardedFail;

    public static ISHandler Instance { get; private set; }

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            IronSourceAdQuality.Initialize(androidKey);
            return;
        }

        DestroyImmediate(gameObject);
    }

    private void RegisterEvents()
    {
        IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent; //Register the ImpressionData Listener

        if (loadBanner) RegisterBannerEvents(); //Add AdInfo Banner Events
        if (loadInter) RegisterInterstitialEvents(); //Add AdInfo Interstitial Events
        if (loadRewarded) RegisterRewardedEvents(); //Add AdInfo Rewarded Video Events
    }

    private void UnregisterEvents()
    {
        IronSourceEvents.onImpressionDataReadyEvent -= ImpressionDataReadyEvent; //Unregister the ImpressionData Listener
        IronSourceEvents.onSdkInitializationCompletedEvent -= SdkInitializationCompletedEvent;

        if (loadBanner) UnregisterBannerEvents(); //Remove AdInfo Banner Events
        if (loadInter) UnregisterInterstitialEvents(); //Remove Interstitial Events
        if (loadRewarded) UnregisterRewardedEvents(); //Remove RewardedVideo Events
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!_isInitSuccessful) return;
        IronSource.Agent.onApplicationPause(pauseStatus);
    }

    public void Init(bool setConsent)
    {
        IronSource.Agent.validateIntegration();
        IronSource.Agent.setConsent(setConsent);
        IronSource.Agent.shouldTrackNetworkState(true);

#if UNITY_ANDROID
        if (loadBanner)
            IronSource.Agent.init(androidKey, IronSourceAdUnits.BANNER);
        if (loadInter)
            IronSource.Agent.init(androidKey, IronSourceAdUnits.INTERSTITIAL);
        if (loadRewarded)
            IronSource.Agent.init(androidKey, IronSourceAdUnits.REWARDED_VIDEO);
#elif UNITY_IPHONE
        if(loadBanner) 
            IronSource.Agent.init(iosKey, IronSourceAdUnits.BANNER);
        if(loadInter) 
            IronSource.Agent.init(iosKey, IronSourceAdUnits.INTERSTITIAL);
        if(loadRewarded)
            IronSource.Agent.init(iosKey, IronSourceAdUnits.REWARDED_VIDEO);
#endif
    }

    private void SdkInitializationCompletedEvent()
    {
        _isInitSuccessful = true;
        RegisterEvents();
        if (loadBanner) LoadBanner();
        if (loadInter) LoadInterstitial();
    }

  

    #region Banner

    private void RegisterBannerEvents()
    {
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
    }

    private void UnregisterBannerEvents()
    {
        IronSourceBannerEvents.onAdLoadedEvent -= BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent -= BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent -= BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent -= BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent -= BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent -= BannerOnAdLeftApplicationEvent;
    }

    private void LoadBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
        IronSource.Agent.hideBanner();
    }

    public void ShowBanner()
    {
        if (IAPSubscription.Instance != null && IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS)) return;
        if (!_isInitSuccessful) return;
        IronSource.Agent.displayBanner();
    }

    public void HideBanner()
    {
        if (!_isInitSuccessful) return;
        IronSource.Agent.hideBanner();
    }

    /************* Banner AdInfo Delegates *************/
    //Invoked once the banner has loaded
    private void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        ShowBanner();
    }

    //Invoked when the banner loading process has failed.
    private void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        LoadBanner();
    }

    // Invoked when end user clicks on the banner ad
    private void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
    }

    //Notifies the presentation of a full screen content following user click
    private void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        if (adInfo.revenue == null) return;
        QualitySend(ISAdQualityAdType.BANNER, adInfo.revenue.Value);
    }

    //Notifies the presented screen has been dismissed
    private void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
    }

    //Invoked when the user leaves the app
    private void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
    }

    #endregion

    #region Interstitial

    private void RegisterInterstitialEvents()
    {
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;
    }

    private void UnregisterInterstitialEvents()
    {
        IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent -= InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent -= InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent -= InterstitialOnAdClosedEvent;
    }

    private void LoadInterstitial()
    {
        IronSource.Agent.loadInterstitial();
    }

    private bool IsInterstitialReady()
    {
        return _isInitSuccessful && IronSource.Agent.isInterstitialReady();
    }

    public void ShowInterstitial(string adWhere)
    {
#if UA_BUILD
        return;
#endif
        Debug.Log("Show Interstitial");
        if (!IsInterstitialReady()) return;
        IronSource.Agent.showInterstitial();
        AppsFlyer.sendEvent("af_inters_show", null);
        Data4Game.AdsLog(GameDataManager.MaxLevelUnlock, AdType.Interstitial, adWhere);
    }

    private Action interCall;

    public void ShowInterstitial(Action callback)
    {
#if UA_BUILD
        return;
#endif
        
        if (IsInterstitialReady())
        {
            interCall = callback;
            IronSource.Agent.showInterstitial();
        }
        {
            if (callback != null)
            {
                callback.Invoke();
            }
        }
    }

    /************* Interstitial AdInfo Delegates *************/
    //Invoked when the interstitial ad was loaded successfully.
    private void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
    }

    //Invoked when the initialization process has failed.
    private void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        LoadInterstitial();
        interCall?.Invoke();
    }

    //Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    private void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        if (adInfo.revenue != null)
            QualitySend(ISAdQualityAdType.INTERSTITIAL, adInfo.revenue.Value);
        AppsFlyer.sendEvent("af_inters_displayed", null);
    }

    //Invoked when end user clicked on the interstitial ad
    private void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
    }

    //Invoked when the ad failed to show.
    private void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        interCall?.Invoke();
    }

    //Invoked when the interstitial ad closed and the user went back to the application screen.
    private void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        LoadInterstitial();
        interCall?.Invoke();
    }

    //Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
    //This callback is not supported by all networks, and we recommend using it only if  
    //it's supported by all networks you included in your build. 
    private void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
    }

    #endregion

    #region RewardedVideo

    private void RegisterRewardedEvents()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
    }

    private void UnregisterRewardedEvents()
    {
        IronSourceRewardedVideoEvents.onAdOpenedEvent -= RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent -= RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent -= RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent -= RewardedVideoOnAdClickedEvent;
    }

    private void LoadRewardedVideo()
    {
        //It's automatically. If you want to load it manually:
        //Following the doc: https://developers.is.com/ironsource-mobile/unity/rewarded-video-manual-integration-unity/#step-3
    }

    public bool IsRewardedVideoReady()
    {
        return _isInitSuccessful && IronSource.Agent.isRewardedVideoAvailable();
    }

    public void ShowRewardedVideo(Action onSuccess, Action onFail, string adWhere)
    {
#if UA_BUILD
        onSuccess?.Invoke();
        return;
#endif
#if UNITY_EDITOR
        if (_isRewardedSucceed)
            onSuccess?.Invoke();
        else
            onFail?.Invoke();
#endif

        if (!_isInitSuccessful) return;
        
        _onRewardedSuccess = onSuccess;
        _onRewardedFail = onFail;
        _rewardedHasClosed = false;
        _rewardedHasRewarded = false;
        
        IronSource.Agent.showRewardedVideo();
        AppsFlyer.sendEvent("af_rewarded_show", null);
        Data4Game.AdsLog(GameDataManager.MaxLevelUnlock, AdType.Reward, adWhere);
    }


    /************* RewardedVideo AdInfo Delegates *************/
    //Indicates that there’s an available ad.
    //The adInfo object includes information about the ad that was loaded successfully
    //This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    private void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
    }

    //Indicates that no ads are available to be displayed
    //This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    private void RewardedVideoOnAdUnavailable()
    {
    }

    //The Rewarded Video ad view has opened. Your activity will loose focus.
    private void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        if (adInfo.revenue != null)
            QualitySend(ISAdQualityAdType.REWARDED_VIDEO, adInfo.revenue.Value);
        AppsFlyer.sendEvent("af_rewarded_displayed", null);
    }

    //The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    //Note:  The onRewardedVideoAdRewardedEvent and onRewardedVideoAdClosedEvent are asynchronous.
    //Make sure to set up your listener to grant rewards even in cases where onRewardedVideoAdRewarded is fired after the onRewardedVideoAdClosedEvent.
    private void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        _rewardedHasClosed = true;
        CheckFireRewardedEvent();
    }

    //The user completed to watch the video, and should be rewarded.
    //The placement parameter will include the reward data.
    //When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    private void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
        _rewardedHasRewarded = true;
        CheckFireRewardedEvent();
    }

    private void CheckFireRewardedEvent()
    {
        if (!_rewardedHasClosed || !_rewardedHasRewarded) return;
        _onRewardedSuccess?.Invoke();
        _onRewardedSuccess = null;
    }

    //The rewarded video ad was failed to show.
    private void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
        _onRewardedFail?.Invoke();
        _onRewardedFail = null;
    }

    //Invoked when the video ad was clicked.
    //This callback is not supported by all networks, and we recommend using it only if
    //it’s supported by all networks you included in your build.
    private void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
    }

    #endregion

    private void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
    {
        if (impressionData?.revenue == null) return;
        Firebase.Analytics.Parameter[] adParameters =
        {
            new("ad_platform", "ironSource"),
            new("ad_source", impressionData.adNetwork),
            new("ad_unit_name", impressionData.instanceName),
            new("ad_format", impressionData.adUnit),
            new("currency", "USD"),
            new("value", impressionData.revenue.Value)
        };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", adParameters);

        var dic = new Dictionary<string, string>
        {
            { "ad_unit_name", impressionData.instanceName },
            { "ad_format", impressionData.adUnit }
        };
        AppsFlyerAdRevenue.logAdRevenue(impressionData.adNetwork, AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource,
            impressionData.revenue.Value, "USD", dic);
    }
    
    private static void QualitySend(ISAdQualityAdType qualityAdType, double revenue)
    {
        var customMediationRevenue = new ISAdQualityCustomMediationRevenue
        {
            MediationNetwork = ISAdQualityMediationNetwork.LEVEL_PLAY,
            AdType = qualityAdType,
            Revenue = revenue
        };
        IronSourceAdQuality.SendCustomMediationRevenue(customMediationRevenue);
    }
}