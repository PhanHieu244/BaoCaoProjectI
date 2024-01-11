using System;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour, IPopUpContent
{
    [SerializeField] private Button homeButton;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        continueButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            var popup = Hub.Get<UIPause>(PopUpPath.POP_UP_WOOD__UI_PAUSE);
            Hub.Hide(popup.gameObject).Play();
        });
        
        homeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            GameManager.Instance.onOutGame?.Invoke();
            var popup = Hub.Get<UIPause>(PopUpPath.POP_UP_WOOD__UI_PAUSE);
            Hub.Hide(popup.gameObject).Play().OnComplete(() =>
            {
                CanvasManager.Instance.SwitchSceneAnim(() =>
                {
                    GameLoader.Instance.LoadHome();
                });
            });
        });
    }
}
