using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class PreciousTreasureBar : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected GameObject fillBar;
    [SerializeField] protected GameObject tick;
    [SerializeField] protected Text infoText;
    [SerializeField] private RectTransform itemRewardPos;
    [SerializeField] private PreciousTreasureSO preciousTreasureSo;

    private List<IGift> _giftPackageInfo;
    private TreasureChest _treasureChest;
    private IPresenter _giftItem;
    private BlockUIPopUp _blockUIPopUp;
    
    private const float StepRate = 0.02f;
    private const float ItemDisplayDuration = 0.35f;
    private int _currentRingBreakAmount;
    private int _ringBreakAddAmount;
    private int _ringAmountTarget;
    private bool _hasTreasure;
    private bool _hasReward;
    
#if UNITY_EDITOR
    [Space][Space]
    [SerializeField] private int ringbreak;
    [Button()]
    private void Add()
    {
        GameDataManager.RingBreakAdd = ringbreak;
    }
    [Button()]
    private void Refresh()
    {
        GameDataManager.ResetPreciousData();
    }
#endif

    private void Awake()
    {
        _blockUIPopUp = Hub.Get<BlockUIPopUp>(PopUpPath.POP_UP_WOOD__UI_BLOCKUI);
        _giftPackageInfo = new List<IGift>();
    }

    private void OnEnable()
    {
        tick.SetActive(false);
        CheckPreciousTreasureData();
        Setup();
        if (_ringBreakAddAmount <= 0) return;
        StartCoroutine(IEIncrease());
    }

    private static void CheckPreciousTreasureData()
    {
        if (!GameDataManager.IsChangeWeek()) return;
        GameDataManager.ResetPreciousData();
    }

    private IEnumerator IEIncrease()
    {
        Hub.Show(_blockUIPopUp.gameObject);
        _blockUIPopUp.Block();
        yield return new WaitForSeconds(1f);
        IncreaseSlider();
    }

    private void Setup()
    {
        _currentRingBreakAmount = GameDataManager.RingBreakAmount;
        _ringBreakAddAmount = GameDataManager.RingBreakAdd;
        LoadTreasure();
        if(_currentRingBreakAmount <= 0) fillBar.SetActive(false);
        if (!_hasTreasure)
        {
            ShowComplete();
            return;
        }
        _ringAmountTarget = _treasureChest.BreakRingsMission.RingsAmount;
        slider.maxValue = _ringAmountTarget;
        slider.value = _currentRingBreakAmount;
        UpdateVisual();
    }

    private void ShowComplete()
    {
        slider.value = slider.maxValue;
        infoText.text = "Complete";
        tick.SetActive(true);
        fillBar.SetActive(true);
    }
    
    private void UpdateTreasure()
    {
        GameDataManager.TreasureID++;
        GameDataManager.RingBreakAmount = 0;
        Setup();
    }

    private void LoadTreasure()
    {
        _treasureChest = preciousTreasureSo[GameDataManager.TreasureID];
        if (_treasureChest is null)
        {
            _hasTreasure = false;
            return;
        }
        _hasTreasure = true;
        if (_ringBreakAddAmount <= 0 && _giftItem is not null) return;
        _giftItem = PresentPool.Instance.Get(_treasureChest.Gift.GetType());
        _giftItem.SetUpInfo(_treasureChest.Gift);
        _giftItem.Present(itemRewardPos);
    }
    
    private void IncreaseSlider()
    {
        if (!_hasTreasure)
        {
            _blockUIPopUp.UnBlock();
            return;
        }
        if (_ringBreakAddAmount <= 0)
        {
            OnShowReward();
            return;
        }
        fillBar.SetActive(true);
        StartCoroutine(IEIncrease(_ringBreakAddAmount, 0.02f));
    }
    
    private void UpdateVisual()
    {
        infoText.text = $"{_currentRingBreakAmount}/{_ringAmountTarget}";
    }

    private IEnumerator IEIncrease(int increaseValue, float timeStep)
    {
        var timeWaitStep = new WaitForSeconds(timeStep);
        var ringBreakStart = _currentRingBreakAmount;
        var targetValue = _currentRingBreakAmount + increaseValue;
        var step = (_ringAmountTarget * StepRate);
        bool passTarget = false;
        if (targetValue >= _ringAmountTarget)
        {
            targetValue = _ringAmountTarget;
            passTarget = true;
        }
        while (slider.value + step <= targetValue)
        {
            slider.value += step;
            _currentRingBreakAmount = (int)slider.value;
            UpdateVisual();
            yield return timeWaitStep;
        }

        _ringBreakAddAmount -= (targetValue - ringBreakStart);
        _currentRingBreakAmount = targetValue;
        slider.value = targetValue;
        GameDataManager.RingBreakAmount = _currentRingBreakAmount;
        GameDataManager.RingBreakAdd = _ringBreakAddAmount;
        UpdateVisual();
        if (!passTarget)
        {
            OnShowReward();
            yield break;
        }
        AddReward();
    }

    private void OnShowReward()
    {
        _blockUIPopUp.UnBlock();
        if(!_hasReward) return;
        StartCoroutine(ShowReward());
    }
    
    private void AddReward()
    {
        _hasReward = true;
        _treasureChest.Gift.ClaimGift("treasure");
        _giftPackageInfo.Add(_treasureChest.Gift);
        DOTween.Sequence().Append(_giftItem.OnHide(ItemDisplayDuration)).AppendCallback(UpdateTreasure).OnComplete((() =>
        {
            if (!_hasTreasure)
            {
                OnShowReward();
                return;
            }
            _giftItem.OnShow(ItemDisplayDuration);
            IncreaseSlider();
        }));
        
    }

    private IEnumerator ShowReward()
    {
        yield return new WaitForSeconds(0.4f);
        var giftPackageDisplay = Hub.Get<GiftPackageDisplay>(PopUpPath.POP_UP_WOOD__UI_REWARDPRECIOUSTREASURE);
        giftPackageDisplay.transform.SetParent(null);
        giftPackageDisplay.Setup(_giftPackageInfo);
        Hub.Show(giftPackageDisplay.gameObject).Play();
        _hasReward = false;
    }
    
}