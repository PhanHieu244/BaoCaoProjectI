using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBuyCondition
{
    int Coin { get; }
    SkinTier SkinTier { get; }
    bool IsAvailable();
    string GetSkinText();

    public void Setup(SkinTier skinTier, Dictionary<SkinTier, int> coinBySkinTier);
}

[Serializable]
public abstract class BaseBuyCondition : IBuyCondition
{
    [SerializeField] private int coin = 100;
    [SerializeField] private SkinTier skinTier;
    public int Coin => coin;
    public SkinTier SkinTier => skinTier;
    public abstract bool IsAvailable();
    public abstract string GetSkinText();
    public void Setup(SkinTier skinTier, Dictionary<SkinTier, int> coinBySkinTier)
    {
        this.skinTier = skinTier;
        coin = coinBySkinTier[SkinTier];
    }
}

[Serializable]
public class NoneCondition : BaseBuyCondition
{
    public override bool IsAvailable() => true;
    public override string GetSkinText()
    {
        return "Locked";
    }
}

[Serializable]
public class BuyCondition : BaseBuyCondition
{
    [SubclassSelector, SerializeReference] private ICondition _condition;
    public override bool IsAvailable() => _condition.IsAvailable();

    public override string GetSkinText()
    {
        return "Locked";
    }
}


public enum SkinTier
{
    Tier0,
    Tier1,
    Tier2,
    Tier3,
    Tier4,
    Tier5,
    Tier6,
}

