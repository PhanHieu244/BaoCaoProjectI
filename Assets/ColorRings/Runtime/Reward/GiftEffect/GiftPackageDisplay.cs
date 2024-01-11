using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class GiftPackageDisplay : PopUpContent
{
    [SerializeField] private Button claimBut;
    [SerializeField] private Transform itemsGroup;
    [SerializeField] private Transform[] titleChars;
    [SerializeField] private Transform closeTextTransform;
    [SerializeField] private float timeShowTitle = 1.3f;
    [SerializeField] private float zoomScaleTitle = 1.2f;
    private List<IGift> GiftList { get; set; }
    private List<IPresenter> _rewardPresenters;

    private void Start()
    {
        claimBut?.onClick.AddListener(ClaimGiftPackage);
    }

    private void OnEnable()
    {
        foreach (var titleChar in titleChars)
        {
            titleChar.gameObject.SetActive(false);
        }
        closeTextTransform.gameObject.SetActive(false);
        StartCoroutine(IEShowReward());
    }

    private IEnumerator IEShowReward()
    {
        yield return new WaitForSeconds(.42f);
        StartCoroutine(IEShowTitle());
        yield return new WaitForSeconds(timeShowTitle * 1.4f);
        StartCoroutine(IEShowGift());
    }

    private IEnumerator IEShowTitle()
    {
        var zoomScale = new Vector3(zoomScaleTitle, zoomScaleTitle, zoomScaleTitle);
        var duration = (timeShowTitle / titleChars.Length);
        var wait = new WaitForSeconds(duration * 0.8f);
        foreach (var titleChar in titleChars)
        {
            titleChar.gameObject.SetActive(true);
            titleChar.localScale = Vector3.zero;
            DOTween.Sequence().Join(titleChar.DOScale(zoomScale, duration).SetEase(Ease.Linear))
                .Append(titleChar.DOScale(Vector3.one, duration * 1.3f).SetEase(Ease.InBack));
            yield return wait;
        }
        yield return null;
    }

    private IEnumerator IEShowGift()
    {
        var scaleUnit = _rewardPresenters.Count switch
        {
            1 => 2.2f,
            2 => 1.7f,
            _ => 1.2f
        };
        var wait = new WaitForSeconds(0.18f);
        foreach (var rewardPresenter in _rewardPresenters)
        {
            rewardPresenter.OnShow(0.22f, scaleUnit);
            yield return wait;
        }

        closeTextTransform.gameObject.SetActive(true);
        closeTextTransform.localScale = new Vector3(02f, .02f, .02f);
        closeTextTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    
    private void ClaimGiftPackage()
    {
        StartCoroutine(IEClaimGifts());
    }

    private IEnumerator IEClaimGifts()
    {
        Hub.Hide(gameObject).Play().OnComplete(() =>
        {
            foreach (var rewardPresenter in _rewardPresenters)
            {
                rewardPresenter.Release();
            }
            //todo show effect
            GameDataManager.UpdateCoinVisual();
        });

        yield break;
    }

    public void Setup(List<IGift> gifts)
    {
        GiftList = gifts;
        SetupVisual();
    }
    
    public void Setup(params IGift[] gifts)
    {
        GiftList = gifts.ToList();
        SetupVisual();
    }

    private void SetupVisual()
    {
        var deltaPos = 270f;
        var giftAmount = GiftList.Count;
        var listArrangement = ArrangeStrategy.GetRadiusArrange(giftAmount, -itemsGroup.localPosition.y, deltaPos);
        
        int perfectGiftAmount = 6;
        if (giftAmount > perfectGiftAmount)
        {
            itemsGroup.localPosition -= new Vector3(0, 0.15f * deltaPos * (int)((giftAmount - perfectGiftAmount) / 3 + 1), 0);
        }

        _rewardPresenters = new List<IPresenter>();
        for (var index = 0; index < giftAmount; index++)
        {
            var giftInfo = GiftList[index];
            var giftPresenter = PresentPool.Instance.Get(giftInfo.GetType());
            _rewardPresenters.Add(giftPresenter);
            giftPresenter.SetUpInfo(giftInfo);
            giftPresenter.Present(itemsGroup, listArrangement[index], 0f);
        }
    }
}
