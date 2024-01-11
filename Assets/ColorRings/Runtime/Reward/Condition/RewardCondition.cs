using System;
using UnityEngine;

public interface ICondition
{
    bool IsAvailable();
}

[Serializable]
public class LevelCondition : ICondition
{
    [SerializeField] private int level;

    public bool IsAvailable()
    {
        return GameDataManager.MaxLevelToShow > level;
    }
}

[Serializable]
public class EndlessClassicPointCondition : ICondition
{
    [SerializeField] private int point;
    public bool IsAvailable()
    {
        return GameDataManager.HighScoreEndLess >= point;
    }
}

[Serializable]
public class EndlessAdvancedPointCondition : ICondition
{
    [SerializeField] private int point;
    public bool IsAvailable()
    {
        return GameDataManager.HighScoreAdvancedEndLess >= point;
    }
}