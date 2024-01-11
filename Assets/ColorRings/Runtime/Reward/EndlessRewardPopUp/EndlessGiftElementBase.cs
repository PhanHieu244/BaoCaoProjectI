using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JackieSoft;
using UnityEngine;
using UnityEngine.UI;

public class EndlessGiftElementBase : MonoBehaviour, Cell.IView
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected Text pointText;
    [SerializeField] protected Button claimButton;
    [SerializeField] protected GameObject bg, glow, lockObject, tick, targetCompleteMarker;
    [SerializeField] protected Transform itemRewardGroup;
    private IEndlessRewardStrategy _endlessRewardStrategy;
    protected EndlessRewardPackage endlessRewardPackage;
    private List<IPresenter> _presenterRewardList;
    public const float TimeIncrease = 0.7f;
    private int _targetPoint;
    private int _id;
    private bool _isReceived;

    public virtual void Init(EndlessRewardPackage rewardPackage, IEndlessRewardStrategy endlessRewardStrategy ,int id)
    {
        endlessRewardPackage = rewardPackage;
        _endlessRewardStrategy = endlessRewardStrategy;
        _id = id;
        RewardEndlessViewGenerator.onProgressComplete -= OnUpdateProgress;
        RewardEndlessViewGenerator.onProgressComplete += OnUpdateProgress;
        RewardEndlessViewGenerator.onProgressStart -= OnUpdateProgress;
        RewardEndlessViewGenerator.onProgressStart += OnUpdateProgress;
        Setup();
        GenerateItem();
    }

    private void OnUpdateProgress(int currentID)
    {
        if (_id == currentID - 1){
            glow.SetActive(false);
            return;
        }
        OnStartProgress(currentID);
    }

    private void OnStartProgress(int currentID)
    {
        if (_id != currentID + 1) return;
        if (_targetPoint > _endlessRewardStrategy.GetHighScore)
        {
            _endlessRewardStrategy.UpdateCurrentHighScore(_endlessRewardStrategy.GetHighScore);
            return;
        }
        _endlessRewardStrategy.UpdateCurrentHighScore(_targetPoint);
        StartCoroutine(IEIncrease());
    }
    
    protected virtual void Setup()
    {
        _targetPoint = endlessRewardPackage.Point;
        pointText.text = _targetPoint.ToString();
        DevLog.Log(_targetPoint, _endlessRewardStrategy.GetCurrentHighScore);
        var isMissionComplete = _targetPoint <= _endlessRewardStrategy.GetCurrentHighScore;
        slider.value = isMissionComplete ? slider.maxValue : slider.minValue;
        _isReceived = _endlessRewardStrategy.IsReceiveGift(_id);
        claimButton.gameObject.SetActive(isMissionComplete && !_isReceived);
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(ClaimReward);
        glow.SetActive(false);
        tick.SetActive(_isReceived);
        lockObject.SetActive(!isMissionComplete);
        targetCompleteMarker.SetActive(isMissionComplete);
    }

    protected void GenerateItem()
    {
        _presenterRewardList = new List<IPresenter>();
        if (_isReceived){ return;}
        var gifts = endlessRewardPackage.RewardPackage.GetGiftList();
        foreach (var gift in gifts)
        {
            var presenter = PresentPool.Instance.Get(gift.GetType());
            _presenterRewardList.Add(presenter);
            presenter.SetUpInfo(gift);
            presenter.Present(itemRewardGroup, Vector3.zero, 1f);
        }
    }

    private void ClaimReward()
    {
        endlessRewardPackage.RewardPackage.ClaimRewardPackage("EndlessClassicGift");
        claimButton.gameObject.SetActive(false);
        _endlessRewardStrategy.ReceiveGift(_id);
        ReleaseItem();
        _presenterRewardList = null;
        tick.SetActive(true);
    }
    
    protected IEnumerator IEIncrease()
    {
        slider.value = slider.minValue;
        slider.DOValue(slider.maxValue, TimeIncrease).OnComplete((() =>
        {
            RewardEndlessViewGenerator.onProgressComplete.Invoke(_id);
            targetCompleteMarker.SetActive(true);
            ShowGlow();
            if (_endlessRewardStrategy.IsReceiveGift(_id)) return;
            claimButton.gameObject.SetActive(true);
        }));
        yield return new WaitForSeconds(TimeIncrease / 4f);
        Unlock();
        //todo unlock anim
    }

    private void ShowGlow()
    {
        glow.SetActive(true);
        glow.transform.localScale = new Vector3(.02f, 0.02f, .02f);
        glow.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }

    private void Unlock()
    {
        lockObject.transform.DOScale(new Vector3(.02f, 0.02f, .02f), 0.35f).SetEase(Ease.InBack).OnComplete((() =>
        {
            lockObject.SetActive(false);
            lockObject.transform.localScale = Vector3.one;
        }));
    }

    private void OnDisable()
    {
        ReleaseItem();
    }

    private void ReleaseItem()
    {
        if (_presenterRewardList is null) return;
        foreach (var presenterReward in _presenterRewardList)
        {
            presenterReward.Release();
        }
    }
}