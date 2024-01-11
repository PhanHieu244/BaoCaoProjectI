using System.Collections;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRevivalEndlessClassic : UIRevivalByCoin
{
    [SerializeField] private Text currentPoint;
    [SerializeField] private Text maxPoint;

    protected override void OnEnable()
    {
        base.OnEnable();
        currentPoint.text = GameManagerEndless.CurrentScore.ToString();
        maxPoint.text = GameManagerEndless.HighScore.ToString();
    }

    protected override IEnumerator SwitchUILose()
    {
        yield return Hub.Hide(gameObject).Play()
            .OnComplete(ShowLose).WaitForCompletion();
    }

    protected override void ShowLose()
    {
        if (GameManagerEndless.HasHigherScore)
        {
            Hub.Show(Hub.Get<PopUpContent>(PopUpPath.POP_UP_WOOD__UI_ENDLESSCLASSIC_WIN).gameObject).Play();
            return;
        }
        Hub.Show(Hub.Get<PopUpContent>(revivalStrategy.LosePath).gameObject).Play();
    }
}