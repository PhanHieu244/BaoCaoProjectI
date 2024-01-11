using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class EndlessRewardPopUp : PopUpContent
{
    [SerializeField] private Button exitBut;

    private void Awake()
    {
        exitBut.onClick.AddListener(ClosePopUp);
    }

    private void ClosePopUp()
    {
        Hub.Hide(gameObject).Play();
    }
}