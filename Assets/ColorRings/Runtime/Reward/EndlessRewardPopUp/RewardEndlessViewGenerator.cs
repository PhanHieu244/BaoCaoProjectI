using System;
using System.Collections;
using System.Collections.Generic;
using JackieSoft;
using NaughtyAttributes;
using UnityEngine;   
using UnityEngine.UI;

public class RewardEndlessViewGenerator : MonoBehaviour
{
    [SerializeField] private ListView listView;
    [SerializeField] private ScrollRect scrollRect;
    [SubclassSelector, SerializeReference] private IEndlessRewardStrategy _endlessRewardStrategy = new RewardEndlessClassicStrategy();
    [SerializeField] private EndlessRewardDataSO endlessRewardDataSo;
    public static Action<int> onProgressComplete;
    public static Action<int> onProgressStart;
    private EndlessRewardPackage[] _endlessRewardPackages;
    private int _currentIDReward;
    private int _targetIDReward;


#if UNITY_EDITOR
    [Space] [Header("EditorSetting")] [SerializeField]
    private GameObject popup;
    [SerializeField] private int current;
    [SerializeField] private int newHigh;
 
    [Button()]
    private void UpdateClassicScore()
    {
        GameDataManager.ResetClassicEndlessData();
        GameDataManager.CurrentRewardHighScoreEndlessClassic = current;
        GameDataManager.HighScoreEndLess = newHigh;
    }
    
    [Button()]
    private void UpdateAdvancedScore()
    {
        GameDataManager.ResetClassicEndlessData();
        GameDataManager.CurrentRewardHighScoreEndlessClassic = current;
        GameDataManager.HighScoreEndLess = newHigh;
    }

    [Button()]
    private void PopUpToggle()
    {
        popup.SetActive(!popup.activeSelf);
    }
#endif
    
    private void Awake()
    {
        _endlessRewardPackages = endlessRewardDataSo.EndlessRewardPackages;
    }

    private void CheckWeek()
    {
        _endlessRewardStrategy.CheckWeek();
    }

    private void OnEnable()
    {
        CheckWeek();
        listView.data = new List<Cell.IData>();
        _currentIDReward = -1;
        _targetIDReward = -1;
        var currentHighScore = _endlessRewardStrategy.GetCurrentHighScore;
        var newHighScore = _endlessRewardStrategy.GetHighScore;
        //init first data
        listView.data.Add(new EntryEndlessGiftElementData
        {
            endlessRewardStrategy = _endlessRewardStrategy,
            endlessRewardPackage = _endlessRewardPackages[0],
            id = 0
        });
        var point = _endlessRewardPackages[0].Point;
        if (point <= currentHighScore) _currentIDReward = 0;
        if (point <= newHighScore) _targetIDReward = 0;
        //init next data
        for (var id = 1; id < _endlessRewardPackages.Length; id++)
        {
            var rewardPackage = _endlessRewardPackages[id];
            listView.data.Add(new EndlessGiftElementData
            {
                endlessRewardStrategy = _endlessRewardStrategy,
                endlessRewardPackage = rewardPackage,
                id = id
            });
            point = rewardPackage.Point;
            if (point <= currentHighScore) _currentIDReward = id;
            if (point <= newHighScore) _targetIDReward = id;
        }

        listView.Initialize();
        scrollRect.verticalNormalizedPosition = 0;
        MoveToCurrentReward(_currentIDReward, _targetIDReward - _currentIDReward, out var waitTime);
        StartCoroutine(IEActive(waitTime));
    }

    private IEnumerator IEActive(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(IEStartProgress(_currentIDReward));
        StartCoroutine(IEScroll(_targetIDReward - 1, _targetIDReward - _currentIDReward));
    }
    
    private IEnumerator IEStartProgress(int currentID)
    {
        yield return new WaitForSeconds(0.5f);
        onProgressStart.Invoke(currentID);
    }

    private IEnumerator IEScroll(int targetIDToScroll, int delta)
    {
        if(delta <= 2) yield break;
        yield return new WaitForSeconds(EndlessGiftElementBase.TimeIncrease * 1.8f);
        listView.ScrollTo<EndlessGiftElementData>(t => t.id == targetIDToScroll, 
            EndlessGiftElementBase.TimeIncrease * delta * 0.85f);
    }
    
    private void MoveToCurrentReward(int id, int delta, out float time)
    {
        time = 0f;
        if(delta <= 0) id--; //set id to place element in mid
        if (id <= 0) return;
        time = 0.3f * id;
        time = time >= 1.5f ? 1.5f : time;
        listView.ScrollTo<EndlessGiftElementData>(t => t.id == id, time);
    }

    private void OnDisable()
    {
        onProgressComplete = null;
        onProgressStart = null;
    }
}


public interface IEndlessRewardStrategy
{
    int GetCurrentHighScore { get; }
    void UpdateCurrentHighScore(int score);
    int GetHighScore { get; }
    void CheckWeek();
    bool IsReceiveGift(int id);
    void ReceiveGift(int id);
}

[Serializable]
public class RewardEndlessClassicStrategy : IEndlessRewardStrategy
{
    public int GetCurrentHighScore => GameDataManager.CurrentRewardHighScoreEndlessClassic;

    public void UpdateCurrentHighScore(int score)
    {
        GameDataManager.CurrentRewardHighScoreEndlessClassic = score;
    }

    public int GetHighScore => GameDataManager.HighScoreEndLess;
    public void CheckWeek()
    {
        if (GameDataManager.IsChangeWeek())
        {
            GameDataManager.ResetClassicEndlessData();
        }
    }

    public bool IsReceiveGift(int id)
    {
        return GameDataManager.IsReceivedEndlessClassicGift(id);
    }

    public void ReceiveGift(int id)
    {
        GameDataManager.ReceiveEndlessClassicGift(id);
    }
}