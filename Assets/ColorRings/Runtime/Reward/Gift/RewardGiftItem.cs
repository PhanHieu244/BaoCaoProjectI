using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RewardGiftItem : MonoBehaviour
{
    [SerializeField] private Text infoText;
    [SerializeField] private Text infoRewardDisplayText;

    public void SetInfo(IGift gift, bool isRewardDisplay = false)
    {
        Show(isRewardDisplay);
        infoText.text = $"X{gift.GiftInfo}";
        infoRewardDisplayText.text = $"X{gift.GiftInfo}";
    }
    
    private void Show(bool isRewardDisplay)
    {
        infoText.gameObject.SetActive(!isRewardDisplay);
        infoRewardDisplayText.gameObject.SetActive(isRewardDisplay);
    }
    
    public Tween OnShow(float duration = 0.2f)
    {
        gameObject.SetActive(true);
        transform.localScale = new Vector3(.02f, .02f, .02f);
        return transform.DOScale(new Vector3(1f, 1f, 1f), duration).SetEase(Ease.OutBack);
    }

    public Tween OnHide(float duration = 0.2f)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        return transform.DOScale(new Vector3(.02f, .02f, .02f), duration).SetEase(Ease.InBack).OnComplete((() =>
        {
            gameObject.SetActive(false);
        }));
    }

    public void Reload(bool isRewardDisplay = false)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        Show(isRewardDisplay);
    }
    
    public void Reload(Vector3 localScale, bool isRewardDisplay = false)
    {
        gameObject.SetActive(true);
        transform.localScale = localScale;
        Show(isRewardDisplay);
    }
}
