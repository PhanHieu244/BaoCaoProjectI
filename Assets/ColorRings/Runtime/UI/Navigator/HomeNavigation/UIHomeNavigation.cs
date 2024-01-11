using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeNavigation : MonoBehaviour, IPopUpContent
{
    [SerializeField] private Button settingButton;

    private void Start()
    {
        settingButton.onClick?.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            var settingPopup = Hub.Get<UISetting>(PopUpPath.POP_UP_WOOD__UI_SETTING);
            settingPopup.transform.SetParent(null);
            Hub.Show(settingPopup.gameObject).Play();
        });
    }  
}