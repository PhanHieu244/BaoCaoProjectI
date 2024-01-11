using System;
using System.Collections;
using System.Globalization;
using Coffee.UIExtensions;
using Falcon.FalconAnalytics.Scripts.Enum;
using Jackie.Soft;
using JetBrains.Annotations;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Canvas), typeof(GraphicRaycaster))]
public abstract class InGamePowerUpContainer<T> : PowerUpContainer<T>, IPowerUpExecution where T : InGamePowerUpConfig
{
    private Button button;
    private Canvas powerUpCanvas;
    [SerializeField] protected Image icon;
    [SerializeField] protected GameObject effect;
    [SerializeField] protected Text countText;
    [SerializeField] protected GameObject adsPlay;
    [SerializeField] protected GameObject lockItem;
    [SerializeField] protected int sortingOrder;
    [SerializeField] protected Material material;
    [SerializeField] protected Image bg;
    [SerializeField] protected Image bgAmount;
    [SerializeField] protected GameObject blockUI;

    [SerializeField, SortingLayerSelection] protected string layer, layerUp;

    [SerializeField] protected ItemType itemType;
    protected bool isTutorialing;
    protected bool isTutorialLevel;
    protected bool isUnlocked;
    private bool _isSelected;

    #region WinningStreak

    [SerializeField] private Sprite winningStreakButImg;
    private Sprite _normalBgBut;
    private int _winningStreakAmount;
    protected bool IsWinningStreakMode => _winningStreakAmount > 0;
    
    private void ActiveWinningStreakPowerUp(PowerUpType winningStreakType)
    {
        if(GetItemType(winningStreakType) != itemType) return;
        var blinkParticle = PoolManager.Instance["blinkEffect"].Get<UIParticle>(bg.transform);
        blinkParticle.rectTransform.anchoredPosition = bg.rectTransform.anchoredPosition;
        blinkParticle.Play();
        _winningStreakAmount++;
        _normalBgBut = bg.sprite;
        bg.sprite = winningStreakButImg;
        adsPlay.SetActive(false);
        countText.gameObject.SetActive(false);
    }

    protected void UnActiveWinningStreakPowerUp()
    {
        _winningStreakAmount--;
        if (IsWinningStreakMode) return;
        bg.sprite = _normalBgBut;
    }

    private bool WinningStreakOnclick()
    {
        if (!IsWinningStreakMode)
        {
            return false;
        }

        if (InGamePowerUpManager.Instance.PowerUpExecution == null && isUnlocked)
        {
            InGamePowerUpManager.Instance.PowerUpExecution = this;
            Select(true);
            return true;
        }

        if (!ReferenceEquals(InGamePowerUpManager.Instance.PowerUpExecution, this)) return true;
        //block button when use power up in tutorial mode
        InGamePowerUpManager.Instance.PowerUpExecution = null;
        Select(false);
        return true;
    }
    
    private ItemType GetItemType(PowerUpType powerUpType) =>
        powerUpType switch
        {
            PowerUpType.Tile => ItemType.Hammer,
            PowerUpType.Row => ItemType.DestroyARow,
            PowerUpType.Line => ItemType.DestroyACol,
            PowerUpType.Swap => ItemType.Swap,
            _ => ItemType.None
        };

