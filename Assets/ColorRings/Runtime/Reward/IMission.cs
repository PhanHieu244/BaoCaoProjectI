using System;
using UnityEngine;

public interface IMission
{
    bool IsMissionComplete();
}

[Serializable]
public class BreakRingsMission : IMission
{
    [field: SerializeField] public int RingsAmount { get; private set; }
    
    public bool IsMissionComplete()
    {
        return false;
    }
}

[Serializable]
public abstract class PointMission : IMission
{
    [field: SerializeField] public int Point { get; private set; }
    public abstract bool IsMissionComplete();
}
[Serializable]
public class EndlessClassicPointMission : PointMission
{
    public override bool IsMissionComplete()
    {
        return GameDataManager.HighScoreEndLess >= Point;
    }
}