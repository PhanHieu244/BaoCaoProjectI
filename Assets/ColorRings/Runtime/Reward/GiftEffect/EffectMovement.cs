using System;
using DG.Tweening;
using UnityEngine;

public static class EffectMovement
{
    public static void MoveByLine(Transform transform, Vector3 start, Vector3 end, float duration, Ease ease = Ease.Linear ,Action onComplete = null)
    {
        transform.position = start;
        transform.DOMove(end, duration).SetEase(ease).OnComplete((() =>
        {
            onComplete?.Invoke();
        }));

    }
}