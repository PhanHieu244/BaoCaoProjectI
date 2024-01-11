using System.Collections.Generic;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class AdventureLevelInfoFrame : InfoLevelFrame
{
    [SerializeField] private Text levelTitleText;
    [SerializeField] private ColorInfoChallenge colorChallengePrefab;
    [SerializeField] private Transform challengeContain;
    private List<ColorInfoChallenge> colorChallenges;
    private Level _levelData;
    private Skin _skin;
    private int _currentLevel = -1;
    private int _currentSkinID;

    public int Level => _currentLevel + 1;
    
    private void SetupFrame(int nextLevel)
    {
        if (colorChallenges is not null)
        {
            foreach (var colorChallenge in colorChallenges)
            {
                Destroy(colorChallenge.gameObject);
            }
        }

        colorChallenges = new List<ColorInfoChallenge>();
        
        var challenge = _levelData.challenges;
        foreach (var challengeData in challenge)
        {
            if (challengeData is not ColorChallenge.ColorData colorData) continue;
            var colorChallenge = Instantiate(colorChallengePrefab, challengeContain);
            colorChallenges.Add(colorChallenge);
            colorChallenge.Set(colorData, _skin);
        }

        levelTitleText.text =$"Level {GameDataManager.LevelToShow(nextLevel)}";
        playBut.onClick.RemoveAllListeners();
        playBut.onClick.AddListener((() =>     
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            GameLoader.Instance.Load(_currentLevel);
        }));
    }

    public override void InitFrameData(int currentLevel = -1)
    {
        _currentSkinID = GameDataManager.CurrentSkin;
        _skin = GameDataManager.GetSkinByID(_currentSkinID);
        UpdateSkinVisual();
        if (currentLevel == _currentLevel) return;
        _currentLevel = currentLevel;
        _levelData = GameLoader.GetNextLevelInfo(_currentLevel);
        SetupFrame(currentLevel + 1);
    }

    private void UpdateSkinVisual()
    {
        if (colorChallenges is null) return;
        foreach (var colorChallenge in colorChallenges)
        {
            colorChallenge.SetUpVisualChallenge(_skin);
        }
    }
}

public abstract class InfoLevelFrame : PopUpContent
{
    [SerializeField] protected Button playBut;
    [SerializeField] private Button closeBut;
    public abstract void InitFrameData(int currentLevel = -1);
    
    private void Start()
    {
        closeBut?.onClick.AddListener((() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            Hub.Hide(gameObject).Play();
        }));
    }
}