using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using Falcon;
using Falcon.FalconCore.Scripts;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

[PublicAPI]
public abstract class IAPManager<T> : MonoSingleton<T>, ICustomDetailedStoreListener where T : IAPManager<T> {

    protected OnPurchaseSucceed EventPurchaseSucceed;
    protected OnPurchaseFailed EventPurchaseFailed;

    protected IStoreController storeController;
    protected IExtensionProvider extensionProvider;
    protected string cacheProductId;

    public abstract IEnumerable<string> ProductIds { get; }
    public abstract ProductType ProductType { get; }

    public T OnPurchaseSucceed(OnPurchaseSucceed Event) {
        EventPurchaseSucceed = Event;
        return this as T;
    }

    public T OnPurchaseFailed(OnPurchaseFailed Event) {
        EventPurchaseFailed = Event;
        return this as T;
    }

    public void OnInitializeFailed(InitializationFailureReason error) {
        Debug.Log($"on initialize failed: failure reason {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message) {
        Debug.Log($"on initialize failed: failure reason {error}, message {message}");
    }

    public virtual PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
        if (IsInitialized() && IsValidPurchase(args)) {
            // Unlock the appropriate content here.
            var purchase = EventPurchaseSucceed?.Invoke();
            EventPurchaseSucceed = null;
            EventPurchaseFailed = null;

            Data4GamePurchaseEvent(args, purchase ?? default);
            AppsFlyerPurchaseEvent(args.purchasedProduct);
        }

        cacheProductId = string.Empty;

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    [Obsolete]
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
    }

    public virtual void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
        storeController = controller;
        extensionProvider = extensions;
    }

    public virtual void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {
        EventPurchaseFailed?.Invoke();
        EventPurchaseSucceed = null;
        EventPurchaseFailed = null;
        cacheProductId = string.Empty;

        Debug.Log($"on purchase fail:\n" +
                  $" product '{product.definition.storeSpecificId}',\n" +
                  $" failure product {failureDescription.productId},\n" +
                  $" failure message {failureDescription.message}");
    }

    public void Buy(string productID) {
        if (IsInitialized()) {
            var product = storeController.products.WithID(productID);
            if (product is { availableToPurchase: true }) {
                cacheProductId = productID;
                storeController.InitiatePurchase(product);
            }
            else {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initialization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public bool IsInitialized() {
        // Only say we are initialized if both the Purchasing references are set.
        return storeController != null && extensionProvider != null;
    }

    protected bool IsValidPurchase(PurchaseEventArgs args) {
        var product = args.purchasedProduct;
        return product.definition.id == cacheProductId;
    }

    public string GetLocalizePrice(string key, string defaultPriceText) {
        if (storeController != null)
            return storeController.products.WithID(key)
                .metadata.localizedPriceString;

        return defaultPriceText;
    }

    private void AppsFlyerPurchaseEvent(Product product) {
        var eventValue = new Dictionary<string, string>
        {
            { "af_revenue", (product.metadata.localizedPrice * 0.63m).ToString() },
            { "af_content_id", product.definition.id },
            { "af_currency", product.metadata.isoCurrencyCode }
        };

        AppsFlyer.sendEvent("af_purchase", eventValue);
    }

    private void Data4GamePurchaseEvent(PurchaseEventArgs args, Purchase purchase) {
        if (FalconMain.InitComplete) {
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try {
                var result = validator.Validate(args.purchasedProduct.receipt);
                foreach (var productReceipt in result) {
#if UNITY_ANDROID
                    if (productReceipt is GooglePlayReceipt googleReceipt) {
                        DWHLog.Instance.InAppLog(GameDataManager.MaxLevelUnlock,
                            args.purchasedProduct.definition.id,
                            args.purchasedProduct.metadata.isoCurrencyCode,
                            args.purchasedProduct.metadata.localizedPrice.ToString(),
                            googleReceipt.transactionID, googleReceipt.purchaseToken,
                            purchase.where);
                    }
#elif UNITY_IOS
                    if (productReceipt is AppleInAppPurchaseReceipt appleReceipt) {
                        DWHLog.Instance.InAppLog(GameDataManager.MaxLevelUnlock, 
                            args.purchasedProduct.definition.id, 
                            args.purchasedProduct.metadata.isoCurrencyCode, 
                            args.purchasedProduct.metadata.localizedPrice.ToString(),
                            appleReceipt.originalTransactionIdentifier, string.Empty, 
                            purchase.where);
                    }
#endif
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
    }

}

public struct Purchase {
    public string where;
}