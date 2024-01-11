using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualGiftData", menuName = "Scriptable Objects/VisualGift", order = 0)]
public class EffectGiftDataSO : ScriptableObject
{
    [SerializeField] private EffectGift[] visualGifts;

    public EffectGift this[GiftType giftType] => GetVisualByType(giftType);

    public EffectGift GetVisualByType(GiftType giftType)
    {
        foreach (var visualGift in visualGifts)
        {
            if (visualGift.GiftType == giftType) return visualGift;
        }
        DevLog.LogError("Dont have this gift type", giftType);
        return null;
    }
}

[Serializable]
public class EffectGift
{
    [field: SerializeField] public GiftType GiftType { get; private set; }
    [field: SerializeField] public RewardGiftEffectEntity RewardGiftEffect { get; private set; }
}