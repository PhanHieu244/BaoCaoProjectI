using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [SerializeField] private Image darkImage;

    public void SwitchSceneAnim(Action action = null)
    {
        darkImage.enabled = true;
        var startValue = 0f;
        var opacity = darkImage.color;
        DOTween.To(() => startValue, change =>
        {
            opacity.a = change;
            darkImage.color = opacity;
        }, 1, .4f).OnComplete(() =>
        {
            action?.Invoke();
            DevLog.Log("Switch","Game");
            startValue = 1;
            DOTween.To(() => startValue, change =>
            {
                opacity.a = change;
                darkImage.color = opacity;
            }, 0, .3f).OnComplete(() =>
            {
                darkImage.enabled = false;
            });
            
        });

    }

}
