using UnityEngine;
using UnityEngine.UI;

public class UILoseEndlessClassic : UIBaseLose
{
    [SerializeField] private Text currentPoint;
    [SerializeField] private Text maxPoint;

    protected override void OnEnable()
    {
        base.OnEnable();
        currentPoint.text = GameManagerEndless.CurrentScore.ToString();
        maxPoint.text = GameManagerEndless.HighScore.ToString();
    }
}