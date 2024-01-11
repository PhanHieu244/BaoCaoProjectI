using System.Collections;
using System.Collections.Generic;
using Falcon.FalconAnalytics.Scripts.Enum;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIWin : PopUpContent
{
    [SerializeField] private Button bContinue, bReceive;
    [SerializeField] private GameObject goClaimed;
    [SerializeField] private float secondWaitContinue = 2;
    private List<IChallenge> _challenges = new();
    private Dictionary<PowerUpType, (PowerUpData data, InGamePowerUpConfig config)> powerUp = new();
    private bool _isClaimed;
    private int _valueReward;
    private PowerUpType _rewardType;

    private void Awake()
    {
        // ReSharper disable once Unity.UnknownResource
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

        bContinue.onClick.AddListener(PlayNextLevel);
        bReceive.onClick.AddListener(ReceiveReward);
    }

    public void OnEnable()
    {
        ++GameDataManager.TimesPlay;

        AudioManager.Instance.PlaySound(EventSound.Win);
        GameLoader.Instance.StopCountTimeAdsBreak();
        
        _isClaimed = false;
        var data = GameLoader.GetNextLevelInfo(GameDataManager.MaxLevelUnlock);

        _rewardType = data.reward;
        _valueReward = data.rewardAmount;

        goClaimed.GetComponent<UIGift>().Setup(_rewardType, _valueReward);
        
        bContinue.gameObject.SetActive(false);

        StartCoroutine(WaitTurnOnButton());

        GameDataManager.MaxLevelUnlock++;
    }

    public void OnDisable()
    {
        foreach (var challenge in _challenges)
        {
            Destroy(((Component)challenge)?.gameObject);
        }
        
        _challenges.Clear();
        
    }


    private void ReceiveReward()
    {
        if(_isClaimed) return;
        AdsManager.Instance.ShowReward($"WIN_{_rewardType.ToString().ToUpper()}", () =>
        {
            powerUp[_rewardType].data.Count += _valueReward;
            Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock,FlowType.Source, "ads_win_reward", _rewardType.ToString(), _rewardType.ToString(), _valueReward );
            goClaimed.SetActive(true);
            _isClaimed = true;
            PlayNextLevel();
        });
    }

    private void PlayNextLevel()
    {
        GameLoader.Instance.Load(GameDataManager.MaxLevelUnlock, gameObject);
        AudioManager.Instance.PlaySound(EventSound.Click);
    }

    private IEnumerator WaitTurnOnButton()
    {
        yield return new WaitForSeconds(secondWaitContinue);
        bContinue.gameObject.SetActive(true);
    }
    
}