using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinTiersFrameData", menuName = "Scriptable Objects/Skin Tiers Frame Data")]
public class SkinTiersFrame : ScriptableObject
{
    [SerializeField] private SkinTierFrameHashElement[] skinTiersFrame;

    public (Sprite, Sprite) GetSprites(SkinTier tier)
    {
        if (skinTiersFrame == null) return (null, null);

        foreach (var frame in skinTiersFrame)
        {
            if (frame.tier == tier)
                return (frame.selectedSprite, frame.unselectedSprite);
        }

        return (null, null);
    }

    public (Sprite, Sprite) GetSprites(int index)
    {
        if (skinTiersFrame == null) return (null, null);
        if (index >= skinTiersFrame.Length) return (null, null);

        return (skinTiersFrame[index].selectedSprite, skinTiersFrame[index].unselectedSprite);
    }

    public Sprite GetSprite(SkinTier tier, bool selected)
    {
        if (skinTiersFrame == null) return null;

        foreach (var frame in skinTiersFrame)
        {
            if (frame.tier.Equals(tier))
                return selected? frame.selectedSprite : frame.unselectedSprite;
        }

        return null;
    }
}

[Serializable]
public class SkinTierFrameHashElement
{
    [field: SerializeField] public SkinTier tier;
    [field: SerializeField] public Sprite selectedSprite;
    [field: SerializeField] public Sprite unselectedSprite;
}