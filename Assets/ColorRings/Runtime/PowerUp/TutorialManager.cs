using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] TutorialInput tutorialInput;
    public static bool IsTurotialDone = true;

    public TutorialInput TutorialInput
    {
        get => tutorialInput;
    }
    public void StartTutorial(ITutorial tutorial)
    {
        tutorialInput.Setup(tutorial);
        tutorialInput.gameObject.SetActive(true);
        IsTurotialDone = false;
        tutorialInput.MoveHand(GameManager.Instance.BoardEntity.GetBoardPos(Vector2Int.one));
    }

    public void StartTutorialItem(ITutorial tutorial)
    {
        tutorialInput.Setup(tutorial);
        tutorialInput.gameObject.SetActive(true);
        tutorialInput.SetHandClick();
        IsTurotialDone = false;
    }

    public void TutorialDone()
    {
        IsTurotialDone = true;
        tutorialInput.DoneTutorial();
    }

}
