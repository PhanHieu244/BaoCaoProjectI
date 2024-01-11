using System;
using Jackie.Soft;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class StoreListener : MonoBehaviour, IDetailedStoreListener
{
    private ICustomDetailedStoreListener[] listeners;
    private void Awake()
    {
        listeners = GetComponents<ICustomDetailedStoreListener>();
    }

    public void Start() {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var listener in listeners)
        {
            foreach (var productId in listener.ProductIds) {
                builder.AddProduct(productId, listener.ProductType);
            }
        }
        
        UnityPurchasing.Initialize(this, builder);
    }

    [Obsolete]
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        foreach (var listener in listeners)
            listener.OnInitializeFailed(error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        foreach (var listener in listeners)
            listener.OnInitializeFailed(error, message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        foreach (var listener in listeners)
            listener.ProcessPurchase(purchaseEvent);
        return PurchaseProcessingResult.Complete;
    }

    [Obsolete]
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        foreach (var listener in listeners)
            listener.OnPurchaseFailed(product, failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        foreach (var listener in listeners) {
            listener.OnInitialized(controller, extensions);
        }
        Message.Use<Type>().Event(typeof(IStoreInitialization)).Execute<IStoreInitialization>(@event => @event.OnStoreInitializeSucceed());
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        foreach (var listener in listeners)
            listener.OnPurchaseFailed(product, failureDescription);
    }
}

public interface IStoreInitialization
{
    void OnStoreInitializeSucceed();
}