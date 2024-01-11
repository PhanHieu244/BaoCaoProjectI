using UnityEngine;
using UnityEngine.UI;

public class UIWinEndlessClassic : UIBaseLose
{
    [SerializeField] private Text maxPoint;

    protected override void OnEnable()
    {
        base.OnEnable();
        maxPoint.text = GameManagerEndless.CurrentScore.ToString();
    }
}