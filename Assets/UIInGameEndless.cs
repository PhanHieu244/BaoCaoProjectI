using UnityEngine;
using UnityEngine.UI;

public class UIInGameEndless : UIInGame
{
    [SerializeField] private Text highScore;
    [SerializeField] private Text currentScore;

    private void Start()
    {
        highScore.text = GameManagerEndless.HighScore.ToString();
        UpdateScore(0);
    }

    public override void UpdateScore(int score)
    {
        currentScore.text = score.ToString();
    }
    
}
