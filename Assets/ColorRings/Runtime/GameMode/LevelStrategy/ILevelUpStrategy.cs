using System;
using UnityEngine;

public interface ILevelUpStrategy
{ 
    int ColorAmountStart { get; }
    int GetAmountRingsToLevelUp(int maxColorAmount);
}

[Serializable]
public class LevelUpByConstant : ILevelUpStrategy
{
    [Range(2, (int) Color.COUNT - 1)]
    [SerializeField] private int colorAmountStart = 3;
    [SerializeField] private int ringAmountToLevelUp = GameConst.TimesSpawnEndless;

    public int ColorAmountStart => colorAmountStart;
    public int GetAmountRingsToLevelUp(int maxColorAmount) => ringAmountToLevelUp;
}

[Serializable]
public class LevelUpByIncreaseRange : ILevelUpStrategy
{
    [Range(2, (int) Color.COUNT - 1)]
    [SerializeField] private int colorAmountStart = 3;
    [SerializeField] private int ringAmountToLevelUp = GameConst.TimesSpawnEndless, ringIncreaseByLevel;

    public int ColorAmountStart => colorAmountStart;

    public int GetAmountRingsToLevelUp(int maxColorAmount)
    {
        if (maxColorAmount <= colorAmountStart) return ringAmountToLevelUp;
        return (maxColorAmount - colorAmountStart) * ringIncreaseByLevel + ringAmountToLevelUp;
    }
}