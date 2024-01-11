using System;
using UnityEngine;

[Serializable]
public class CoinIapPack : IapPack<CoinIapElement> {
    [SerializeField, IAPKey] private string productId;
    [SerializeField] private string defaultPriceText;
    [SerializeField] private Coin coin = new(600);

    public override void Initialization(Transform parent) {
        base.Initialization(parent);
        element.Coin = coin;
        element.Set(productId, defaultPriceText);
    }

    public override bool Available {
        get => IAPConsumable.Instance != null && IAPConsumable.Instance.IsInitialized();
    }
}