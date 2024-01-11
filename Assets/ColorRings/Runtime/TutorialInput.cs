using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TutorialInput : MonoBehaviour
{
    [SerializeField] private GameObject hand;
    [SerializeField] private Animator animator;
    Vector2Int? coordTarget;
    private ITutorial _tutorial;
    private Vector3 _handPos;
    private static readonly int IsClick = Animator.StringToHash("IsClick");

    private void OnDisable()
    {
        hand.SetActive(false);
    }

    public void MoveHand(Vector2 target)
    {
        if (_handPos == Vector3.zero) _handPos = hand.transform.position;
        hand.transform.position = _handPos;
        this.coordTarget = GameManager.Instance.BoardEntity.InputCoordinateAvailable(target);
        target += new Vector2(0.3f, 0);
        hand.transform.DOMove(target, 1.5f).SetLoops(-1);
        animator.StopPlayback();
    }

    public void SetHandClick()
    {
        if (_tutorial.IsItemTutorial())
        {
            var pos = _tutorial.GetPositionClick();
            hand.transform.position = pos + new Vector2(0.2f, -0.2f);
            if (gameObject.activeSelf)
            {
                animator.SetBool(IsClick, true);
            }
        }
    }

    public void Setup(ITutorial tutorial)
    {
        this._tutorial = tutorial;
    }

    private void OnMouseUpAsButton()
    {
        if (_tutorial.IsDoneAStep())
        {
            if(_tutorial.HaveNextStep())
            {
                StartCoroutine(DoneAStep());
            }
            else if(_tutorial.IsDoneTutorial())
            {
                DoneTutorial();
            }
        } else if(_tutorial.IsDoneTutorial()) DoneTutorial();
    }

    public void ContinueTutorial()
    {
        var pos = _tutorial.GetPositionClick();
        if (pos != Vector2.positiveInfinity)
        {
            SetHandClick();
        }
    }

    public void DoneTutorial()
    {
        hand.transform.DOKill();
        hand.SetActive(false);
        TutorialManager.IsTurotialDone = true;
        this.gameObject.SetActive(false);
    }

    private IEnumerator DoneAStep()
    {
        hand.transform.DOKill();
        hand.SetActive(false);
        TutorialManager.IsTurotialDone = true;
        
        yield return new WaitForSeconds(.3f);
        
        hand.SetActive(true);
        TutorialManager.IsTurotialDone = false;
        this.gameObject.SetActive(true);
        MoveHand(GameManager.Instance.BoardEntity.GetBoardPos(_tutorial.GetTargetCoord()));
    }
}
