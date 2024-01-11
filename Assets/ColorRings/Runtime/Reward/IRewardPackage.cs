using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Falcon.FalconAnalytics.Scripts.Enum;
using UnityEngine;

public interface IRewardPackage
{
    void ClaimRewardPackage(string claimWhere, string itemType = "");
}

public abstract class RewardPackage : IRewardPackage
{
    public abstract IGift[] GetGiftList();
    protected void ClaimAllGift<T>(T[] gifts, string itemID, string itemType = "") where T : IGift
    {
        if(gifts is null) return;
        foreach (var gift in gifts)
        {
            gift.ClaimGift(itemID, itemType);
        }
    }

    public abstract void ClaimRewardPackage(string claimWhere, string itemType = "");
}

[Serializable]
public class GiftRewardPackage : RewardPackage
{
    [SerializeReference, SubclassSelector] private IGift[] gifts;
    public override IGift[] GetGiftList() => gifts;
    public override void ClaimRewardPackage(string claimWhere, string itemType = "")
    {
        ClaimAllGift(gifts, claimWhere, itemType);
    }
}

[Serializable]
public class PowerUpOnlyRewardPackage : RewardPackage
{
    [SerializeReference, SubclassSelector] private PowerUpGift[] powerUpGifts;
    public override IGift[] GetGiftList() => powerUpGifts;
    public override void ClaimRewardPackage(string claimWhere, string itemType = "")
    {
        ClaimAllGift(powerUpGifts, claimWhere, itemType);
    }
}

[Serializable]
public class CoinOnlyRewardPackage : RewardPackage
{
    [SerializeField] private Coin[] coinGifts;
    public override IGift[] GetGiftList() => coinGifts;
    public override void ClaimRewardPackage(string claimWhere, string itemType = "")
    {
        ClaimAllGift(coinGifts, claimWhere, itemType);
    }
}


public interface IGift
{
    GiftType GiftType { get; }
    void ClaimGift(string claimWhere, string itemType = "");
    string GiftInfo { get; }

    string ToText();
    string ToText(string format);
}

public enum GiftType
{
    Coin,
    ColPowerUp,
    RowPowerUp,
    HammerPowerUp,
    SwapPowerUp,
}

[Presenter(typeof(CoinPresenter))]
[Serializable]
public class Coin : IGift
{
    [field: SerializeField] public ObscuredInt Value { get; private set; } = 100;

    public Coin(ObscuredInt value) {
        this.Value = value;
    }
    
    public Coin() { }
    

    public GiftType GiftType => GiftType.Coin;

    public void ClaimGift(string claimWhere, string itemType = "")
    {
        GameDataManager.AddCoins(Value, false);
        var type = string.IsNullOrEmpty(itemType) ? "CoinGift" : itemType;
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Source, type,$"coin_{claimWhere}", "coin", Value);
    }

    public string GiftInfo => Value.ToString();
    public string ToText() => Value.ToString();
    public string ToText(string format) => Value.ToString(format);
}

[Serializable]
public abstract class PowerUpGift : IGift
{
    [SerializeField] protected ObscuredInt rewardPowerUpAmount = 1;
    public PowerUpGift() { }
    public abstract GiftType GiftType { get; }
    public abstract void ClaimGift(string claimWhere, string itemType = "");

    public string GiftInfo => rewardPowerUpAmount.ToString();
    public string ToText() => rewardPowerUpAmount.ToString();
    public string ToText(string format) => rewardPowerUpAmount.ToString(format);
}

[Presenter(typeof(ColPresenter))]
[Serializable]
public class DestroyColGift : PowerUpGift
{
    public override GiftType GiftType => GiftType.ColPowerUp;
    public override void ClaimGift(string claimWhere, string itemType = "")
    {
        GameDataManager.DestroyColItemAmount += rewardPowerUpAmount;
        var type = string.IsNullOrEmpty(itemType) ? "LineGift" : itemType;
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Source, type,$"line_{claimWhere}", "line", rewardPowerUpAmount);
    }

}

[Presenter(typeof(RowPresenter))]
[Serializable]
public class DestroyRowGift : PowerUpGift
{
    public override GiftType GiftType => GiftType.RowPowerUp;
    public override void ClaimGift(string claimWhere, string itemType = "")
    {
        GameDataManager.DestroyRowItemAmount += rewardPowerUpAmount;
        var type = string.IsNullOrEmpty(itemType) ? "RowGift" : itemType;
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Source, type,$"row_{claimWhere}", "row", rewardPowerUpAmount);
    }
    
}

[Presenter(typeof(HammerPresenter))]
[Serializable]
public class HammerGift : PowerUpGift
{
    public override GiftType GiftType => GiftType.HammerPowerUp;
    public override void ClaimGift(string claimWhere, string itemType = "")
    {
        GameDataManager.DestroyTileItemAmount += rewardPowerUpAmount;
        var type = string.IsNullOrEmpty(itemType) ? "TileGift" : itemType;
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Source, type,$"tile_{claimWhere}", "tile", rewardPowerUpAmount);
    }
    
}

[Presenter(typeof(SwapPresenter))]
[Serializable]
public class SwapRingGift : PowerUpGift
{
    public override GiftType GiftType => GiftType.SwapPowerUp;
    public override void ClaimGift(string claimWhere, string itemType = "")
    {
        GameDataManager.SwapItemAmount += rewardPowerUpAmount;
        var type = string.IsNullOrEmpty(itemType) ? "SwapGift" : itemType;
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Source, type,$"swap_{claimWhere}", "swap", rewardPowerUpAmount);
    }
}

[Presenter(typeof(SkinPresent))]
[Serializable]
public class SkinGift : IGift
{
    [field: SerializeField] public ObscuredInt ID { get; private set; }
    public SkinGift(ObscuredInt id) {
        if (ID > GameDataManager.SkinAmount)
        {
            Debug.LogError("dont have this skin id");
            return;
        }
        this.ID = id;
    }
    
    public SkinGift() { }

    public GiftType GiftType => GiftType.Coin;

    public void ClaimGift(string claimWhere, string itemType = "")
    {
        if (ID > GameDataManager.SkinAmount)
        {
            Debug.LogError("dont have this skin id");
            return;
        }
        GameDataManager.UnlockSkinByID(ID);
        var type = string.IsNullOrEmpty(itemType) ? "SkinGift" : itemType;
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Source, type,$"skin_{claimWhere}_{ID}", "skin", ID);
    }

    public string GiftInfo => ID.ToString();
    public string ToText() => ID.ToString();
    public string ToText(string format) => ID.ToString(format);
}