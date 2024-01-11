using System;
using DG.Tweening;
using Falcon.FalconAnalytics.Scripts.Enum;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class PopUpBuySkin : PopUpContent
{
    [SerializeField] private Text coinText;
    [SerializeField] private Button bBuy;
    [SerializeField] private Button bOut;
    private SkinItemsUnity _skinItemsUnity;
    private int _coin;
    private int _idSkin;

    public static event Action OnBuySkin;

    private void Start()
    {
        bOut.onClick.AddListener(Hide);
        bBuy.onClick.AddListener(Buy);
    }

    public void SetUp(int idSkin, int coin, SkinItemsUnity skinItemsUnity)
    {
        _skinItemsUnity = skinItemsUnity;
        coinText.text = coin.ToString();
        _coin = coin;
        _idSkin = idSkin;
        gameObject.SetActive(true);
    }

    private void Buy()
    {
        if (!GameDataManager.CanSubCoins(_coin, false)) return;
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock,FlowType.Sink, "CoinShopSkin", 
            $"CoinSkin_{_idSkin}", "coin", _coin);
        GameDataManager.UnlockSkinByID(_idSkin);
        GameDataManager.OnCoinChange.Invoke(GameDataManager.Coin);
        Hide();
        OnBuySkin?.Invoke();
    }

    private void Hide()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        Hub.Hide(Hub.Get<PopUpContent>(PopUpPath.POP_UP_UI_POPUP_BUY_SKIN).gameObject).Play();
    }

    private void OnDestroy()
    {
        OnBuySkin = null;
    }
}