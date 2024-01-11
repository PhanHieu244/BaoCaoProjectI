using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DisplayWinningStreakBar : MonoBehaviour
{
    [SerializeReference, SubclassSelector] private IDisplayWinningStreakCondition _condition = new DisplayWSInGame();
    [SerializeField] private float height;
    private RectTransform _rectTransform;
    private Image _bgImage;
    private BaseWinningStreakBar _winningStreakBar;

    private void Start()
    {
        if (_condition.IsAvailable()) return;
        UnDisplayBar();
    }

    private void UnDisplayBar()
    {
        _winningStreakBar = GetComponentInChildren<BaseWinningStreakBar>();
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, height);
        _winningStreakBar?.gameObject.SetActive(false);
    }
}

public interface IDisplayWinningStreakCondition
{
    bool IsAvailable();
}

[Serializable]
public class DisplayWSInGame : IDisplayWinningStreakCondition
{
    public bool IsAvailable()
    {
        return WinningStreakActive.isActive;
    }
}

[Serializable]
public class DisplayWSInInfo : IDisplayWinningStreakCondition
{
    [SerializeField] private AdventureLevelInfoFrame adventureLevelInfoFrame;
    
    public bool IsAvailable()
    {
        return WinningStreakActive.IsActiveWinningStreak(adventureLevelInfoFrame.Level);
    }
}