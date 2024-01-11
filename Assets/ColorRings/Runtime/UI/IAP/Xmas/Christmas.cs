using Puzzle.UI;
using UnityEngine;

public class Christmas : MonoBehaviour, IPopUpContent {
    private void OnEnable()
    {
        IAPCacheData.OnBuyIap += Hide;
    }

    private void OnDisable()
    {
        IAPCacheData.OnBuyIap -= Hide;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}