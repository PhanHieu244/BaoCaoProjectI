using DG.Tweening;
using Falcon.FalconAnalytics.Scripts.Enum;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class RewardBonusCoinPopUp : PopUpContent
{
    [SerializeField] private Text coinText;
    [SerializeField] private Button bContinue;

    private void Awake()
    {
        bContinue.onClick.AddListener(() =>
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
    }

    private void Start()
    {
        var coinReward = CoinBonusPattern.coinAmount;
        coinText.text = coinReward.ToString();
        //update data   
        GameDataManager.MaxLevelUnlock++;
        GameDataManager.AddCoins(coinReward);
        Data4Game.ResourceLog(GameDataManager.MaxLevelUnlock + 1,FlowType.Source, "CoinWin", 
            $"CoinBonusReward", "coin", coinReward);
    }
}