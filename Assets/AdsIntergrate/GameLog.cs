using System;
using System.Collections;
using System.Collections.Generic;
using Falcon.FalconAnalytics.Scripts.Enum;
using UnityEngine;

public class GameLog : MonoSingleton<GameLog>
{
    private DateTime timeStartGame;

    protected override void Awake()
    {
        if (Instance is not null) return;
        base.Awake();
    }

    protected override void OnDestroy()
    {
    }

    public void OnStartGame()
    {
        timeStartGame = DateTime.UtcNow;    
    }

    private TimeSpan GetTime()
    {
        return DateTime.UtcNow - timeStartGame;
    }

    public void OnWin()
    {
        Data4Game.LevelLog(GameDataManager.MaxLevelUnlock, GetTime(), 0, "Normal", LevelStatus.Pass);
    }

    public void OnLose()
    {
        var dataLog = UIInGame.Instance.dataLevelLog;
        if (dataLog.LogLevel(out var level, out var difficult))
        {
            Data4Game.LevelLog(level, GetTime(), 0, difficult, LevelStatus.Fail);
            DevLog.Log("data level log", level);
            if (dataLog.LogAdditive(out var id, out var numeral, out var propertyName))
            {
                Data4Game.PropertyLog(id, propertyName, numeral.ToString(), 1);
            }
            return;
        }
        if (UIInGame.Instance.ChallengeManager.GetChallangeType() == ChallangeType.Endless)
        {
            Data4Game.LevelLog(1, GetTime(), 0, "Endless", LevelStatus.Fail);
            return;
        }
        Data4Game.LevelLog(GameDataManager.MaxLevelUnlock, GetTime(), 0, "Normal", LevelStatus.Fail);
    }
    
}
