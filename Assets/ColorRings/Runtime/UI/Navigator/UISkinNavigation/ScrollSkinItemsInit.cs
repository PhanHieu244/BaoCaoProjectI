using System;
using System.Collections.Generic;
using DG.Tweening;
using JackieSoft;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class ScrollSkinItemsInit : MonoBehaviour
{
    [SerializeField] private ListView listView;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Image[] ringImages;
    [SerializeField] private Text conditionText;
    private SkinItemsUnity _skinItemsUnity;

    private void OnDataChange(int i)
    {
        _skinItemsUnity.onDataChange?.Invoke();    
    }
    
    private void OnEnable()
    {
        listView.data = new List<Cell.IData>();
        _skinItemsUnity = new SkinItemsUnity();
        _skinItemsUnity.onBuy += ShowPopUp;
        GameDataManager.OnCoinChange += OnDataChange;
        _skinItemsUnity.onShowInfo += (condition, sprites) =>
        {
            conditionText.text = condition;
            ringImages[(int)RingSize.SMALL_RING].sprite = sprites[(int)RingSize.SMALL_RING];
            ringImages[(int)RingSize.MEDIUM_RING].sprite = sprites[(int)RingSize.MEDIUM_RING];  
            ringImages[(int)RingSize.BIG_RING].sprite = sprites[(int)RingSize.BIG_RING];  
        }; 
        _skinItemsUnity.ShowPreview();
        //init data scroll shop
        var idQueue = GetPriorityQueue();

        var maxItem = SkinItemGroup.MaxItemInGroupAmount;
        int count = 0;
        var ids = new int[maxItem];
        while(idQueue.TryPeek(out var id))
        {
            if (IsShowThisSkin(id)) 
            {
                ids[count] = id;
                count++;
            }
            idQueue.Dequeue();
            if (count < maxItem && idQueue.Count > 0) continue;
            var idsData = new int[count];
            for (int i = 0; i < count; i++)
            {
                idsData[i] = ids[i];
            }
            listView.data.Add(new SkinItemGroupData
            {
                ids = idsData,
                skinItemsUnity = _skinItemsUnity
            });
            count = 0;
        }
        listView.Initialize();
        scrollRect.verticalNormalizedPosition = 1;
    }

    private void OnDisable()
    {
        GameDataManager.OnCoinChange -= OnDataChange;
    }

    protected virtual bool IsShowThisSkin(int id)
    {
        return true;
    }

    private void ShowPopUp(int idSkin, int coin)
    {
        var popUpBuySkin = Hub.Get<PopUpBuySkin>(PopUpPath.POP_UP_UI_POPUP_BUY_SKIN);
        popUpBuySkin.transform.SetParent(null);
        Hub.Show(popUpBuySkin.gameObject).Play();
        popUpBuySkin.SetUp(idSkin, coin, _skinItemsUnity);
    }

    protected virtual Queue<int> GetPriorityQueue()
    {
        const float rate = 0.8f;
        var shopData = GameDataManager.SkinShopData;
        var shopOnlyIds = shopData.ShopOnlySkinsID;
        var idQueue = new Queue<int>();
        var conditionNotComplete = new List<int>();
        var notEnoughCoin = new List<int>();
        var enoughRateCoin = new List<int>();
        var shopSkinAvailable = new List<int>();
        var coin = GameDataManager.Coin;
        //main skin
        var currentID = GameDataManager.CurrentSkin;
        idQueue.Enqueue(currentID);
        //add complete condition, enough coin
        for (int i = 0; i < shopOnlyIds.Length; i++)
        {
            var id = shopOnlyIds[i];
            var condition = shopData.GetConditionByID(id);
            if (GameDataManager.SkinAvailable(id))
            {
                shopSkinAvailable.Add(id);
                continue;
            }
            if (!condition.IsAvailable())
            {
                conditionNotComplete.Add(id);
                continue;
            }
            if (coin < condition.Coin)
            {
                if (coin >= condition.Coin * rate)
                {
                    enoughRateCoin.Add(id);
                    continue;
                }
                notEnoughCoin.Add(id);
                continue;
            }
            idQueue.Enqueue(id);
        }

        //condition complete, coin >= 80% coin to buy
        foreach (var id in enoughRateCoin)
        {
            idQueue.Enqueue(id);
        }
        
        //add skin available
        
        foreach (var id in shopData.LockSkinsID)
        {
            if(id == currentID) continue;
            if (!GameDataManager.SkinAvailable(id)) continue;
            if(shopSkinAvailable.Contains(id)) continue;
            shopSkinAvailable.Add(id);
        }
        
        foreach (var id in shopData.RewardSkinsID)
        {
            if(id == currentID) continue;
            if (!GameDataManager.SkinAvailable(id)) continue;
            if(shopSkinAvailable.Contains(id)) continue;
            shopSkinAvailable.Add(id);
        }
        
        foreach (var id in shopSkinAvailable)
        {
            if(id == currentID) continue;
            idQueue.Enqueue(id);
        }

        //add not enough coin
        foreach (var id in notEnoughCoin)
        {
            idQueue.Enqueue(id);
        }
        
        //add condition not complete
        foreach (var id in conditionNotComplete)
        {
            idQueue.Enqueue(id);
        }

        return idQueue;
    }
}