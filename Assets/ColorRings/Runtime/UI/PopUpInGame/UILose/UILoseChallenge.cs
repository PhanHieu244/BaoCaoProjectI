using System;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UILoseChallenge : UIBaseLose
{
    [SerializeField] private Text levelText;
    [SerializeField] private GameObject targetGroup;
    [SerializeField] private ColorItemTarget itemPrefab;

    protected override void OnEnable()
    {
        base.OnEnable();
        levelText.text = $"LEVEL {GameDataManager.LevelToShow(GameDataManager.MaxLevelUnlock + 1)}";
        var colorChallenges = UIInGame.Instance.ChallengeManager.GetColorChallenge();
        foreach (var colorChallenge in colorChallenges)
        {
            var colorItem = Instantiate(itemPrefab, targetGroup.transform);
            
            colorItem.SetUp(colorChallenge.ColorOfChallenge, colorChallenge.IsComplete());
        }
    }
}