using System;
using System.Collections.Generic;
using Coffee.UIEffects;
using ColorRings.Runtime.UI.Navigator.HomeNavigation;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class LevelInfoButton : MonoBehaviour
{
    [SerializeField] private LevelInfoButtonVisual[] visuals;
    private Dictionary<LevelScrollType, LevelInfoButtonVisual> _visualDict;
    private LevelScrollType _levelType = LevelScrollType.Normal;
    private Image _buttonImage;
    private Button _levelBut;
    private Text[] _textInChildren;
    private Text _levelText;
    private Text _modeText;
    private UIShadow[] _uiShadows;
    private UIShadow[] _modeShadows;
    private Outline _outline;

    private void Awake()
    {
        _buttonImage = GetComponent<Image>();
        _levelBut = GetComponent<Button>();
        _textInChildren = GetComponentsInChildren<Text>();
        if (_textInChildren.Length >= 1)
            _levelText = _textInChildren[0];
        if (_textInChildren.Length >= 2)
            _modeText = _textInChildren[1];
        if (transform.childCount >= 1)
            _uiShadows = transform.GetChild(0).GetComponents<UIShadow>();
        if (transform.childCount >= 2)
            _modeShadows = transform.GetChild(1).GetComponents<UIShadow>();
        InitDictionary();
        _levelBut.onClick.AddListener(OnShowLevelInfo);
        LevelScrollViewInit.onShowCurrentLevelType += SetupButton;
    }

    private void InitDictionary()
    {
        _visualDict = new Dictionary<LevelScrollType, LevelInfoButtonVisual>();
        foreach (var visual in visuals)
        {
            _visualDict[visual.LevelScrollType] = visual;
        }
    }

    private void SetupButton(LevelScrollType levelType)
    {
        _levelType = levelType;
        if (!_visualDict.ContainsKey(levelType))
        {
            DevLog.Log($"Dont have this type",levelType.ToString());
            levelType = LevelScrollType.Normal;
        }
        
        var visual = _visualDict[levelType];
        if (_buttonImage == null || _uiShadows == null) return;
        _buttonImage.sprite = visual.ButtonSprite;
        //_levelText.color = visual.TextColor;
        foreach (var uiShadow in _uiShadows)
        {
            uiShadow.effectColor = visual.OutlineColor;
        }

        // test
        if (levelType == LevelScrollType.Hard || levelType == LevelScrollType.SuperHard ||
            levelType == LevelScrollType.HardRewardSkin || levelType == LevelScrollType.SuperHardRewardSkin)
        {
            _levelText.fontSize = 110;
            _levelText.rectTransform.anchoredPosition = new Vector2(-10, 70f);

            _modeText.text = visual.ModeText;
            _modeText.fontSize = visual.ModeTextSize;
            //_modeText.color = visual.ModeTextColor;
            _modeText.gameObject.SetActive(true);
            foreach (var modeShadow in _modeShadows)
            {
                modeShadow.effectColor = visual.ModeOutlineColor;
            }
        }
        else
        {
            _levelText.fontSize = 120;
            _levelText.rectTransform.anchoredPosition = new Vector2(-10, 40f);

            _modeText.gameObject.SetActive(false);
        }
    }

    private void OnShowLevelInfo()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        string path = _levelType switch
        {
            LevelScrollType.Normal => PopUpPath.POP_UP_WOOD__UI_ADVENTUREINFO,
            LevelScrollType.Bonus => PopUpPath.POP_UP_WOOD__UI_BONUSINFO,
            _ => PopUpPath.POP_UP_WOOD__UI_ADVENTUREINFO
        };
        var frame = Hub.Get<InfoLevelFrame>(path);
        Hub.Show(frame.gameObject).Play();
        frame.transform.SetParent(null);
        frame.InitFrameData(GameDataManager.MaxLevelUnlock);
    }

}

[Serializable]
public class LevelInfoButtonVisual
{
    [field: SerializeField] public LevelScrollType LevelScrollType { get; private set; }
    [field: SerializeField] public Sprite ButtonSprite { get; private set; }
    [field: SerializeField] public UnityEngine.Color TextColor { get; private set; }
    [field: SerializeField] public UnityEngine.Color OutlineColor { get; private set; }
    [field: SerializeField] public string ModeText { get; private set; }
    [field: SerializeField] public int ModeTextSize { get; private set; }
    [field: SerializeField] public UnityEngine.Color ModeTextColor { get; private set; }
    [field: SerializeField] public UnityEngine.Color ModeOutlineColor { get; private set; }
}