    #endregion

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        powerUpCanvas = GetComponent<Canvas>();
        powerUpCanvas.overrideSorting = true;
        powerUpCanvas.sortingLayerName = layer;
        powerUpCanvas.sortingOrder = sortingOrder;
        button.onClick.AddListener(OnClick);
        icon.sprite = powerUpConfig.sprite;
        if (winningStreakButImg == null) winningStreakButImg = bg.sprite;
        CheckUnLock();
        Count(powerUpData.Count);
        UIManager.onCountPowerUpChange += UpdateCount;
        UIManager.onWinningStreakReward += ActiveWinningStreakPowerUp;
        
    }

    private void OnDestroy()
    {
        UIManager.onWinningStreakReward -= ActiveWinningStreakPowerUp;
        UIManager.onCountPowerUpChange -= UpdateCount;
    }

    protected bool HaveCamera() => Camera.main is not null;

    protected virtual void OnClick()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        if (WinningStreakOnclick())
        {
            return;
        }
        
        var level = GameManager.Instance.Level;
        isTutorialing = false;
        var tutorial = level.tutorialMod;
        if (tutorial != null && !TutorialManager.IsTurotialDone &&
            tutorial.IsNormalTutorial()) return;
        if (tutorial != null && tutorial.GetItemType() != itemType && !tutorial.IsDoneTutorial()) return;
        if (tutorial != null && tutorial.GetItemType() == itemType) isTutorialing = true;

        if (InGamePowerUpManager.Instance.PowerUpExecution == null && isUnlocked)
        {
            if (powerUpData.Count <= 0 && !isTutorialing)
            {
                AdsManager.Instance.ShowReward($"IN_GAME_ADD_ITEM_{powerUpData.ToString().ToUpper()}" ,() =>
                {
                    powerUpData.Count++;
                    Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock,FlowType.Source, "ads_in_game_reward", powerUpData.ToString(), powerUpData.ToString(), 1 );
                    Select(false);
                    Count(powerUpData.Count);
                });
            }
            else
            {
                InGamePowerUpManager.Instance.PowerUpExecution = this;
                if (!isTutorialing)
                {
                    powerUpData.Count--;
                }
                Select(true);
                Count(powerUpData.Count);
            }
        }
        else 
        {
            if (ReferenceEquals(InGamePowerUpManager.Instance.PowerUpExecution, this))
            {
                //block button when use power up in tutorial mode
                if (tutorial != null && isTutorialing && !tutorial.IsDoneTutorial()) return;
                if (!isTutorialing)
                {
                    powerUpData.Count++;
                }
                InGamePowerUpManager.Instance.PowerUpExecution = null;
                Select(false);
                Count(powerUpData.Count);
            }
        }

        //Check Tutorial
        if (tutorial != null && tutorial.IsItemTutorial() && !tutorial.IsDoneTutorial())
        {
            tutorial.IsClicked();
            TutorialManager.Instance.TutorialInput.ContinueTutorial();
        }

    }

    protected virtual void Select(bool onSelected)
    {
        _isSelected = onSelected;
        if (powerUpCanvas is null) return;
        powerUpCanvas.sortingLayerName = onSelected ? layerUp : layer;
        effect.SetActive(onSelected);
        blockUI.SetActive(onSelected);
    }

    public void Select(bool onSelected, bool isTutorialSelect)
    {
        Select(onSelected);
        if (!isTutorialSelect) return;
        adsPlay.SetActive(false);
        countText.gameObject.SetActive(true);
        countText.text = "∞";
    }

    private void UpdateCount()
    {
        _isSelected = false;
        if (IsWinningStreakMode)
        {
            UnActiveWinningStreakPowerUp();
        }
        Count(powerUpData.Count);
    }

    protected virtual void Count(int count)
    {
        var isAdsPlayActive = count < 1 && !_isSelected; 
        adsPlay.SetActive(isAdsPlayActive);
        countText.gameObject.SetActive(!isAdsPlayActive);

        if (count >= 0)
        {
            countText.text = count.ToString(CultureInfo.CurrentCulture);
        }

        if (!isTutorialing) return;
        adsPlay.SetActive(false);
        countText.gameObject.SetActive(true);
        countText.text = "∞";
        lockItem.SetActive(false);
    }
    
    
    public virtual IEnumerator IEExecute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        Select(false);
        if(isUpdateCount) UpdateCount();
        Message.Use<Type>().Event(typeof(ChallengeManager)).Execute<ChallengeManager>(c => c.Submit());
        var tutorial = GameManager.Instance.Level.tutorialMod;

        // Check Tutorial Done
        if (tutorial != null && tutorial.IsItemTutorial())
        {
            TutorialManager.Instance.TutorialDone();
        }
        else
        {
            if (powerUpData != null)
            {
                Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Sink, "using_item_in_game", powerUpData.ToString(), powerUpData.ToString(), 1);
            }
        }
        //Need to check Win Lose
        yield break;
    }

    private void SetMaterial(Material materialPowerUp)
    {
        icon.material = materialPowerUp;
        bg.material = materialPowerUp;
        bgAmount.material = materialPowerUp;
    }

    private void SetLockItem()
    {
        if (isUnlocked || powerUpData.Count > 0)
        {
            lockItem.SetActive(false);
            return;
        }
        lockItem.SetActive(true);
        adsPlay.SetActive(false);
    }

    public void Unlock()
    {
        var indexItem = (int)itemType;
        if (GameDataManager.MaxItemUnlock < indexItem) GameDataManager.MaxItemUnlock = indexItem;
        SetMaterial(null);
        isUnlocked = true;
        SetLockItem();
    }

    private void CheckUnLock()
    {
        var itemIndex = (int)itemType;
        if (itemIndex > GameDataManager.MaxItemUnlock)
        {
            isUnlocked = false;
            SetMaterial(material);
        } else
        {
            isUnlocked = true;
            SetMaterial(null);
        }
        SetLockItem();
    }

    public override string ToString() => powerUpData.ToString();

    private class NormalClickStrategy
    {
        private void OnClick()
        {
            
        }
    }
}



public enum ItemType
{
    None,
    Hammer,
    DestroyARow,
    DestroyACol,
    Swap
}