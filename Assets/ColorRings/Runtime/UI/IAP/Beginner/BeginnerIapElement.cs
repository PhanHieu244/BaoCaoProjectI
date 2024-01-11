public class BeginnerIapElement : ConsumableIapElement {
    protected override Purchase OnPurchaseSucceed()
    {
        IAPCacheData.ConsumeBeginnerPack();
        return base.OnPurchaseSucceed();
    }
}