using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Forecaster : MonoBehaviour
{
    [SubclassSelector, SerializeReference] private IForecast _forecast = new Forecast();
    [SerializeField] private Button adsButton;
    [SerializeField] private Image[] ringsRender;

    private void Awake()
    {
        
        adsButton.onClick.AddListener(() =>
        {
            AdsManager.Instance.ShowReward("FORECAST_ADS", () =>
            {
                Active();
                AudioManager.Instance.PlaySound(EventSound.Click);
                GameManager.Instance.RingSpawner.OnActiveForecaster();
            });
        });
    }

    private void Active()
    {
        GameManager.Instance.RingSpawner.OnForecast += ShowForecast;
        _forecast.InitForecast(this);
        adsButton.gameObject.SetActive(false);
    }
    
    public void UnActive()
    {
        GameManager.Instance.RingSpawner.OnForecast -= ShowForecast;
        _forecast.ResetForecast();
        adsButton.gameObject.SetActive(true);
    }

    private void ShowForecast(Color[] pattern)
    {
        if (!gameObject.activeSelf) return;
        _forecast?.ShowForecast(pattern);
    }

    public Image[] RingsRender
    {
        get => ringsRender;
        set => ringsRender = value;
    }
}
