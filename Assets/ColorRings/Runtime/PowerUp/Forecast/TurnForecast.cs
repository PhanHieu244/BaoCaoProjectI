using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TurnForecast : Forecast
{
    [SerializeField] private GameObject countBackground;
    [SerializeField] private Text turnForecast;
    [SerializeField] private int turnAmount = 6;
    private int _countTurn;

    public override void ShowForecast(Color[] pattern)
    {
        if (forecaster is null) return;
        base.ShowForecast(pattern);
        _countTurn--;
        turnForecast.text = $"{_countTurn}";
        if (_countTurn > 0) return;
        forecaster.UnActive();
    }

    public override void InitForecast(Forecaster forecaster)
    {
        base.InitForecast(forecaster);
        countBackground.SetActive(true);
        turnForecast.text = $"{turnAmount}";
        _countTurn = turnAmount;
    }

    public override void ResetForecast()
    {
        countBackground.SetActive(false);
        base.ResetForecast();
    }
}