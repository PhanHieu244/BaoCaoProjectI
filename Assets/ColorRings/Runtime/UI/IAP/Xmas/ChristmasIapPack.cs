using System;

[Serializable]
public class ChristmasIapPack : IapPack<ChristmasIapElement>
{
    public override bool Available {
        get => IAPConsumable.Instance != null && IAPConsumable.Instance.IsInitialized() && IAPCacheData.ChristmasPackAvailable;
    }
}