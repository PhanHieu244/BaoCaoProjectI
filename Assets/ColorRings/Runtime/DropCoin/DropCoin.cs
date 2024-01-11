 using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCoin : MonoBehaviour
{
    [SerializeField] private GameObject coinPref;
    [SerializeField] private int dropAmount;
    [SerializeField] private float duration;
    [SerializeField] private Canvas thisCanvas;
    [SerializeField] private Vector2 targetPosition = new Vector2(-400, 800);
    
    private static float waiting = 0f;
    private static float appearTransitionDuration = 0.3f;
    private static float disappearTransitionDuration = 0.5f;

    public void DropCoins(Vector2 dropPosition, float duration)
    {
        for (int i = 0; i < dropAmount; i++)
        {
            RectTransform newCoinRect = PoolManager.Instance["CoinInCanvas"].Get<RectTransform>();
            DOVirtual.DelayedCall(waiting + disappearTransitionDuration + duration, () => PoolManager.Instance["CoinInCanvas"].Release(newCoinRect));

            newCoinRect.anchoredPosition = dropPosition;
            newCoinRect.localScale = Vector3.one;
            newCoinRect.SetParent(thisCanvas.transform);
            
            //newCoinRect.DOScale(1f, 0.3f).SetDelay(waiting + appearTransitionDuration / dropAmount * i).SetEase(Ease.OutBack);
            newCoinRect.DOAnchorPos(newCoinRect.anchoredPosition + Random.insideUnitCircle * 100, 0.3f).SetDelay(waiting).SetEase(Ease.OutSine);
            newCoinRect.DOAnchorPos(targetPosition, duration).SetDelay(waiting + appearTransitionDuration + duration / dropAmount * i);
            newCoinRect.DOScale(0f, duration).SetDelay(waiting + disappearTransitionDuration + duration / dropAmount * i);
        }
    }

    public void DropCoins(Vector2 dropPosition, Vector2 targetPos, float duration)
    {
        targetPosition = targetPos;
        DropCoins(dropPosition, duration);
    }
}
