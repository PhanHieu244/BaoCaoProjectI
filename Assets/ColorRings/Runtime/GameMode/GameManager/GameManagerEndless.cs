using System;
using UnityEngine;

public class GameManagerEndless : GameManager
{
    [SubclassSelector, SerializeReference] private IUpdatePoint _updatePointStrategy = new UpdateClassicPoint();
    public static int CurrentScore { get; private set; }
    public static int HighScore { get; private set; }
    public static bool HasHigherScore { get; private set; }
    
    protected override void Start()
    {
        CurrentScore = 0;
        HighScore = _updatePointStrategy.HighScorePoint;
        OnLoseGame += SavePoint;
        base.Start();
    }

    protected override void ActionCombo(int comboCount)
    {
        _updatePointStrategy.UpdatePoint(comboCount);
        CurrentScore = _updatePointStrategy.CurrentPoint;
        base.ActionCombo(comboCount);
    }

    protected override void SpawnRing()
    {
        AudioManager.Instance.PlaySound(EventSound.SpawnRing);
        RingSpawner.SpawnEndless();
    }

    private void SavePoint()
    {
        _updatePointStrategy.SavePoint();
        HasHigherScore = _updatePointStrategy.HasHigherScore;
    }
}

public interface IUpdatePoint
{
    int CurrentPoint { get; }
    int HighScorePoint { get; }
    bool HasHigherScore { get; }
    void UpdatePoint(int comboCount);
    void SavePoint();
}

[Serializable]
public class UpdateClassicPoint : IUpdatePoint
{
    public int CurrentPoint { get; private set; }
    public int HighScorePoint => GameDataManager.HighScoreEndLess;
    public bool HasHigherScore => _hasHigher;
    private int _pointPerRing = 10;
    private bool _hasHigher;

    public void UpdatePoint(int comboCount)
    {
        if (comboCount <= 0) return;
        CurrentPoint += comboCount * _pointPerRing;
        UIInGameEndless.Instance.UpdateScore(CurrentPoint);
    }
    public void SavePoint()
    {
        if (CurrentPoint < GameDataManager.HighScoreEndLess) return;
        _hasHigher = true;
        GameDataManager.HighScoreEndLess = CurrentPoint;
    }
}


[Serializable]
public class UpdateAdvancedPoint : IUpdatePoint
{
    public int CurrentPoint { get; private set; }
    public int HighScorePoint => GameDataManager.HighScoreAdvancedEndLess;
    public bool HasHigherScore => _hasHigher;
    private int _pointPerRing = 10;
    private bool _hasHigher;
    
    public void UpdatePoint(int comboCount)
    {
        if (comboCount <= 0) return;
        CurrentPoint += comboCount * _pointPerRing;
        UIInGameEndless.Instance.UpdateScore(CurrentPoint);
    }
    public void SavePoint()
    {
        if (CurrentPoint < GameDataManager.HighScoreAdvancedEndLess) return;
        _hasHigher = true;
        GameDataManager.HighScoreAdvancedEndLess = CurrentPoint;
    }
}
