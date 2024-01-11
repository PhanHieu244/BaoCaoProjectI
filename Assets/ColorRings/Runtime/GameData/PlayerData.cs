using System;
using UnityEngine;

namespace ColorRings.Runtime.GameData
{
    [Serializable]
    public class PlayerData : IData
    {
        //1 - unlock
        //0 lock
        //-1 unlock but Lock by event
        //-2 lock and lock by event
        public int[] skinItemsUnlock;
        public int currentSkin;
        public int coin;
        public int skinStreak;
        public int winningStreak;
        public int currentWeek;
        public int isInWinningStreakProgress;
        public PlayerData()
        {
            skinItemsUnlock = new int[1];
            skinItemsUnlock[0] = 1;
        }
    }
}