using System;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat;
using CodeStage.AntiCheat.Storage;

public class UISetting : MonoBehaviour, IPopUpContent
{
    [SerializeField] private Button exitButton;

    [SerializeField] private Toggle sound;
    [SerializeField] private Toggle music;
    [SerializeField] private Toggle vibrate;

    private void Awake()
    {
        exitButton.onClick.AddListener(() =>
        {
            var popup = Hub.Get<UISetting>(PopUpPath.POP_UP_WOOD__UI_SETTING);
            Hub.Hide(popup.gameObject).Play();
            AudioManager.Instance.PlaySound(EventSound.Click);
        });
        
        sound.isOn = ObscuredPrefs.Get(GameConst.SoundState, 1) == 1;
        music.isOn = ObscuredPrefs.Get(GameConst.MusicState, 1) == 1;
        vibrate.isOn = ObscuredPrefs.Get(GameConst.VibrateState, 1) == 1;

        sound.onValueChanged.AddListener(AudioManager.Instance.SwitchStateSound);
        music.onValueChanged.AddListener(AudioManager.Instance.SwitchStateMusic);

        vibrate.onValueChanged.AddListener((bool t) =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
        });
        
        vibrate.onValueChanged.AddListener(PhoneManager.Instance.SwitchOnOffViberate);

    }
}
