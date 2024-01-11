using System;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.UI;

public class PopupConsent : MonoBehaviour
{
    private const string AlreadyConfirmPopupConsent = "already_confirm_popup_consent";
    [SerializeField] private GameObject panelConsent;
    [SerializeField] private Button yes;
    //[SerializeField] private Button no;
    [SerializeField] private AgeSlider ageSlider;

    private void Awake()
    {
        yes.onClick.AddListener(OnClickBtnYes); 
        //no.onClick.AddListener(OnClickBtnNo);
    }

    private void Start()
    {
        var consentNum = ObscuredPrefs.Get(AlreadyConfirmPopupConsent, 0);

        if (consentNum == 0)
        {
            panelConsent.SetActive(true);
            return;
        }

        panelConsent.SetActive(false);
        ISHandler.Instance.Init(consentNum == 1);
    }

    private void OnClickBtnYes()
    {
        panelConsent.SetActive(false);
        Data4Game.PropertyLog(0, "User_Age", ageSlider.GetAge().ToString(), 1);
        AudioManager.Instance.PlaySound(EventSound.Click);
        ObscuredPrefs.Set(AlreadyConfirmPopupConsent, 1);
        ISHandler.Instance.Init(true);
    }


    private void OnClickBtnNo()
    {
        panelConsent.SetActive(false);
        Data4Game.PropertyLog(0, "User_Age", ageSlider.GetAge().ToString(), 1);
        AudioManager.Instance.PlaySound(EventSound.Click);
        ObscuredPrefs.Set(AlreadyConfirmPopupConsent, 2);
        ISHandler.Instance.Init(false);
    }
    
}