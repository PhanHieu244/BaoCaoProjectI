using System;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIInGame : MonoSingleton<UIInGame>
{
    [SubclassSelector, SerializeReference] public IDataLevelLog dataLevelLog = new NoneLevelLog(); 
    [SerializeField] private ChallengeManager challengeManager;
    [SerializeField] private Text tLevel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private InGamePowerUpContainer<InGamePowerUpConfig> hammerItem;
    [SerializeField] private InGamePowerUpContainer<InGamePowerUpConfig>  destroyRowItem;
    [SerializeField] private InGamePowerUpContainer<InGamePowerUpConfig>  destroyColItem;
    [SerializeField] private InGamePowerUpContainer<InGamePowerUpConfig>  swapItem;

    protected override void Awake()
    {
        base.Awake();
        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        
        pauseButton.onClick.AddListener(() =>
        {
            if (!TutorialManager.IsTurotialDone) return;
            AudioManager.Instance.PlaySound(EventSound.Click);
            var popup = Hub.Get<UIPause>(PopUpPath.POP_UP_WOOD__UI_PAUSE);
            Hub.Show(popup.gameObject).Play();
        });
        
    }

    public virtual void UpdateScore(int score)
    {
        
    }

    public void SetUp(Level level, string levelName)
    {
        tLevel.text = levelName;
        challengeManager.SetUp(level.challenges);
    }


    public ChallengeManager ChallengeManager => challengeManager;

    public Vector2 GetItemPos(ItemType type)
    {
        switch (type)
        {
            case ItemType.None:
                break;
            case ItemType.Hammer:
                return GetHammerItemPos();
            case ItemType.DestroyARow:
                return GetDestroyRowItemPos();
            case ItemType.DestroyACol:
                return GetDestroyColPos();
            case ItemType.Swap:
                return GetSwapItemPos();
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        return Vector2.zero;
    }

    public Vector2 GetItemPos(PowerUpType type) => type switch
    {
        PowerUpType.Tile => hammerItem.transform.position,
        PowerUpType.Swap => swapItem.transform.position,
        PowerUpType.Row => destroyRowItem.transform.position,
        PowerUpType.Line => destroyColItem.transform.position,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        /*PowerUpType.Tile => hammerItem.GetComponent<RectTransform>()?.position ?? Vector2.zero,
        PowerUpType.Swap => swapItem.GetComponent<RectTransform>()?.position ?? Vector2.zero,
        PowerUpType.Row => destroyRowItem.GetComponent<RectTransform>()?.position ?? Vector2.zero,
        PowerUpType.Line => destroyColItem.GetComponent<RectTransform>()?.position ?? Vector2.zero,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)*/
    };

    private Vector2 GetHammerItemPos()
    {
        hammerItem.Unlock();
        hammerItem.Select(true, true);
        return hammerItem.transform.position;
    }

    private Vector2 GetDestroyRowItemPos()
    {
        destroyRowItem.Unlock();
        destroyRowItem.Select(true, true);
        return destroyRowItem.transform.position;
    }

    private Vector2 GetDestroyColPos()
    {
        destroyColItem.Unlock();
        destroyColItem.Select(true, true);
        return destroyColItem.transform.position;
    }

    private Vector2 GetSwapItemPos()
    {
        swapItem.Unlock();
        swapItem.Select(true, true);
        return swapItem.transform.position;
    }
}


public interface IScore
{
    int Score { get; set; }
}

public interface IDataLevelLog
{
    bool LogLevel(out int level,out string difficult);
    bool LogAdditive(out int id, out int numeral,out string propertyName);
}

[Serializable]
public class NoneLevelLog : IDataLevelLog
{
    public bool LogLevel(out int level, out string difficult)
    {
        level = -1;
        difficult = "";
        return false;
    }

    public bool LogAdditive(out int id, out int numeral, out string propertyName)
    {
        id = -1;
        numeral = -1;
        propertyName = "";
        return false;
    }
}

[Serializable]
public class AdvancedModeLevelLog : IDataLevelLog
{
    public bool LogLevel(out int level, out string difficult)
    {
        level = BoardShapeEndlessManager.BoardID;
        difficult = "Advanced Mode";
        return true;
    }

    public bool LogAdditive(out int id, out int numeral, out string propertyName)
    {
        id = BoardShapeEndlessManager.BoardID;
        propertyName = "Advanced Mode Score";
        numeral = GameManagerEndless.CurrentScore;
        return true;
    }
}