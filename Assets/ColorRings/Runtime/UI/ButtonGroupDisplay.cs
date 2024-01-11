using System;
using Falcon.FalconCore.Scripts.FalconABTesting.Scripts.Model;
using Jackie.Soft;
using UnityEngine;

public class ButtonGroupDisplay : MonoBehaviour, Message.ICallback, IStoreInitialization
{
    [SerializeField] private ButtonClick[] buttonList;

    private void OnEnable() {
        FalconConfig.OnUpdateFromNet += Refresh;
        IAPCacheData.OnBuyIap += OnStoreInitializeSucceed;
        Message.Use<Type>().With(this).Sub(typeof(IStoreInitialization));
        
        OnStoreInitializeSucceed();
    }

    private void Refresh(object sender, EventArgs e) => OnStoreInitializeSucceed();

    private void OnDisable()
    {
        FalconConfig.OnUpdateFromNet += Refresh;
        IAPCacheData.OnBuyIap -= OnStoreInitializeSucceed;
        Message.Use<Type>().With(this).UnSub(typeof(IStoreInitialization));
    }

    public void OnStoreInitializeSucceed() {
        foreach (var buttonClick in buttonList)
        {
            buttonClick.gameObject.SetActive(buttonClick.IsAvailable); 
        }
    }
}