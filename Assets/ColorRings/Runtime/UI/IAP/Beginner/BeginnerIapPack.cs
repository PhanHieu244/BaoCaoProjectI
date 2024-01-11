using System;

[Serializable]
public class BeginnerIapPack : IapPack<BeginnerIapElement> {
    public override bool Available {
        get => IAPConsumable.Instance && IAPConsumable.Instance.IsInitialized() && IAPCacheData.BeginnerPackAvailable;
    }
}