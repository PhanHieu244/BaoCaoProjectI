using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class CoinIapElement : MonoBehaviour {
    [SerializeField] private Text labelPrice;
    [SerializeField] private CoinPresenter coinPresenter;
    
    private Coin coin;
    private string productId; 
    
    public Coin Coin {
        set
        {
            this.coin = value;
            coinPresenter.SetUpInfo(value);
        }
    }

    public void Set(string productId, string defaultPriceText) {
        this.productId = productId;
        labelPrice.text = IAPConsumable.Instance.GetLocalizePrice(productId, defaultPriceText);
    }

    public void OnBuyPack() {
        IAPCacheData.isPurchaseSucceed = true;
        IAPConsumable.Instance.OnPurchaseSucceed(OnPurchaseSucceed).Buy(productId);
    }

    private Purchase OnPurchaseSucceed() {
        coin.ClaimGift("shop");
        var package = Hub.Get<GiftPackageDisplay>(PopUpPath.POP_UP_WOOD__UI_REWARDGIFTPACKAGE, null);
        package.Setup(coin);
        Hub.Show(package.gameObject).Play();
        return new Purchase { where = "shop" };
    }
}