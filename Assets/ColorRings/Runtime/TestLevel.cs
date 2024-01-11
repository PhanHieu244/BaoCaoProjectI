using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class TestLevel : MonoBehaviour
{
    [SerializeField] private int levelToTest;

    private void Start()
    {
        GameLoader.Instance.Load(CalculateCustomLevel());        
    }

    private int CalculateCustomLevel()
    {
        //if (levelToTest == null) return;

        //int customLevelNum = GameDataManager.MaxLevelUnlock;
        //try
        //{
        //    customLevelNum = Int32.Parse(levelToTest.name.Substring(6));
        //}
        //finally
        //{
        //    GameDataManager.MaxLevelUnlock = customLevelNum + GameDataManager.CountLevelBonus(customLevelNum);
        //    DevLog.Log("Max level unlocked: ", levelToTest.name);
        //}

        //GameDataManager.MaxLevelUnlock = levelToTest - 1 + Mathf.CeilToInt(((float) levelToTest - 1) / (float) GameConst.BonusLevelRange);
        //return levelToTest + (levelToTest - levelToTest % GameConst.BonusLevelRange) / GameConst.BonusLevelRange;
        
        int trueLevel = levelToTest * GameConst.BonusLevelRange / (GameConst.BonusLevelRange - 1) - 1;

        return trueLevel;
    }
}
