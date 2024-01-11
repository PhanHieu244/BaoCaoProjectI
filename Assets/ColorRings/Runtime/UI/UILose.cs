using Puzzle.UI;
using System.Collections.Generic;
using Falcon.FalconAnalytics.Scripts.Enum;
using UnityEngine;
using UnityEngine.UI;

public class UILose : MonoBehaviour, IPopUpContent
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button receiveGiftButton;
    [SerializeField] private GameObject goClaimed;

    private Dictionary<PowerUpType, (PowerUpData data, InGamePowerUpConfig config)> powerUp = new();

    private bool _isClaimed;
    private int _valueReward;
    private PowerUpType _rewardType;

    private void Awake()
    {
        var configs = Resources.LoadAll<InGamePowerUpConfig>("PowerUps");
        // ReSharper disable once Unity.UnknownResource
        var data = Resources.LoadAll<PowerUpData>("PowerUps");

        foreach (var config in configs)
        {
            if (powerUp.ContainsKey(config.type))
            {
                powerUp[config.type] = (null, config);
            }
            else
            {
                powerUp.Add(config.type, (null, config));
            }
        }

        foreach (var datum in data)
        {
            if (powerUp.ContainsKey(datum.type))
            {
                var pu = powerUp[datum.type];
                pu.data = datum;
                powerUp[datum.type] = pu;
            }
            else
            {
                powerUp.Add(datum.type, (datum, null));
            }
        }

        retryButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound(EventSound.Click);
            ReloadLevel();
        });

        receiveGiftButton.onClick.AddListener(ReceiveReward);
    }

    private void OnEnable()
    {
        GameLoader.Instance.StopCountTimeAdsBreak();
        ++GameDataManager.TimesPlay;
        AudioManager.Instance.PlaySound(EventSound.Lose);
        _isClaimed = false;

        var data = GameLoader.GetNextLevelInfo(GameDataManager.MaxLevelUnlock);
        _rewardType = data.reward;
        _valueReward = data.rewardAmount;

        goClaimed.GetComponent<UIGift>().Setup(_rewardType, _valueReward);
    }

    private void ReceiveReward()
    {
        if (_isClaimed) return;
        AdsManager.Instance.ShowReward($"LOSE_{_rewardType.ToString().ToUpper()}", () =>
        {
            powerUp[_rewardType].data.Count += _valueReward;
            Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock,FlowType.Source, "ads_lose_reward", _rewardType.ToString(), _rewardType.ToString(), _valueReward );
            goClaimed.SetActive(true);
            _isClaimed = true;
            ReloadLevel();
        });
        AudioManager.Instance.PlaySound(EventSound.Click);
    }

    private void ReloadLevel()
    {
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
}
