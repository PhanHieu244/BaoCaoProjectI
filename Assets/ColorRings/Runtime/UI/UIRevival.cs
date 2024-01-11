using System.Collections;
using DG.Tweening;
using Puzzle.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRevival : PopUpContent
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button noRetryButton;

    private void Awake()
    {
        retryButton.onClick.RemoveAllListeners();
        retryButton.onClick.AddListener(DestroyLine);
        
        noRetryButton.onClick.RemoveAllListeners();
        noRetryButton.onClick.AddListener(() =>
        {
            StartCoroutine(SwitchUILose());
        });
        
    }

    private IEnumerator SwitchUILose()
    {
        /*yield return Hub.Hide(Hub.Get<UIRevival>(PopUpPath.POP_UP_WOOD__UI_REVIVAL).gameObject).Play()
            .OnComplete(ShowLose).WaitForCompletion();*/
        yield return null;
    }

    private void ShowLose()
    {
        /*Hub.Show(Hub.Get<UILose>(PopUpPath.POP_UP_WOOD__UI_LOSE).gameObject).Play();*/
    }

    private void DestroyLine()
    {
        /*AdsManager.Instance.ShowReward($"REVIVAL_DESTROYROWCOL", () =>
        {
            Hub.Hide(Hub.Get<UIRevival>(PopUpPath.POP_UP_WOOD__UI_REVIVAL).gameObject).Play()
                .OnComplete(() =>
                {
                    InGamePowerUpManager.Instance.DestroyRowAndCol();
                });
        });*/
    }

}
