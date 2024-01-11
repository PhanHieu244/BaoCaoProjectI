using System;
using System.Collections.Generic;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIBaseLose : PopUpContent
{
    private readonly Dictionary<PowerUpType, (PowerUpData data, InGamePowerUpConfig config)> _powerUp = new();
    [SerializeField] private Button bHome, bTryAgain;
    protected virtual void Awake()
    {
        var configs = Resources.LoadAll<InGamePowerUpConfig>("PowerUps");
        // ReSharper disable once Unity.UnknownResource
        var data = Resources.LoadAll<PowerUpData>("PowerUps");

        foreach (var config in configs)
        {
            if (_powerUp.ContainsKey(config.type))
            {
                _powerUp[config.type] = (null, config);
            }
            else
            {
                _powerUp.Add(config.type, (null, config));
            }
        }

        foreach (var datum in data)
        {
            if (_powerUp.ContainsKey(datum.type))
            {
                var pu = _powerUp[datum.type];
                pu.data = datum;
                _powerUp[datum.type] = pu;
            }
            else
            {
                _powerUp.Add(datum.type, (datum, null));
            }
        }

        
        bHome.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            var popup = Hub.Get<UIPause>(PopUpPath.POP_UP_WOOD__UI_PAUSE);
            Hub.Hide(popup.gameObject).Play().OnComplete(() =>
            {
                CanvasManager.Instance.SwitchSceneAnim(() =>
                {
                    GameLoader.Instance.LoadHome();
                });
            });
        });
        bTryAgain.onClick.AddListener(ReloadLevel);
    }
    
    private void ReloadLevel()
    {
        AudioManager.Instance.PlaySound(EventSound.Click);
        var challengeType = UIInGame.Instance.ChallengeManager.GetChallangeType();
        if (challengeType == ChallangeType.Endless)
        {
            GameLoader.Instance.Load(GameManager.Instance.Level.ModeGameType, gameObject);
        }
        else
        {
            GameLoader.Instance.Load(GameDataManager.MaxLevelUnlock, gameObject);
        }
    }

    protected virtual void OnEnable()
    {
        GameLoader.Instance.StopCountTimeAdsBreak();
        AudioManager.Instance.PlaySound(EventSound.Lose);
        ++GameDataManager.TimesPlay;
    }
}