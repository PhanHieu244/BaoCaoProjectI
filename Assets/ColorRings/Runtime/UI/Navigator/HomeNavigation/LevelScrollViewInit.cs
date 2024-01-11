using System;
using System.Collections.Generic;
using JackieSoft;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace ColorRings.Runtime.UI.Navigator.HomeNavigation
{
    public class LevelScrollViewInit : MonoBehaviour
    {
        [SerializeField] private ListView listView;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Button backScroll;
        [SerializeField] private int total = 20;
        private LevelScrollType _currentLevelType;
        public static Action<LevelScrollType> onShowCurrentLevelType;
        
        private void Start()
        {
            backScroll.onClick.AddListener(BackToCurrentLevel);
            onShowCurrentLevelType?.Invoke(_currentLevelType);
        }

        private void OnEnable()
        {
            listView.data = new List<Cell.IData>();
            var currentLevel = GameDataManager.MaxLevelUnlock + 1;
            GameDataManager.GetSpecialLevel(total, out var levelsBonus, out var levelsRewardSkin);
            //init current level 
            _currentLevelType = GetLevelType(currentLevel);
            onShowCurrentLevelType?.Invoke(_currentLevelType);
            listView.data.Add(new LevelActiveButtonScrollData
            {
                level = GameDataManager.LevelToShow(currentLevel),
                levelScrollType = _currentLevelType
            });
            
            //init next level
            for (var i = 1; i <= total; i++)
            {
                listView.data.Add(new LevelButtonScrollData
                {
                    level = GameDataManager.LevelToShow(currentLevel + i),
                    levelScrollType = GetLevelType(currentLevel + i)
                });
            }
            listView.data.Add(new LineConnectData());
            listView.Initialize();
            scrollRect.verticalNormalizedPosition = 0;

            LevelScrollType GetLevelType(int level)
            {
                if (levelsBonus.TryPeek(out int peek) && level == peek)
                {
                    levelsBonus.Dequeue();
                    return LevelScrollType.Bonus;
                }
                
                ModeGameType levelMode = GameLoader.GetNextLevelInfo(level - 1).ModeGameType;
                if (levelMode == ModeGameType.Hard)
                {
                    if (levelsRewardSkin.TryPeek(out peek) && level == peek)
                    {
                        levelsRewardSkin.Dequeue();
                        return LevelScrollType.HardRewardSkin;
                    }

                    return LevelScrollType.Hard;
                }

                if (levelMode == ModeGameType.SuperHard)
                {
                    if (levelsRewardSkin.TryPeek(out peek) && level == peek)
                    {
                        levelsRewardSkin.Dequeue();
                        return LevelScrollType.SuperHardRewardSkin;
                    }
                    return LevelScrollType.SuperHard;
                }

                if (levelsRewardSkin.TryPeek(out peek) && level == peek)
                {
                    levelsRewardSkin.Dequeue();
                    return LevelScrollType.RewardSkin;
                }
                
                return LevelScrollType.Normal;
            }
        }

        [Button]
        private void BackToCurrentLevel()
        {
            listView.ScrollTo<LevelActiveButtonScrollData>(t => true, 1);
        }
    }
}