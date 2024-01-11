using DG.Tweening;
using Puzzle.UI;
using System;
using System.Collections;
using UnityEngine;

public class AdsManager : PersistentSingleton<AdsManager>
{

    [SerializeField] private GameObject adsBreak;
    [HideInInspector] public bool isShowedAdsBreak = false;

    protected override void Awake()
    {
        base.Awake();
        ShowBanner();
    }

    public IEnumerator StartCountAdsBreak()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameConst.SecondAdsBreak);
            // Show Popup
            if (IAPSubscription.Instance != null && IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS)) yield break;
            if (GameDataManager.MaxLevelUnlock == 0 || GameManager.Instance.gameState != GameState.Playing) continue;
            Hub.Show(Hub.Get<AdsBreak>(PopUpPath.POP_UP_UI_ADSBREAK).gameObject).Play();
            StartCoroutine(GameManager.Instance.IECancelInsert());
            yield return new WaitForSeconds(2);
            ShowInter("ADSBREAK_INGAME");
            isShowedAdsBreak = true;
            Hub.Hide(Hub.Get<AdsBreak>(PopUpPath.POP_UP_UI_ADSBREAK).gameObject).Play();
        }
    }
    
    public void ShowReward(string adsWhere, Action onSucceed = null, Action onFail = null)
    {
        ISHandler.Instance.ShowRewardedVideo(onSucceed, onFail, adsWhere);
    }

    public void ShowInter(string adsWhere)
    {
        if (IAPSubscription.Instance != null && IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS)) return;
        if (GameDataManager.MaxLevelUnlock == 0) return;
        ISHandler.Instance.ShowInterstitial(adsWhere);
    }
    
    private void ShowBanner()
    {
        if (IAPSubscription.Instance != null && IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS)) return;
        ISHandler.Instance.ShowBanner();
    }

    public void ResetTimesAdsBreak()
    {
        isShowedAdsBreak = false;
    }
    
}
