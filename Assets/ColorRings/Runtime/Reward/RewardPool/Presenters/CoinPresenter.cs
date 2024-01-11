using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CoinPresenter : MonoPresenter<Coin> {
    [SerializeField] private Text labelValue;

    public override void SetUpInfo(Coin reward) {
        labelValue.text = reward.Value.ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
    }
}