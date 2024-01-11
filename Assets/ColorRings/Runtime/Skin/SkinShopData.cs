using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
#endif

[CreateAssetMenu(fileName = "SkinShopData", menuName = "Scriptable Objects/Skin Shop Data")]
public class SkinShopData : ScriptableObject
{
    [field: Header("ReadOnly")]
    [field: SerializeField, ReadOnly] public int[] RewardSkinsID { get; private set; }
    [field: SerializeField, ReadOnly] public int[] ShopOnlySkinsID { get; private set; }
    [field: SerializeField, ReadOnly] public int[] LockSkinsID { get; private set; }
    [SubclassSelector, SerializeReference, ReadOnly] private IBuyCondition[] conditions;

    public IBuyCondition GetConditionByID(int id)
    {
        // var length = ShopOnlySkinsID.Length;
        // for (int i = 0; i < length; i++)
        // {
        //     if (id != ShopOnlySkinsID[i]) continue;
        //     return conditions[i];
        // }
        //
        // return null;

        if (id >= conditions.Length || id < 0) 
            return null;

        return conditions[id];
    }
    
#if UNITY_EDITOR
    [Space] [Header("Editor Setting")] 
    [SerializeField] private SkinTierHashElement[] skinTierHashElements;
    [SerializeField] private List<RewardData> settingRewardSkin;
    [SerializeField, ReadOnly] private List<SkinDataInShop> shopOnlySkinData;
    private const string PathHashSkin = "Skin Selector/HashSkinData";
    private Dictionary<SkinTier, int> _coinBySkinTier;

    private void InitSkinTierData()
    {
        _coinBySkinTier = new Dictionary<SkinTier, int>();
        foreach (var skinTierHashElement in skinTierHashElements)
        {
            _coinBySkinTier[skinTierHashElement.SkinTier] = skinTierHashElement.Coin;
        }
    }
    
    [Button]
    private void InitRawRewardData()
    {
        var hashSkin = Resources.Load<HashSkinData>(PathHashSkin);
        var skinsName = hashSkin.ResourceSkinNames;
        var rewardCacheList = settingRewardSkin?? new List<RewardData>();
        var newList = new List<RewardData>();
        foreach (var skinName in skinsName)
        {
            var data = new RewardData
            {
                name = hashSkin.GetNameToLoadByNameSkin(skinName)
            };
            foreach (var rewardData in rewardCacheList.Where(rewardData => rewardData.name.Equals(data.name)))
            {
                data.lockSkin = rewardData.lockSkin;
                data.skinTier = rewardData.skinTier;
                break;
            }
            newList.Add(data);
        }
        settingRewardSkin = newList;
    }
    
    [Button]
    private void InitRawShopOnlySkinData()
    {
        InitSkinTierData();
        var shopCacheList = shopOnlySkinData?? new List<SkinDataInShop>();
        var newList = new List<SkinDataInShop>();
        foreach (var skinRewardData in settingRewardSkin)
        {
            var data = new SkinDataInShop
            {
                name = skinRewardData.name,
                condition = new NoneCondition(),
            };
            if (data.condition is BaseBuyCondition buyCondition)
            {
                buyCondition.Setup(skinRewardData.skinTier, _coinBySkinTier);
            }
            newList.Add(data);
        }
        shopOnlySkinData = newList;
    }

    [Button]
    private void UpdateData()
    {
        var hashSkin = Resources.Load<HashSkinData>(PathHashSkin);
        var rewardIDList = new List<int>();
        var shopOnlyIDList = new List<int>();
        var lockSkinData = new List<int>();
        var conditionList = new List<IBuyCondition>();
        //add lock skin data
        foreach (var skinRewardData in settingRewardSkin)
        {
            if (!skinRewardData.lockSkin) continue;
            lockSkinData.Add(hashSkin.HashIDByNameToLoad(skinRewardData.name));
        }
        //add reward skin data
        foreach (var skinRewardData in settingRewardSkin)
        {
            if (skinRewardData.lockSkin) continue;
            if (!skinRewardData.isInShop) continue;
            if(skinRewardData.skinTier > SkinTier.Tier3) continue;
            rewardIDList.Add(hashSkin.HashIDByNameToLoad(skinRewardData.name));
        }
        //add shop skin data
        foreach (var skinDataInShop in shopOnlySkinData)
        {
            conditionList.Add(skinDataInShop.condition);
            bool isInShop = settingRewardSkin.All(rewardData => !rewardData.name.Equals(skinDataInShop.name) || rewardData.isInShop);
            if(!isInShop) continue;
            var id = hashSkin.HashIDByNameToLoad(skinDataInShop.name);
            if(lockSkinData.Contains(id)) continue;
            shopOnlyIDList.Add(id);
        }

        ShopOnlySkinsID = shopOnlyIDList.ToArray();
        conditions = conditionList.ToArray();
        RewardSkinsID = rewardIDList.ToArray();
        LockSkinsID = lockSkinData.ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
        AssetDatabase.Refresh();
    }

    [Serializable]
    public class RewardData
    {
        public string name;
        public bool isInShop = true;
        public SkinTier skinTier;
        public bool lockSkin;
    }

    [Serializable]
    public class SkinDataInShop
    {
        public string name;
        [SubclassSelector, SerializeReference] public IBuyCondition condition;
    }

    [Serializable]
    public class SkinTierHashElement
    {
        [field: SerializeField] public SkinTier SkinTier { get; private set; }
        [field: SerializeField] public int Coin { get; private set; }
    }
#endif
}
