using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public interface IForecast
{
    public void ShowForecast(Color[] pattern);
    public void InitForecast(Forecaster forecaster);

    public void ResetForecast();
}

[Serializable]
public class Forecast : IForecast
{
    protected const float Scale = 1f;
    protected Forecaster forecaster;
    private Image[] _ringsRender;

    public virtual void ShowForecast(Color[] pattern)
    {
        if (forecaster is null) return;
        for (var i = 0; i < pattern.Length; i++)
        {
            if (pattern[i] is Color.NONE)
            {
                _ringsRender[i].gameObject.SetActive(false);
                continue;
            }
            _ringsRender[i].gameObject.SetActive(true);
            _ringsRender[i].sprite = GameManager.Instance.GetSkin(pattern[i], (RingSize)i);
            _ringsRender[i].transform.localPosition = Vector3.zero;
            _ringsRender[i].transform.localScale = Vector3.one * Scale;
        }

        forecaster.transform.localScale = new Vector3(.02f, .02f, .02f);
        forecaster.transform.DOScale(Vector3.one, .2f).SetEase(Ease.OutBack);
    }

    public virtual void InitForecast(Forecaster forecaster)
    {
        this.forecaster = forecaster;
        _ringsRender = forecaster.RingsRender;
    }

    public virtual void ResetForecast()
    {
        for (int i = 0; i < _ringsRender.Length; i++)
        {
            _ringsRender[i].gameObject.SetActive(false);
        }
    }
}