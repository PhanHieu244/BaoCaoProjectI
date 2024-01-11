using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class NoAdsIapElement : MonoBehaviour {
    [SerializeField] private Text labelPrice;
    [SerializeField] private RemainTimeDisplay remainTimeDisplay;
    [SerializeField] private Image buyButImage;
    [SerializeField] private Sprite normalButSprite;
    [SerializeField] private Sprite lockButSprite;
    [SerializeField] private GameObject monthlyText, activeText, unactiveText;
    [SerializeField] private UIShiny shiny;
    [SerializeField, IAPKey] private string productId;
    [SerializeField] private string purchaseWhere;

    private void Awake() {
        labelPrice.text = IAPSubscription.Instance.GetLocalizePrice(productId, "1.99$");
    }

    private void OnEnable() {
        if (IAPSubscription.Instance.IsSub(productId)) {
            Debug.Log("Sub");
            DisablePack();
            return;
        }

        EnablePack();
    }

    private void DisablePack() {
        remainTimeDisplay.OnExpireTime -= EnablePack;
        remainTimeDisplay.OnExpireTime += EnablePack;
        monthlyText.SetActive(false);
        activeText.SetActive(false);
        unactiveText.SetActive(true);
        remainTimeDisplay.gameObject.SetActive(true);
        buyButImage.sprite = lockButSprite;
        shiny.enabled = false;
    }

    private void EnablePack() {
        remainTimeDisplay.gameObject.SetActive(false);
        buyButImage.sprite = normalButSprite;
        monthlyText.SetActive(true);
        activeText.SetActive(true);
        unactiveText.SetActive(false);
        shiny.enabled = true;
    }

    public void OnBuyPack() {
        if (IAPSubscription.Instance.IsSub(productId)) return;
        IAPCacheData.isPurchaseSucceed = true;
        IAPSubscription.Instance.OnPurchaseSucceed(OnPurchaseSucceed).Buy(productId);
    }

    private Purchase OnPurchaseSucceed() {
        DevLog.Log("No ads complete", "done");
        DisablePack();
        ISHandler.Instance.HideBanner();
        return new Purchase { where = purchaseWhere };
    }


}