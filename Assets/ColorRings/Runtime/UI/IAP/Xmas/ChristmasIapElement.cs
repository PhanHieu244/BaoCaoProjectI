public class ChristmasIapElement : ConsumableIapElement
{
    protected override Purchase OnPurchaseSucceed()
    {
        IAPCacheData.ConsumeChristmasPack();
        return base.OnPurchaseSucceed();
    }
}