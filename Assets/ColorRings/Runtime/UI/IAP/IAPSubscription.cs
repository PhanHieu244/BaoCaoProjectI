using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

public class IAPSubscription : IAPManager<IAPSubscription> {

    public override IEnumerable<string> ProductIds { get; } = new[]
    {
        IAPKey.S_NO_ADS,
    };

    public override ProductType ProductType => ProductType.Subscription;

    private bool IsSubTo(string productId) {
        var subscriptionProduct = storeController.products.WithID(productId);
        try {
            // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
            if (subscriptionProduct.receipt == null) {
                return false;
            }

            //The intro_json parameter is optional and is only used for the App Store to get introductory information.
            var subscriptionManager = new SubscriptionManager(subscriptionProduct, null);

            // The SubscriptionInfo contains all of the information about the subscription.
            // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
            var info = subscriptionManager.getSubscriptionInfo();

            return info.isSubscribed() == Result.True;
        }
        catch (Exception e) {
            return false;
        }
    }


#if UNITY_EDITOR
    private Dictionary<string, bool> IsSubDict = new Dictionary<string, bool>();
    public override PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (IsInitialized() && IsValidPurchase(args))
        {
            if (IsSubDict.TryAdd(args.purchasedProduct.definition.id, true))
            {
                IsSubDict[args.purchasedProduct.definition.id] = true;
            }
        }
        return base.ProcessPurchase(args);
    }
#endif


    public bool IsSub(string key) {
#if UNITY_EDITOR
        return IsInitialized() && IsSubDict.TryGetValue(key, out var isSub) && isSub;
#endif
        return IsInitialized() && IsSubTo(key);
    }

    public DateTime GetExpireDateTime(string productId) {
        var subscriptionProduct = storeController.products.WithID(productId);
        try {
            // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
            if (subscriptionProduct.receipt == null) {
                DevLog.Log("date", "dont receipt");
                return DateTime.UtcNow.AddDays(-999);
            }

            //The intro_json parameter is optional and is only used for the App Store to get introductory information.
            var subscriptionManager = new SubscriptionManager(subscriptionProduct, null);

            // The SubscriptionInfo contains all of the information about the subscription.
            // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
            var info = subscriptionManager.getSubscriptionInfo();
            DevLog.Log("date", info.getExpireDate());
            return info.getExpireDate();
        }
        catch (Exception e) {
            DevLog.LogError("error", e);
            return DateTime.UtcNow.AddDays(-999);
        }
    }
}