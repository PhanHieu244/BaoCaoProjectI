using JackieSoft;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonScrollBase : MonoBehaviour, Cell.IView
{
    [Header("Setup button level")]
    [SerializeField] private Text textLevel;
    [SerializeField] private Text newSkinText;
    [SerializeField] private Image image;
    [SerializeField] private GameObject skinRewardRibbon;
    [SerializeField] private Sprite normalLevel;
    [SerializeField] private Sprite bonusLevel;
    [SerializeField] private Sprite skinRewardLevel;
    [SerializeField] private Sprite hardLevel;
    [SerializeField] private Sprite superHardLevel;
    [Header("Setting text level pos")]
    [SerializeField] private Vector3 normalTextPos = new (0, 27f, 0);
    [SerializeField] private Vector3 skinTextPos = new (0, 27f, 0);
    [Header("Setting level text size")]
    [SerializeField] private Vector2 normalTextSize = new (0, 27f);
    [SerializeField] private Vector2 skinTextSize = new (0, 10);

    public virtual void Init(string level ,LevelScrollType levelScrollType)
    {
        textLevel.text = level;
        newSkinText.gameObject.SetActive(
            levelScrollType == LevelScrollType.RewardSkin ||
            levelScrollType == LevelScrollType.HardRewardSkin ||
            levelScrollType == LevelScrollType.SuperHardRewardSkin
        );
        switch (levelScrollType)
        {
            case LevelScrollType.Normal:
                skinRewardRibbon.SetActive(false);
                Setup(normalLevel, normalTextPos, normalTextSize);
                break;
            case LevelScrollType.Bonus:
                skinRewardRibbon.SetActive(false);
                Setup(bonusLevel, normalTextPos, normalTextSize);
                break;
            case LevelScrollType.RewardSkin:
                skinRewardRibbon.SetActive(false);
                Setup(skinRewardLevel, skinTextPos, skinTextSize);
                break;
            case LevelScrollType.Hard:
                skinRewardRibbon.SetActive(false);
                Setup(hardLevel, normalTextPos, normalTextSize);
                break;
            case LevelScrollType.SuperHard:
                skinRewardRibbon.SetActive(false);
                Setup(superHardLevel, normalTextPos, normalTextSize);
                break;
            case LevelScrollType.HardRewardSkin:
                skinRewardRibbon.SetActive(true);
                Setup(hardLevel, skinTextPos, skinTextSize);
                break;
            case LevelScrollType.SuperHardRewardSkin:
                skinRewardRibbon.SetActive(true);
                Setup(superHardLevel, skinTextPos, skinTextSize);
                break;
            default:
                DevLog.Log("dont have this level type", levelScrollType);
                skinRewardRibbon.SetActive(false);
                Setup(normalLevel, normalTextPos, normalTextSize);
                break;
        }
    }

    private void Setup(Sprite buttonImage, Vector3 localTextLevelPos, Vector2 textSize)
    {
        var textLevelTransform = textLevel.rectTransform;
        image.sprite = buttonImage;
        image.preserveAspect = true;
        image.SetNativeSize();
        textLevelTransform.localPosition = localTextLevelPos;
        textLevelTransform.sizeDelta = textSize;
    }
}

public enum LevelScrollType
{
    Normal,
    Bonus,
    RewardSkin,
    Hard,
    SuperHard,
    HardRewardSkin,
    SuperHardRewardSkin
}