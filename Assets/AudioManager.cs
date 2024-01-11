using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeStage.AntiCheat.Storage;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] private AudioSource sound;
    [SerializeField] private AudioSource music;

    #region Music

    // [SerializeField] private AudioClip mBackGroundMenu;
    [SerializeField] private AudioClip mBackGroundInGame;

    #endregion

    #region Sound
    
    [Serializable]
    public struct Sound
    {
        public EventSound eventSound;
        public AudioClip audioClip;
    }

    [SerializeField] private List<Sound> soundList = new();

    #endregion

    protected override void Awake()
    {
        base.Awake();

        sound.mute = ObscuredPrefs.Get(GameConst.SoundState, 1) == 0;
        music.mute = ObscuredPrefs.Get(GameConst.MusicState, 1) == 0;

        music.loop = true;
        music.clip = mBackGroundInGame;
        music.Play();
    }

    public void SwitchStateSound(bool soundState)
    {
        GameDataManager.SoundState = soundState ? 1 : 0;
        sound.mute = !soundState;
        PlaySound(EventSound.Click);
    }

    public void SwitchStateMusic(bool musicState)
    {
        GameDataManager.MusicState = musicState ? 1 : 0;
        music.mute = !musicState;
        PlaySound(EventSound.Click);
    }

    public void PlayBackMusic()
    {
        
    }

    #region SoundFunction

    public void PlaySound(EventSound eventSound)
    {
        foreach (var soundId in soundList.Where(soundId => soundId.eventSound == eventSound))
        {
            sound.PlayOneShot(soundId.audioClip);
            break;
        }
    }

    #endregion
    
}

public enum EventSound
{
    Click,
    ClickRing,
    ClosePopUp,
    Win,
    Lose,
    Combo0,
    Combo1,
    Combo2,
    Combo3,
    Hammer,
    Rocket,
    Shuffle,
    SpawnRing,
    LevelComplete
}