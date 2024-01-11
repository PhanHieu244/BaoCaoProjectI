using System.Collections;
using System.Linq;
using Falcon.FalconAnalytics.Scripts.Enum;
using UnityEngine;

public class IGCSwap : InGamePowerUpContainer<InGamePowerUpConfig>
{
    protected override void OnClick()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        var tutorial = GameManager.Instance.Level.tutorialMod;
        if (tutorial != null && !TutorialManager.IsTurotialDone && tutorial.IsNormalTutorial()) return;
        if (tutorial != null && tutorial.GetItemType() != itemType) return;
        if (tutorial != null && tutorial.GetItemType() == itemType) isTutorialing = true;

        if (InGamePowerUpManager.Instance.PowerUpExecution == null && isUnlocked)
        {
            if (IsWinningStreakMode)
            {
                StartCoroutine(IEExecute(GameManager.Instance, Vector2Int.zero));
                UnActiveWinningStreakPowerUp();
                Count(powerUpData.Count);
                return;
            }
            if (powerUpData.Count <= 0 && !isTutorialing)
            {
                AdsManager.Instance.ShowReward($"IN_GAME_ADD_ITEM_{powerUpData.ToString().ToUpper()}", () =>
                {
                    powerUpData.Count++;
                    Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock, FlowType.Source, "using_item_in_game", powerUpData.ToString(), powerUpData.ToString(), 1);
                    Count(powerUpData.Count);
                });
            }
            else
            {
                if (!isTutorialing)
                {
                    powerUpData.Count--;
                }
                StartCoroutine(IEExecute(GameManager.Instance, Vector2Int.zero));
                Count(powerUpData.Count);
            }
        }
        
    }

    public override IEnumerator IEExecute(GameManager gameManager, Vector2Int coordinate, bool isUpdateCount = true)
    {
        var tutorial = GameManager.Instance.Level.tutorialMod;
        if (tutorial != null && tutorial.IsItemTutorial())
        {
            TutorialManager.Instance.TutorialDone();
            RingHolder.IsLock = false;
        }
        AudioManager.Instance.PlaySound(EventSound.Shuffle);
        var patterns = gameManager.Board.AvailablePattern(gameManager.RingSpawner.level.sizePatterns.ToArray()).ToArray();
        yield return gameManager.RingSpawner.IESpawnBySizePattern(patterns[Random.Range(0, patterns.Length)]);
        yield return base.IEExecute(gameManager, coordinate, isUpdateCount);
    }
}
