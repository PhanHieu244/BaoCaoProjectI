using System.Collections.Generic;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableIapElement : MonoBehaviour
{
    [Header("Sort same order to Gift match Presenter")]
    [SerializeReference, SubclassSelector] private List<IGift> gifts;
    [SerializeField] private List<MonoBehaviour> presenters;
    [Space]
    [SerializeField] private Text labelPrice;
    [SerializeField, IAPKey] private string productId;
    [SerializeField] private string defaultPriceText;
    [SerializeField] private string purchaseWhere = "shop";
    

    private void Awake()
    {
        for (var i = 0; i < presenters.Count; i++)
            ((IPresenter)presenters[i]).SetUpInfo(gifts[i]);
        labelPrice.text = IAPConsumable.Instance.GetLocalizePrice(productId, defaultPriceText);
    }

    public void OnBuyPack()
    {
        IAPCacheData.isPurchaseSucceed = true;
        IAPConsumable.Instance.OnPurchaseSucceed(OnPurchaseSucceed).Buy(productId);
    }

    protected virtual Purchase OnPurchaseSucceed() {
        foreach (var gift in gifts)
        {
            gift.ClaimGift(purchaseWhere);
        }
        var package = Hub.Get<GiftPackageDisplay>(PopUpPath.POP_UP_WOOD__UI_REWARDGIFTPACKAGE, null);
        package.Setup(gifts);
        Hub.Show(package.gameObject).Play();

        return new Purchase { where = purchaseWhere };
    }
}