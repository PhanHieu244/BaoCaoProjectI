using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CoinBonusPattern : MonoBehaviour
{
    [SerializeField] private Text coinRewardText;
    [SerializeField] private Image coinRewardIcon;
    public static int coinAmount;
    private int _coinRewardByRing;

    private void Start()
    {
        GameManager.Instance.OnComboComplete += RewardCoinByRing;
        _coinRewardByRing = GameConst.CoinBaseRewardInBonusLevel +
                            GameDataManager.MaxLevelUnlock / GameConst.CoinRangeToIncreaseByLevel;
    }

    private void OnEnable()
    {
        coinAmount = 0;
        UpdateUI();
    }

    private void RewardCoinByRing(int rings)
    {
        coinAmount += rings * _coinRewardByRing;

        DoCoinIconEffect(3);
        //UpdateUI();
        DOVirtual.DelayedCall(1f, () => UpdateUI());
    }

    public void UpdateUI()
    {
        coinRewardText.text = coinAmount.ToString();
    }

    private void DoCoinIconEffect(int loop)
    {
        for (int i = 0; i < loop; i++)
        {
            coinRewardIcon.rectTransform.DOScale(new Vector3(1.3f, 0.7f, 1f), 0.1f).SetDelay(0.4f + i * 0.3f);
            coinRewardIcon.rectTransform.DOScale(new Vector3(0.7f, 1.3f, 1f), 0.1f).SetDelay(0.5f + i * 0.3f);
        }
        coinRewardIcon.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.1f).SetDelay(0.6f + (loop - 1) * 0.3f);
    }
}