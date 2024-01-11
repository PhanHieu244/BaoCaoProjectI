using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollSkinItemInitShop : ScrollSkinItemsInit
{
    protected override bool IsShowThisSkin(int id)
    {
        return GameDataManager.SkinShopData.ShopOnlySkinsID.Contains(id) && !GameDataManager.SkinAvailable(id);
    }
}
