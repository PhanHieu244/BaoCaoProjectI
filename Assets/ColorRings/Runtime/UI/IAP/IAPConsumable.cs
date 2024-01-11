using System.Collections.Generic;
using UnityEngine.Purchasing;

public class IAPConsumable : IAPManager<IAPConsumable>
{
    public override IEnumerable<string> ProductIds { get; } = new[]
    {
        IAPKey.C_BEGINNER,
        IAPKey.C_CHRISTMAS,
        IAPKey.C_COIN_PACK_1,
        IAPKey.C_COIN_PACK_2,
        IAPKey.C_COIN_PACK_3,
        IAPKey.C_COIN_PACK_4,
        IAPKey.C_COIN_PACK_5,
        IAPKey.C_COIN_PACK_6,
        IAPKey.C_COIN_PACK_7
    };

    public override ProductType ProductType => ProductType.Consumable;
}