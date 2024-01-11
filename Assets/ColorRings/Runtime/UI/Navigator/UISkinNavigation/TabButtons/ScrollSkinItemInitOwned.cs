using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollSkinItemInitOwned : ScrollSkinItemsInit
{
    protected override bool IsShowThisSkin(int id)
    {
        return GameDataManager.SkinAvailable(id);
    }

    protected override Queue<int> GetPriorityQueue()
    {
        const float rate = 0.8f;
        var shopData = GameDataManager.SkinShopData;
        var idQueue = new Queue<int>();
        
        //main skin
        var currentID = GameDataManager.CurrentSkin;
        idQueue.Enqueue(currentID);
            
        //add complete condition, enough coin
        for (int id = 0; id < GameDataManager.SkinAmount; id++)
        {
            if (id == currentID) continue;
            
            idQueue.Enqueue(id);
        }
        
        return idQueue;
    }
}
