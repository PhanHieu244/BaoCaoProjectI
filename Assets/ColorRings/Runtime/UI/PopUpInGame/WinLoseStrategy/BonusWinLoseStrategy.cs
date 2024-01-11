using System;
using System.Collections;
using DG.Tweening;
using Puzzle.UI;

[Serializable]
public class BonusWinLoseStrategy : WinLoseUIStrategy
{
    public override void Initialize()
    {
        Hub.Hide(Hub.Get<PopUpContent>(PopUpPath.POP_UP_UI_ADSBREAK).gameObject).Play();
    }

    public override IEnumerator OnWinGame()
    {
        GameLog.Instance.OnWin();
        yield return null;
    }

    public override IEnumerator OnLoseGame()
    {
        GameLog.Instance.OnLose();
        if (GameManager.Instance.LoseCount <= 1)
        {
            yield return Hub.Show(Hub.Get<PopUpContent>(PopUpPath.POP_UP_WOOD__UI__NORMALMODE_REVIVALBYCOIN).gameObject).Play();
            yield break;
        }
        yield return Hub.Show(Hub.Get<PopUpContent>(PopUpPath.POP_UP_WOOD__UI_BONUS_WIN).gameObject).Play();
    }
}