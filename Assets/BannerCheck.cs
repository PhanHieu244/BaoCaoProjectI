using System;
using Jackie.Soft;
using UnityEngine;

public class BannerCheck : MonoBehaviour, IStoreInitialization, Message.ICallback
{
    private void OnEnable()
    {
        Message.Use<Type>().With(this).Sub(typeof(IStoreInitialization));
        OnStoreInitializeSucceed();
    }


    private void OnDisable()
    {
        Message.Use<Type>().With(this).UnSub(typeof(IStoreInitialization));
    }

    public void OnStoreInitializeSucceed()
    {
        if (ISHandler.Instance == null) return;
        ISHandler.Instance.ShowBanner();
        if (IAPSubscription.Instance == null) return;
        if (!IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS)) return;
        ISHandler.Instance.HideBanner();
    }
}