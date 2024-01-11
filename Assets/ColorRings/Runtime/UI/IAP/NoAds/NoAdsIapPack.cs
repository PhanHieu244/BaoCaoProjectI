using System;

[Serializable]
public class NoAdsIapPack : IapPack<NoAdsIapElement> {
    public override bool Available {
        get => IAPSubscription.Instance != null && IAPSubscription.Instance.IsInitialized() && !IAPSubscription.Instance.IsSub(IAPKey.S_NO_ADS);
    }
}