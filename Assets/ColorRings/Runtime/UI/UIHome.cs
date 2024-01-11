using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : MonoBehaviour, IPopUpContent
{
    [SerializeField] private Button settingButton;

    private void Awake()
    {
        settingButton.onClick.AddListener(() =>
        {
            var settingPopup = Hub.Get<UISetting>(PopUpPath.POP_UP_WOOD__UI_SETTING);
            Hub.Show(settingPopup.gameObject).Play();
        });
    }
}

