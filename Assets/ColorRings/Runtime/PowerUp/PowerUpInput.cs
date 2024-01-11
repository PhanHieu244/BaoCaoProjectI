using UnityEngine;

public class PowerUpInput : MonoBehaviour
{
    private void OnEnable()
    {
        RingHolder.IsLock = true;
    }

    private void OnDisable()
    {
        RingHolder.IsLock = false;
    }

    private void OnMouseUpAsButton()
    {
        var tutorial = GameManager.Instance.Level.tutorialMod;
        if (isTutorialItemMode(tutorial))
        {
            if (isClickedFollowTutorial(tutorial))
            {
                tutorial.IsClicked();
                InGamePowerUpManager.Instance.StartCoroutine(InGamePowerUpManager.Instance.IEActivePowerUp(RingHolder.GetMouseWorldPosition()));
            }
        }
        else
        {
            InGamePowerUpManager.Instance.StartCoroutine(InGamePowerUpManager.Instance.IEActivePowerUp(RingHolder.GetMouseWorldPosition()));
        }
    }

    private bool isTutorialItemMode(ITutorial tutorial)
    {
        if (tutorial == null) return false;
        if (!tutorial.IsItemTutorial()) return false;
        if (tutorial.IsDoneTutorial()) return false;
        return true;
    }

    private bool isClickedFollowTutorial(ITutorial tutorial)
    {
        if (tutorial.IsDoneTutorial()) return true;
        var posTarget = tutorial.GetPositionClick();
        var coordTarget = GameManager.Instance.BoardEntity.InputCoordinateAvailable(posTarget);
        var posUp = RingHolder.GetMouseWorldPosition();
        var coordUp = GameManager.Instance.BoardEntity.InputCoordinateAvailable(posUp);
        return coordUp == coordTarget;
    }
}
